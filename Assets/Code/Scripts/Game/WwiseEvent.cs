using UnityEngine;
using Event = AK.Wwise.Event;

namespace Code.Scripts.Game
{
    [CreateAssetMenu(menuName = "Sound/WwiseEvent", fileName = "WwiseEvent", order = 0)]
    public class WwiseEvent : ScriptableObject
    {
        [SerializeField] private Event start;
        [SerializeField] private Event stop;
        public void SetOn(GameObject gameObject)
        {
            start.Post(gameObject);
        }

        public void SetOff(GameObject gameObject)
        {
            if (stop.Id != 0)
            {
                stop.Post(gameObject);
            }
            else
            {
                start.Stop(gameObject);
            }
        }
    }
}
