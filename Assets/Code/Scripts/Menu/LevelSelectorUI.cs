using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private LevelList levelList;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Button backButton; // Botón de Back asignable desde el inspector

    private List<Button> generatedButtons = new List<Button>();

    void Start()
    {
        foreach (var level in levelList.levels)
        {
            GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);
            Button newButton = newButtonObj.GetComponent<Button>();
            var buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

            string sceneName = level.SceneName;

            if (buttonText != null)
                buttonText.text = sceneName;

            newButton.onClick.AddListener(() => SceneManager.LoadScene(sceneName));

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
