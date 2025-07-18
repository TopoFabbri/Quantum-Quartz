using Code.Scripts.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Code.Scripts.Level
{
    public class ChainMaker : MonoBehaviour
    {
        [HeaderPlus("Size")]
        [SerializeField] private int chainLength = 3;
        [SerializeField] private float linkSize = 1f;

        [HeaderPlus("Style")]
        [SerializeField] private Sprite linkSprite;
        [SerializeField] private Material linkMaterial;
        [SerializeField] private bool hasEnd;
        [SerializeField] private InteractableComponent interactable;
        [SerializeField] private int linkOrderInLayer = -1;

        private readonly List<GameObject> links = new();
        private readonly List<HingeJoint2D> hinges = new();
        private GameObject end;
        private DistanceJoint2D joint;
        private Rigidbody2D rb;

#if UNITY_EDITOR
        [HeaderPlus("Generation")]
        [InspectorButton("Clear")]
        private void ClearChain()
        {
            hinges.Clear();

            foreach (GameObject link in links)
                DestroyImmediate(link, true);

            if (end)
                DestroyImmediate(end, true);

            if (!joint)
                joint = GetComponent<DistanceJoint2D>();

            if (joint)
                DestroyImmediate(joint, true);

            if (!rb)
                rb = GetComponent<Rigidbody2D>();

            if (rb)
                DestroyImmediate(rb, true);

            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject, true);

            links.Clear();
        }

        [InspectorButton("Generate")]
        private void GenerateChain()
        {
            if (!linkSprite || (!interactable && hasEnd))
            {
                Debug.LogError("No sprite or end provided");
                return;
            }

            ClearChain();

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

        [MenuItem("Tools/Custom/Generate All Chains")]
        private static void GenerateAllChains()
        {
            List<ChainMaker> chainObjs;
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                //In Prefab Mode
                chainObjs = PrefabStageUtility.GetCurrentPrefabStage().FindComponentsOfType<ChainMaker>().ToList();
            }
            else if (Selection.gameObjects.Length > 0 && Selection.gameObjects.Any((item) => item.GetComponentInChildren<ChainMaker>(true) != null))
            {
                //Selecting ChainMaker GameObjects
                chainObjs = new List<ChainMaker>();
                foreach (GameObject go in Selection.gameObjects)
                {
                    chainObjs.AddRange(go.GetComponentsInChildren<ChainMaker>(true));
                }
            }
            else
            {
                //In Scene Mode not selecting anything
                chainObjs = GameObject.FindObjectsOfType<ChainMaker>(true).ToList();
            }

            foreach (ChainMaker chainMaker in chainObjs)
            {
                chainMaker.GenerateChain();
            }
        }

        private void GenerateEnd()
        {
            end = Instantiate(interactable).gameObject;
            end.name = "End";
            end.transform.parent = transform;

            joint = gameObject.AddComponent<DistanceJoint2D>();

            end.AddComponent<Rigidbody2D>();
            joint.connectedAnchor = new Vector2(0f, .5f);
            joint.maxDistanceOnly = true;

            end.transform.position = transform.position + Vector3.down * (linkSize * chainLength + .5f);
        
            CircleCollider2D endCollider = end.GetComponent<CircleCollider2D>();
            Rigidbody2D endRb = end.GetComponent<Rigidbody2D>();

            joint.connectedBody = endRb;
            endCollider.isTrigger = true;
            endRb.mass = 10f;
        
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
        
            SpriteRenderer tmpLinkRenderer = tempLink.AddComponent<SpriteRenderer>();

            tmpLinkRenderer.sprite = linkSprite;
            tmpLinkRenderer.sharedMaterial = linkMaterial;
            tmpLinkRenderer.sortingOrder = linkOrderInLayer;
            
            Rigidbody2D tempRb = tempLink.GetComponent<Rigidbody2D>();

            tempRb.angularDrag = 0f;
            tempRb.drag = 5f;
        
            if (i > 0)
            {
                hinges[i - 1].connectedBody = tempRb;
            }
            else
            {
                HingeJoint2D startHinge = tempLink.AddComponent<HingeJoint2D>();

                startHinge.limits = new JointAngleLimits2D { min = 0f, max = 0f };

                rb = gameObject.AddComponent<Rigidbody2D>();

                rb.bodyType = RigidbodyType2D.Static;

                startHinge.connectedBody = rb;
                startHinge.anchor = Vector2.up * linkSize / 2f;
            }
        }
#endif
    }
}