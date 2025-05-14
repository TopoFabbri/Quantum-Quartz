using Code.Scripts.Colors;
using Code.Scripts.Player;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Wall grab state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GrabState<T> : WallState<T>
    {
        private GrabSettings GrabSettings => settings as GrabSettings;
        
        private readonly BarController barController;
        
        public GrabState(T id, StateSettings.StateSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, BarController barController) : base(id, stateSettings, rb, transform, mb, playerSfx)
        {
            this.barController = barController;
            
            barController.AddBar(ColorSwitcher.QColour.Green, GrabSettings.staminaRegenSpeed, GrabSettings.staminaMitigation, GrabSettings.initStaminaCut);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            barController.GetBar(ColorSwitcher.QColour.Green).Use();
        }
    }
}