using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class DroneTrigger : InteractableComponent
    {
        [SerializeField] private Transform target;

        private void Start()
        {
            foreach (SpriteRenderer sprite in transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = false;
            }
        }

        protected override void OnInteracted()
        {
            GameManager.Instance.Drone.GoToPosition(target);
        }
    }
}
