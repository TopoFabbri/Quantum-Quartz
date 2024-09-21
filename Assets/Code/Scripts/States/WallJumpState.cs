using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallJumpState<T> : JumpState<T>
    {
        protected WjmpSettings WjmpSettings => settings as WjmpSettings;
        
        public bool FacingRight { get; set; }

        private bool canMove;
        
        public WallJumpState(T id, StateSettings.StateSettings stateSettings, MonoBehaviour mb, Rigidbody2D rb, Transform transform) : base(id, stateSettings, mb, rb, transform)
        {
        }

        public override void OnEnter()
        {
            rb.velocity = Vector2.zero;
            
            base.OnEnter();
            
            rb.velocity = new Vector2(FacingRight ? WjmpSettings.wallJumpForce : -WjmpSettings.wallJumpForce, rb.velocity.y);
            
            canMove = false;
            mb.StartCoroutine(WaitAndReturnInput(WjmpSettings.noInputTime));
        }

        public override void OnExit()
        {
            base.OnExit();
            
            canMove = false;
        }

        public override void OnUpdate()
        {
            if (!canMove)
                Input = 0f;
            
            base.OnUpdate();
        }

        /// <summary>
        /// Stop input for given time
        /// </summary>
        /// <param name="noInputTime">Time to wait</param>
        /// <returns></returns>
        private IEnumerator WaitAndReturnInput(float noInputTime)
        {
            yield return new WaitForSeconds(noInputTime);
            canMove = true;
        }

        public override void SpawnDust()
        {
            Vector2 position = transform.position;
            Vector2 direction = FacingRight ? Vector2.left : Vector2.right;
            
            RaycastHit2D hit = Physics2D.Raycast(position, direction, WjmpSettings.wallCheckDis, LayerMask.GetMask("Default"));
            
            Debug.DrawLine(position, position + direction * WjmpSettings.wallCheckDis, Color.red, 0.1f);
            
            if (hit.collider == null)
                return;
            
            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;
            
            Transform parent = hit.collider.transform;
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, FacingRight ? -90f : 90f);
            
            Object.Instantiate(WjmpSettings.dust, hit.point, rotation, parent);
        }
    }
}