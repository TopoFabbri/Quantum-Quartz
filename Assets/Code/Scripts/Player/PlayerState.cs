using AYellowpaper.SerializedCollections;
using Code.Scripts.Animation;
using Code.Scripts.Camera;
using Code.Scripts.Colors;
using Code.Scripts.FSM;
using Code.Scripts.Input;
using Code.Scripts.Level;
using Code.Scripts.States;
using Code.Scripts.StateSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Player
{
    using State = BaseState<string>;
    using TransitionContext = FiniteStateMachine<string>.TransitionContext;

    public class PlayerState : MonoBehaviour
    {
        public static event Action<bool> OnFlip;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private BarController staminaBar;
        [SerializeField] private TextMeshProUGUI stateDebugText;
        [SerializeField] private PlayerSfx playerSfx;
        [SerializeField] private FsmAnimationController animController;
        [SerializeField] private ParticleSystem dashPs;
        [SerializeField] private ParticleSystem djmpPs;
        [SerializeField] private ParticleSystem djmpPs2;
        [SerializeField] private ParticleSystem gldePs;
        [SerializeField] private ParticleSystem wjmpPs;
        [SerializeField] private GlobalSettings globalSettings;
        [SerializeField] private SerializedDictionary<string, StateSettings.StateSettings> settings;
        [SerializeField] private RectTransform gearFX;
        
        private readonly FiniteStateMachine<string> stateMachine = new FiniteStateMachine<string>(2);
#pragma warning disable IDE1006 // Naming Styles
        public SharedContext sharedContext { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

        private Action unsubscribeStates;

        private bool shouldTp = false;
        private bool exitTp = false;
        private bool jumpPressed = false;
        private bool djmpPressed = false;
        private bool dashPressed = false;
        private bool grabPressed = false;
        private bool glidePressed = false;
        private bool contextualPressed = false;

        private void Awake()
        {
            sharedContext = new SharedContext(rb, col, transform, this, playerSfx, globalSettings, stateMachine, gearFX);
            CreateStateMachine();
        }

        private void Start()
        {
            if (!sharedContext.facingRight)
            {
                Flip();
            }
        }

        private void OnEnable()
        {
            InputManager.AbilityPress += OnAbilityPressHandler;
            InputManager.AbilityRelease += OnAbilityReleaseHandler;
            ColorSwitcher.ColorChanged += OnChangedColorHandler;

            sharedContext.CamController.MoveCam += PausePlayer;
            sharedContext.CamController.StopCam += ResumePlayer;
            sharedContext.OnCheckFlip += CheckFlip;

            if (animController)
            {
                stateMachine.StateChanged += animController.OnStateChangedHandler;
                OnFlip += animController.OnFlipHandler;
            }
        }

        private void Update()
        {
            stateMachine.Update();

            if (stateDebugText)
            {
                stateDebugText.text = stateMachine.CurrentState.ID;
            }
        }

        private void FixedUpdate()
        {
            CheckFlip();
            stateMachine.FixedUpdate();

            if (sharedContext.RecalculateIsGrounded())
            {
                sharedContext.djmpAvailable = true;
            }
        }

        private void OnDisable()
        {
            InputManager.AbilityPress -= OnAbilityPressHandler;
            InputManager.AbilityRelease -= OnAbilityReleaseHandler;
            ColorSwitcher.ColorChanged -= OnChangedColorHandler;

            sharedContext.CamController.MoveCam -= PausePlayer;
            sharedContext.CamController.StopCam -= ResumePlayer;
            sharedContext.OnCheckFlip -= CheckFlip;

            if (animController)
            {
                stateMachine.StateChanged += animController.OnStateChangedHandler;
                OnFlip -= animController.OnFlipHandler;
            }
        }

        private void OnDestroy()
        {
            unsubscribeStates?.Invoke();
        }

        /// <summary>
        /// Initialise the finite state machine and its transitions
        /// </summary>
        void CreateStateMachine()
        {
            // =====================================
            // ||          Create States          ||
            // =====================================
            IdleState     <string> idle = new("Idle"                                                                                       );
            TpState       <string> tlpt = new("TP",     sharedContext                                                                      );
            PauseState    <string> paus = new("Pause",  sharedContext                                                                      );
            ExitTpState   <string> extp = new("ExitTP", settings["ExitTP"] as SpawnSettings,  sharedContext                                );
            MoveState     <string> move = new("Move",   settings["Move"]   as MoveSettings,   sharedContext                                );
            SpringState   <string> spng = new("Spring", settings["Spring"] as SpringSettings, sharedContext                                );
            JumpState     <string> jump = new("Jump",   settings["Jump"]   as JumpSettings,   sharedContext                                );
            DjmpState     <string> djmp = new("Djmp",   settings["Djmp"]   as DjmpSettings,   sharedContext, djmpPs,          djmpPs2      );
            DeathState    <string> deth = new("Death",  settings["Death"]  as DeathSettings,  sharedContext                                );
            SpawnState    <string> spwn = new("Spawn",  settings["Spawn"]  as SpawnSettings,  sharedContext                                );
            WallJumpState <string> wjmp = new("Wjmp",   settings["Wjmp"]   as WjmpSettings,   sharedContext                                );
            FallState     <string> fall = new("Fall",   settings["Fall"]   as FallSettings,   sharedContext                                );
            WallState     <string> wall = new("Wall",   settings["Wall"]   as WallSettings,   sharedContext                                );
            DashState     <string> dash = new("Dash",   settings["Dash"]   as DashSettings,   sharedContext, staminaBar,      dashPs       );
            GlideState    <string> glde = new("Glide",  settings["Glide"]  as GlideSettings,  sharedContext, staminaBar,      gldePs       );
            GrabState     <string> grab = new("Grab",   settings["Grab"]   as GrabSettings,   sharedContext, staminaBar                    );
            EdgeState     <string> edge = new("Edge",   settings["Edge"]   as EdgeSettings,   sharedContext, animController                );

            List<State> states = new List<State>
            {
                idle,
                tlpt,
                extp,
                paus,
                move,
                spng,
                jump,
                djmp,
                deth,
                spwn,
                wjmp,
                fall,
                wall,
                dash,
                glde,
                grab,
                edge
            };

            foreach (State state in states)
            {
                stateMachine.AddState(state);
            }


            // ====================================
            // ||      Subscribe to Events       ||
            // ====================================
            void onExitSpawn()
            {
                jumpPressed = false;
                djmpPressed = false;
                dashPressed = false;
            }
            void onEnterJump()
            {
                jumpPressed = false;
            }
            void onEnterDJump()
            {
                djmpPressed = false;
            }
            void onEnterDash()
            {
                dashPressed = false;
            }
            void onPlayerTp()
            {
                shouldTp = true;
            }
            void onEnterTeleport()
            {
                shouldTp = false;
            };
            void onEnterWall()
            {
                if (sharedContext.PreviousStateType != typeof(GrabState<string>))
                {
                    Flip();
                }
            };

            spwn.onExit += onExitSpawn;
            jump.onEnter += onEnterJump;
            djmp.onEnter += onEnterDJump;
            dash.onEnter += onEnterDash;
            LevelChanger.PlayerTp += onPlayerTp;
            tlpt.onEnter += onEnterTeleport;
            wall.onEnter += onEnterWall;
            wjmp.onEnter += move.ResetSpeed;

            unsubscribeStates = () =>
            {
                spwn.onExit -= onExitSpawn;
                jump.onEnter -= onEnterJump;
                djmp.onEnter -= onEnterDJump;
                dash.onEnter -= onEnterDash;
                LevelChanger.PlayerTp -= onPlayerTp;
                tlpt.onEnter -= onEnterTeleport;
                wall.onEnter -= onEnterWall;
                wjmp.onEnter -= move.ResetSpeed;
            };

            // ====================================
            // ||         Set Up Context         ||
            // ====================================
            TransitionContext context = stateMachine.Context;
            context.AddCondition("StoppedMoving", move.StoppedMoving);
            context.AddCondition("IsGrounded", sharedContext.RecalculateIsGrounded);
            context.AddCondition("HasJumped", () => jump.HasJumped);
            context.AddCondition("HasDoubleJumped", () => djmp.HasJumped);
            context.AddCondition("HasWallJumped", () => wjmp.HasJumped);
            context.AddCondition("VisuallyOnEdge", edge.IsVisuallyOnEdge);
            context.AddCondition("HasInput", () => sharedContext.Input != 0);
            context.AddCondition("NoWall", () => !move.WallCheck());
            context.AddCondition("CanEnterWall", wall.CanEnterWall);
            context.AddCondition("Spring", () => sharedContext.spring.HasValue);
            context.AddCondition("CanCoyoteJump", () => sharedContext.canCoyoteJump);
            context.AddCondition("NeutralVelocity", () => Mathf.Abs(rb.velocity.y) < sharedContext.GlobalSettings.neutralSpeed);
            context.AddCondition("DownVelocity", () => rb.velocity.y < -sharedContext.GlobalSettings.neutralSpeed);
            context.AddCondition("UpVelocity", () => rb.velocity.y > sharedContext.GlobalSettings.neutralSpeed);
            context.AddCondition("IsBlue", () => ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Blue);
            context.AddCondition("IsRed", () => ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Red);
            context.AddCondition("IsGreen", () => ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Green);
            context.AddCondition("IsYellow", () => ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Yellow);
            context.AddCondition("HasRedStamina", () => !staminaBar.GetBar(ColorSwitcher.QColor.Red).Depleted);
            context.AddCondition("HasGreenStamina", () => !staminaBar.GetBar(ColorSwitcher.QColor.Green).Depleted);
            context.AddCondition("HasYellowStamina", () => !staminaBar.GetBar(ColorSwitcher.QColor.Yellow).Depleted);
            context.AddCondition("ExitSpawn", () => spwn.Ended);
            context.AddCondition("ExitExitTP", () => extp.Ended);
            context.AddCondition("ExitDash", () => dash.Ended);
            context.AddCondition("ExitTeleport", () => exitTp);
            context.AddCondition("ExitDeath", () => deth.Ended);
            context.AddCondition("NotFalling", () => !sharedContext.Falling);
            context.AddCondition("JumpPressed", () => jumpPressed);
            context.AddCondition("DoubleJumpPressed", () => djmpPressed);
            context.AddCondition("DashPressed", () => dashPressed);
            context.AddCondition("GrabPressed", () => grabPressed || (contextualPressed && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Green));
            context.AddCondition("GlidePressed", () => glidePressed || (contextualPressed && ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Yellow));
            context.AddCondition("Died", () => sharedContext.died);
            context.AddCondition("Teleport", () => shouldTp);

            // ====================================
            // ||       Create Transitions       ||
            // ====================================
            #region Transitions

            // Idle transitions
            // - Spawn       >
            // - Exit TP     >
            // - Move        >
            // - Edge        >
            // - Fall        >
            // - Jump        >
            // - Double Jump >
            // - Wall Jump   >
            // - Spring      >
            // - Wall        >
            // - Glide       >
            stateMachine.AddTransition(spwn, idle, new[] { "ExitSpawn" });
            stateMachine.AddTransition(extp, idle, new[] { "ExitExitTP" });
            stateMachine.AddTransition(move, idle, new[] { "StoppedMoving" });
            stateMachine.AddTransition(edge, idle, new[] { "IsGrounded" }, new[] { "VisuallyOnEdge" });
            stateMachine.AddTransition(fall, idle, new[] { "IsGrounded", "NotFalling" });
            stateMachine.AddTransition(wall, idle, new[] { "IsGrounded" });
            stateMachine.AddTransition(glde, idle, new[] { "IsGrounded" });

            // Move transitions
            // - Idle        >
            // - Edge        >
            // - Fall        >
            // - Jump        >
            // - Spring      >
            stateMachine.AddTransition(idle, move, new[] { "HasInput", "NoWall" });
            stateMachine.AddTransition(edge, move, new[] { "HasInput", "NoWall" });
            stateMachine.AddTransition(fall, move, new[] { "IsGrounded", "HasInput", "NotFalling", "NoWall" });

            // Jump transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Fall        >
            stateMachine.AddTransition(idle, jump, new[] { "JumpPressed", "IsGrounded" });
            stateMachine.AddTransition(move, jump, new[] { "JumpPressed", "IsGrounded" });
            stateMachine.AddTransition(edge, jump, new[] { "JumpPressed", "IsGrounded" });
            stateMachine.AddTransition(fall, jump, new[] { "JumpPressed", "CanCoyoteJump" });

            // Fall transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Jump        >
            // - Double Jump >
            // - Wall Jump   >
            // - Spring      >
            // - Dash        >
            // - Wall        >
            // - Glide       >
            stateMachine.AddTransition(idle, fall, new[] { "" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(move, fall, new[] { "" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(edge, fall, new[] { "" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(jump, fall, new[] { "" }, new[] { "NotFalling" });
            stateMachine.AddTransition(djmp, fall, new[] { "" }, new[] { "NotFalling" });
            stateMachine.AddTransition(wjmp, fall, new[] { "" }, new[] { "NotFalling" });
            stateMachine.AddTransition(spng, fall, new[] { "" }, new[] { "NotFalling" });
            stateMachine.AddTransition(dash, fall, new[] { "ExitDash" });
            stateMachine.AddTransition(wall, fall, () => Math.Sign(sharedContext.Input) == (sharedContext.facingRight ? 1 : -1) || !wall.IsTouchingWall(!sharedContext.facingRight) || context.IsFalse("IsGreen"));
            stateMachine.AddTransition(glde, fall, () => context.IsFalse("GlidePressed") || context.IsFalse("HasYellowStamina"));

            // Dash transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Fall        >
            // - Jump        >
            // - Wall Jump   >
            // - Spring      >
            stateMachine.AddTransition(idle, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(move, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(edge, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(fall, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(jump, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(wjmp, dash, new[] { "DashPressed" });
            stateMachine.AddTransition(spng, dash, new[] { "DashPressed" });

            // Double Jump transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Fall        >
            // - Jump        >
            // - Wall Jump   >
            // - Spring      >
            stateMachine.AddTransition(idle, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(move, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(edge, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(fall, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(jump, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(wjmp, djmp, new[] { "DoubleJumpPressed" });
            stateMachine.AddTransition(spng, djmp, new[] { "DoubleJumpPressed" });

            // Wall transitions
            // - Fall        >
            // - Jump        >
            // - Double Jump >
            // - Wall Jump   >
            // - Spring      >
            // - Grab        >
            stateMachine.AddTransition(fall, wall, new[] { "CanEnterWall" });
            stateMachine.AddTransition(jump, wall, new[] { "CanEnterWall", "GrabPressed", "HasGreenStamina" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(djmp, wall, new[] { "CanEnterWall", "GrabPressed", "HasGreenStamina" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(wjmp, wall, new[] { "CanEnterWall", "GrabPressed", "HasGreenStamina" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(spng, wall, new[] { "CanEnterWall", "GrabPressed", "HasGreenStamina" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(grab, wall, () => context.IsFalse("GrabPressed") || context.IsFalse("HasGreenStamina") || !wall.IsTouchingWall(!sharedContext.facingRight));

            // Wall Jump transitions
            // - Wall        >
            // - Grab        >
            stateMachine.AddTransition(wall, wjmp, new[] { "JumpPressed" });
            stateMachine.AddTransition(grab, wjmp, new[] { "JumpPressed" });

            // Glide transitions
            // - Fall        >
            stateMachine.AddTransition(fall, glde, new[] { "GlidePressed", "HasYellowStamina" });

            // Edge transitions
            // - Idle        >
            // - Fall        >
            // - Glide       >
            stateMachine.AddTransition(idle, edge, new[] { "IsGrounded", "VisuallyOnEdge" });
            stateMachine.AddTransition(fall, edge, new[] { "IsGrounded", "VisuallyOnEdge", "NotFalling" });
            stateMachine.AddTransition(glde, edge, new[] { "IsGrounded", "VisuallyOnEdge" });

            // Spring transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Fall        >
            // - Jump        >
            // - Double Jump >
            // - Wall Jump   >
            // - Spring      > Allows chaining springs
            // - Dash        >
            // - Wall        >
            // - Glide       >
            stateMachine.AddTransition(idle, spng, new[] { "Spring" });
            stateMachine.AddTransition(move, spng, new[] { "Spring" });
            stateMachine.AddTransition(edge, spng, new[] { "Spring" });
            stateMachine.AddTransition(fall, spng, new[] { "Spring" });
            stateMachine.AddTransition(jump, spng, new[] { "Spring" });
            stateMachine.AddTransition(djmp, spng, new[] { "Spring" });
            stateMachine.AddTransition(wjmp, spng, new[] { "Spring" });
            stateMachine.AddTransition(spng, spng, new[] { "Spring" });
            stateMachine.AddTransition(dash, spng, new[] { "Spring" });
            stateMachine.AddTransition(wall, spng, new[] { "Spring" });
            stateMachine.AddTransition(glde, spng, new[] { "Spring" });

            // Grab transitions
            // - Spawn       >
            stateMachine.AddTransition(wall, grab, new[] { "GrabPressed", "HasGreenStamina" });

            // Death transitions
            // - All         >
            stateMachine.AddGlobalTransition(deth, new[] { "Died" }, new[] { "Teleport", "ExitTeleport" });

            // Spawn transitions
            // - Death       >
            stateMachine.AddTransition(deth, spwn, new[] { "ExitDeath" });

            // Teleport transitions
            // - All         >
            stateMachine.AddGlobalTransition(tlpt, new[] { "Teleport" }, new[] { "ExitTeleport", "Died" });

            #endregion

            // ====================================
            // ||      Set Up State Machine      ||
            // ====================================
            stateMachine.SetCurrentState(extp);
            stateMachine.Init();

            // =====================================
            // ||   Set Up Animation Controller   ||
            // =====================================
            if (animController)
            {
                animController.AddState(idle.ID, 0);
                animController.AddState(move.ID, 1);
                animController.AddState(jump.ID, 2);
                animController.AddState(fall.ID, 3);
                animController.AddState(dash.ID, 4);
                animController.AddState(djmp.ID, 5);
                animController.AddState(deth.ID, 6);
                animController.AddState(spwn.ID, 7);
                animController.AddState(tlpt.ID, 8);
                animController.AddState(extp.ID, 9);
                animController.AddState(wall.ID, 10);
                animController.AddState(wjmp.ID, 11);
                animController.AddState(glde.ID, 12);
                animController.AddState(grab.ID, 13);
                animController.AddState(edge.ID, 14);
                animController.AddState(spng.ID, 15);
            }
        }

        private void CheckFlip()
        {
            if (!sharedContext.BlockMoveInput && !typeof(IPreventFlip).IsAssignableFrom(sharedContext.CurrentStateType))
            {
                float input = sharedContext.Input;
                if (
                    sharedContext.facingRight ?
                    (input < 0 || (input == 0 && sharedContext.Speed.x < 0))
                    : (input > 0 || (input == 0 && sharedContext.Speed.x > 0))
                )
                {
                    Flip();
                }
            }
        }

        /// <summary>
        /// Flip character
        /// </summary>
        public void Flip()
        {
            sharedContext.facingRight = !sharedContext.facingRight;

            OnFlip?.Invoke(sharedContext.facingRight);
        }

        public void OnJump(bool isPressed)
        {
            bool contextualColor = ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Blue || ColorSwitcher.Instance.CurrentColor == ColorSwitcher.QColor.Yellow;
            bool doContextual = (contextualColor && InputManager.activeMap.GetContextualBYPower()) || InputManager.activeMap.GetContextualPower();
            if (doContextual && (!isPressed || (!sharedContext.canCoyoteJump && !sharedContext.IsGrounded)))
            {
                // If in air with a contextual power mapping, use ability
                if (isPressed)
                {
                    OnAbilityPressHandler(true);
                }
                else
                {
                    OnAbilityReleaseHandler(true);
                }
            }
            else if (isPressed)
            {
                jumpPressed = true;
                StartCoroutine(EndJumpBufferTime(globalSettings.jumpBufferTime));
            }
        }

        /// <summary>
        /// Count and end jump buffer time
        /// </summary>
        /// <param name="jumpBufferTime">Time to count</param>
        IEnumerator EndJumpBufferTime(float jumpBufferTime)
        {
            yield return new WaitForSeconds(jumpBufferTime);
            jumpPressed = false;
        }

        /// <summary>
        /// Manage player actions when color is changed
        /// </summary>
        /// <param name="color">New color</param>
        void OnChangedColorHandler(ColorSwitcher.QColor color)
        {
            contextualPressed = false;
            grabPressed = false;
            glidePressed = false;
            if (color == ColorSwitcher.QColor.Green)
            {
                sharedContext.DoWallCooldown(sharedContext.GlobalSettings.greenWallDelay);
            }
        }

        /// <summary>
        /// Handle player ability input
        /// </summary>
        void OnAbilityPressHandler() => OnAbilityPressHandler(false);
        void OnAbilityPressHandler(bool contextual)
        {
            switch (ColorSwitcher.Instance.CurrentColor)
            {
                case ColorSwitcher.QColor.None:
                    break;
                case ColorSwitcher.QColor.Red:
                    if (!staminaBar.GetBar(ColorSwitcher.QColor.Red).Depleted)
                    {
                        dashPressed = true;
                    }
                    break;
                case ColorSwitcher.QColor.Blue:
                    if (sharedContext.djmpAvailable)
                    {
                        djmpPressed = true;
                    }
                    break;
                case ColorSwitcher.QColor.Green:
                    if (contextual)
                    {
                        contextualPressed = true;
                    }
                    else
                    {
                        grabPressed = true;
                    }
                    break;
                case ColorSwitcher.QColor.Yellow:
                    if (contextual)
                    {
                        contextualPressed = true;
                    }
                    else
                    {
                        glidePressed = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle player ability release input
        /// </summary>
        void OnAbilityReleaseHandler() => OnAbilityReleaseHandler(false);
        void OnAbilityReleaseHandler(bool contextual = false)
        {
            if (contextual)
            {
                contextualPressed = false;
            }

            switch (ColorSwitcher.Instance.CurrentColor)
            {
                case ColorSwitcher.QColor.None:
                    break;
                case ColorSwitcher.QColor.Red:
                    break;
                case ColorSwitcher.QColor.Blue:
                    break;
                case ColorSwitcher.QColor.Green:
                    if (!contextual)
                    {
                        grabPressed = false;
                    }
                    break;
                case ColorSwitcher.QColor.Yellow:
                    if (!contextual)
                    {
                        glidePressed = false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Pause player fsm
        /// </summary>
        void PausePlayer()
        {
            animController.TogglePauseAnim(true);
            stateMachine.InterruptState(stateMachine.GetState("Pause"));
        }

        /// <summary>
        /// Resume player fsm
        /// </summary>
        void ResumePlayer()
        {
            animController.TogglePauseAnim(false);
            stateMachine.StopInterrupt();
        }

        /// <summary>
        /// Stop teleport state (Called from animation)
        /// </summary>
        private void EndTp()
        {
            exitTp = true;
        }
    }
}
