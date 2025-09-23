using UnityEngine;

namespace Code.Scripts.Menu
{
    public static class FullScreenManager
    {
        public static void InitializeFullScreen()
        {

            if (!PlayerPrefs.HasKey("HasLaunchedBefore"))
            {
                SetFullScreen(true);
                PlayerPrefs.SetInt("HasLaunchedBefore", 1);
                PlayerPrefs.Save();
            }
            else
            {
                bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
                SetFullScreen(isFullScreen);
            }
        }

        public static void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void ToggleFullScreen()
        {
            bool isFullScreen = !Screen.fullScreen;
            SetFullScreen(isFullScreen);
        }
    }
}
