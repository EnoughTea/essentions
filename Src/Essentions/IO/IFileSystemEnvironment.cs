namespace Essentions.IO
{
    /// <summary>Represents the file system environment app operates in.</summary>
    /// <remarks>
    /// <example>
    /// <code>
    /// /// &lt;summary&gt;Example implementation.&lt;/summary&gt;
    /// public sealed class FileSystemEnvironment : IFileSystemEnvironment
    /// {
    ///     public FileSystemEnvironment()
    ///     {
    ///         Globber = new Globber(this);
    ///         FS = new FileSystem();
    ///         WorkingDirectory = new DirectoryPath(System.IO.Directory.GetCurrentDirectory());
    ///     }
    ///
    ///     public DirectoryPath ApplicationRoot => new DirectoryPath(
    ///         System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
    ///
    ///     public IFileSystem FS { get; set; }
    ///
    ///     public IGlobber Globber { get; set; }
    ///
    ///     public DirectoryPath WorkingDirectory {
    ///         get { return System.IO.Directory.GetCurrentDirectory(); }
    ///         set { SetWorkingDirectory(value); }
    ///     }
    ///
    ///     public string[] GetLogicalDrives() {
    ///         return System.IO.Directory.GetLogicalDrives();
    ///     }
    ///
    ///     public DirectoryPath GetSpecialPath(SpecialPath path)
    ///     {
    ///         switch (path) {
    ///             case SpecialPath.ApplicationData:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    ///             case SpecialPath.CommonApplicationData:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
    ///             case SpecialPath.LocalApplicationData:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    ///             case SpecialPath.ProgramFiles:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
    ///             case SpecialPath.ProgramFilesX86:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
    ///             case SpecialPath.Windows:
    ///                 return new DirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Windows));
    ///             case SpecialPath.LocalTemp:
    ///                 return new DirectoryPath(System.IO.Path.GetTempPath());
    ///         }
    ///
    ///         throw new NotSupportedException($"The special path '{path}' is not supported.");
    ///     }
    ///
    ///     public bool Is64BitOS()
    ///     {
    ///         return Environment.Is64BitProcess;
    ///     }
    ///
    ///     public bool IsUnix()
    ///     {
    ///         var platform = (int)Environment.OSVersion.Platform;
    ///         return (platform == 4) || (platform == 6) || (platform == 128);
    ///     }
    ///
    ///     private static void SetWorkingDirectory(DirectoryPath path)
    ///     {
    ///         if (path.IsRelative) {
    ///             throw new InvalidOperationException("Working directory can not be set to a relative path.");
    ///         }
    ///
    ///         System.IO.Directory.SetCurrentDirectory(path.FullPath);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IFileSystemEnvironment
    {
        /// <summary>Gets the file system.</summary>
        /// <value>The file system.</value>
        IFileSystem FS { get; set; }

        /// <summary>Gets the globber.</summary>
        /// <value>The globber.</value>
        IGlobber Globber { get; set; }

        /// <summary>Gets whether or not the current operative system is 64 bit.</summary>
        /// <returns>Whether or not the current operative system is 64 bit.</returns>
        bool Is64BitOS();

        /// <summary>Determines whether the current machine is running Unix.</summary>
        /// <returns>Whether or not the current machine is running Unix.</returns>
        bool IsUnix();

        /// <summary>Gets the application root path.</summary>
        /// <value>The application root path.</value>
        DirectoryPath ApplicationRoot { get; }

        /// <summary>Gets or sets the working directory.</summary>
        /// <value>The working directory.</value>
        DirectoryPath WorkingDirectory { get; set; }

        /// <summary>Retrieves the names of the logical drives on this computer.</summary>
        /// <remarks>Windows uses form like "&lt;drive letter&gt;:\".</remarks>
        /// <returns>Names of the logical drives on this computer.</returns>
        string[] GetLogicalDrives();

        /// <summary>Gets a special path.</summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="DirectoryPath"/> to the special path.</returns>
        DirectoryPath GetSpecialPath(SpecialPath path);
    }
}