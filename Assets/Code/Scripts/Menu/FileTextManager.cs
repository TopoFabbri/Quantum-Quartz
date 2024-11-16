using System;
using Code.Scripts.Game;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Menu
{
    public class FileTextManager : MonoBehaviour
    {
        [SerializeField] private Animator animator; 
        [SerializeField] private TextMeshProUGUI totalTimerText; 
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private TextMeshProUGUI collectiblesText;
        [SerializeField] private int saveSlot;
        
        private void OnEnable()
        {
            totalTimerText.gameObject.SetActive(false);
            deathsText.gameObject.SetActive(false);
            collectiblesText.gameObject.SetActive(false);
            Stats.LoadStats(saveSlot);
            Stats.LoadTexts(totalTimerText,deathsText);
        }

        private void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName("IdleHighLighted") || stateInfo.IsName("Pressed"))
            {
                totalTimerText.gameObject.SetActive(true);
                deathsText.gameObject.SetActive(true);
                collectiblesText.gameObject.SetActive(true);
            }
            else
            {
                totalTimerText.gameObject.SetActive(false);
                deathsText.gameObject.SetActive(false);
                collectiblesText.gameObject.SetActive(false);
            }
        }
    }
}
