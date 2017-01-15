using System.Collections.Generic;
using System.Text;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="StringBuilder" />. </summary>
    public static class StringBuilderExtensions
    {
        /// <summary> Enumerates this <see cref="StringBuilder" /> returning its contents. </summary>
        /// <returns> Sequence of characters in the <see cref="StringBuilder" />.</returns>
        public static IEnumerable<char> ToEnumerable(this StringBuilder sb)
        {
            Check.NotNull(sb);

            for (var i = 0; i < sb.Length; i++) yield return sb[i];
        }
    }
}