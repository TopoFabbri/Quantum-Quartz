using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    }
}
