using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private float maxDis = 20f;
        [SerializeField] private LayerMask mask;
        
        [SerializeField] private Transform origin;
        [SerializeField] private Transform end;
        [SerializeField] private LineRenderer line;
        
        private void Update()
        {
            line.SetPosition(0, origin.position);
            line.SetPosition(1, end.position);
            
            FindCollisionPoint();
        }
        
        private void FindCollisionPoint()
        {
            RaycastHit2D hit = Physics2D.Raycast(origin.position, -origin.up, maxDis, mask);

            if (!hit.collider)
            {
                end.position = origin.position - origin.up * maxDis;
                end.rotation = origin.rotation;
                return;
            }
            
            end.position = hit.point;
            end.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }
}
