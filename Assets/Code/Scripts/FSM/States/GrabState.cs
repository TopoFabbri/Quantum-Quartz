using Code.Scripts.FSM;
using Code.Scripts.Game.Managers;
using Code.Scripts.Player;
using Code.Scripts.States.Settings;

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
        
        public GrabState(T id, GrabSettings stateSettings, SharedContext sharedContext, FsmAnimationController animator, BarController barController) : base(id, stateSettings.wallSettings, sharedContext, animator)
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
            barController.GetBar(ColorSwitcher.QColor.Green).Use();
        }
    }
}