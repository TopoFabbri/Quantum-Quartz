//https://github.com/wmjoers/CameraScaler
using UnityEngine;

namespace Code.Scripts.Game
{
    [RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] protected int targetWidth = 1920;
        [SerializeField] protected int targetHeight = 1080;

        private Camera cam;
        private int lastWidth = 0;
        private int lastHeight = 0;

        protected void Awake()
        {
            cam = GetComponent<Camera>();
        }

        protected void Update()
        {
            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                float scaleValue = ((float)Screen.width * targetHeight) / ((float)Screen.height * targetWidth);

                Rect rect = new();
                if (scaleValue < 1f)
                {
                    rect.x = 0;
                    rect.y = (1 - scaleValue) / 2;
                    rect.width = 1;
                    rect.height = scaleValue;
                }
                else
                {
                    scaleValue = 1 / scaleValue;
                    rect.x = (1 - scaleValue) / 2;
                    rect.y = 0;
                    rect.width = scaleValue;
                    rect.height = 1;
                }

                cam.rect = rect;
                lastWidth = Screen.width;
                lastHeight = Screen.height;
            }
        }
    }
}