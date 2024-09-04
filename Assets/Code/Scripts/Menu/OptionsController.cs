using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
