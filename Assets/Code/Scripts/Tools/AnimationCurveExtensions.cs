using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Tools
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Computes a smooth and reduced Hermite spline representing the derivative of an input Hermite spline.
        /// Useful for computing velocity or rate of change over time from a value-time spline.
        /// </summary>
        /// <param name="curve">The input AnimationCurve (Hermite spline style).</param>
        /// <param name="maxSamples">How many dense samples to generate per keyframe segment. Higher = smoother.</param>
        /// <param name="maxError">Allowed deviation before keeping a key (lower = more accurate, higher = fewer keys).</param>
        /// <param name="smoothingWindow">Number of neighbors to use when smoothing tangents. Larger = smoother result.</param>
        /// <returns>A reduced and smoothed AnimationCurve representing the derivative of the input curve.</returns>
        public static AnimationCurve Derivative(this AnimationCurve curve, int maxSamples = 50, float maxError = 0.008f, int smoothingWindow = 3)
        {
            // Ensure the input is valid (at least two keyframes are required)
            if (curve == null || curve.length < 2)
                return new AnimationCurve();

            List<Keyframe> originalKeyframes = curve.keys.ToList();

            // Step 1: Sample a dense derivative curve from the Hermite spline
            List<Keyframe> denseDerivativeSamples = SampleDerivative(originalKeyframes, maxSamples);

            // Step 2: Smooth the tangents in the dense derivative samples to remove jagged noise
            float[] smoothedTangents = SmoothTangents(denseDerivativeSamples, smoothingWindow);

            // Step 3: Apply the smoothed tangents back into the dense curve
            ApplyTangents(denseDerivativeSamples, smoothedTangents);

            // Step 4: Reduce the number of keys based on how flat the derivative is (i.e., insignificant change)
            List<Keyframe> reducedCurve = ReduceKeys(denseDerivativeSamples, maxError);

            return new AnimationCurve(reducedCurve.ToArray());
        }

        /// <summary>
        /// Computes a smooth and reduced Hermite spline representing the second derivative of an input Hermite spline.
        /// Useful for computing acceleration or curvature over time from a value-time spline.
        /// </summary>
        /// <param name="curve">The input AnimationCurve (Hermite spline style).</param>
        /// <param name="maxSamples">How many dense samples to generate per keyframe segment. Higher = smoother.</param>
        /// <param name="maxError">Allowed deviation before keeping a key (lower = more accurate, higher = fewer keys).</param>
        /// <param name="smoothingWindow">Number of neighbors to use when smoothing tangents. Larger = smoother result.</param>
        /// <returns>A reduced and smoothed AnimationCurve representing the second-order derivative of the input curve.</returns>
        public static AnimationCurve SecondDerivative(this AnimationCurve curve, int maxSamples = 50, float maxError = 0.008f, int smoothingWindow = 3)
        {
            if (curve == null || curve.length < 2)
                return new AnimationCurve();

            List<Keyframe> originalKeyframes = curve.keys.ToList();

            // Step 1: Sample dense second derivative curve using Hermite basis function second derivatives
            List<Keyframe> denseSecondDerivative = SampleSecondDerivative(originalKeyframes, maxSamples);

            // Step 2: Smooth tangents to preserve visual continuity
            float[] smoothedTangents = SmoothTangents(denseSecondDerivative, smoothingWindow);
            ApplyTangents(denseSecondDerivative, smoothedTangents);

            // Step 3: Reduce number of keys based on local curvature flatness
            List<Keyframe> reducedCurve = ReduceKeys(denseSecondDerivative, maxError);

            return new AnimationCurve(reducedCurve.ToArray());
        }

        /// <summary>
        /// Computes a smooth and reduced Hermite spline representing the third derivative of an input Hermite spline.
        /// Useful for computing "jerk" or the rate of change of acceleration over time from a value-time spline.
        /// </summary>
        /// <param name="curve">The input AnimationCurve (Hermite spline style).</param>
        /// <param name="maxSamples">How many dense samples to generate per keyframe segment. Higher = smoother.</param>
        /// <param name="maxError">Allowed deviation before keeping a key (lower = more accurate, higher = fewer keys).</param>
        /// <param name="smoothingWindow">Number of neighbors to use when smoothing tangents. Larger = smoother result.</param>
        /// <returns>A reduced and smoothed AnimationCurve representing the third-order derivative of the input curve.</returns>
        public static AnimationCurve ThirdDerivative(this AnimationCurve curve, int maxSamples = 50, float maxError = 0.008f, int smoothingWindow = 3)
        {
            if (curve == null || curve.length < 2)
                return new AnimationCurve();

            List<Keyframe> originalKeyframes = curve.keys.ToList();

            // Step 1: Sample dense third derivative curve using Hermite basis function third derivatives
            List<Keyframe> denseThirdDerivative = SampleThirdDerivative(originalKeyframes, maxSamples);

            // Step 2: Smooth tangents to preserve visual continuity
            float[] smoothedTangents = SmoothTangents(denseThirdDerivative, smoothingWindow);
            ApplyTangents(denseThirdDerivative, smoothedTangents);

            // Step 3: Reduce number of keys based on local change flatness
            List<Keyframe> reducedCurve = ReduceKeys(denseThirdDerivative, maxError);

            return new AnimationCurve(reducedCurve.ToArray());
        }

        /// <summary>
        /// Samples the derivative of the Hermite spline by evaluating its analytical derivative over a dense grid.
        /// </summary>
        private static List<Keyframe> SampleDerivative(List<Keyframe> keyframes, int maxSamples)
        {
            return SampleHermiteDerivative(keyframes, 1, maxSamples);
        }

        /// <summary>
        /// Samples the second derivative of the Hermite spline by evaluating its analytical second derivative over a dense grid.
        /// </summary>
        private static List<Keyframe> SampleSecondDerivative(List<Keyframe> keyframes, int maxSamples)
        {
            return SampleHermiteDerivative(keyframes, 2, maxSamples);
        }

        /// <summary>
        /// Samples the third derivative of the Hermite spline by evaluating its analytical third derivative over a dense grid.
        /// </summary>
        private static List<Keyframe> SampleThirdDerivative(List<Keyframe> keyframes, int maxSamples)
        {
            return SampleHermiteDerivative(keyframes, 3, maxSamples);
        }

        /// <summary>
        /// Samples the Nth derivative of a Hermite spline by evaluating its analytical derivative over a dense grid.
        /// This function generalizes sampling for first, second, or third derivatives.
        /// </summary>
        /// <param name="keyframes">The input keyframes of the Hermite spline.</param>
        /// <param name="order">Derivative order to compute (1 = first, 2 = second, 3 = third).</param>
        /// <param name="maxSamples">Number of samples per segment for accuracy.</param>
        /// <returns>List of sampled Keyframes representing the Nth derivative of the original spline.</returns>
        private static List<Keyframe> SampleHermiteDerivative(List<Keyframe> keyframes, int order, int maxSamples)
        {
            List<Keyframe> sampledKeys = new List<Keyframe>();

            for (int segmentIndex = 0; segmentIndex < keyframes.Count - 1; segmentIndex++)
            {
                Keyframe keyStart = keyframes[segmentIndex];
                Keyframe keyEnd = keyframes[segmentIndex + 1];

                float tStart = keyStart.time;
                float tEnd = keyEnd.time;
                float duration = tEnd - tStart;

                // Prevent NaN/Infinity due to zero or near-zero duration
                if (duration <= Mathf.Epsilon)
                    continue;

                float valueStart = keyStart.value;
                float valueEnd = keyEnd.value;

                float tangentStart = keyStart.outTangent;
                float tangentEnd = keyEnd.inTangent;

                // Sample across the segment using normalized time [0, 1]
                for (int sampleIndex = 0; sampleIndex < maxSamples; sampleIndex++)
                {
                    float s = (float)sampleIndex / (maxSamples - 1); // Normalized [0..1]
                    float t = tStart + s * duration;                 // Real time value

                    // Get the Nth-order derivatives of the Hermite basis functions
                    float[] d = HermiteBasisDerivative(order, s);

                    // Compute the Nth derivative at this point using scaled basis functions
                    float derivative = (
                        d[0] * valueStart +
                        d[1] * tangentStart * duration +
                        d[2] * valueEnd +
                        d[3] * tangentEnd * duration
                    ) / Mathf.Pow(duration, order);

                    sampledKeys.Add(new Keyframe(t, derivative, 0f, 0f));
                }
            }

            return sampledKeys;
        }

        /// <summary>
        /// Returns the Nth-order derivatives of Hermite basis functions at a normalized parameter s.
        /// Supports up to the 3rd derivative. Beyond that, values are zero (cubic spline).
        /// </summary>
        /// <param name="order">Derivative order (1 = first, 2 = second, 3 = third).</param>
        /// <param name="s">Normalized time within the segment [0, 1].</param>
        /// <returns>Array of 4 floats corresponding to the derivatives of h00, h10, h01, h11.</returns>
        private static float[] HermiteBasisDerivative(int order, float s)
        {
            switch (order)
            {
                case 1:
                    return new float[]
                    {
                6f * s * s - 6f * s,        // h00'
                3f * s * s - 4f * s + 1f,   // h10'
                -6f * s * s + 6f * s,       // h01'
                3f * s * s - 2f * s         // h11'
                    };
                case 2:
                    return new float[]
                    {
                12f * s - 6f,              // h00''
                6f * s - 4f,               // h10''
                -12f * s + 6f,             // h01''
                6f * s - 2f                // h11''
                    };
                case 3:
                    return new float[]
                    {
                12f,                      // h00'''
                6f,                       // h10'''
                -12f,                     // h01'''
                6f                        // h11'''
                    };
                default:
                    return new float[] { 0f, 0f, 0f, 0f }; // All higher derivatives are 0 for cubic Hermite splines
            }
        }

        //// <summary>
        /// Computes smoothed tangent values for a list of keyframes using a moving average over a window.
        /// Helps reduce noise and sharp transitions in the derivative curves.
        /// </summary>
        /// <param name="keyframes">The list of sampled keyframes to smooth.</param>
        /// <param name="window">Number of neighboring keyframes on each side to include in the smoothing window.</param>
        /// <returns>An array of smoothed tangent values matching the input keyframe count.</returns>
        private static float[] SmoothTangents(List<Keyframe> keyframes, int window)
        {
            int count = keyframes.Count;
            float[] smoothedTangents = new float[count];

            for (int i = 0; i < count; i++)
            {
                int start = Mathf.Max(0, i - window);
                int end = Mathf.Min(count - 1, i + window);

                float sum = 0f;
                int samples = 0;

                for (int j = start + 1; j <= end; j++)
                {
                    float dx = keyframes[j].time - keyframes[j - 1].time;

                    if (dx <= Mathf.Epsilon) // Skip zero or near-zero time differences
                        continue;

                    float dy = keyframes[j].value - keyframes[j - 1].value;

                    float slope = dy / dx;

                    if (!float.IsInfinity(slope) && !float.IsNaN(slope))
                    {
                        sum += slope;
                        samples++;
                    }
                }

                smoothedTangents[i] = samples > 0 ? sum / samples : 0f;
            }

            return smoothedTangents;
        }

        /// <summary>
        /// Applies tangent values to the inTangent and outTangent of each keyframe in the list.
        /// Ensures smooth visual interpolation between keyframes.
        /// </summary>
        /// <param name="keyframes">The list of keyframes to update (tangents are modified in-place).</param>
        /// <param name="tangents">Array of tangent values to assign, one per keyframe.</param>
        private static void ApplyTangents(List<Keyframe> keyframes, float[] tangents)
        {
            for (int i = 0; i < keyframes.Count; i++)
            {
                float safeTangent = tangents[i];
                if (float.IsNaN(safeTangent) || float.IsInfinity(safeTangent))
                    safeTangent = 0f;

                Keyframe key = keyframes[i];
                key.inTangent = safeTangent;
                key.outTangent = safeTangent;
                keyframes[i] = key;
            }
        }


        /// <summary>
        /// Reduces the number of keyframes by removing those whose values can be approximated by neighboring segments
        /// within a specified error tolerance.
        /// </summary>
        /// <param name="keyframes">Input list of dense keyframes to simplify.</param>
        /// <param name="maxError">Maximum allowed deviation from original value before keeping a key.</param>
        /// <returns>A reduced list of keyframes preserving curve shape within error tolerance.</returns>
        private static List<Keyframe> ReduceKeys(List<Keyframe> keyframes, float maxError)
        {
            if (keyframes.Count < 3)
                return new List<Keyframe>(keyframes);

            List<Keyframe> reduced = new List<Keyframe>();
            reduced.Add(keyframes[0]);

            for (int i = 1; i < keyframes.Count - 1; i++)
            {
                Keyframe prev = reduced.Last();
                Keyframe next = keyframes[i + 1];

                float t = keyframes[i].time;
                float expectedValue = Mathf.Lerp(prev.value, next.value, (t - prev.time) / (next.time - prev.time));
                float actualValue = keyframes[i].value;

                if (Mathf.Abs(expectedValue - actualValue) > maxError)
                {
                    reduced.Add(keyframes[i]);
                }
            }

            reduced.Add(keyframes.Last());
            return reduced;
        }

        /// <summary>
        /// Draws the AnimationCurve as a 2D curve in world space using Gizmos (X = time, Y = value).
        /// Useful for visual debugging of splines and their derivatives.
        /// </summary>
        /// <param name="curve">The AnimationCurve to draw.</param>
        /// <param name="origin">World-space origin to start drawing from.</param>
        /// <param name="scale">Scale applied to the curve in world space (X: time scale, Y: value scale).</param>
        /// <param name="color">Color to draw the curve with.</param>
        /// <param name="steps">Number of line segments used to draw the curve. Higher = smoother.</param>
        public static void DrawCurveGizmos(this AnimationCurve curve, Vector3 origin, Vector2 scale, Color color, Vector2? right = null, Vector2? up = null, int steps = 200)
        {
            if (curve == null || curve.length < 2)
                return;

            up = up.HasValue ? up.Value.normalized: Vector2.up;
            right = right.HasValue ? right.Value.normalized : Vector2.right;
            Gizmos.color = color;

            float timeStart = curve.keys[0].time;
            float timeEnd = curve.keys[curve.length - 1].time;
            float timeRange = timeEnd - timeStart;
            float dt = timeRange / steps;

            Vector3 prev = (Vector2)origin + up.Value * curve.Evaluate(timeStart) * scale.y;

            for (int i = 1; i <= steps; i++)
            {
                float t = timeStart + i * dt;
                float value = curve.Evaluate(t);
                Vector3 current = (Vector2)origin + right.Value * (t - timeStart) * scale.x + up.Value * value * scale.y;

                Gizmos.DrawLine(prev, current);
                prev = current;
            }
        }
    }
}
