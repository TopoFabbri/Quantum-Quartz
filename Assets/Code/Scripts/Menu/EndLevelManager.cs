using Code.Scripts.Game;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Menu
{
    /// <summary>
    /// Manage end level menu actions
    /// </summary>
    public class EndLevelManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeTxt;

        private void OnEnable()
        {
            timeTxt.text = Stats.Time.ToStr;
        }
    }
}
