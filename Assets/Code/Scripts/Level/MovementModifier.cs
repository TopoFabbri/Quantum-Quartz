using Code.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Level
{
    [CreateAssetMenu(menuName = "Custom/MovementModifier", fileName = "MovementModifier", order = 0)]
    public class MovementModifier : ScriptableObject
    {
        public enum GrabEffect
        {
            Default,
            Disabled,
            Forced
        }

        public float exitTime = 0f;
        public GrabEffect grabEffect = GrabEffect.Default;

        [HeaderPlus("Movement Multipliers")]
        public float accel = 1;
        public float maxSpeed = 1;
        public float minSpeed = 1;
        public float groundFriction = 1;
        public float airFriction = 1;

        [HeaderPlus("Modifier Loss Speed")]
        public float passiveStep = 100;
        public float groundStep = 100;
        public float stopStep = 100;
        public float slowJumpStep = 100;

        public MovementModifier InterpolateTowards(MovementModifier other, float time)
        {
            float t = Mathf.Clamp01(exitTime <= 0 ? 1 : time / exitTime);
            MovementModifier output = Instantiate(this);
            output.exitTime = other.exitTime;
            output.accel = Mathf.Lerp(accel, other.accel, t);
            output.maxSpeed = Mathf.Lerp(maxSpeed, other.maxSpeed, t);
            output.minSpeed = Mathf.Lerp(minSpeed, other.minSpeed, t);
            output.groundFriction = Mathf.Lerp(groundFriction, other.groundFriction, t);
            output.airFriction = Mathf.Lerp(airFriction, other.airFriction, t);
            return output;
        }
    }
}
