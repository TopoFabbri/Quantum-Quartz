using System;
using System.Collections.Generic;
using Code.Scripts.Colors;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace Code.Scripts.Platforms
{
    public class PlatformsController : MonoBehaviour
    {
        [Serializable]
        public struct Platforms
        {
            public Tilemap tileMap;
            public ShadowCaster2D shadowCaster2D;
            public List<Collider2D> colliders;
        }

        [SerializeField] private Platforms redPlatforms;
        [SerializeField] private Platforms bluePlatforms;
        [SerializeField] private Platforms greenPlatforms;
        [SerializeField] private Platforms yellowPlatforms;

        [SerializeField] private float deactivatedAlpha = 0.05f;

        private void Start()
        {
            TurnOffAllPlatforms();
        }

        private void OnEnable()
        {
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
                    ActivatePlatforms(redPlatforms);
                    break;

                case ColorSwitcher.QColors.Blue:
                    ActivatePlatforms(bluePlatforms);
                    break;

                case ColorSwitcher.QColors.Green:
                    ActivatePlatforms(greenPlatforms);
                    break;

                case ColorSwitcher.QColors.Yellow:
                    ActivatePlatforms(yellowPlatforms);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ActivatePlatforms(Platforms platforms, bool activate = true)
        {
            if (platforms.tileMap)
                platforms.tileMap.color = new Color(platforms.tileMap.color.r, platforms.tileMap.color.g,
                    platforms.tileMap.color.b, activate ? 1 : deactivatedAlpha);

            if (platforms.shadowCaster2D)
                platforms.shadowCaster2D.enabled = activate;

            foreach (Collider2D collider in platforms.colliders)
                collider.enabled = activate;
        }

        private void TurnOffAllPlatforms()
        {
            ActivatePlatforms(redPlatforms, false);
            ActivatePlatforms(bluePlatforms, false);
            ActivatePlatforms(greenPlatforms, false);
            ActivatePlatforms(yellowPlatforms, false);
        }
    }
}