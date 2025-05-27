using Code.Scripts.Colors;
using Code.Scripts.FSM;
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
        
        public GrabState(T id, GrabSettings stateSettings, SharedContext sharedContext, BarController barController) : base(id, stateSettings.wallSettings, sharedContext)
        {
            this.grabSettings = stateSettings;
            this.barController = barController;
            
            barController.AddBar(ColorSwitcher.QColour.Green, grabSettings.staminaRegenSpeed, grabSettings.staminaMitigation, grabSettings.initStaminaCut);
            barController.GetBar(ColorSwitcher.QColour.Green).AddConditionalRegenSpeed(() => !sharedContext.IsGrounded ? 0 : null);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            canMove = false;

            sharedContext.Rigidbody.gravityScale = 0f;
            sharedContext.Rigidbody.velocity = Vector2.zero;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            barController.GetBar(ColorSwitcher.QColour.Green).Use();
        }
    }
}