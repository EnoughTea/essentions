using System.Diagnostics;

namespace Essentions.IO.Globbing.Nodes
{
    [DebuggerDisplay("$")]
    internal sealed class UnixRoot : GlobNode
    {
        [DebuggerStepThrough]
        public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
        {
            visitor.VisitUnixRoot(this, context);
        }
    }
}