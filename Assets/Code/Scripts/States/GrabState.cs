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
        protected readonly GrabSettings grabSettings;
        
        private readonly BarController barController;
        
        public GrabState(T id, GrabSettings stateSettings, Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, BarController barController) : base(id, stateSettings.wallSettings, rb, transform, mb, playerSfx)
        {
            this.grabSettings = stateSettings;
            this.barController = barController;
            
            barController.AddBar(ColorSwitcher.QColour.Green, grabSettings.staminaRegenSpeed, grabSettings.staminaMitigation, grabSettings.initStaminaCut);
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