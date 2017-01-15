using System;
using System.Globalization;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Int extensions methods. </summary>
    public static class IntExtensions
    {
        private static readonly double _Log2 = Math.Log(2);

        /// <summary> Clamps given value by given max and min. </summary>
        /// <param name="value">Value to clamp.</param>
        /// <param name="min">Min clamping bound.</param>
        /// <param name="max">Max clamping bound</param>
        /// <returns>Clamped value.</returns>
        public static int Clamp(this int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        ///     Gets digit at the specified position for a positional notation, based on the number 10.
        /// </summary>
        /// <param name="value">The number to get a digit from.</param>
        /// <param name="position">The position in the decimal representation.</param>
        /// <param name="indexStartsAtMostSignificantDigit">
        ///     if set to <c>true</c>, will count digits from the
        ///     most significant digit down to less significant digits;
        ///     otherwise, will start at the lowest digit and work upwards.
        /// </param>
        /// <param name="digitsAreAlwaysPositive">
        ///     If set to <c>true</c>, will return positive digits even for negative
        ///     numbers; otherwise, digits returned for negative numbers will be negative themselves.
        /// </param>
        /// <returns>Digit at the specified position.</returns>
        public static int DigitAtPosition(this int value, int position, bool indexStartsAtMostSignificantDigit = true,
                                          bool digitsAreAlwaysPositive = true)
        {
            if (value == 0) return 0;

            const bool indexStartsAtZero = true;
            if (indexStartsAtZero) position += 1;

            var count = DigitCount(value);
            if (indexStartsAtMostSignificantDigit) position = count - (position - 1);

            if ((position > 0) && (position <= count)) {
                value = value / (int)Math.Pow(10d, position - 1d);
                var digit = value % 10;
                return digitsAreAlwaysPositive ? (digit < 0 ? -digit : digit) : digit;
            }

            return 0;
        }

        /// <summary> Counts the amount of digits in a number, based on the decimal representation. </summary>
        /// <param name="value">The number to count digits in.</param>
        /// <returns> Amount of digits in the number, based on the decimal representation.</returns>
        public static int DigitCount(this int value)
        {
            if (value == 0) return 1;

            var positive = value < 0 ? -value : value;
            return (int)Math.Floor(Math.Log10(positive) + 1);
        }

        /// <summary> Extracts the high part of the given integer. </summary>
        /// <param name="value">The number to get high part for.</param>
        /// <returns>Last 2 bytes of the given number.</returns>
        public static int GetHighWord(this int value)
        {
            return value >> 16; // preserve zeroes would be: value & (0xFFFF << 16)
        }

        /// <summary> Extracts the low part of the given integer. </summary>
        /// <param name="value">The number to get low part for.</param>
        /// <returns>First 2 bytes of the given number.</returns>
        public static int GetLowWord(this int value)
        {
            return value & 0x0000FFFF;
        }

        /// <summary> Determines whether the given bit is set or not. </summary>
        /// <param name="value">The number.</param>
        /// <param name="bitIndex">The bit index.</param>
        /// <returns> <c>true</c> if bit is set; otherwise, <c>false</c>. </returns>
        public static bool IsBitSet(this int value, int bitIndex)
        {
            return (value & (1 << bitIndex)) != 0;
        }

        /// <summary> Determines whether the specified number is even. </summary>
        /// <param name="value">The number to check.</param>
        /// <returns> <c>true</c> if the number is even; otherwise, <c>false</c>. </returns>
        public static bool IsEven(this int value)
        {
            return (value & 1) == 0;
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
            this int value,
            int lowerBound,
            int upperBound,
            int allowance = 0,
            bool inclusive = true)
        {
            var maxBound = Math.Max(lowerBound, upperBound);
            var minBound = Math.Min(lowerBound, upperBound);
            return inclusive
                ? (value <= maxBound + allowance) && (value >= minBound - allowance)
                : (value < maxBound + allowance) && (value > minBound - allowance);
        }

        /// <summary> Returns true if value is a power of two. </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if value is a power of two; false otherwise.</returns>
        public static bool IsPoT(this int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
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
        public static int Modulo(this int a, int m, bool preferPositiveReminder = true)
        {
            Check.NonZero(m);

            int r;
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

        /// <summary> Normalizes value to range [0, 1] from the specified range. </summary>
        /// <param name="value">The value to map to the specified range.</param>
        /// <param name="min">If value is oldMin then 0 is returned.</param>
        /// <param name="max">If value is oldMax then 1 is returned.</param>
        /// <returns>Value mapped to the range [0, 1].</returns>
        public static float Normalize(this int value, int min, int max)
        {
            Check.Less(min, max);

            var normalized = (value - min) / (float)(max - min);
            var first = normalized <= 1 ? normalized : 1;
            return first >= 0 ? first : 0;
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
        public static float Remap(this int value, int oldMin, int oldMax, int newMin, int newMax)
        {
            Check.Less(oldMin, oldMax);
            Check.Less(newMin, newMax);

            return value.Normalize(oldMin, oldMax) * (newMax - newMin) + newMin;
        }

        /// <summary> Sets the given bit. </summary>
        /// <param name="value">The number to change.</param>
        /// <param name="bitIndex">The bit to set.</param>
        /// <param name="bitFlag">Bit flag to set.</param>
        /// <returns>Integer with bit changed.</returns>
        public static int SetBit(this int value, int bitIndex, bool bitFlag)
        {
            return bitFlag ? value | (1 << bitIndex) : value & ~(1 << bitIndex);
        }

        /// <summary> Sets the high word in the integer. </summary>
        /// <param name="value">The number.</param>
        /// <param name="highWord">The value for high word.</param>
        /// <returns>Integer with changed high word.</returns>
        public static int SetHighWord(this int value, int highWord)
        {
            return (value & 0x0000FFFF) + (highWord << 16);
        }

        /// <summary> Sets the low word in the integer. </summary>
        /// <param name="value">The number.</param>
        /// <param name="lowWord">The value for low word.</param>
        /// <returns>Integer with changed low word.</returns>
        public static int SetLowWord(this int value, int lowWord)
        {
            return (int)((value & 0xFFFF0000) + (lowWord & 0x0000FFFF));
        }

        /// <summary> If the specified value is not even, returns the next (bigger) even number. </summary>
        /// <param name="value">The value to test.</param>
        /// <returns> Specified value or next even number.</returns>
        public static int ToEven(this int value)
        {
            return IsEven(value) ? value : value + 1;
        }

        /// <summary> Returns a culture invariant <see cref="string" /> that represents this instance. </summary>
        /// <param name="value">The single to convert to string.</param>
        /// <param name="format">A numeric format string.</param>
        /// <returns> A culture invariant <see cref="string" /> that represents this instance. </returns>
        [NotNull]
        public static string ToInvString(this int value, [CanBeNull] string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary> Rounds value off to the nearest multiple of the specified number. </summary>
        /// <example>
        ///     <code>
        /// int alpha = 123.<see cref="ToMultiple" />(64);
        /// Assert(alpha == 128);
        /// </code>
        /// </example>
        /// <param name="value">The value to round.</param>
        /// <param name="multiple">The number which multiples will be used for rounding.</param>
        /// <returns>Rounded value.</returns>
        public static int ToMultiple(this int value, int multiple)
        {
            if ((value == 0) || (multiple == 0)) return 0;

            return (int)Math.Round(value / (float)multiple, MidpointRounding.AwayFromZero) * multiple;
        }

        /// <summary> Returns the next power of two number, which is bigger than the specified value. </summary>
        /// <param name="value">The value to find bigger power-of-two number.</param>
        /// <returns>Next power-of-two number or zero if it overflows.</returns>
        public static int ToNextPoT(this int value)
        {
            if (IsPoT(value)) return value;

            var result = (int)Math.Pow(2, (int)(Math.Log(value) / _Log2) + 1);
            return result > 0 ? result : 1;
        }

        /// <summary>
        ///     Converts the given number to a string which always has the sign.
        ///     Zero can optionally has a plus sign instead of being signless.
        /// </summary>
        public static string ToSignString(this int value, bool zeroHasSign = false)
        {
            return value.ToString(zeroHasSign ? "+0;-#" : "+#;-#;0");
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
        public static int WrapClamp(this int value, int min, int max)
        {
            if ((min <= value) && (value < max)) return value;

            var rem = (value - min) % (max - min);
            return rem + (rem < 0 ? max : min);
        }
    }
}