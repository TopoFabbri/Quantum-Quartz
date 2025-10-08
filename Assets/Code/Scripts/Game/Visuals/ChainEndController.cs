using Code.Scripts.Game.Managers;
using UnityEngine;

namespace Code.Scripts.Game.Visuals
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChainEndController : InteractableComponent
    {
        [SerializeField] private float minForce = 2f;
        [SerializeField] private float maxForce = 3f;
        [SerializeField] private WwiseEvent hitChainsEvent;
        
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void OnInteracted()
        {
            hitChainsEvent.SetOn(gameObject);
            rb.AddForce(GameManager.Instance.Player.GetSpeed() * Random.Range(minForce, maxForce), ForceMode2D.Impulse);
        }
    }
}