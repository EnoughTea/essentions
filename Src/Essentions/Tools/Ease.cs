using System;

namespace Essentions.Tools
{
    /// <summary> Interpolation type. </summary>
    public enum InterpolationKind
    {
        /// <summary> Typical linear interpolation. </summary>
        Linear,

        /// <summary> Sinerp: t = Math.Sin(t * Math.Pi * 0.5). </summary>
        Sin,

        /// <summary> Coserp: t = 1 - Math.Cos(t * Math.Pi * 0.5). </summary>
        Cos,

        /// <summary> Quadratic interpolation: t*t. </summary>
        Quadratic,

        /// <summary> Cubic interpolation: t*t*t. </summary>
        Cubic,

        /// <summary> Smoothstep interpolation: t*t * (3 - 2*t). </summary>
        Smooth,

        /// <summary> "Smootheststep" interpolation: t*t*t * (t * (6*t - 15) + 10). </summary>
        Smoothest
    }

    /// <summary> Contains interpolation methods. </summary>
    public static class Ease
    {
        private const double PiOver2 = System.Math.PI / 2;

        private const float PiOver2f = (float)System.Math.PI / 2f;

        /// <summary> Generic interpolator - from start to end in <paramref name="t" />. </summary>
        /// <param name="start">Starting position</param>
        /// <param name="end">Ending position.</param>
        /// <param name="t">
        ///     Amount to interpolate between the two values. 0 means <paramref name="start" /> and 1 means
        ///     <paramref name="end" />. Must be between 0.0 and 1.0.
        /// </param>
        /// <param name="kind">Interpolation type.</param>
        /// <returns>Position between start and end for the given <paramref name="t" />.</returns>
        /// <exception cref="ArgumentException">Unknown interpolation kind.</exception>
        public static float Do(float start, float end, float t, InterpolationKind kind = InterpolationKind.Linear)
        {
            switch (kind) {
                case InterpolationKind.Linear:
                    break;
                case InterpolationKind.Sin:
                    t = (float)System.Math.Sin(t * PiOver2f);
                    break;
                case InterpolationKind.Cos:
                    t = (float)(1 - System.Math.Cos(t * PiOver2f));
                    break;
                case InterpolationKind.Quadratic:
                    t = t * t;
                    break;
                case InterpolationKind.Cubic:
                    t = t * t * t;
                    break;
                case InterpolationKind.Smooth:
                    t = t * t * (3f - 2f * t);
                    break;
                case InterpolationKind.Smoothest:
                    t = t * t * t * (t * (6f * t - 15f) + 10f);
                    break;
                default:
                    throw new ArgumentException("Unknown interpolation kind.", nameof(kind));
            }

            return (end - start) * t + start;
        }

        /// <summary> Generic interpolator - from start to end in <paramref name="t" />. </summary>
        /// <param name="start">Starting position</param>
        /// <param name="end">Ending position.</param>
        /// <param name="t">
        ///     Amount to interpolate between the two values. 0 means <paramref name="start" /> and 1 means
        ///     <paramref name="end" />. Must be between 0.0 and 1.0.
        /// </param>
        /// <param name="kind">Interpolation type.</param>
        /// <returns>Position between start and end for the given <paramref name="t" />.</returns>
        /// <exception cref="ArgumentException">Unknown interpolation kind.</exception>
        public static double Do(double start, double end, double t,
                                InterpolationKind kind = InterpolationKind.Linear)
        {
            switch (kind) {
                case InterpolationKind.Linear:
                    break;
                case InterpolationKind.Sin:
                    t = System.Math.Sin(t * PiOver2);
                    break;
                case InterpolationKind.Cos:
                    t = 1 - System.Math.Cos(t * PiOver2);
                    break;
                case InterpolationKind.Quadratic:
                    t = t * t;
                    break;
                case InterpolationKind.Cubic:
                    t = t * t * t;
                    break;
                case InterpolationKind.Smooth:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpolationKind.Smoothest:
                    t = t * t * t * (t * (6 * t - 15) + 10);
                    break;
                default:
                    throw new ArgumentException("Unknown interpolation kind.", nameof(kind));
            }

            return (end - start) * t + start;
        }

        /// <summary> Converts time to the interpolation amount in range [0, 1]. </summary>
        /// <param name="currentTime">The current time.</param>
        /// <param name="targetTime">The target time.</param>
        /// <returns>Interpolation amount in range [0, 1].</returns>
        public static float TimePercent(float currentTime, float targetTime)
        {
            if (targetTime == 0) return 1;

            var amount = currentTime / targetTime;
            return amount < 0 ? 0 : amount > 1 ? 1 : amount; // Clamp amount to [0, 1] range.
        }

        /// <summary> Converts time to the interpolation amount in range [0, 1]. </summary>
        /// <param name="currentTime">The current time.</param>
        /// <param name="targetTime">The target time.</param>
        /// <returns>Interpolation amount in range [0, 1].</returns>
        public static double TimePercent(double currentTime, double targetTime)
        {
            if (targetTime == 0) return 1;

            var amount = currentTime / targetTime;
            return amount < 0 ? 0 : amount > 1 ? 1 : amount; // Clamp amount to [0, 1] range.
        }
    }
}