using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Code.Scripts.Game;
using Code.Scripts.Level;
using UnityEngine.SceneManagement;

public class GauntletSelectorUI : MonoBehaviour
{
    [SerializeField] private GauntletsList gauntletList;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Button backButton;

    [Header("Popup Confirmation")]
    [SerializeField] private GameObject confirmPopup;
    [SerializeField] private GauntletPurchasePopup gauntletPurchasePopup;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private List<Button> generatedButtons = new List<Button>();
    private GauntletsList.GauntletData gauntletToBuy;

    void Start()
    {
        GenerateButtons();
        ConfigureNavigation();
        SetupPopup();
    }

    private void GenerateButtons()
    {
        foreach (GauntletsList.GauntletData gauntlet in gauntletList.gauntlets)
        {
            GameObject newButtonObj = Instantiate(buttonPrefab, buttonContainer);
            Button newButton = newButtonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = gauntlet.gauntletName + 
                    (gauntlet.isUnlocked ? "" : $": {gauntlet.costInKeys}");
            }

            if (gauntlet.isUnlocked)
            {
                // Ya comprado
                newButton.onClick.AddListener(() => SceneManager.LoadScene(gauntlet.sceneReference.BuildIndex));
            }
            else
            {
                // No comprado
                newButton.onClick.AddListener(() => TryPurchase(gauntlet));
                newButton.interactable = true; 
            }

            generatedButtons.Add(newButton);
        }
    }

    private void TryPurchase(GauntletsList.GauntletData gauntlet)
    {
        gauntletToBuy = gauntlet;
        gauntletPurchasePopup.Open(gauntletToBuy, this);
    }

    private void SetupPopup()
    {
        confirmPopup.SetActive(false);

        confirmYesButton.onClick.AddListener(() =>
        {
            if (gauntletToBuy != null)
            {
                // int playerKeys = Stats.CurrentKeys; // 🔑 aquí usás tu sistema real
                // if (playerKeys >= gauntletToBuy.costInKeys)
                // {
                //     Stats.CurrentKeys -= gauntletToBuy.costInKeys;
                //     gauntletToBuy.isUnlocked = true;
                
                    // Refrescar UI
                    foreach (Transform child in buttonContainer) Destroy(child.gameObject);
                    generatedButtons.Clear();
                    GenerateButtons();
                    ConfigureNavigation();
              
            }
            confirmPopup.SetActive(false);
        });

        confirmNoButton.onClick.AddListener(() =>
        {
            confirmPopup.SetActive(false);
        });
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
