using Code.Scripts.Game.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Game.Triggers
{
    public class Checkpoint : InteractableComponent
    {
        [Header("References")]
        [SerializeField] private BoxCollider2D trigger;
        [SerializeField] private Transform spawnPoint;
        
        [Header("Visuals")]
        [SerializeField] private List<SpriteRenderer> spritesToHide;

        private void Start()
        {
            foreach (SpriteRenderer sprite in spritesToHide)
                sprite.enabled = false;
        }

        protected override void OnInteracted()
        {
            GameManager.Instance.Player.SaveCheckpoint(spawnPoint.position);
        }
    }
}
