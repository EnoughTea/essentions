namespace Essentions.IO
{
    /// <summary>Specifies how the operating system should open a file.</summary>
    /// <filterpriority>2</filterpriority>
    public enum FileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
    }
}