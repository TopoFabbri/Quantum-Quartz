using Code.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class DroneTrigger : InteractableComponent
    {
        [SerializeField] private float speedMultiplier = 1;
        [SerializeField] private bool reusable = false;
        [SerializeField] private bool noCollisionMove = false;
        [SerializeField] private bool noCollisionAfter = false;
        [SerializeField] private GameObject enableAfter;
        [SerializeField] private Transform target;

        private void Awake()
        {
            foreach (SpriteRenderer sprite in transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = false;
            }
        }

        protected override void OnInteracted()
        {
            GameManager.Instance.Drone.GetComponent<Collider2D>().enabled = !noCollisionMove;
            GameManager.Instance.Drone.GoToPosition(target, speedMultiplier, OnPositionReached);
            if (!reusable)
            {
                transform.parent.gameObject.SetActive(false);
            }
        }

        private void OnPositionReached()
        {
            GameManager.Instance.Drone.GetComponent<Collider2D>().enabled = !noCollisionAfter;
            if (enableAfter)
            {
                enableAfter.SetActive(true);
            }
        }
    }
}
