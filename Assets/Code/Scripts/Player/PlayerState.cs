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
        public class SharedContext
        {
            public Rigidbody2D Rigidbody { get; private set; }
            public Transform Transform { get; private set; }
            public MonoBehaviour MonoBehaviour { get; private set; }
            public PlayerSfx PlayerSfx { get; private set; }
            public GlobalSettings GlobalSettings { get; private set; }
            public CameraController CamController { get; private set; }
            public float Input { get; private set; }
            public Type PreviousStateType => stateMachine.PreviousState?.GetType();
            public Type CurrentStateType => stateMachine.CurrentState?.GetType();

            public bool facingRight = false;
            public bool falling = false;
            public bool died = false;
            public bool canCoyoteJump = false;

            private readonly FiniteStateMachine<string> stateMachine;

            public SharedContext(Rigidbody2D rb, Transform transform, MonoBehaviour mb, PlayerSfx playerSfx, GlobalSettings globalSettings, FiniteStateMachine<string> stateMachine)
            {
                Rigidbody = rb;
                Transform = transform;
                MonoBehaviour = mb;
                PlayerSfx = playerSfx;
                GlobalSettings = globalSettings;
                this.stateMachine = stateMachine;

                InputManager.Move += OnMoveHandler;

                if (UnityEngine.Camera.main?.transform.parent != null)
                {
                    UnityEngine.Camera.main.transform.parent.TryGetComponent(out CameraController camController);
                    CamController = camController;
                }
            }

            ~SharedContext()
            {
                InputManager.Move -= OnMoveHandler;
            }

            /// <summary>
            /// Handle player move action
            /// </summary>
            /// <param name="input">Input value</param>
            void OnMoveHandler(Vector2 input)
            {
                Input = input.x;
            }

            /// <summary>
            /// Check if player is on ground
            /// </summary>
            /// <returns>True if on the ground</returns>
            public bool IsGrounded()
            {
                Vector2 pos = Transform.position;
                RaycastHit2D hit = Physics2D.Raycast(pos + GlobalSettings.groundCheckOffset, Vector2.down, GlobalSettings.groundCheckRadius, GlobalSettings.groundLayer);
                bool grounded = hit.collider && (hit.collider.CompareTag("Floor") || hit.collider.CompareTag("Platform"));

                if (GlobalSettings.shouldDraw)
                {
                    Debug.DrawLine(
                        pos + GlobalSettings.groundCheckOffset,
                        pos + GlobalSettings.groundCheckOffset + Vector2.down * GlobalSettings.groundCheckRadius,
                        grounded ? Color.green : Color.red
                    );
                    Debug.DrawLine(
                        pos + GlobalSettings.groundCheckOffset,
                        pos + GlobalSettings.groundCheckOffset + Vector2.down * GlobalSettings.groundCheckRadius,
                        grounded ? Color.green : Color.red
                    );
                }
                return grounded;
            }
        }

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BarController staminaBar;
        [SerializeField] private TextMeshProUGUI stateDebugText;
        [SerializeField] private PlayerSfx playerSfx;
        [SerializeField] private FsmAnimationController animController;
        [SerializeField] private DeathController deathController;
        [SerializeField] private ParticleSystem dashPs;
        [SerializeField] private ParticleSystem djmpPs;
        [SerializeField] private ParticleSystem djmpPs2;
        [SerializeField] private ParticleSystem gldePs;
        [SerializeField] private ParticleSystem wjmpPs;
        [SerializeField] private GlobalSettings globalSettings;
        [SerializeField] private SerializedDictionary<string, StateSettings.StateSettings> settings;
        private readonly FiniteStateMachine<string> stateMachine = new FiniteStateMachine<string>(2);
#pragma warning disable IDE1006 // Naming Styles
        public SharedContext sharedContext { get; private set; }
