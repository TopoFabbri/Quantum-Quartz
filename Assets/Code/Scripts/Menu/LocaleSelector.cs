using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
   private bool active = false;

   private void Start()
   {
      int id= PlayerPrefs.GetInt("LocaleKey", 0);
      ChangeLocale(id);
   }

   public void ChangeLocale(int localeId)
   {
      if (active)
         return;

      StartCoroutine(SetLocale(localeId));

   }
   
   IEnumerator SetLocale(int localeId)
   {
      active=true;
      yield return LocalizationSettings.InitializationOperation;
      LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
      PlayerPrefs.SetInt("LocaleKey", localeId);
      active = false;
   }
}
