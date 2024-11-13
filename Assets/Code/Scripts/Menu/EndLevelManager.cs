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
        [SerializeField] private TextMeshProUGUI collectiblesTxt;
        [SerializeField] private TextMeshProUGUI deathsTxt;

        private void OnEnable()
        {
            timeTxt.text = Stats.Time.ToStr;
            deathsTxt.text = Stats.GetDeaths().ToString();
        }
    }
}
