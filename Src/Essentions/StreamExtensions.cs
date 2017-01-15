using System;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="Stream" />. </summary>
    public static class StreamExtensions
    {
        /// <summary> Measures the difference in stream position after executing the specified delegate. </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="doIo">The action to execute on stream.</param>
        /// <returns>Position difference in bytes.</returns>
        public static long MeasurePositionDifference([NotNull] this Stream stream, [NotNull] Action<Stream> doIo)
        {
            Check.NotNull(stream)
                 .NotNull(doIo);

            var pos = stream.Position;
            doIo(stream);
            return stream.Position - pos;
        }

        /// <summary> Reads a structure of given marshallable type from byte array representation in the stream. </summary>
        /// <typeparam name="T">Marshallable structure type. </typeparam>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>Resulting struct.</returns>
        public static T ReadStruct<T>([NotNull] this Stream stream) where T : struct
        {
            Check.NotNull(stream);

            var size = Marshal.SizeOf(typeof(T));
            var buffer = new byte[size];
            stream.Read(buffer, 0, size);
            return buffer.ToStruct<T>();
        }
    }
}