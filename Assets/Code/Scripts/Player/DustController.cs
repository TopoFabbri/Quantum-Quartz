using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Player
{
    /// <summary>
    /// Manage dust functioning
    /// </summary>
    public class DustController : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private int animCount;

        private void OnEnable()
        {
            anim.SetInteger("Anim", Random.Range(0, animCount));
        }

        /// <summary>
        /// Destroy dust
        /// </summary>
        private void Destroy()
        {
            Destroy(gameObject);
        }
    }
}