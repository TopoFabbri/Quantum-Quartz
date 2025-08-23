using Code.Scripts.Colors;
using Code.Scripts.Level;
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
            
            barController.AddBar(ColorSwitcher.QColor.Green, grabSettings.staminaRegenSpeed, grabSettings.staminaMitigation, grabSettings.initStaminaCut);
            barController.GetBar(ColorSwitcher.QColor.Green).AddConditionalRegenSpeed(() => !sharedContext.IsGrounded ? 0 : null);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sharedContext.BlockMoveInput = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            sharedContext.BlockMoveInput = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (sharedContext.CurMovementModifier.grabEffect != MovementModifier.GrabEffect.Forced)
            {
                barController.GetBar(ColorSwitcher.QColor.Green).Use();
            }
        }
    }
}