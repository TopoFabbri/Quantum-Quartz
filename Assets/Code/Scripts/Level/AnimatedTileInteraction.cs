using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class AnimatedTileInteraction : MonoBehaviour
{
    [SerializeField] private List<Collider2D> interactorList;
    
    private Tilemap tilemap;
    private Dictionary<Collider2D, (Vector3Int, Vector3Int)> interactorBounds = new Dictionary<Collider2D, (Vector3Int, Vector3Int)>();

    private void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    private void OnEnable()
    {
        if (interactorList.Count == 0)
        {
            this.enabled = false;
        }
        else
        {
            Vector3Int invalid = new Vector3Int(int.MaxValue, int.MaxValue);
            foreach (Collider2D interactor in interactorList)
            {
                interactorBounds[interactor] = (invalid, invalid);
            }
        }
    }

    void Update()
    {
        Vector3Int tilePos = Vector3Int.zero;

        foreach (Collider2D interactor in interactorList)
        {
            (Vector3Int, Vector3Int) prevTileBounds = interactorBounds[interactor];
            Vector3Int minTilePos = tilemap.WorldToCell(interactor.bounds.min);
            Vector3Int maxTilePos = tilemap.WorldToCell(interactor.bounds.max);

            for (int x = minTilePos.x; x <= maxTilePos.x; x++)
            {
                tilePos.x = x;
                for (int y = minTilePos.y; y <= maxTilePos.y; y++)
                {
                    tilePos.y = y;

                    tilemap.SetTileAnimationFlags(tilePos, TileAnimationFlags.LoopOnce);

                    if (tilePos.x > prevTileBounds.Item2.x || tilePos.x < prevTileBounds.Item1.x || tilePos.y > prevTileBounds.Item2.y || tilePos.y < prevTileBounds.Item1.y)
                    {
                        tilemap.SetAnimationTime(tilePos, 0);
                    }
                }
            }

            interactorBounds[interactor] = (minTilePos, maxTilePos);
        }
    }
}
