using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.Menu
{
    public class ButtonSelector : MonoBehaviour
    {
        [SerializeField] private Button defaultButton;

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            }
        }
    }
}
