namespace Essentions
{
    /// <summary> Extension methods for <see cref="long" />. </summary>
    public static class LongExtensions
    {
        /// <summary> Extracts the high part of the given long integer. </summary>
        /// <param name="value">The number to get high part for.</param>
        /// <returns>Last 4 bytes of the given number.</returns>
        public static int GetHighInt(this long value)
        {
            return (int)(value >> 32);
        }

        /// <summary> Extracts the low part of the given long integer. </summary>
        /// <param name="value">The number to get low part for.</param>
        /// <returns>First 4 bytes of the given number.</returns>
        public static int GetLowInt(this long value)
        {
            const long lowMask = (1L << 32) - 1;
            return (int)(value & lowMask);
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
        public static long Modulo(this long a, long m, bool preferPositiveReminder = true)
        {
            Check.NonZero(m);

            long r;
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
    }
}