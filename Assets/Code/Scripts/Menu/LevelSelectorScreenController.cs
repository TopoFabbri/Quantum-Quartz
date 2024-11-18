using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class LevelSelectorScreenController : MonoBehaviour
    {
        public RectTransform contentPanel;
        public float slideDuration = 0.5f;

        private RectTransform selectedButton;
        private Vector2 targetPosition;
        private bool isSliding = false;

        void Update()
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected != null && currentSelected.transform.parent == contentPanel)
            {
                RectTransform newSelectedButton = currentSelected.GetComponent<RectTransform>();

                if (newSelectedButton != selectedButton)
                {
                    selectedButton = newSelectedButton;
                    UpdateTargetPosition();
                }
            }

            if (isSliding)
            {
                contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition,
                    Time.deltaTime / slideDuration);

                if (Vector2.Distance(contentPanel.anchoredPosition, targetPosition) < 0.1f)
                {
                    contentPanel.anchoredPosition = targetPosition;
                    isSliding = false;
                }
            }
        }

        private void UpdateTargetPosition()
        {
            float contentWidth = contentPanel.rect.width;
            float buttonWidth = selectedButton.rect.width;
            float buttonSpacing = GetSpacing();
            float paddingLeft = GetPaddingLeft();

            float offset = (contentWidth / 2f) - (buttonWidth / 2f);
            float buttonCenterX = -selectedButton.anchoredPosition.x + offset + paddingLeft + buttonSpacing;

            targetPosition = new Vector2(buttonCenterX, contentPanel.anchoredPosition.y);

            isSliding = true;
        }

        private float GetSpacing()
        {
            HorizontalLayoutGroup layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
            return layoutGroup != null ? layoutGroup.spacing : 0f;
        }

        private float GetPaddingLeft()
        {
            HorizontalLayoutGroup layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
            return layoutGroup != null ? layoutGroup.padding.left : 0f;
        }
    }
}