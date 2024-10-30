using Code.Scripts.Tools;
using UnityEngine;

namespace Code.Scripts.Level
{
    public class SelectableCrystals : MonoBehaviourSingleton<SelectableCrystals>
    {
        [SerializeField] private bool blue = true;
        [SerializeField] private bool red = false;
        [SerializeField] private bool green = false;
        [SerializeField] private bool yellow = false;

        public static bool Blue => Instance.blue;
        public static bool Red => Instance.red;
        public static bool Green => Instance.green;
        public static bool Yellow => Instance.yellow;
    }
}
