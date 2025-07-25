using Code.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class Checkpoint : MonoBehaviour
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController playerController))
            {
                playerController.SaveCheckpoint(spawnPoint.position);
            }
        }
    }
}
