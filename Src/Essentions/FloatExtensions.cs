using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="float" />. </summary>
    public static class FloatExtensions
    {
        private const float _180OverPi = 180f / (float)Math.PI;

        private const float Pi = (float)Math.PI;

        private const float Pi2 = 2f * (float)Math.PI;

        private const float PiOver180 = (float)Math.PI / 180f;

        internal const float Tolerance = 1e-6f;

        /// <summary> Clamps given value using specified clamping bounds. </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Min clamping bound.</param>
        /// <param name="max">Max clamping bound</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp(this float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary> Test to see if a value equals zero using epsilon. </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns>True if value nearly equals zero, false otherwise.</returns>
        public static bool EqualsZero(this float value, float epsilon = Tolerance)
        {
            return NearlyEquals(value, 0f, epsilon);
        }

        /// <summary> Finds only the fractional component of a number: always positive. </summary>
        /// <param name="value">The number whose fractional component to get.</param>
        /// <returns>The fractional component of a number: always positive.</returns>
        public static float Frac(this float value)
        {
            var abs = Math.Abs(value);
            return abs - (float)Math.Floor(abs);
        }

        /// <summary> Checks if given values is between bounds with given allowance. </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        /// <param name="allowance">Allowance for bounds.</param>
        /// <param name="inclusive">if set to <c>true</c>, range bounds will be inclusive.</param>
        /// <returns> <c>true</c> if value is in the range; otherwise, <c>false</c>. </returns>
        /// <returns>True, if value lies between bounds; false otherwise.</returns>
        public static bool IsInRange(
            this float value,
            float lowerBound,
            float upperBound,
            float allowance = 0,
            bool inclusive = true)
        {
            Check.Less(lowerBound, upperBound);

            return inclusive
                ? (value <= upperBound + allowance) && (value >= lowerBound - allowance)
                : (value < upperBound + allowance) && (value > lowerBound - allowance);
        }

        /// <summary>
        ///     Finds the remainder after division of one number by another.
        ///     Do not confuse with <c>%</c> operator, often called 'mod'; see remarks.
        /// </summary>
        /// <remarks>
        ///     <example>
        ///         It is easy to see difference with <c>%</c> operator:
        ///         <c>-21.Modulo(4) == 3</c>, because -21 + 4 x 6 is 3. But <c>-21 % 4 == -1</c>.
        ///         Note that <c>-12.Modulo(-10) == 8</c>, which is compliant to number theory.
        ///         But in Python, Ruby, Scala, Scheme and JS <c>-12 mod -10 == -2</c>.
        ///         To chose compliance with these languages, pass <c>false</c> for <paramref name="preferPositiveReminder" />.
        ///     </example>
        /// </remarks>
        /// <param name="a">First value.</param>
        /// <param name="m">Second value.</param>
        /// <param name="preferPositiveReminder">
        ///     If set to <c>true</c>, works be compliant to number theory, where the positive remainder is always chosen.
        ///     Also known as common residue: the value of <c>b</c>, where <c>a=b (mod m)</c>,
        ///     taken to be nonnegative and smaller than <c>m</c>.
        ///     If set to <c>false</c>, will do what most modern modulo implementations do: pick remainder closer to zero
        ///     return a value in the <c>[m+1, 0]</c> range for a negative divisor <c>m</c>.
        /// </param>
        /// <returns>Calculated modulus.</returns>
        public static float Modulo(this float a, float m, bool preferPositiveReminder = true)
        {
            Check.NonZero(m);

            float r;
            if (preferPositiveReminder) {
                if (m < 0) m = -m;
                //puts a in the [-m+1, m-1] range using the remainder operator
                r = a % m;
                r = r < 0 ? r + m : r;
            }
            else {
                //puts a in the [-m+1, m-1] range using the remainder operator
                r = a % m;

                //if the remainder is less than zero, add m to put it in the [0, m-1] range if m is positive
                //if the remainder is greater than zero, add m to put it in the [m-1, 0] range if m is negative
                if (((m > 0) && (r < 0)) || ((m < 0) && (r > 0))) r = r + m;
            }

            return r;
        }

        /// <summary> Checks if two values are considered equal using given absolute epsilon. </summary>
        /// <remarks>
        ///     I really recommend reading following article because it is very informative:
        ///     https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
        /// </remarks>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns>
        ///     True if values are considered equal to each other, false otherwise.
        /// </returns>
        public static bool NearlyEquals(this float a, float b, float epsilon = Tolerance)
        {
            return Tools.Math.WithinEpsilon(a, b, epsilon);
        }

        /// <remarks>
        ///     I really recommend reading following article because it is very informative:
        ///     https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
        /// </remarks>
        public static bool NearlyEqualsUlps(this float a, float b, int maxUlps = 1)
        {
            Check.Positive(maxUlps);

            // Different signs means they do not match:
            if (a < 0 != b < 0) return a == b; // Check for equality to make sure +0==-0

            var diff = BitConverter.DoubleToInt64Bits(a) - BitConverter.DoubleToInt64Bits(b);
            if (diff == long.MinValue) diff = 0;

            diff = Math.Abs(diff);
            return diff <= maxUlps;
        }

        /// <summary> Normalizes value to range [0, 1] from the specified range. </summary>
        /// <param name="value">The value to map to the specified range.</param>
        /// <param name="min">If value is oldMin then 0 is returned.</param>
        /// <param name="max">If value is oldMax then 1 is returned.</param>
        /// <returns>Value mapped to the range [0, 1].</returns>
        public static float Normalize(this float value, float min, float max)
        {
            Check.Less(min, max);

            var normalized = (value - min) / (max - min);
            var first = normalized <= 1f ? normalized : 1f;
            return first >= 0f ? first : 0f;
        }

        /// <summary>
        ///     Rounds up or down to a whole number by using the fractional part of the input value
        ///     as the probability that the value will be rounded up.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <param name="rand">The random generator.</param>
        /// <returns>Rounded value.</returns>
        /// <remarks>
        ///     This is useful if we wish to round values and then sum them without generating a rounding bias.
        ///     For monetary rounding this problem is solved with rounding to e.g. the nearest even number which
        ///     then causes a bias towards even numbers.
        ///     This solution is more appropriate for certain types of scientific values.
        /// </remarks>
        public static float ProbabilisticRound(this float value, [NotNull] Random rand)
        {
            if (value.Equals(0f)) return 0f;

            var abs = Math.Abs(value);
            var integerPart = (float)Math.Floor(abs);
            var fractionalPart = abs - integerPart;

            if (rand.NextDouble() < fractionalPart) return value > 0f ? integerPart + 1f : -(integerPart + 1f);

            return value > 0f ? integerPart : -integerPart;
        }

        /// <summary>
        ///     Remaps the value from its original range to the corresponding location
        ///     in the specified second range.
        /// </summary>
        /// <param name="value">The value in the range [oldMin, oldMax].</param>
        /// <param name="oldMin">The minimum of the starting range.</param>
        /// <param name="oldMax">The maximum of the starting range.</param>
        /// <param name="newMin">The minimum out the resulting range.</param>
        /// <param name="newMax">The maximum out the resulting range.</param>
        /// <returns>Value mapped from the range [oldMin, oldMax] to [newMin, newMax].</returns>
        public static float Remap(this float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            Check.Less(oldMin, oldMax);
            Check.Less(newMin, newMax);

            return value.Normalize(oldMin, oldMax) * (newMax - newMin) + newMin;
        }

        /// <summary> Converts from radians to degrees. </summary>
        /// <param name="value">Angle in radians.</param>
        /// <returns>Angle in degrees.</returns>
        public static float ToDegrees(this float value)
        {
            return value * _180OverPi;
        }

        /// <summary> Returns a culture invariant <see cref="string" /> that represents this instance. </summary>
        /// <param name="value">The single to convert to string.</param>
        /// <param name="format">A numeric format string.</param>
        /// <returns> A culture invariant <see cref="string" /> that represents this instance. </returns>
        [NotNull]
        public static string ToInvString(this float value, [CanBeNull] string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary> Converts from degrees to radians. </summary>
        /// <param name="value">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static float ToRadians(this float value)
        {
            return value * PiOver180;
        }

        /// <summary> Returns the angle expressed in radians between -Pi and Pi. </summary>
        /// <param name="value">The angle in radians by default, but can be optionally treated as degrees.</param>
        /// <param name="degrees">If set to true, angle will be treated as in degrees.</param>
        /// <returns>Angle expressed in radians between -Pi and Pi.</returns>
        public static float WrapAngle(this float value, bool degrees = false)
        {
            if (degrees) value = value * PiOver180;

            value = (float)Math.IEEERemainder(value, Pi2);
            if (value <= -Pi) value += Pi2;
            else if (value > Pi) value -= Pi2;

            return value;
        }

        /// <summary>
        ///     Clamps a value between 2 values, but wraps the value around.
        ///     So that 1 + <paramref name="max" /> would result in 1 + <paramref name="min" />, and
        ///     <paramref name="min" /> - 1 would result in <paramref name="max" /> - 1.
        /// </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>Clamped value.</returns>
        public static float WrapClamp(this float value, float min, float max)
        {
            if ((min <= value) && (value < max)) return value;

            var rem = (value - min) % (max - min);
            return rem + (rem < 0 ? max : min);
        }
    }
}