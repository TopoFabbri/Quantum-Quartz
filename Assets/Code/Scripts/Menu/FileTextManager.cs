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
        [SerializeField] private TextMeshProUGUI level1TimerText; 
        [SerializeField] private TextMeshProUGUI level2TimerText; 
        [SerializeField] private TextMeshProUGUI level3TimerText; 
        [SerializeField] private TextMeshProUGUI level4TimerText; 
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private int saveSlot;
        
        private void OnEnable()
        {
            totalTimerText.gameObject.SetActive(false);
            deathsText.gameObject.SetActive(false);
            Stats.LoadStats(saveSlot);
            Stats.LoadTexts(totalTimerText,level1TimerText, level2TimerText, level3TimerText, level4TimerText,deathsText);
        }

        private void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName("IdleHighLighted"))
            {
                totalTimerText.gameObject.SetActive(true);
                level1TimerText.gameObject.SetActive(true);
                level2TimerText.gameObject.SetActive(true);
                level3TimerText.gameObject.SetActive(true);
                level4TimerText.gameObject.SetActive(true);
                deathsText.gameObject.SetActive(true);
            }
            else
            {
                totalTimerText.gameObject.SetActive(false);
                level1TimerText.gameObject.SetActive(false);
                level2TimerText.gameObject.SetActive(false);
                level3TimerText.gameObject.SetActive(false);
                level4TimerText.gameObject.SetActive(false);
                deathsText.gameObject.SetActive(false);
            }
        }
    }
}