#pragma warning restore IDE1006 // Naming Styles

        public static event Action<bool> OnFlip;

        public MoveState<string> tempMoveState;
        public JumpState<string> tempJumpState;
        public DashState<string> tempDashState;
        public DjmpState<string> tempDjmpState; // <-------------------------- Used to check if double jump is available, and to reset the double jump
        public TpState<string> tempTlptState;
        public ExitTpState<string> tempExtpState;

        Action unsubscribeTemp;

        bool touchingFloor = false;
        bool shouldTp = false;

        bool jumpPressed = false;
        bool djmpPressed = false;
        bool dashPressed = false;
        bool grabPressed = false;
        bool glidePressed = false;

        private void Awake()
        {
            sharedContext = new SharedContext(rb, transform, this, playerSfx, globalSettings, stateMachine);
            CreateStateMachine();
        }

        private void OnDestroy()
        {
            unsubscribeTemp();
        }

        /// <summary>
        /// Initialise the finite state machine and its transitions
        /// </summary>
        void CreateStateMachine()
        {
            // =====================================
            // ||         Create States           ||
            // =====================================
            IdleState     <string> idle = new("Idle"                                                                                       );
            TpState       <string> tlpt = new("TP",     sharedContext                                                                      );
            ExitTpState   <string> extp = new("ExitTP", sharedContext                                                                      );
            PauseState    <string> paus = new("Pause",  sharedContext                                                                      );
            MoveState     <string> move = new("Move",   settings["Move"]   as MoveSettings,   sharedContext                                );
            SpringState   <string> spng = new("Spring", settings["Spring"] as SpringSettings, sharedContext                                );
            JumpState     <string> jump = new("Jump",   settings["Jump"]   as JumpSettings,   sharedContext                                );
            DjmpState     <string> djmp = new("Djmp",   settings["Djmp"]   as DjmpSettings,   sharedContext, djmpPs,          djmpPs2      );
            DeathState    <string> deth = new("Death",  settings["Death"]  as DeathSettings,  sharedContext, deathController               );
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
            // ||        Set Up Context          ||
            // ====================================
            TransitionContext context = stateMachine.Context;
            context.AddCondition("StoppedMoving", move.StoppedMoving);
            context.AddCondition("IsGrounded", sharedContext.IsGrounded);
            context.AddCondition("HasJumped", () => jump.HasJumped);
            context.AddCondition("HasDoubleJumped", () => djmp.HasJumped);
            context.AddCondition("HasWallJumped", () => wjmp.HasJumped);
            context.AddCondition("OnEdge", edge.IsOnEdge);
            context.AddCondition("HasInput", () => sharedContext.Input != 0);
            context.AddCondition("NoWall", () => !move.WallCheck());
            context.AddCondition("CanEnterWall", wall.CanEnterWall);
            context.AddCondition("Spring", () => spng.IsActivated);
            context.AddCondition("CanCoyoteJump", () => sharedContext.canCoyoteJump);
            context.AddCondition("NeutralVelocity", () => rb.velocity.y == 0);
            context.AddCondition("DownVelocity", () => rb.velocity.y < 0);
            context.AddCondition("UpVelocity", () => rb.velocity.y > 0);
            context.AddCondition("IsBlue", () => ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Blue);
            context.AddCondition("IsRed", () => ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Red);
            context.AddCondition("IsGreen", () => ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Green);
            context.AddCondition("IsYellow", () => ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Yellow);
            context.AddCondition("HasRedStamina", () => !staminaBar.GetBar(ColorSwitcher.QColour.Red).Depleted);
            context.AddCondition("HasGreenStamina", () => !staminaBar.GetBar(ColorSwitcher.QColour.Green).Depleted);
            context.AddCondition("HasYellowStamina", () => !staminaBar.GetBar(ColorSwitcher.QColour.Yellow).Depleted);
            context.AddCondition("ExitSpawn", () => spwn.Ended);
            context.AddCondition("ExitExitTP", () => extp.Ended);
            context.AddCondition("ExitDash", () => dash.Ended);
            context.AddCondition("ExitTeleport", () => tlpt.Ended);
            context.AddCondition("ExitDeath", () => deth.Ended);

            // ------------------------------------
            // !!   PlayerController variables   !!
            // ------------------------------------
            // - These need to be replaced/modified whenever possible

            tempMoveState = move;
            tempJumpState = jump;
            tempDashState = dash;
            tempDjmpState = djmp;
            tempTlptState = tlpt;
            tempExtpState = extp;

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

            unsubscribeTemp = () =>
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


            context.AddCondition("TouchingFloor", () => touchingFloor);
            context.AddCondition("NotFalling", () => !sharedContext.falling);
            context.AddCondition("JumpPressed", () => jumpPressed);
            context.AddCondition("DoubleJumpPressed", () => djmpPressed);
            context.AddCondition("DashPressed", () => dashPressed);
            context.AddCondition("GrabPressed", () => grabPressed);
            context.AddCondition("GlidePressed", () => glidePressed);
            context.AddCondition("Died", () => sharedContext.died);
            context.AddCondition("Teleport", () => shouldTp);

            // ====================================
            // ||      Create Transitions        ||
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
            stateMachine.AddTransition(edge, idle, new[] { "IsGrounded" }, new[] { "OnEdge" });
            stateMachine.AddTransition(fall, idle, new[] { "IsGrounded", "NotFalling" });
            stateMachine.AddTransition(jump, idle, new[] { "IsGrounded", "HasJumped",       "TouchingFloor" }, new[] { "DownVelocity" });
            stateMachine.AddTransition(djmp, idle, new[] { "IsGrounded", "HasDoubleJumped", "TouchingFloor" }, new[] { "DownVelocity" });
            stateMachine.AddTransition(wjmp, idle, new[] { "IsGrounded", "HasWallJumped",   "TouchingFloor" }, new[] { "DownVelocity" });
            stateMachine.AddTransition(spng, idle, new[] { "IsGrounded", "TouchingFloor" }, new[] { "DownVelocity" });
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
            stateMachine.AddTransition(jump, move, new[] { "IsGrounded", "HasJumped", "TouchingFloor" }, new[] { "DownVelocity" });
            stateMachine.AddTransition(spng, move, new[] { "IsGrounded", "TouchingFloor" }, new[] { "DownVelocity" });

            // Jump transitions
            // - Idle        >
            // - Move        >
            // - Edge        >
            // - Fall        >
            stateMachine.AddTransition(idle, jump, new[] { "JumpPressed", "IsGrounded" });
            stateMachine.AddTransition(move, jump, new[] { "JumpPressed", "IsGrounded" });
            stateMachine.AddTransition(edge, jump, new[] { "JumpPressed", "OnEdge" });
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
            stateMachine.AddTransition(idle, fall, new[] { "" }, new[] { "NeutralVelocity", "IsGrounded" });
            stateMachine.AddTransition(move, fall, new[] { "" }, new[] { "NeutralVelocity", "IsGrounded" });
            stateMachine.AddTransition(edge, fall, new[] { "DownVelocity" }, new[] { "IsGrounded", "OnEdge" });
            stateMachine.AddTransition(jump, fall, new[] { "DownVelocity" });
            stateMachine.AddTransition(djmp, fall, new[] { "DownVelocity" }, new[] { "IsGrounded" });
            stateMachine.AddTransition(wjmp, fall, new[] { "DownVelocity" });
            stateMachine.AddTransition(spng, fall, new[] { "DownVelocity" });
            stateMachine.AddTransition(dash, fall, new[] { "ExitDash" });
            stateMachine.AddTransition(wall, fall, () => !wall.IsTouchingWall() || context.IsFalse("IsGreen"));
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

            // Double Jump transitions
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
            stateMachine.AddTransition(grab, wall, () => context.IsFalse("GrabPressed") || context.IsFalse("HasGreenStamina"));

            // Wall Jump transitions
            // - Wall        >
            // - Grab        >
            stateMachine.AddTransition(wall, wjmp, new[] { "JumpPressed" });
            stateMachine.AddTransition(grab, wjmp, new[] { "JumpPressed" });

            // Glide transitions
            // - Fall        >
            stateMachine.AddTransition(fall, glde, new[] { "GlidePressed", "HasYellowStamina" });

            // Edge transitions
            // - Idle       >
            // - Fall        >
            // - Glide       >
            stateMachine.AddTransition(idle, edge, new[] { "OnEdge" });
            stateMachine.AddTransition(fall, edge, new[] { "OnEdge" }, new[] { "DownVelocity" });
            stateMachine.AddTransition(glde, edge, new[] { "OnEdge" });

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
            stateMachine.AddTransition(idle, spng, new[] { "Spring" });
            stateMachine.AddTransition(move, spng, new[] { "Spring" });
            stateMachine.AddTransition(jump, spng, new[] { "Spring" });
            stateMachine.AddTransition(fall, spng, new[] { "Spring" });
            stateMachine.AddTransition(dash, spng, new[] { "Spring" });
            stateMachine.AddTransition(djmp, spng, new[] { "Spring" });
            stateMachine.AddTransition(wall, spng, new[] { "Spring" });
            stateMachine.AddTransition(wjmp, spng, new[] { "Spring" });
            stateMachine.AddTransition(glde, spng, new[] { "Spring" });
            stateMachine.AddTransition(edge, spng, new[] { "Spring" });

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

            // Exit Teleport transitions
            // - Teleport    >
            stateMachine.AddTransition(tlpt, extp, new[] { "ExitTeleport" });

            #endregion

            // ====================================
            // ||      Create Transitions        ||
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






        // --------------------------------------------

        private void Start()
        {
            if (!sharedContext.facingRight)
            {
                Flip();
            }
        }

        private void OnEnable()
        {
            InputManager.Jump += OnJumpPressedHandler;
            InputManager.AbilityPress += OnAbilityPressHandler;
            InputManager.AbilityRelease += OnAbilityReleaseHandler;
            ColorSwitcher.ColorChanged += OnChangedColorHandler;

            sharedContext.CamController.MoveCam += PausePlayer;
            sharedContext.CamController.StopCam += ResumePlayer;

            if (animController)
            {
                stateMachine.StateChanged += animController.OnStateChangedHandler;
                OnFlip += animController.OnFlipHandler;
            }
        }

        private void OnDisable()
        {
            InputManager.Jump -= OnJumpPressedHandler;
            InputManager.AbilityPress -= OnAbilityPressHandler;
            InputManager.AbilityRelease -= OnAbilityReleaseHandler;
            ColorSwitcher.ColorChanged -= OnChangedColorHandler;

            sharedContext.CamController.MoveCam -= PausePlayer;
            sharedContext.CamController.StopCam -= ResumePlayer;

            if (animController)
            {
                stateMachine.StateChanged += animController.OnStateChangedHandler;
                OnFlip -= animController.OnFlipHandler;
            }
        }

        private void Update()
        {
            if (sharedContext.IsGrounded())
            {
                tempDjmpState.Reset();
            }

            stateMachine.Update();

            if (stateDebugText)
            {
                stateDebugText.text = stateMachine.CurrentState.ID;
            }
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();

            if (sharedContext.falling && rb.velocity.y == 0f)
            {
                sharedContext.falling = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (
                collision.enabled
                && (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Platform"))
                && sharedContext.IsGrounded()
            )
            {
                touchingFloor = true;
                sharedContext.falling = false;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (
                collision.enabled
                && (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Platform"))
                && sharedContext.IsGrounded()
            )
            {
                touchingFloor = true;
                sharedContext.falling = false;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (
                collision.enabled && sharedContext.CurrentStateType != typeof(FallState<string>)
                && (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Platform"))
            )
            {
                touchingFloor = false;
            }
        }

        /// <summary>
        /// Flip character
        /// </summary>
        public void Flip()
        {
            if (sharedContext.CurrentStateType == typeof(DashState<string>))
                return;

            sharedContext.facingRight = !sharedContext.facingRight;

            OnFlip?.Invoke(sharedContext.facingRight);
        }

        /// <summary>
        /// Handle player jump action
        /// </summary>
        void OnJumpPressedHandler(float value)
        {
            bool contextualColor = ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Blue || ColorSwitcher.Instance.CurrentColour == ColorSwitcher.QColour.Yellow;
            bool doContextual = (contextualColor && InputManager.activeMap.GetContextualBYPower()) || InputManager.activeMap.GetContextualPower();
            if (doContextual && (value == 0 || (!sharedContext.canCoyoteJump && (sharedContext.falling || !sharedContext.IsGrounded()))))
            {
                // If in air with a contextual power mapping, use ability
                if (value != 0)
                {
                    OnAbilityPressHandler();
                }
                else
                {
                    OnAbilityReleaseHandler();
                }
            }
            else if (value != 0)
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
        /// <param name="colour">New color</param>
        void OnChangedColorHandler(ColorSwitcher.QColour colour)
        {
            if (colour != ColorSwitcher.QColour.Green)
            {
                grabPressed = false;
            }

            if (colour != ColorSwitcher.QColour.Yellow)
            {
                glidePressed = false;
            }
        }

        /// <summary>
        /// Handle player ability input
        /// </summary>
        void OnAbilityPressHandler()
        {
            switch (ColorSwitcher.Instance.CurrentColour)
            {
                case ColorSwitcher.QColour.None:
                    break;
                case ColorSwitcher.QColour.Red:
                    if (!staminaBar.GetBar(ColorSwitcher.QColour.Red).Depleted)
                    {
                        dashPressed = true;
                    }
                    break;
                case ColorSwitcher.QColour.Blue:
                    if (tempDjmpState.JumpAvailable)
                    {
                        djmpPressed = true;
                    }
                    break;
                case ColorSwitcher.QColour.Green:
                    grabPressed = true;
                    break;
                case ColorSwitcher.QColour.Yellow:
                    glidePressed = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle player ability release input
        /// </summary>
        void OnAbilityReleaseHandler()
        {
            switch (ColorSwitcher.Instance.CurrentColour)
            {
                case ColorSwitcher.QColour.None:
                    break;
                case ColorSwitcher.QColour.Red:
                    break;
                case ColorSwitcher.QColour.Blue:
                    break;
                case ColorSwitcher.QColour.Green:
                    grabPressed = false;
                    break;
                case ColorSwitcher.QColour.Yellow:
                    glidePressed = false;
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
            tempTlptState.OnEnd();
        }

        /// <summary>
        /// Stop exit teleport state (Called from animation)
        /// </summary>
        private void EndExitTp()
        {
            tempExtpState.OnEnd();
        }
    }
}
