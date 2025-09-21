using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.Menu
{
    public class MenuButtonsHighlighter : MonoBehaviour
    {
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private GameObject lastSelected;

        void Update()
        {
            GameObject current = EventSystem.current?.currentSelectedGameObject;

            if (current != lastSelected)
            {
                if (lastSelected != null)
                {
                    TextMeshProUGUI lastText = lastSelected.GetComponentInChildren<TextMeshProUGUI>();
                    if (lastText != null)
                        lastText.color = normalColor;
                }

                if (current != null)
                {
                    TextMeshProUGUI currentText = current.GetComponentInChildren<TextMeshProUGUI>();
                    if (currentText != null)
                        currentText.color = selectedColor;
                }

                lastSelected = current;
            }
        }
    }
}
