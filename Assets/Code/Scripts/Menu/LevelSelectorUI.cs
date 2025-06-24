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
                selectOnDown = (i < generatedButtons.Count - 1) ? generatedButtons[i + 1] : null
            };

            generatedButtons[i].navigation = nav;
        }
    }
}