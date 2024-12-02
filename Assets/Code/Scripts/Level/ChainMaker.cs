using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Level
{
    [ExecuteInEditMode]
    public class ChainMaker : MonoBehaviour
    {
        [Header("Size")] [SerializeField] private int chainLength = 3;
        [SerializeField] private float linkSize = 1f;

        [Header("Style")] [SerializeField] private Sprite linkSprite;
        [SerializeField] private bool hasEnd;
        [SerializeField] private Sprite endSprite;

        [Header("Generation")] [SerializeField]
        private bool generate;

        [SerializeField] private bool clear;

        private readonly List<GameObject> links = new();
        private readonly List<HingeJoint2D> hinges = new();
        private GameObject end;
        private DistanceJoint2D joint;
        private Rigidbody2D rb;

        private void Update()
        {
            if (clear)
            {
                clear = false;
                ClearChain();
            }

            if (!generate)
                return;

            generate = false;

            if (!linkSprite || (!endSprite && hasEnd))
            {
                Debug.LogError("No sprite provided");
                return;
            }

            ClearChain();

            GenerateChain();
        }

        private void ClearChain()
        {
            hinges.Clear();

            foreach (GameObject link in links)
                DestroyImmediate(link);

            if (end)
                DestroyImmediate(end);

            if (!joint)
                joint = GetComponent<DistanceJoint2D>();

            if (joint)
                DestroyImmediate(joint);

            if (!rb)
                rb = GetComponent<Rigidbody2D>();

            if (rb)
                DestroyImmediate(rb);

            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            links.Clear();
        }

        private void GenerateChain()
        {
            for (int i = 0; i < chainLength; i++)
            {
                GameObject tempLink = new()
                {
                    transform =
                    {
                        parent = transform
                    }
                };

                SetupLink(tempLink, i);
            }

            if (hasEnd)
                GenerateEnd();
        }

        private void GenerateEnd()
        {
            end = new GameObject
            {
                name = "End",
                transform =
                {
                    parent = transform
                }
            };

            joint = gameObject.AddComponent<DistanceJoint2D>();

            joint.connectedBody = end.AddComponent<Rigidbody2D>();
            joint.connectedAnchor = new Vector2(0f, .5f);
            joint.maxDistanceOnly = true;

            end.AddComponent<SpriteRenderer>().sprite = endSprite;
            end.AddComponent<CircleCollider2D>();
            end.AddComponent<ChainEndController>();
            end.transform.position = transform.position + Vector3.down * (linkSize * chainLength + .5f);
        
            CircleCollider2D endCollider = end.GetComponent<CircleCollider2D>();
            Rigidbody2D endRb = end.GetComponent<Rigidbody2D>();
        
            endCollider.isTrigger = true;
            endRb.mass = 10f;
            endRb.angularDrag = .2f;
        
            hinges.Last().connectedBody = end.GetComponent<Rigidbody2D>();
        }

        private void SetupLink(GameObject tempLink, int i)
        {
            tempLink.name = "Link " + i;
            tempLink.transform.localPosition = Vector3.down * (linkSize * i + linkSize / 2f);

            links.Add(tempLink);

            hinges.Add(tempLink.AddComponent<HingeJoint2D>());

            hinges[i].anchor = Vector2.down * linkSize / 2f;
            hinges[i].limits = new JointAngleLimits2D { min = -90f, max = 90f };
            hinges[i].useLimits = true;
        
            tempLink.AddComponent<SpriteRenderer>().sprite = linkSprite;
        
            if (i > 0)
            {
                tempLink.AddComponent<Rigidbody2D>();

                hinges[i - 1].connectedBody = tempLink.GetComponent<Rigidbody2D>();
            }
            else
            {
                HingeJoint2D startHinge = tempLink.AddComponent<HingeJoint2D>();

                rb = gameObject.AddComponent<Rigidbody2D>();

                rb.bodyType = RigidbodyType2D.Static;

                startHinge.connectedBody = rb;
                startHinge.anchor = Vector2.up * linkSize / 2f;
            }
        }
    }
}