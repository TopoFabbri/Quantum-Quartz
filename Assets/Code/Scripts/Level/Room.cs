using UnityEngine;

namespace Code.Scripts.Level
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D roomTrigger;
        [SerializeField] private Vector2 moveRange;
        
        private Vector2 camRange;
        
        private static UnityEngine.Camera _cam;

        private static UnityEngine.Camera Cam
        {
            get
            {
                if (!_cam)
                    _cam = UnityEngine.Camera.main;
                
                return _cam;
            }
        }

        public Vector2 MoveRange => moveRange;
        
        private static Transform Player { get; set; }
        public static Room ActiveRoom { get; private set; }

        private void OnDrawGizmosSelected()
        {
            CalculateCameraRange();
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, camRange * 2f);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;
            
            if (!Player)
                Player = other.transform;
            
            ActiveRoom = this;
        }
        
        private void Update()
        {
            if (ActiveRoom != this)
                return;
            
            CalculateCameraRange();
        }

        private void CalculateCameraRange()
        {
            if (Cam != null)
                camRange = (Cam.ViewportToWorldPoint(new Vector3(1, 1, Cam.nearClipPlane)) -
                            Cam.ViewportToWorldPoint(new Vector3(0, 0, Cam.nearClipPlane))) / 2f;
            
            camRange += moveRange;
            
            roomTrigger.size = camRange * 2f - Vector2.one;
        }
    }
}
