using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Menu
{
    public class ResolutionController : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private List<Resolution> filteredResolutions;

        private int currentResolutionIndex = 0;
        private void Start()
        {
            filteredResolutions = new List<Resolution>();

            resolutionDropdown.ClearOptions();

            filteredResolutions.Add(AddResolution(1280, 720, Screen.currentResolution.refreshRateRatio));
            filteredResolutions.Add(AddResolution(1600, 900, Screen.currentResolution.refreshRateRatio));
            filteredResolutions.Add(AddResolution(1920, 1080, Screen.currentResolution.refreshRateRatio));

            List<string> options = new List<string>();
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio.value + "Hz";
                options.Add(resolutionOption);

                if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();

            ApplySavedResolution();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
            PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
            PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
            PlayerPrefs.Save();
        }

        private void ApplySavedResolution()
        {
            int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
            int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
            Screen.SetResolution(width, height, Screen.fullScreen);
        }

        private Resolution AddResolution(int width, int height, RefreshRate refreshRate)
        {
            return new Resolution
            {
                width = width,
                height = height,
                refreshRateRatio = refreshRate
            };
        }
    }
}
