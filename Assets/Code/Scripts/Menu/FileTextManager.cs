using System;
using Code.Scripts.Game;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class FileTextManager : MonoBehaviour
    {
        [SerializeField] private Animator animator; 
        [SerializeField] private TextMeshProUGUI timerText; 
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private int saveSlot;
        
        private void OnEnable()
        {
            timerText.gameObject.SetActive(false);
            deathsText.gameObject.SetActive(false);
            Stats.LoadStats(saveSlot);
            Stats.LoadTexts(timerText,deathsText);
        }

        private void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.IsName("IdleHighLighted"))
            {
                timerText.gameObject.SetActive(true);
                deathsText.gameObject.SetActive(true);
            }
            else
            {
                timerText.gameObject.SetActive(false);
                deathsText.gameObject.SetActive(false);
            }
        }
    }
}
