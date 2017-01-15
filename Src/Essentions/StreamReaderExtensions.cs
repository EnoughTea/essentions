using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="StreamReader" />. </summary>
    public static class StreamReaderExtensions
    {
        /// <summary> Reads stream line by line until the end. </summary>
        /// <param name="reader">The stream to read.</param>
        /// <returns>Stream contents line by line.</returns>
        [NotNull]
        public static IEnumerable<string> ReadLines([NotNull] this StreamReader reader)
        {
            Check.NotNull(reader);

            string line;
            while ((line = reader.ReadLine()) != null) yield return line;
        }
    }
}