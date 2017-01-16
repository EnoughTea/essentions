using System;
using System.Collections.Generic;
using System.Linq;

namespace Essentions.IO
{
    internal static class PathCollapser
    {
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/></exception>
        public static string Collapse(Path path)
        {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }

            var stack = new Stack<string>();
            var segments = path.FullPath.Split('/', '\\');
            foreach (var segment in segments) {
                if (segment == ".") {
                    continue;
                }
                if (segment == "..") {
                    if (stack.Count > 1) {
                        stack.Pop();
                    }
                    continue;
                }
                stack.Push(segment);
            }
            string collapsed = string.Join("/", stack.Reverse());
            return collapsed == string.Empty ? "." : collapsed;
        }
    }
}