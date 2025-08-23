using System;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Player
{
    public class WallStateColliders : MonoBehaviour
    {
        [HeaderPlus("Settings")] [SerializeField]
        private float width = .5f;

        [HeaderPlus("Variables")] [SerializeField]
        private float top = 1f;
        [SerializeField] private float upMid = 1f / 1.5f;
        [SerializeField] private float midLow = 1f / 3f;
        [SerializeField] private float low = 0f;

        [HeaderPlus("Debugging")] [SerializeField]
        private bool draw;
        [SerializeField] private Color drawColour = Color.green;
        [SerializeField] private Vector2 drawOffset = Vector2.zero;
        

        private BoxCollider2D upCollider;
        private BoxCollider2D midCollider;
        private BoxCollider2D lowCollider;

        [HeaderPlus("Generation")]
        [InspectorButton("Generate Wall State Colliders")]
        public void GenerateColliders()
        {
            if (upCollider)
                DestroyImmediate(upCollider);
            
            if (midCollider)
                DestroyImmediate(midCollider);
            
            if (lowCollider)
                DestroyImmediate(lowCollider);
            
            upCollider = CreateCollider(top, upMid);
            midCollider = CreateCollider(upMid, midLow);
            lowCollider = CreateCollider(midLow, low);
        }

        private BoxCollider2D CreateCollider(float upper, float lower)
        {
            // The offset should be the center of the box.
            Vector2 offset = new(0, (upper + lower) / 2f);
            Vector2 size = new(width, upper - lower);
            
            BoxCollider2D tmpCollider = gameObject.AddComponent<BoxCollider2D>();
            
            tmpCollider.offset = offset;
            tmpCollider.size = size;

            return tmpCollider;
        }

        private void OnDrawGizmosSelected()
        {
            if (!draw) return;

            DrawBox(top, upMid);
            DrawBox(upMid, midLow);
            DrawBox(midLow, low);
        }

        private void DrawBox(float upper, float lower)
        {
            Vector2 position = (Vector2)transform.position + drawOffset;
            Gizmos.color = drawColour;
            Vector2 center = position + new Vector2(0, (upper + lower) / 2f);
            Vector2 size = new(width, upper - lower);
            Gizmos.DrawWireCube(center, size);
        }
    }
}