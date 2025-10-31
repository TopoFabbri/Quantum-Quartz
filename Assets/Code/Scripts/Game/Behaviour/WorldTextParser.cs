using Code.Scripts.Game.Managers;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Game.Behaviour
{
    [RequireComponent(typeof(TMP_Text))]
    public class WorldTextParser : MonoBehaviour
    {
        [SerializeField]
        private float characterDelay = 0;

        private TMP_Text tmpText;
        private Coroutine coroutine;
        private int textIdx = 1;

#pragma warning disable IDE1006 // Naming Styles
        private string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                if (Application.isPlaying)
                {
                    OnTextUpdate(value);
                }
                else
                {
                    _text = value;
                }
            }
        }
#pragma warning restore IDE1006 // Naming Styles

        private void Start()
        {
            tmpText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            ClearText();
            StartText();
        }

        public void ClearText()
        {
            tmpText.text = "";
            tmpText.enabled = false;
            textIdx = 1;
        }

        public void StartText()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                ClearText();
            }

            coroutine = StartCoroutine(TextLoop(text));
        }

        private void OnTextUpdate(string value)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            _text = value ?? "";
            coroutine = StartCoroutine(TextLoop(text));
        }

        IEnumerator TextLoop(string str)
        {
            if (!tmpText)
            {
                Debug.LogError("Error: No text component found on " + gameObject.name);
                yield break;
            }

            tmpText.text = "";
            tmpText.enabled = true;

            float wait = 0;
            for (int i = 1; i < str.Length && textIdx < str.Length; i++)
            {
                string text = DialogueManager.Instance.ProcessText(str, ref i, ref wait);
                if (text != null && i >= textIdx)
                {
                    textIdx = i;
                    tmpText.text = text;
                    wait += characterDelay;

                    if (wait >= Time.deltaTime)
                    {
                        yield return new WaitForSecondsRealtime(wait);
                        wait = 0;
                    }
                }
            }

            textIdx = str.Length;
            tmpText.text = DialogueManager.Instance.ProcessText(str, ref textIdx, ref wait);
        }
    }
}
