using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class FireflyController : MonoBehaviour
{
    const float MIN_SIZE = 2.0f;

    [SerializeField] private Light2D light;
    [SerializeField] private GameObject lightBorder;

    private Vector2 prevSize = Vector2.negativeInfinity;

    private void Reset()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        prevSize = renderer.size;
    }

    private void OnDrawGizmosSelected()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer.size.x < MIN_SIZE || renderer.size.y < MIN_SIZE)
        {
            renderer.size = new Vector2(Mathf.Max(renderer.size.x, MIN_SIZE), Mathf.Max(renderer.size.y, MIN_SIZE));
        }

        if (prevSize == Vector2.negativeInfinity)
        {
            prevSize = renderer.size;
        }
        else if (Vector2.Distance(renderer.size, prevSize) != 0)
        {
            for (int i = 0; i < lightBorder.transform.childCount; i++)
            {
                Transform childTransform = lightBorder.transform.GetChild(i).transform;
                if (childTransform.localScale.x > childTransform.localScale.y)
                {
                    float offset = (renderer.size.y - prevSize.y) * 0.5f * Mathf.Sign(childTransform.localPosition.y);
                    childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y + offset, childTransform.localPosition.z);
                    childTransform.localScale = new Vector3(childTransform.localScale.x * (renderer.size.x / prevSize.x), childTransform.localScale.y, childTransform.localScale.z);
                }
                else
                {
                    float offset = (renderer.size.x - prevSize.x) * 0.5f * Mathf.Sign(childTransform.localPosition.x);
                    childTransform.localPosition = new Vector3(childTransform.localPosition.x + offset, childTransform.localPosition.y, childTransform.localPosition.z);
                    childTransform.localScale = new Vector3(childTransform.localScale.x, childTransform.localScale.y * (renderer.size.y / prevSize.y), childTransform.localScale.z);
                }
            }
            
            prevSize = renderer.size;
        }
    }
}
