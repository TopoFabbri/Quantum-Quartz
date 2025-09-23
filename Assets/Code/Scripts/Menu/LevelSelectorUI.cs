using Code.Scripts.Game.Triggers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class LevelSelectorUI : MonoBehaviour
    {
        [SerializeField] private LevelList levelList;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Button backButton;

        private List<Button> generatedButtons = new List<Button>();

        void Start()
        {
            foreach (LevelList.LevelData level in levelList.levels)
            {
                GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);
                Button newButton = newButtonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = level.SceneName;
                }

                newButton.onClick.AddListener(() => LevelChanger.Instance.LoadLevel(level));

                generatedButtons.Add(newButton);

                if (generatedButtons.Count == 1)
                {
                    newButton.Select();
                }
            }

            ConfigureNavigation();
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
                var backNav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = generatedButtons[generatedButtons.Count - 1]
                };

                backButton.navigation = backNav;
            }
        }
    }
}
