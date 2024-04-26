using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Input;
using UnityEngine;

public class PlatformsController : MonoBehaviour
{
    [SerializeField] private GameObject[] RedPlatforms;
    [SerializeField] private GameObject[] BluePlatforms;
    [SerializeField] private GameObject[] GreenPlatforms;
    [SerializeField] private GameObject[] YellowPlatforms;

    private int currentColor = 0;

    private void OnEnable()
    {
        InputManager.Color1 += SetColor1;
        InputManager.Color2 += SetColor2;
        InputManager.Color3 += SetColor3;
        InputManager.Color4 += SetColor4;
        TurnOffAllPlatforms();
    }
    
    private void SetColor1()
    {
        currentColor = 1;
        TogglePlatforms();
    }

    private void SetColor2()
    {
        currentColor = 2;
        TogglePlatforms();
    }

    private void SetColor3()
    {
        currentColor = 3;
        TogglePlatforms();
    }

    private void SetColor4()
    {
        currentColor = 4;
        TogglePlatforms();
    }
    
    private void TogglePlatforms()
    {
        TurnOffAllPlatforms();

        switch (currentColor)
        {
            case 1:
                TurnOnPlatforms(RedPlatforms);
                Debug.Log("Current color: Red");
                break;
            case 2:
                TurnOnPlatforms(BluePlatforms);
                Debug.Log("Current color: Blue");
                break;
            case 3:
                TurnOnPlatforms(GreenPlatforms);
                Debug.Log("Current color: Green");
                break;
            case 4:
                TurnOnPlatforms(YellowPlatforms);
                Debug.Log("Current color: Yellow");
                break;
            default:
                Debug.LogError($"Invalid color: {currentColor}");
                break;
        }
    }
    
    private void TurnOnPlatforms(GameObject[] platforms)
    {
        foreach (var platform in platforms)
        {
            platform.SetActive(true);
            // Set alpha to 1 (fully visible)
            var spriteRenderer = platform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
            // Enable collider
            var collider = platform.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
    
    private void TurnOffAllPlatforms()
    {
        foreach (var platform in RedPlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (var platform in BluePlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (var platform in GreenPlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (var platform in YellowPlatforms)
        {
            DeactivatePlatform(platform);
        }
    }
    
    private void DeactivatePlatform(GameObject platform)
    {
        var spriteRenderer = platform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.05f);
        }
        
        var collider = platform.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void OnDestroy()
    {
        InputManager.Color1 -= SetColor1;
        InputManager.Color2 -= SetColor2;
        InputManager.Color3 -= SetColor3;
        InputManager.Color4 -= SetColor4;
    }
}
