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

        [SerializeField] private bool useTime;
        [SerializeField] private float timeOn;
        [SerializeField] private float timeOff;
        [SerializeField] private float timeOffset;

        private bool isOn;

        private void Start()
        {
            if (!useTime)
            {
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

            if (hit.collider.TryGetComponent(out IKillable killable))
                killable.Kill();
        }

        private IEnumerator ToggleOnOff()
        {
            yield return new WaitForSeconds(timeOffset);

            do
            {
                yield return new WaitForSeconds(timeOff);
                isOn = true;

                yield return new WaitForSeconds(timeOn);
                isOn = false;
            } while (enabled);
        }
    }
}