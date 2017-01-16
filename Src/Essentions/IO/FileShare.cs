using System;

namespace Essentions.IO
{
    /// <summary>Contains constants for controlling the kind of access other file stream objects can have
    ///  to the same file.</summary>
    /// <filterpriority>2</filterpriority>
    [Flags]
    public enum FileShare
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Write | Read,
        Delete = 4,
        Inheritable = 16,
    }
}