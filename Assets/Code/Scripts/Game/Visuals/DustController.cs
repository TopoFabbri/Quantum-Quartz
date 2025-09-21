using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Scripts.Game.Visuals
{
    /// <summary>
    /// Manage dust functioning
    /// </summary>
    public class DustController : MonoBehaviour
    {
        private static int _lastAnim;
        private static readonly int Anim = Animator.StringToHash("Anim");
        
        [SerializeField] private Animator anim;
        [SerializeField] private int animCount;

        private void OnEnable()
        {
            int animNum;

            do
            {
                animNum = Random.Range(0, animCount);
            } while (animNum == _lastAnim);

            anim.SetInteger(Anim, animNum);
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