using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GauntletPurchasePopup : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private GauntletsList.GauntletData pendingGauntlet;
    private GauntletSelectorUI selector;

    private void Awake()
    {
        popupPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void Open(GauntletsList.GauntletData gauntlet, GauntletSelectorUI selectorRef)
    {
        pendingGauntlet = gauntlet;
        selector = selectorRef;

        messageText.text = $"Comprar {gauntlet.gauntletName} por {gauntlet.costInKeys} 🔑 ?";
        popupPanel.SetActive(true);
        confirmButton.Select();
    }

    private void OnConfirm()
    {
        //Restar llaves del jugador si tiene las necesarias

        popupPanel.SetActive(false);
    }

    private void OnCancel()
    {
        popupPanel.SetActive(false);
    }
}