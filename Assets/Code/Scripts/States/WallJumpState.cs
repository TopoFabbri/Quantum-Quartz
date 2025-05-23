using System.Collections;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    public class WallJumpState<T> : JumpState<T>
    {
        protected readonly WjmpSettings wjmpSettings;
        
        public bool FacingRight { get; set; }

        private bool canMove;
        
        public WallJumpState(T id, WjmpSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb) : base(id, stateSettings.jumpSettings, rb, transform, mb)
        {
            this.wjmpSettings = stateSettings;
        }

        public override void OnEnter()
        {
            rb.velocity = Vector2.zero;
            
            base.OnEnter();
            
            rb.velocity = new Vector2(FacingRight ? wjmpSettings.wallJumpForce : -wjmpSettings.wallJumpForce, rb.velocity.y);
            
            canMove = false;
            mb.StartCoroutine(WaitAndReturnInput(wjmpSettings.noInputTime));
        }

        public override void OnExit()
        {
            base.OnExit();
            
            canMove = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!canMove)
                Input = 0f;
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
            
            RaycastHit2D hit = Physics2D.Raycast(position, direction, moveSettings.wallCheckDis, LayerMask.GetMask("Default"));
            
            Debug.DrawLine(position, position + direction * moveSettings.wallCheckDis, Color.red, 0.1f);
            
            if (hit.collider == null)
                return;
            
            if (!hit.collider.CompareTag("Floor") && !hit.collider.CompareTag("Platform"))
                return;
            
            Transform parent = hit.collider.transform;
            
            Quaternion rotation = Quaternion.Euler(0f, 0f, FacingRight ? -90f : 90f);
            
            Object.Instantiate(jumpSettings.dust, hit.point, rotation, parent);
        }
    }
}