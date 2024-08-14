using System;
using System.Collections;
using Code.Scripts.Level;
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
        [SerializeField] private LayerMask mask;

        [SerializeField] private Transform origin;
        [SerializeField] private Transform end;
        [SerializeField] private LineRenderer line;
        [SerializeField] private Animator animator;
        
        [SerializeField] private bool useTime;
        [SerializeField] private float timeOn;
        [SerializeField] private float timeOff;
        [SerializeField] private float timeOffset;

        private bool isOn;
        private static float _windupTime = 0.6f;

        private void Start()
        {
            if (!useTime)
            {
                animator.SetBool("On", true);
                isOn = true;
                return;
            }

            StartCoroutine(ToggleOnOff());
        }

        private void Update()
        {
            if (!isOn)
            {
                line.enabled = false;
                end.gameObject.SetActive(false);
                return;
            }

            line.enabled = true;
            end.gameObject.SetActive(true);

            line.SetPosition(0, origin.position + origin.right * 0.5f + origin.up * 0.03f);
            line.SetPosition(1, end.position + origin.up * 0.03f);

            FindCollisionPoint();
        }

        private void FindCollisionPoint()
        {
            RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.right, maxDis, mask);

            if (!hit.collider)
            {
                end.position = origin.position + origin.right * maxDis;
                end.rotation = origin.rotation;
                return;
            }

            end.position = hit.point;
            end.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (hit.collider.TryGetComponent(out IKillable killable))
                killable.Kill();
        }

        private IEnumerator ToggleOnOff()
        {
            yield return new WaitForSeconds(timeOffset);

            do
            {
                yield return new WaitForSeconds(timeOff - _windupTime);
                animator.SetBool("On", true);
                
                yield return new WaitForSeconds(_windupTime);
                TurnOn();

                yield return new WaitForSeconds(timeOn);
                animator.SetBool("On", false);
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