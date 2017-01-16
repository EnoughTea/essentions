using System;

namespace Essentions.IO
{
    /// <summary>Responsible for retrieving information about the current machine.</summary>
    public static class Machine
    {
        private static readonly object _Locker = new object();

        private static Func<bool> _is64BitOS = delegate {
            throw new NotImplementedException("Do not forget to call Machine.Init()");
        };

        private static Func<bool> _isUnix = delegate {
            throw new NotImplementedException("Do not forget to call Machine.Init()");
        };

        /// <summary>
        /// Determines if the current operative system is 64 bit.
        /// </summary>
        /// <returns>Whether or not the current operative system is 64 bit.</returns>
        public static Func<bool> Is64BitOS
        {
            get {
                lock (_Locker) {
                    return _is64BitOS;
                }
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                lock (_Locker) {
                    _is64BitOS = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the current machine is running Unix.
        /// </summary>
        /// <returns>Whether or not the current machine is running Unix.</returns>
        public static Func<bool> IsUnix
        {
            get {
                lock (_Locker) {
                    return _isUnix;
                }
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                lock (_Locker) {
                    _isUnix = value;
                }
            }
        }

        /// <summary>Initializes the specified is64 bit os.</summary>
        /// <param name="is64Bit">Function used for determining whether we are in x64 OS or not.</param>
        /// <param name="isUnix">Function used for determining whether we are on Unix or not.</param>
        /// <exception cref="ArgumentNullException"><paramref name="is64Bit" /> or <paramref name="isUnix" />
        /// is <see langword="null" /></exception>
        public static void Init(Func<bool> is64Bit, Func<bool> isUnix)
        {
            if (is64Bit == null) {
                throw new ArgumentNullException(nameof(is64Bit));
            }

            if (isUnix == null) {
                throw new ArgumentNullException(nameof(isUnix));
            }

            lock (_Locker) {
                Is64BitOS = is64Bit;
                IsUnix = isUnix;
            }
        }
    }
}