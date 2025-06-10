using System.Collections.Generic;
using Code.Scripts.Interfaces;
using Code.Scripts.States;
using Code.Scripts.StateSettings;
using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Obstacles
{
    public class SpringController : MonoBehaviour
    {
        [HeaderPlus("Spring:")]
        [SerializeField] private float force = 20f;

        [HeaderPlus("Animation:")]
        [SerializeField] private Animator animator;
        [SerializeField] private string activateTrigger = "Activate";

        [HeaderPlus("Draw:")]
        [SerializeField] private Color color = Color.green;
        [SerializeField] private SpringSettings springSettings;

        private readonly List<ISpringable> springables = new();

        private void OnDrawGizmosSelected()
        {
            /*
            Vector3 start = transform.position;
            Vector3 direction = transform.up * force;
            direction *= scaleLength;

            Gizmos.color = color;
            Gizmos.DrawLine(start, start + direction);
            Gizmos.DrawLine(start + direction, start + direction - direction.normalized * arrowSize + Vector3.Cross(direction.normalized, Vector3.forward) * arrowSize);
            Gizmos.DrawLine(start + direction, start + direction - direction.normalized * arrowSize - Vector3.Cross(direction.normalized, Vector3.forward) * arrowSize);
            */
            if (springSettings != null)
            {
                Vector3 direction = transform.up * force;
                float step = 0.01f;
                float fallTime = 0;
                Vector2 lastPos = transform.position;
                Vector2 lastForce = Vector2.zero;
                List<Vector3> curve = new List<Vector3> { transform.position };
                for (float i = 0; i < springSettings.springCurve.Duration; i += step)
                {
                    Vector2 force = SpringState<string>.CalculateSpring(direction, springSettings.springCurve, i, springSettings.fallSettings.fallCurve, ref fallTime, 0.001f);
                    /*
                    Vector2 force = springSettings.springCurve.SampleVelocity(i) * direction;
                    float fallInfluence = 1 - direction.normalized.y;
                    fallTime += step * fallInfluence;
                    float fallForce = fallInfluence * springSettings.fallSettings.fallCurve.SampleVelocity(fallTime);
                    force.y = force.y * (1 - fallInfluence) + fallForce;
                    */
                    Vector2 speed = (force + lastForce) * 0.5f;
                    Vector2 newPos = lastPos + speed * step;
                    curve.Add(newPos);
                    curve.Add(newPos);
                    lastPos = newPos;
                    lastForce = force;
                }
                curve.RemoveAt(curve.Count - 1);
                Gizmos.color = color;
                Gizmos.DrawLineList(curve.ToArray());
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out ISpringable springable)) return;
            Activate(springable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent(out ISpringable springable)) return;
            
            springables.Remove(springable);
        }

        private void Activate(ISpringable springable)
        {
            if (springables.Contains(springable)) return;

            springables.Add(springable);

            StartCoroutine(springable.Spring(new ISpringable.SpringDefinition(transform.position, (Vector2)transform.up * force)));

            animator.SetBool(activateTrigger, true);
        }
        
        private void EndAnimation()
        {
            animator.SetBool(activateTrigger, false);
        }

        public void PlaySound()
        {
            SfxController.PlaySpring(gameObject);
        }
    }
}