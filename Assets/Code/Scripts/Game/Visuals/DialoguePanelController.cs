using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Game.Visuals
{
    public class DialoguePanelController : MonoBehaviour
    {
        [SerializeField] private float portraitOffset = 352;
        [SerializeField] private Animator dialoguePanelAnim;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button dialogueButton;
        [SerializeField] private RectTransform textBox;

        private Rect defaultTextBoxRect;
        private bool panelVisible = false;

        private void Awake()
        {
            defaultTextBoxRect = new Rect();
            defaultTextBoxRect.min = textBox.offsetMin;
            defaultTextBoxRect.max = textBox.offsetMax;
            dialogueText.text = "";
            dialoguePanelAnim.SetInteger("Portrait", (int)Conversation.PortraitAnimation.None);
        }

        public IEnumerator Show()
        {
            dialogueText.text = "";
            dialoguePanelAnim.gameObject.SetActive(true);
            dialoguePanelAnim.SetInteger("Portrait", (int)Conversation.PortraitAnimation.None);
            dialoguePanelAnim.SetBool("ShowPanel", true);
            dialogueButton.Select();
            dialoguePanelAnim.speed = 1;
            yield return new WaitUntil(() => panelVisible);
        }

        public void OnShown()
        {
            panelVisible = true;
        }

        public IEnumerator Hide()
        {
            dialogueText.text = "";
            dialoguePanelAnim.SetInteger("Portrait", (int)Conversation.PortraitAnimation.None);
            dialoguePanelAnim.SetBool("ShowPanel", false);
            dialoguePanelAnim.speed = 1;
            yield return new WaitUntil(() => !panelVisible);
            dialoguePanelAnim.gameObject.SetActive(false);
        }

        public void OnHidden()
        {
            panelVisible = false;
        }

        public void SetPortrait(Conversation.PortraitAnimation animation, Conversation.PortraitAlignment alignment)
        {
            dialoguePanelAnim.speed = 1;
            textBox.offsetMin = defaultTextBoxRect.min + Vector2.right * (alignment.HasFlag(Conversation.PortraitAlignment.Left) ? portraitOffset : 0);
            textBox.offsetMax = defaultTextBoxRect.max + Vector2.left * (alignment.HasFlag(Conversation.PortraitAlignment.Right) ? portraitOffset : 0);
            dialoguePanelAnim.SetInteger("Portrait", (int)animation);
        }

        public void FreezePortrait()
        {
            dialoguePanelAnim.speed = 0;
            int layerIdx = Mathf.Max(dialoguePanelAnim.GetLayerIndex("Portrait"), 0);
            dialoguePanelAnim.Play(dialoguePanelAnim.GetCurrentAnimatorStateInfo(layerIdx).shortNameHash, layerIdx, 0);
        }

        public void SetText(string text)
        {
            dialogueText.text = text;
        }
    }
}
