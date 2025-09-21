using Code.Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class GauntletPurchasePopup : MonoBehaviour
    {
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private GauntletsList.GauntletData pendingGauntlet;
        private GauntletSelectorUI selector;
        private Button previousSelected;

        private void Awake()
        {
            popupPanel.SetActive(false);
            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);
        }

        public void Open(GauntletsList.GauntletData gauntlet, GauntletSelectorUI selectorRef, Button sourceButton)
        {
            pendingGauntlet = gauntlet;
            selector = selectorRef;
            previousSelected = sourceButton;

            popupPanel.SetActive(true);
            confirmButton.Select();
        }

        private void OnConfirm()
        {
            int price = pendingGauntlet.costInKeys;

            if (Stats.SpendCollectibles(price))
            {
                pendingGauntlet.isUnlocked = true;

                int previousIndex = -1;
                if (previousSelected != null && previousSelected.transform.parent == selector.transform)
                {
                    previousIndex = previousSelected.transform.GetSiblingIndex();
                }

                popupPanel.SetActive(false);
                selector.RefreshUI();

                var newButtons = selector.GetButtons();
                if (newButtons.Count > 0)
                {
                    int indexToSelect = Mathf.Clamp(previousIndex, 0, newButtons.Count - 1);
                    newButtons[indexToSelect].Select();
                }
            }
            else
            {
                Debug.Log("Not enough collectibles!");
            }
        }



        private void OnCancel()
        {
            popupPanel.SetActive(false);
            RestoreFocus();
        }

        private void RestoreFocus()
        {
            if (previousSelected != null)
                previousSelected.Select();
        }
    }
}