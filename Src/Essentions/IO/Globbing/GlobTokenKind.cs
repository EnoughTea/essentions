namespace Essentions.IO.Globbing
{
    internal enum GlobTokenKind
    {
        Wildcard,
        CharacterWildcard,
        DirectoryWildcard,
        PathSeparator,
        Identifier,
        WindowsRoot,
        Parent,
        EndOfText
    }
}