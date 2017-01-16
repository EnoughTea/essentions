using System.Diagnostics;

namespace Essentions.IO.Globbing
{
    [DebuggerDisplay("{ToString(),nq}")]
    internal abstract class GlobNode
    {
        public GlobNode Next { get; internal set; }

        public abstract void Accept(GlobVisitor visitor, GlobVisitorContext context);
    }
}