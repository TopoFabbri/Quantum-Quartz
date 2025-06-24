using Code.Scripts.Colors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class FireflyController : MonoBehaviour
{
    const float MIN_SIZE = 2.0f;

    [SerializeField] private ColorSwitcher.QColor color = ColorSwitcher.QColor.Blue;
    [SerializeField] private new Light2D light;
    [SerializeField] private GameObject lightBorder;
    [HideInInspector]
    [SerializeField] private Vector2 borderPadding = Vector2.zero;

    private Vector2 prevSize = Vector2.negativeInfinity;
    private Transform player;
    private new SpriteRenderer renderer;
    private new BoxCollider2D collider;

    private void Reset()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        prevSize = renderer.size;
        collider.size = renderer.size;

#if UNITY_EDITOR
        EditorUtility.SetDirty(collider);
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        if (renderer.size.x < MIN_SIZE || renderer.size.y < MIN_SIZE)
        {
            renderer.size = new Vector2(Mathf.Max(renderer.size.x, MIN_SIZE), Mathf.Max(renderer.size.y, MIN_SIZE));
        }

        if (float.IsInfinity(prevSize.magnitude))
        {
            prevSize = renderer.size;
        }
        else
        {
            Vector2 size = Vector2.Distance(renderer.size, prevSize) > Vector2.Distance(collider.size, prevSize) ? renderer.size : collider.size;

            if (Vector2.Distance(size, prevSize) != 0)
            {
                for (int i = 0; i < lightBorder.transform.childCount; i++)
                {
                    Transform childTransform = lightBorder.transform.GetChild(i).transform;
                    if (Mathf.Abs(childTransform.localPosition.x) < Mathf.Abs(childTransform.localPosition.y))
                    {
                        float offset = (size.y - prevSize.y) * 0.5f * Mathf.Sign(childTransform.localPosition.y);
                        childTransform.localPosition = new Vector3(childTransform.localPosition.x, childTransform.localPosition.y + offset, childTransform.localPosition.z);
                        childTransform.localScale = new Vector3(childTransform.localScale.x * (size.x / prevSize.x), childTransform.localScale.y, childTransform.localScale.z);
                    }
                    else
                    {
                        float offset = (size.x - prevSize.x) * 0.5f * Mathf.Sign(childTransform.localPosition.x);
                        childTransform.localPosition = new Vector3(childTransform.localPosition.x + offset, childTransform.localPosition.y, childTransform.localPosition.z);
                        childTransform.localScale = new Vector3(childTransform.localScale.x * (size.y / prevSize.y), childTransform.localScale.y, childTransform.localScale.z);
                    }
                }

                prevSize = size;
                renderer.size = size;
                collider.size = size;

                EditorUtility.SetDirty(renderer);
                EditorUtility.SetDirty(collider);
                EditorUtility.SetDirty(this);
            }
        }
    }
#endif

    private void Start()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (player)
        {
            float xOffset = renderer.size.x * 0.5f - borderPadding.x;
            float yOffset = renderer.size.y * 0.5f - borderPadding.y;
            light.transform.position = new Vector2(
                Mathf.Clamp(player.position.x, transform.position.x - xOffset, transform.position.x + xOffset),
                Mathf.Clamp(player.position.y, transform.position.y - yOffset, transform.position.y + yOffset)
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger || !col.CompareTag("Player"))
            return;

        light.enabled = true;
        player = col.transform;
        ColorSwitcher.Instance.LockColor(color);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.isTrigger || !col.CompareTag("Player"))
            return;

        light.enabled = false;
        light.transform.localPosition = Vector3.zero;
        player = null;
        ColorSwitcher.Instance.UnlockColor(color);
    }
}
