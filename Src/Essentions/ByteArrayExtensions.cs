using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for array of <see cref="byte" />s. </summary>
    public static class ByteArrayExtensions
    {
        /// <summary> Finds content of a byte array inside another byte array. </summary>
        /// <param name="buffer">The array to look in.</param>
        /// <param name="pattern">The pattern to find.</param>
        /// <returns>Index of the found pattern, or -1.</returns>
        public static int IndexOf([NotNull] this byte[] buffer, [NotNull] byte[] pattern)
        {
            Check.NotNull(buffer)
                 .NotNull(pattern);

            if ((buffer.Length == 0) || (pattern.Length == 0) || (pattern.Length > buffer.Length)) return -1;

            var found = false;
            int i, j;
            for (i = 0, j = 0; i < buffer.Length;) {
                if (buffer[i++] != pattern[j++]) {
                    j = 0;
                    continue;
                }

                if (j == pattern.Length) {
                    found = true;
                    break;
                }
            }

            return found ? i - pattern.Length : -1;
        }


        /// <summary> Creates a memory stream and writes this byte array to it.</summary>
        /// <param name="buffer">Byte array to write to stream.</param>
        /// <returns>Memory stream with bytes written to it.</returns>
        public static MemoryStream ToMemoryStream([NotNull] this byte[] buffer)
        {
            var memStream = new MemoryStream();
            memStream.Write(buffer, 0, buffer.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        /// <summary> Restores the structure of given marshallable type from its byte array representation. </summary>
        /// <typeparam name="T"> Marshallable structure type.</typeparam>
        /// <param name="buffer">Array of bytes representing a structure.</param>
        /// <returns>Structure restored from byte array.</returns>
        public static T ToStruct<T>([NotNull] this byte[] buffer) where T : struct
        {
            Check.NotNullOrEmpty(buffer);

            var structSize = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(structSize);
            try {
                Marshal.Copy(buffer, 0, ptr, structSize);
                return (T)Marshal.PtrToStructure(ptr, typeof(T));
            }
            finally {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}