using System;

namespace Essentions.IO
{
    /// <summary>Defines constants for read, write, or read/write access to a file.</summary>
    /// <filterpriority>2</filterpriority>
    [Flags]
    public enum FileAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = Write | Read,
    }
}