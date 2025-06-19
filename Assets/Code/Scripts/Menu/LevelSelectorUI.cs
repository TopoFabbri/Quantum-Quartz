using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private LevelList levelList;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    void Start()
    {
        foreach (var level in levelList.levels)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            var buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

            string sceneName = level.SceneName;

            if (buttonText != null)
                buttonText.text = sceneName;

            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}