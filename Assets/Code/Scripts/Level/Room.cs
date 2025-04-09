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

        private static Transform Player { get; set; }
        public static Room ActiveRoom { get; private set; }

        private void OnDrawGizmosSelected()
        {
            CalculateCameraRange();
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
            
            Vector3 targetPos = Player.position;
            targetPos.z = Cam.transform.position.z;
            
            Cam.transform.position = Vector3.Lerp(Cam.transform.position, targetPos, Time.deltaTime * 10f);
            
            Vector2 minMovement = (Vector2)transform.position - moveRange;
            Vector2 maxMovement = (Vector2)transform.position + moveRange;
            
            Cam.transform.position = new Vector3(Mathf.Clamp(Cam.transform.position.x, minMovement.x, maxMovement.x),
                Mathf.Clamp(Cam.transform.position.y, minMovement.y, maxMovement.y), Cam.transform.position.z);
        }

        private void CalculateCameraRange()
        {
            if (Cam != null)
                camRange = (Cam.ViewportToWorldPoint(new Vector3(1, 1, Cam.nearClipPlane)) -
                            Cam.ViewportToWorldPoint(new Vector3(0, 0, Cam.nearClipPlane))) / 2f;
            
            camRange += moveRange;
            
            roomTrigger.size = camRange * 2f;
        }
    }
}
