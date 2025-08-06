using Code.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class SubstanceTrigger : MonoBehaviour
    {
        [SerializeField] private MovementModifier modifier;

        public MovementModifier Modifier => modifier;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player") || !other.TryGetComponent(out PlayerController player))
                return;

            player.EnterSubstance(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.isTrigger || !other.CompareTag("Player") || !other.TryGetComponent(out PlayerController player))
                return;

            player.LeaveSubstance(this);
        }
    }
}
