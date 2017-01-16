using System.Diagnostics;

namespace Essentions.IO.Globbing.Nodes
{
    [DebuggerDisplay("*")]
    internal sealed class WildcardSegment : MatchableNode
    {
        [DebuggerStepThrough]
        public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
        {
            visitor.VisitWildcardSegmentNode(this, context);
        }

        public override bool IsMatch(string value)
        {
            return true;
        }
    }
}