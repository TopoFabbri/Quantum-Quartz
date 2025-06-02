using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Code.Scripts.Player;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Scripts.Tools
{
    [CreateAssetMenu(menuName = "Custom/VelocityCurve", fileName = "VelocityCurve", order = 0)]
    public class VelocityCurve : ScriptableObject
    {
        [SerializeField] private bool upwards = true;
        [Range(0.001f, 60)]
        [SerializeField] private float heightScale = 1;
        [ToggleShow(nameof(upwards), true)]
        [SerializeField] private bool scaleDuration = false;
        [Range(0.001f, 5)]
        [SerializeField] private float duration = 1;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private AnimationCurve velocityCurve;
        [SerializeField] private AnimationCurve accelerationCurve;

        [HideInInspector]
        [SerializeField] private float lastDuration = 1;
        [HideInInspector]
        [SerializeField] private float lastHeightScale = 1;

        public float HeightScale => heightScale;
        public float Duration => duration;

        private void OnValidate()
        {
            Keyframe key = positionCurve.keys[0];
            key.time = 0;
            key.value = 0;
            positionCurve.MoveKey(0, key);

            if (lastDuration != duration || lastHeightScale != heightScale)
            {
                Vector2 scaling = new Vector2(duration / lastDuration, heightScale / lastHeightScale);
                List<Keyframe> tempKeys = positionCurve.keys.ToList();

                if (upwards || scaleDuration)
                {
                    //Scale duration
                    for (int i = 0; i < tempKeys.Count; i++)
                    {
                        Keyframe tempKey = tempKeys[i];
                        tempKey.time *= scaling.x;
                        tempKeys[i] = tempKey;
                    }
                }
                else
                {
                    //Crop duration
                    for (int i = 0; i < positionCurve.keys.Length; i++)
                    {
                        Keyframe tempKey = tempKeys[i];
                        if (tempKey.time > duration)
                        {
                            tempKey.time = duration;
                            tempKey.value = positionCurve.Evaluate(duration);
                            tempKeys[i] = tempKey;
                            tempKeys = tempKeys.GetRange(0, i);
                            break;
                        }
                    }

                    if (lastDuration < duration)
                    {
                        int lastIdx = positionCurve.keys.Length - 1;
                        tempKeys.Add(new Keyframe(duration, positionCurve[lastIdx].value));
                        /* Free tangents make derivative less precise
                        AnimationUtility.SetKeyRightTangentMode(positionCurve, lastIdx, AnimationUtility.TangentMode.Free);
                        Keyframe temp = positionCurve.keys[lastIdx];
                        temp.outTangent = 0;
                        positionCurve.MoveKey(lastIdx, temp);
                        */
                    }
                }

                // Scale height and tangents
                for (int i = 0; i < tempKeys.Count; i++)
                {
                    Keyframe tempKey = tempKeys[i];
                    tempKey.value *= scaling.y;
                    tempKey.inTangent *= scaling.y / scaling.x;
                    tempKey.outTangent *= scaling.y / scaling.x;
                    tempKeys[i] = tempKey;
                }

                // Update the keyframes
                positionCurve.keys = tempKeys.ToArray();
                lastDuration = duration;
                lastHeightScale = heightScale;
            }

            int last = positionCurve.keys.Length - 1;
            key = positionCurve.keys[last];
            key.time = duration;
            key.value = (upwards ? 1 : -1) * heightScale;
            positionCurve.MoveKey(last, key);

            velocityCurve = positionCurve?.Derivative(50, 0.0000003f, 3);
            accelerationCurve = positionCurve?.SecondDerivative(50, 0.0000003f, 3);
            //thirdCurve = positionCurve?.ThirdDerivative(50, 0.0000003f, 3);
        }

        public float SamplePosition(float time)
        {
            return ExtendedEvaluate(time, positionCurve, velocityCurve);
        }

        public float SampleVelocity(float time)
        {
            return ExtendedEvaluate(time, velocityCurve, accelerationCurve);
        }

        public float SampleAcceleration(float time)
        {
            return ExtendedEvaluate(time, accelerationCurve, null);
        }

        private float ExtendedEvaluate(float time, AnimationCurve curve, AnimationCurve derivative)
        {
            float point = curve.Evaluate(Mathf.Min(duration, time));
            if (time <= duration)
            {
                return point;
            }
            else
            {
                float extension = time - duration;
                return point + (derivative == null ? 0 : derivative.Evaluate(duration) * extension);
            }
        }

        public void Draw(Vector3 origin, float customWidth = float.NaN, Vector2? right = null, Vector2? up = null)
        {
            float width = float.IsNaN(customWidth) ? duration : customWidth;
            positionCurve.DrawCurveGizmos(origin, new Vector2(width / duration, 1), Color.blue, right, up);
        }

        public void DrawExtra(Vector3 origin, float customHeight = float.NaN, float customWidth = float.NaN, Vector2? right = null, Vector2? up = null)
        {
            float width = float.IsNaN(customWidth) ? duration : customWidth;
            float height = float.IsNaN(customHeight) ? heightScale : customHeight;
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

            velocityCurve.DrawCurveGizmos(origin, new Vector2(width / duration, (height / heightScale) / GetHeight(velocityCurve)), Color.green, right, up);
            accelerationCurve.DrawCurveGizmos(origin, new Vector2(width / duration, (height / heightScale) / GetHeight(accelerationCurve)), Color.red, right, up);
        }
    }

    [ExecuteInEditMode]
    public class VelocityCurveVisualizer : MonoBehaviour
    {
        public PlayerState playerState;

        public VelocityCurve jumpCurve;
        public VelocityCurve fallCurve;
        public bool showExtra = false;
        public bool useCustomWidth = false;
        [ToggleShow(nameof(useCustomWidth))]
        public float customWidthScale = 1;

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
            if (jumpCurve || fallCurve)
            {
                float jumpDuration = jumpCurve?.Duration ?? 0;
                float fallDuration = fallCurve?.Duration ?? 0;
                float width = (useCustomWidth ? customWidthScale : 1) * (jumpDuration + fallDuration);
                float jumpWidth = width * jumpDuration / (jumpDuration + fallDuration);
                float height = jumpCurve?.SamplePosition(jumpDuration) ?? 0;
                Vector3 fallOrigin = transform.position + (jumpCurve ? transform.right * jumpWidth + transform.up * height : Vector3.zero);

                jumpCurve?.Draw(transform.position, jumpWidth, transform.right, transform.up);
                fallCurve?.Draw(fallOrigin, width - jumpWidth, transform.right, transform.up);


                if (playerState?.sharedContext != null)
                {
                    if (!playerState.sharedContext.Falling)
                    {
                        if (jumpCurve)
                        {
                            float progress = playerState.sharedContext.jumpFallTime / jumpCurve.Duration;
                            Vector3 curPoint = transform.position + transform.right * jumpWidth * progress + transform.up * jumpCurve.SamplePosition(playerState.sharedContext.jumpFallTime);
                            Debug.DrawRay(curPoint, transform.right * -10, Color.cyan);
                        }
                    }
                    else
                    {
                        if (fallCurve)
                        {
                            float progress = playerState.sharedContext.jumpFallTime / fallCurve.Duration;
                            Vector3 curPoint = fallOrigin + transform.right * (width - jumpWidth) * progress + transform.up * fallCurve.SamplePosition(playerState.sharedContext.jumpFallTime);
                            Debug.DrawRay(curPoint, transform.right * -10, Color.cyan);
                        }
                    }
                }

                if (showExtra)
                {
                    jumpCurve?.DrawExtra(transform.position, float.NaN, jumpWidth, transform.right, transform.up);
                    fallOrigin = transform.position + (jumpCurve ? transform.right * jumpWidth : Vector3.zero);
                    fallCurve?.DrawExtra(fallOrigin, jumpCurve?.HeightScale ?? float.NaN, width - jumpWidth, transform.right, transform.up);
                }
            }
        }
#endif
    }
}
