namespace Essentions.Tools
{
    /// <summary> Contains something math-related. </summary>
    internal static class Math
    {
        /// <summary>
        ///     The number of binary digits used to represent the binary number for a single precision floating
        ///     point value. i.e. there are this many digits used to represent the
        ///     actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        private const int SingleWidth = 24;

        /// <summary>
        ///     The number of binary digits used to represent the binary number for a double precision floating
        ///     point value. i.e. there are this many digits used to represent the
        ///     actual number, where in a number as: 0.134556 * 10^5 the digits are 0.134556 and the exponent is 5.
        /// </summary>
        private const int DoubleWidth = 53;

        /// <summary>
        ///     Standard epsilon, the maximum relative precision of IEEE 754 single-precision floating numbers (32 bit).
        ///     According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly float SinglePrecision = (float)(2 * System.Math.Pow(2, -SingleWidth));

        /// <summary>
        ///     Standard epsilon, the maximum relative precision of IEEE 754 double-precision floating numbers (64 bit).
        ///     According to the definition of Prof. Higham and used in the ISO C standard and MATLAB.
        /// </summary>
        public static readonly double DoublePrecision = 2 * System.Math.Pow(2, -DoubleWidth);

        public static bool WithinEpsilon(float a, float b)
        {
            return WithinEpsilon(a, b, FloatExtensions.Tolerance);
        }

        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            return (a >= b - epsilon) && (a <= b + epsilon);
        }

        public static bool WithinEpsilon(double a, double b)
        {
            return WithinEpsilon(a, b, DoubleExtensions.Tolerance);
        }

        public static bool WithinEpsilon(double a, double b, double epsilon)
        {
            return (a >= b - epsilon) && (a <= b + epsilon);

            // TODO: Test implementation from http://floating-point-gui.de/errors/comparison/
            //double diff = Math.Abs(a - b);

            //if (a.Equals(b)) {
            //    // handle infinities
            //    return true;
            //}

            //if (a.Equals(0f) || b.Equals(0f) || diff < DoublePrecision) {
            //    // a or b is zero or both are extremely close to it
            //    // use absolute error
            //    return diff < epsilon;
            //}

            //// use relative error
            //return diff / (Math.Abs(a) + Math.Abs(b)) < epsilon;
        }
    }
}