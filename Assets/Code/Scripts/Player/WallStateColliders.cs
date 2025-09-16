using Code.Scripts.Tools;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class WallStateColliders : MonoBehaviour
    {
        [HeaderPlus("Settings")] [SerializeField]
        private float width = .5f;

        [HeaderPlus("Variables")]
        [SerializeField] private float topPosition = 1.5f;
        [SerializeField] private float topSize = .6f;
        [SerializeField] private float midPosition = 1f;
        [SerializeField] private float midSize = .6f;
        [SerializeField] private float lowPos = .5f;
        [SerializeField] private float lowSize = .6f;
        
        [HeaderPlus("Debugging")] [SerializeField]
        private bool draw;
        [SerializeField] private Color drawColour = Color.green;
        [SerializeField] private Vector2 drawOffset = Vector2.zero;
        
        [field: SerializeField] public BoxCollider2D UpCollider { get; private set; }
        [field: SerializeField] public BoxCollider2D MidCollider { get; private set; }
        [field: SerializeField] public BoxCollider2D LowCollider { get; private set; }

#if UNITY_EDITOR
        [HeaderPlus("Generation")]
        [InspectorButton("Generate Wall State Colliders")]
        public void GenerateColliders()
        {
            if (UpCollider)
                DestroyImmediate(UpCollider);
            
            if (MidCollider)
                DestroyImmediate(MidCollider);
            
            if (LowCollider)
                DestroyImmediate(LowCollider);
            
            UpCollider = CreateCollider(topPosition, topSize);
            MidCollider = CreateCollider(midPosition, midSize);
            LowCollider = CreateCollider(lowPos, lowSize);
            EditorUtility.SetDirty(gameObject);
        }
#endif

        private BoxCollider2D CreateCollider(float pos, float size)
        {
            // The offset should be the center of the box.
            Vector2 offset = new(0, pos);
            Vector2 colliderSize = new(width, size);
            
            BoxCollider2D tmpCollider = gameObject.AddComponent<BoxCollider2D>();
            
            tmpCollider.offset = offset;
            tmpCollider.size = colliderSize;
            tmpCollider.enabled = false;

            return tmpCollider;
        }

        private void OnDrawGizmosSelected()
        {
            if (!draw) return;

            DrawBox(topPosition, topSize);
            DrawBox(midPosition, midSize);
            DrawBox(lowPos, lowSize);
        }

        private void DrawBox(float position, float size)
        {
            Vector2 drawPosition = (Vector2)transform.position + drawOffset + Vector2.up * position;
            Vector2 drawSize = new(width, size);
            Gizmos.color = drawColour;
            
            Gizmos.DrawWireCube(drawPosition, drawSize);
        }
    }
}