using Code.Scripts.Game;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class MenuMusicController : MonoBehaviour
    {

        [SerializeField] private WwiseEvent musicEvent;

        private void Start()
        {
            musicEvent.SetOn(gameObject);
        }

        private void OnDestroy()
        {
            musicEvent.SetOff(gameObject);
        }
    }
}
