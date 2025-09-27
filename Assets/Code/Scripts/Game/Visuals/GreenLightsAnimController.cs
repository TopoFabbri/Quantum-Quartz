using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Scripts.Game.Visuals
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class GreenLightsAnimController : MonoBehaviour
    {
        [Serializable]
        public enum LookDirection
        {
            Up,
            Down
        }

        [Serializable]
        public enum LightColour
        {
            Blue,
            Green,
            Pink
        }

        [SerializeField] private LookDirection lookDirection;
        [SerializeField] private LightColour colour;
        [SerializeField] private List<Color> lightColours;
        [SerializeField] private List<Sprite> sprites;
        
        private readonly List<Light2D> lights = new();
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private static readonly int LookingUp = Animator.StringToHash("LookingUp");
        private static readonly int Colour = Animator.StringToHash("Colour");

        private void OnDrawGizmosSelected()
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (lights.Count <= 0)
                lights.AddRange(GetComponentsInChildren<Light2D>());
            
            foreach (Light2D light2D in lights)
                light2D.color = lightColours[(int)colour];
            
            spriteRenderer.sprite = (lookDirection, color: colour) switch
            {
                (LookDirection.Down, LightColour.Blue) => sprites[0],
                (LookDirection.Up, LightColour.Blue) => sprites[1],
                (LookDirection.Down, LightColour.Green) => sprites[2],
                (LookDirection.Up, LightColour.Green) => sprites[3],
                (LookDirection.Down, LightColour.Pink) => sprites[4],
                (LookDirection.Up, LightColour.Pink) => sprites[5],
                _ => spriteRenderer.sprite
            };
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        
            switch (lookDirection)
            {
                case LookDirection.Up:
                    animator.SetBool(LookingUp, true);
                    break;
                case LookDirection.Down:
                    animator.SetBool(LookingUp, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            animator.SetInteger(Colour, (int)colour);
        }
    }
}