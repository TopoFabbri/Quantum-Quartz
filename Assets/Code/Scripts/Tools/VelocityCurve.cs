using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
    [CreateAssetMenu(menuName = "Custom/VelocityCurve", fileName = "VelocityCurve", order = 0)]
    public class VelocityCurve : ScriptableObject
    {
        [SerializeField] private bool upwards = true;
        [SerializeField] private float heightScale = 1;
        [ToggleShow(nameof(upwards), true)]
        [SerializeField] private bool scaleDuration = false;
        [SerializeField] private float duration = 1;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private AnimationCurve velocityCurve;
        [SerializeField] private AnimationCurve accelerationCurve;

        [HideInInspector]
        [SerializeField] private float lastDuration = 1;

        public float Duration => duration;

        private void OnValidate()
        {
            Keyframe key = positionCurve.keys[0];
            key.time = 0;
            key.value = 0;
            positionCurve.MoveKey(0, key);

            if (lastDuration != duration)
            {
                if (!upwards && !scaleDuration)
                {
                    float scaling = lastDuration / duration;
                    bool crop = false;
                    Keyframe[] tempKeys = positionCurve.keys;
                    for (int i = 0; i < positionCurve.keys.Length; i++)
                    {
                        if (!crop)
                        {
                            tempKeys[i].time *= scaling;
                            if (tempKeys[i].time > 1)
                            {
                                tempKeys[i].time = 1;
                                tempKeys[i].value = positionCurve.Evaluate(1 / scaling);
                                crop = true;
                                continue;
                            }
                        }
                        else
                        {
                            tempKeys = tempKeys.ToList().GetRange(0, i).ToArray();
                            break;
                        }
                    }
                    positionCurve.keys = tempKeys;

                    if (lastDuration < duration)
                    {
                        int lastIdx = positionCurve.keys.Length - 1;
                        positionCurve.AddKey(1, positionCurve[lastIdx].value);
                        /* Free tangents make derivative less precise
                        AnimationUtility.SetKeyRightTangentMode(positionCurve, lastIdx, AnimationUtility.TangentMode.Free);
                        Keyframe temp = positionCurve.keys[lastIdx];
                        temp.outTangent = 0;
                        positionCurve.MoveKey(lastIdx, temp);
                        */
                    }
                }
                lastDuration = duration;
            }

            int last = positionCurve.keys.Length - 1;
            key = positionCurve.keys[last];
            key.time = 1;
            if (upwards)
            {
                key.value = 1;
            }
            positionCurve.MoveKey(last, key);

            velocityCurve = positionCurve?.Derivative(50, 0.0000003f, 3);
            accelerationCurve = positionCurve?.SecondDerivative(50, 0.0000003f, 3);
            //thirdCurve = positionCurve?.ThirdDerivative(50, 0.0000003f, 3);
        }

        public float SamplePosition(float time)
        {
            return positionCurve.Evaluate(time / duration) * heightScale;
        }

        public float SampleVelocity(float time)
        {
            return velocityCurve.Evaluate(time / duration) * heightScale;
        }

        public float SampleAcceleration(float time)
        {
            return accelerationCurve.Evaluate(time / duration) * heightScale;
        }

        public void Draw(Vector3 origin, bool showExtra, float customWidth = float.NaN)
        {
            float width = float.IsNaN(customWidth) ? duration : customWidth;
            positionCurve.DrawCurveGizmos(origin, new Vector2(width, heightScale), Color.blue);
            if (showExtra)
            {
                float GetHeight(AnimationCurve curve)
                {
                    float max = float.MinValue;
                    float min = float.MaxValue;
                    foreach (Keyframe key in curve.keys)
                    {
                        max = Mathf.Max(max, key.value);
                        min = Mathf.Min(min, key.value);
                    }
                    return max - min;
                }

                velocityCurve.DrawCurveGizmos(origin, new Vector2(width, heightScale / GetHeight(velocityCurve)), Color.green);
                accelerationCurve.DrawCurveGizmos(origin, new Vector2(width, heightScale / GetHeight(accelerationCurve)), Color.red);
            }
        }
    }

    [ExecuteInEditMode]
    public class VelocityCurveVisualizer : MonoBehaviour
    {
        public VelocityCurve jumpCurve;
        public VelocityCurve fallCurve;
        public bool showExtra = false;
        public bool useCustomWidth = false;
        [ToggleShow(nameof(useCustomWidth))]
        public float customWidth = 0;

#if UNITY_EDITOR
        [MenuItem("GameObject/Create Custom/Velocity Curve Visualizer", false, 10)]
        static void Create(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("Velocity Curve Visualizer", typeof(VelocityCurveVisualizer));
            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        private void OnDrawGizmos()
        {
            float width = useCustomWidth ? customWidth : (jumpCurve?.Duration ?? 0 + fallCurve?.Duration ?? 0);
            float jumpWidth = jumpCurve ? width * jumpCurve.Duration / (jumpCurve.Duration + (fallCurve?.Duration ?? 0)) : 0;
            jumpCurve?.Draw(transform.position, showExtra, jumpWidth);
            float height = jumpCurve?.SamplePosition(jumpCurve.Duration) ?? 0;
            Vector3 fallOrigin = transform.position + (jumpCurve ? transform.right * jumpWidth + transform.up * height : Vector3.zero);
            fallCurve?.Draw(fallOrigin, showExtra, width - jumpWidth);

            
        }
#endif
    }
}
