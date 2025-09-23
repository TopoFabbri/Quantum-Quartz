using System.Collections.Generic;
using Code.Scripts.Game.Managers;
using Code.Scripts.Game.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class GauntletSelectorUI : MonoBehaviour
    {
        [SerializeField] private GauntletsList gauntletList;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI keysText;

        [Header("Popup Confirmation")]
        [SerializeField] private GauntletPurchasePopup gauntletPurchasePopup;

        private List<Button> generatedButtons = new List<Button>();

        private void Start()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            foreach (Transform child in buttonContainer) Destroy(child.gameObject);
            generatedButtons.Clear();

            foreach (GauntletsList.GauntletData gauntlet in gauntletList.gauntlets)
            {
                GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);
                Button newButton = newButtonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = gauntlet.gauntletName + (gauntlet.isUnlocked ? "" : $": {gauntlet.costInKeys}");
                }

                if (gauntlet.isUnlocked)
                {
                    // Ya comprado -> carga escena
                    newButton.onClick.AddListener(() => SceneManager.LoadScene(gauntlet.sceneReference.BuildIndex));
                }
                else
                {
                    // No comprado -> abre popup
                    newButton.onClick.AddListener(() => gauntletPurchasePopup.Open(gauntlet, this, newButton));
                }

                generatedButtons.Add(newButton);
            }

            ConfigureNavigation();

            // Seleccionar primer botón por defecto
            if (generatedButtons.Count > 0)
                generatedButtons[0].Select();

            RefreshKeysUI();
        }

        private void ConfigureNavigation()
        {
            for (int i = 0; i < generatedButtons.Count; i++)
            {
                var nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = (i > 0) ? generatedButtons[i - 1] : null,
                    selectOnDown = (i < generatedButtons.Count - 1) ? generatedButtons[i + 1] : null,
                    selectOnRight = backButton != null ? backButton : null
                };

                generatedButtons[i].navigation = nav;
            }

            if (backButton != null && generatedButtons.Count > 0)
            {
                var lastButton = generatedButtons[generatedButtons.Count - 1];
                var lastNav = lastButton.navigation;
                lastNav.selectOnDown = backButton;
                lastButton.navigation = lastNav;

                var backNav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = lastButton,
                    selectOnUp = lastButton
                };
                backButton.navigation = backNav;
            }
        }

        public List<Button> GetButtons()
        {
            return generatedButtons;
        }

        public void RefreshKeysUI()
        {
            if (keysText != null)
            {
                keysText.text = $"$ {Stats.GetCollectiblesCount(LevelChanger.Instance.LevelList)}";
            }
        }
    }
}
