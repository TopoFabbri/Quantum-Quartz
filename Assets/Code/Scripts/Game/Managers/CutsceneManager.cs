using Code.Scripts.Input;
using Code.Scripts.Tools;
using System;
using System.Collections;
using UnityEngine;

namespace Code.Scripts.Game.Managers
{
    [RequireComponent(typeof(Animator))]
    public class CutsceneManager : MonoBehaviourSingleton<CutsceneManager>
    {
        // Amimation Events can't have bool inputs, but they can have enum inputs
        public enum Boolean
        {
            True,
            False
        }

        [SerializeField] Transform playerDestinationX;
        [SerializeField] Transform fakePlayer;

        Animator anim;

        private void Start()
        {
            anim = GetComponent<Animator>();
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            for (int i = 0; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i, 1);
            }
        }

        public void TriggerAnimation(string triggerName)
        {
            anim.SetTrigger(triggerName);
        }

        public void TriggerSound(string triggerName)
        {
            AkSoundEngine.PostEvent(triggerName, gameObject);
        }

        public void UnlockColor(ColorSwitcher.QColor color)
        {
            ColorSwitcher.Instance.EnableColor(color);
        }

        public void SwapColor(ColorSwitcher.QColor color)
        {
            ColorSwitcher.Instance.SwapColor(color);
        }

        public void MovePlayerToDestinationX(string chainTriggerName)
        {
            if (!playerDestinationX)
            {
                Debug.LogError("Error: No player destination provided");
                return;
            }

            EnableInputs(Boolean.False);
            StartCoroutine(MovePlayerX(chainTriggerName));
        }

        // The discard parameter is there to hide this function from Animation Events.
        // The only way I know of to hide functions is to have 2+ parameters
        private IEnumerator MovePlayerX(string chainTriggerName, bool _ = false)
        {
            Transform player = GameManager.Instance.Player.transform;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            float destinationX = playerDestinationX.position.x;
            float xOffset = destinationX - player.position.x;
            while (xOffset != 0)
            {
                InputManager.Instance.CutsceneMoveX(Mathf.Clamp(xOffset, -1, 1));
                yield return new WaitForFixedUpdate();

                if (playerDestinationX)
                {
                    destinationX = playerDestinationX.position.x;
                }
                xOffset = destinationX - player.position.x;

                if (InputManager.Instance.enabled)
                {
                    break;
                }

                if (xOffset * Math.Sign(xOffset) <= rb.velocity.x * 1.5f * Time.fixedDeltaTime * Math.Sign(xOffset))
                {
                    InputManager.Instance.CutsceneMoveX(0);
                    player.position = new Vector3(destinationX, player.position.y, player.position.z);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    xOffset = 0;
                }
            }

            if (xOffset == 0 && !string.IsNullOrWhiteSpace(chainTriggerName))
            {
                TriggerAnimation(chainTriggerName);
            }
        }

        public void UpdateFakePlayerPosition()
        {
            if (!fakePlayer)
            {
                Debug.LogError("Error: No fake player provided");
                return;
            }
            var player = GameManager.Instance.Player;
            fakePlayer.position = player.transform.position;
            fakePlayer.rotation = player.IsFacingRight ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        }

        public void FacePlayerRight(Boolean right)
        {
            GameManager.Instance.Player.FaceRight(right == Boolean.True);
        }

        public void EnablePlayer(Boolean enable)
        {
            GameManager.Instance.Player.gameObject.SetActive(enable == Boolean.True);
        }

        public void EnableInputs(Boolean enable)
        {
            InputManager.Instance.enabled = enable == Boolean.True;
            if (enable == Boolean.False)
            {
                InputManager.Instance.CutsceneMoveX(0);
            }
        }

        public void EnablePauseMusic(Boolean enable)
        {
            GameManager.Instance.SetMusicFaded(enable == Boolean.True);
        }

        public void SetGameSpeed(float speed)
        {
            Time.timeScale = speed;
        }

        string chainTriggerName_StartDialogue = null;
        public void QueueChainTrigger_StartDialogue(string chainTriggerName)
        {
            chainTriggerName_StartDialogue = chainTriggerName;
        }
        public void StartDialogue(Conversation conversation)
        {
            if (!DialogueManager.HasInstance)
            {
                Debug.LogError("Error: No Dialogue Manager available");
                return;
            }

            string chainTriggerName = chainTriggerName_StartDialogue;
            chainTriggerName_StartDialogue = null;

            EnableInputs(Boolean.True);
            DialogueManager.Instance.StartDialogue(conversation, () => {
                if (!string.IsNullOrWhiteSpace(chainTriggerName))
                {
                    TriggerAnimation(chainTriggerName);
                }
            });
        }

        string chainTriggerName_WaitForInput = null;
        public void QueueChainTrigger_WaitForInput(string chainTriggerName)
        {
            chainTriggerName_WaitForInput = chainTriggerName;
        }
        public void WaitForInput(InputManager.InputAction input)
        {
            string chainTriggerName = chainTriggerName_WaitForInput;
            chainTriggerName_WaitForInput = null;

            if (!string.IsNullOrWhiteSpace(chainTriggerName))
            {
                InputManager.Instance.WaitForInputAction(input, () => {
                    EnableInputs(Boolean.True);
                    TriggerAnimation(chainTriggerName);
                });
            }
        }
    }
}
