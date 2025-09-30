using Code.Scripts.Game.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Code.Scripts.Game.Behaviour
{
    [RequireComponent(typeof(Tilemap), typeof(Collider2D))]
    public class FakeTileController : InteractableComponent
    {
        private struct TileGroup
        {
            public bool isHiding;
            public bool hasPlayedAudio;
            public HashSet<Vector3Int> tiles;
        }

        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float fadeAlpha = 0.2f;
        [SerializeField] private float alphaSoundRange = 0.5f;
        [SerializeField] private WwiseEvent soundEvent;

        private Tilemap tilemap;
        private Collider2D col;
        private Dictionary<Vector3Int, TileGroup> tileGroups = new Dictionary<Vector3Int, TileGroup>();
        private Coroutine coroutine = null;
        private List<Vector2Int> searchAttemptOffsets = new List<Vector2Int> {
            Vector2Int.zero, // None
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left, // Cardinal directions
            Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right, Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right // Diagonals
        };

        private void Start()
        {
            tilemap = gameObject.GetComponent<Tilemap>();
            col = gameObject.GetComponent<CompositeCollider2D>();
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        protected override void OnInteracted()
        {
            Collider2D playerCol = GameManager.Instance.Player.GetComponent<Collider2D>();
            var dist = Physics2D.Distance(playerCol, col);
            Vector3Int tilePos = tilemap.WorldToCell(col.ClosestPoint(dist.pointA));
            HashSet<Vector3Int> tilePositions = new HashSet<Vector3Int>();
            Vector3Int minTile = new Vector3Int(int.MaxValue, int.MaxValue, 0);

            // Find triggered tile
            foreach (Vector2Int offset in searchAttemptOffsets)
            {
                Vector3Int searchTilePos = tilePos + (Vector3Int)offset;
                if (tilemap.HasTile(searchTilePos))
                {
                    tilePos = searchTilePos;
                    break;
                }
            }
            if (!tilemap.HasTile(tilePos))
            {
                Debug.LogError("Error: Unable to find fake tile");
                return;
            }

            // Identify if triggered tile is already part of a fake wall that's being processed
            foreach (Vector3Int key in tileGroups.Keys)
            {
                TileGroup group = tileGroups[key];
                if (group.tiles.Contains(tilePos))
                {
                    if (group.isHiding)
                    {
                        tileGroups[key] = new TileGroup
                        {
                            isHiding = false,
                            hasPlayedAudio = group.hasPlayedAudio,
                            tiles = group.tiles
                        };
                        if (coroutine == null)
                        {
                            StartCoroutine(Processing());
                        }
                    }
                    return;
                }
            }

            // Recursive flood fill
            void AddTile(Vector3Int tile)
            {
                if (tilemap.HasTile(tile) && !tilePositions.Contains(tile))
                {
                    tilePositions.Add(tile);
                    tilemap.RemoveTileFlags(tile, TileFlags.LockColor);
                    if (tile.x < minTile.x || (tile.x == minTile.x && tile.y < minTile.y))
                    {
                        minTile = tile;
                    }
                    AddTile(tile + Vector3Int.up);
                    AddTile(tile + Vector3Int.right);
                    AddTile(tile + Vector3Int.down);
                    AddTile(tile + Vector3Int.left);
                }
            }

            AddTile(tilePos);

            tileGroups.Add(minTile, new TileGroup
            {
                isHiding = false,
                hasPlayedAudio = false,
                tiles = tilePositions
            });
            if (coroutine == null)
            {
                StartCoroutine(Processing());
            }
        }

        protected override void OnStopInteraction()
        {
            Collider2D playerCol = GameManager.Instance.Player.GetComponent<Collider2D>();
            var dist = Physics2D.Distance(playerCol, col);
            Vector3Int tilePos = tilemap.WorldToCell(col.ClosestPoint(dist.pointA));

            // Find triggered tile
            foreach (Vector2Int offset in searchAttemptOffsets)
            {
                Vector3Int searchTilePos = tilePos + (Vector3Int)offset;
                if (tilemap.HasTile(searchTilePos))
                {
                    tilePos = searchTilePos;
                    break;
                }
            }

            // Identify if triggered tile is already part of a fake wall that's being processed
            foreach (Vector3Int key in tileGroups.Keys)
            {
                TileGroup group = tileGroups[key];
                if (group.tiles.Contains(tilePos))
                {
                    if (!group.isHiding)
                    {
                        tileGroups[key] = new TileGroup
                        {
                            isHiding = true,
                            hasPlayedAudio = group.hasPlayedAudio,
                            tiles = group.tiles
                        };
                    }
                    return;
                }
            }
        }

        private void ApplyAlpha(Vector3Int key, float alpha)
        {
            Color color = tilemap.color;
            color.a = alpha;
            foreach (Vector3Int tile in tileGroups[key].tiles)
            {
                tilemap.SetColor(tile, color);
            }
        }

        private IEnumerator Processing()
        {
            while (tileGroups.Count > 0)
            {
                List<Vector3Int> deleteKeys = new List<Vector3Int>();
                float progressScale = 1 / (1f - fadeAlpha);
                float timeStep = Time.deltaTime / fadeDuration;

                List<Vector3Int> keyset = new List<Vector3Int>(tileGroups.Keys);
                foreach (Vector3Int key in keyset)
                {
                    TileGroup group = tileGroups[key];
                    float alpha = tilemap.GetColor(key).a;
                    // 1 = 1 (Opaque) | 0 = fadeAlpha
                    float progress = (alpha - fadeAlpha) * progressScale;

                    if (progress >= 0 && !group.isHiding)
                    {
                        if (!group.hasPlayedAudio && progress < alphaSoundRange)
                        {
                            group.hasPlayedAudio = true;
                            tileGroups[key] = group;
                        }
                        float newAlpha = Mathf.Max(0, (progress - timeStep)) * (1f - fadeAlpha) + fadeAlpha;
                        ApplyAlpha(key, newAlpha);
                    }
                    else if (progress < 1 && group.isHiding)
                    {
                        float newAlpha = Mathf.Min(1, (progress + timeStep)) * (1f - fadeAlpha) + fadeAlpha;
                        ApplyAlpha(key, newAlpha);
                    }
                    else if (progress >= 1 && group.isHiding)
                    {
                        deleteKeys.Add(key);
                    }
                }

                foreach (Vector3Int key in deleteKeys)
                {
                    tileGroups.Remove(key);
                }
                yield return new WaitForEndOfFrame();
            }

            coroutine = null;
        }
    }
}
