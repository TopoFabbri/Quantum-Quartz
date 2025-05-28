using System.Collections;
using Code.Scripts.Interfaces;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Obstacles
{
    /// <summary>
    /// Manage laser actions
    /// </summary>
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private float maxDis = 20f;
        [SerializeField] private float width = .8f;
        [SerializeField] private LayerMask mask;

        [SerializeField] private Transform origin;
        [SerializeField] private Transform end;
        [SerializeField] private LineRenderer line;
        [SerializeField] private Animator animator;

        [SerializeField] private bool useTime;
        [SerializeField] private float timeOn;
        [SerializeField] private float timeOff;
        [SerializeField] private float timeOffset;
        
        private const float WindupTime = 0.6f;
        
        private bool isOn;
        private static readonly int AnimatorOn = Animator.StringToHash("On");

        private void Start()
        {
            if (!useTime)
            {
                animator.SetBool(AnimatorOn, true);
                isOn = true;
                return;
            }

            StartCoroutine(ToggleOnOff());
        }

        private void LateUpdate()
        {
            if (!isOn)
            {
                line.enabled = false;
                end.gameObject.SetActive(false);
                return;
            }

            line.enabled = true;
            end.gameObject.SetActive(true);

            FindCollisionPoint();

            line.SetPosition(0, origin.position + origin.right * 0.5f);
            line.SetPosition(1, end.position);
        }

        private void FindCollisionPoint()
        {
            RaycastHit2D hit = Physics2D.BoxCast(origin.position, new Vector2(width, 0.1f), 0, origin.right, maxDis, mask);
            //RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.right, maxDis, mask);

            float dis = hit ? hit.distance : maxDis;
            
            if (!hit.collider)
            {
                end.position = origin.position + origin.right * dis;
                end.rotation = origin.rotation;
                return;
            }

            end.position = origin.position + origin.right * Vector2.Dot(hit.point - new Vector2(origin.position.x, origin.position.y), origin.right);
            end.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.collider.TryGetComponent(out IKillable killable))
                killable.Kill();

            hit = Physics2D.Raycast(origin.position + origin.up * width / 2f, origin.right, dis, mask);

            if (hit)
            {
                if (hit.collider.TryGetComponent(out killable))
                    killable.Kill();
            }

            hit = Physics2D.Raycast(origin.position - origin.up * width / 2f, origin.right, dis, mask);

            if (!hit)
                return;

            if (hit.collider.TryGetComponent(out killable))
                killable.Kill();
        }

        private IEnumerator ToggleOnOff()
        {
            yield return new WaitForSeconds(timeOffset);

            do
            {
                yield return new WaitForSeconds(timeOff - WindupTime);
                animator.SetBool(AnimatorOn, true);

                yield return new WaitForSeconds(WindupTime);
                TurnOn();

                yield return new WaitForSeconds(timeOn);
                animator.SetBool(AnimatorOn, false);
                TurnOff();
            } while (enabled);
        }

        private void TurnOn()
        {
            isOn = true;
        }

        private void TurnOff()
        {
            isOn = false;
        }
    }
}