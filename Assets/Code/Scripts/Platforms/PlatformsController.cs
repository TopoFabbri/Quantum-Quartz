using Code.Scripts.Input;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformsController : MonoBehaviour
{
    [SerializeField] private GameObject[] RedPlatforms;
    [SerializeField] private GameObject[] BluePlatforms;
    [SerializeField] private GameObject[] GreenPlatforms;
    [SerializeField] private GameObject[] YellowPlatforms;
    
    [SerializeField] private float deactivatedAlpha = 0.05f;

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
                break;
            case 2:
                TurnOnPlatforms(BluePlatforms);
                break;
            case 3:
                TurnOnPlatforms(GreenPlatforms);
                break;
            case 4:
                TurnOnPlatforms(YellowPlatforms);
                break;
        }
    }
    
    private void TurnOnPlatforms(GameObject[] platforms)
    {
        foreach (var platform in platforms)
        {
            platform.SetActive(true);
            // Set alpha to 1 (fully visible)
            SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            }
            // Enable collider
            Collider2D collider = platform.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            
            if (platform.TryGetComponent(out ShadowCaster2D shadowCaster2D))
                shadowCaster2D.enabled = true;
        }
    }
    
    private void TurnOffAllPlatforms()
    {
        foreach (GameObject platform in RedPlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (GameObject platform in BluePlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (GameObject platform in GreenPlatforms)
        {
            DeactivatePlatform(platform);
        }
        foreach (GameObject platform in YellowPlatforms)
        {
            DeactivatePlatform(platform);
        }
    }
    
    private void DeactivatePlatform(GameObject platform)
    {
        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, deactivatedAlpha);
        }
        
        Collider2D collider = platform.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        if (platform.TryGetComponent(out ShadowCaster2D shadowCaster2D))
            shadowCaster2D.enabled = false;
    }

    private void OnDestroy()
    {
        InputManager.Color1 -= SetColor1;
        InputManager.Color2 -= SetColor2;
        InputManager.Color3 -= SetColor3;
        InputManager.Color4 -= SetColor4;
    }
}
