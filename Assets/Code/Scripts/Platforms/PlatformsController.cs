using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Code.Scripts.Platforms
{
    public class PlatformsController : MonoBehaviour
    {
        [FormerlySerializedAs("RedPlatforms")] [SerializeField] private GameObject[] redPlatforms;
        [FormerlySerializedAs("BluePlatforms")] [SerializeField] private GameObject[] bluePlatforms;
        [FormerlySerializedAs("GreenPlatforms")] [SerializeField] private GameObject[] greenPlatforms;
        [FormerlySerializedAs("YellowPlatforms")] [SerializeField] private GameObject[] yellowPlatforms;

        [SerializeField] private float deactivatedAlpha = 0.05f;

        private void OnEnable()
        {
            TurnOffAllPlatforms();
            ColorSwitcher.ColorChanged += TogglePlatforms;
        }
        
        private void OnDisable()
        {
            ColorSwitcher.ColorChanged -= TogglePlatforms;
        }

        private void TogglePlatforms(ColorSwitcher.QColors color)
        {
            TurnOffAllPlatforms();

            switch (color)
            {
                case ColorSwitcher.QColors.None:
                    break;
            
                case ColorSwitcher.QColors.Red:
                    TurnOnPlatforms(redPlatforms);
                    break;
            
                case ColorSwitcher.QColors.Blue:
                    TurnOnPlatforms(bluePlatforms);
                    break;
            
                case ColorSwitcher.QColors.Green:
                    TurnOnPlatforms(greenPlatforms);
                    break;
            
                case ColorSwitcher.QColors.Yellow:
                    TurnOnPlatforms(yellowPlatforms);
                    break;
            
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void TurnOnPlatforms(IEnumerable<GameObject> platforms)
        {
            foreach (GameObject platform in platforms)
            {
                platform.SetActive(true);

                SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                        1f);
                }

                Collider2D collider = platform.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = true;
                }

                if (platform.TryGetComponent(out ShadowCaster2D shadowCaster2D))
                    shadowCaster2D.enabled = true;
            }
        }

        private void TurnOffAllPlatforms()
        {
            foreach (GameObject platform in redPlatforms)
            {
                DeactivatePlatform(platform);
            }

            foreach (GameObject platform in bluePlatforms)
            {
                DeactivatePlatform(platform);
            }

            foreach (GameObject platform in greenPlatforms)
            {
                DeactivatePlatform(platform);
            }

            foreach (GameObject platform in yellowPlatforms)
            {
                DeactivatePlatform(platform);
            }
        }

        private void DeactivatePlatform(GameObject platform)
        {
            SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                    deactivatedAlpha);
            }

            Collider2D collider = platform.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (platform.TryGetComponent(out ShadowCaster2D shadowCaster2D))
                shadowCaster2D.enabled = false;
        }
    }
}