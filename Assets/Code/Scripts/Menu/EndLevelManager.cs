using Code.Scripts.Game;
using Code.Scripts.Game.Managers;
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
        [SerializeField] private LevelList levelList;
        
        private void OnEnable()
        {
            timeTxt.text = TimeCounter.Time.ToStr;
            deathsTxt.text = "Deaths: " + Stats.GetDeaths();
            collectiblesTxt.text = "Keys: " + Stats.GetCollectiblesCount(levelList);
        }
    }
}
