using System;
using System.Diagnostics;

namespace Essentions.IO.Globbing.Nodes
{
    [DebuggerDisplay("{Drive,nq}:")]
    internal sealed class WindowsRoot : GlobNode
    {
        public string Drive { get; }

        /// <exception cref="ArgumentNullException"><paramref name="drive"/> is <see langword="null"/></exception>
        public WindowsRoot(string drive)
        {
            if (drive == null)
            {
                throw new ArgumentNullException(nameof(drive));
            }

            Drive = drive;
        }

        [DebuggerStepThrough]
        public override void Accept(GlobVisitor visitor, GlobVisitorContext context)
        {
            visitor.VisitWindowsRoot(this, context);
        }
    }
}