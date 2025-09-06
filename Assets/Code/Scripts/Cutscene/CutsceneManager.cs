using Code.Scripts.Colors;
using Code.Scripts.Dialogue;
using Code.Scripts.Game;
using Code.Scripts.Input;
using Code.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CutsceneManager : MonoBehaviourSingleton<CutsceneManager>
{
    [SerializeField] Transform playerDestinationX;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void TriggerAnimation(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void UnlockColor(ColorSwitcher.QColor color)
    {
        ColorSwitcher.Instance.EnableColor(color);
    }

    public void MovePlayerToDestinationX(string chainTriggerName)
    {
        EnableInputs(false);
        StartCoroutine(MovePlayerX(chainTriggerName));
    }

    IEnumerator MovePlayerX(string chainTriggerName)
    {
        Transform player = GameManager.Instance.Player.transform;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        float xOffset = playerDestinationX.position.x - player.position.x;
        while (xOffset != 0)
        {
            InputManager.Instance.CutsceneMoveX(Mathf.Clamp(xOffset, -1, 1));
            yield return new WaitForFixedUpdate();

            xOffset = playerDestinationX.position.x - player.position.x;

            if (InputManager.Instance.enabled)
            {
                break;
            }

            if (xOffset * Math.Sign(xOffset) <= rb.velocity.x * 1.5f * Time.fixedDeltaTime * Math.Sign(xOffset))
            {
                player.position = new Vector3(playerDestinationX.position.x, player.position.y, player.position.z);
                rb.velocity = new Vector2(0, rb.velocity.y);
                xOffset = 0;
            }
        }

        if (xOffset == 0 && !string.IsNullOrWhiteSpace(chainTriggerName))
        {
            TriggerAnimation(chainTriggerName);
        }
    }

    public void EnablePlayer(bool enable)
    {
        GameManager.Instance.Player.gameObject.SetActive(enable);
    }

    public void EnableInputs(bool enable)
    {
        InputManager.Instance.enabled = enable;
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
                EnableInputs(true);
                TriggerAnimation(chainTriggerName);
            });
        }
    }
}
