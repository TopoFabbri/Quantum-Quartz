using System.Collections;
using UnityEngine;

using UnityEngine.Localization.Settings;

namespace Code.Scripts.Menu
{
    public class LocaleSelector : MonoBehaviour
    {
        private Coroutine coroutine = null;

        private void Start()
        {
            string localeName = PlayerPrefs.GetString("LocaleKey", null);
            ChangeLocale(localeName);
        }

        public void ChangeLocale(string localeName)
        {
            if (string.IsNullOrWhiteSpace(localeName)) return;

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(SetLocale(localeName));
        }

        IEnumerator SetLocale(string localeName)
        {
            yield return LocalizationSettings.InitializationOperation;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.AvailableLocales.Locales[i].LocaleName.Contains(localeName))
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                    PlayerPrefs.SetString("LocaleKey", localeName);
                    break;
                }
            }
        }
    }
}
