namespace Essentions.IO
{
    /// <summary>Represents the file system environment app operates in.</summary>
    /// <remarks>
    /// <example>
    /// <code>
    /// Initialize Machine class first:
    /// Machine.Init(
    ///     () =&gt; Environment.Is64BitOperatingSystem,
    ///     () =&gt; {
    ///         var platform = (int)Environment.OSVersion.Platform;
    ///         return (platform == 4) || (platform == 6) || (platform == 128);
    /// });
    ///
    /// /// &lt;summary&gt;Example implementation.&lt;/summary&gt;
    /// public sealed class FileSystemEnvironment : IFileSystemEnvironment
    /// {
    ///     public FileSystemEnvironment()
    ///     {
    ///         FS = new FileSystem();
    ///         Globber = new Globber();
    ///         WorkingDirectory = new DirectoryPath(System.IO.Directory.GetCurrentDirectory());
    ///     }
    ///
    ///     public IFileSystem FS { get; }
    ///
    ///     public IGlobber Globber { get; }
    ///
    ///     public DirectoryPath WorkingDirectory {
    ///         get { return System.IO.Directory.GetCurrentDirectory(); }
    ///         set { SetWorkingDirectory(value); }
    ///     }
    ///
    ///     public bool Is64BitOS()
    ///     {
    ///         return Machine.Is64BitOS();
    ///     }
    ///
    ///     public bool IsUnix()
    ///     {
    ///         return Machine.IsUnix();
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
    ///     public DirectoryPath GetApplicationRoot()
    ///     {
    ///         var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    ///         return new DirectoryPath(path);
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
        /// <summary>
        /// Gets the file system.
        /// </summary>
        /// <value>The file system.</value>
        IFileSystem FS { get; set; }

        /// <summary>
        /// Gets the globber.
        /// </summary>
        /// <value>The globber.</value>
        IGlobber Globber { get; set; }

        /// <summary>
        /// Gets whether or not the current operative system is 64 bit.
        /// </summary>
        /// <returns>Whether or not the current operative system is 64 bit.</returns>
        bool Is64BitOS();

        /// <summary>
        /// Determines whether the current machine is running Unix.
        /// </summary>
        /// <returns>Whether or not the current machine is running Unix.</returns>
        bool IsUnix();

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        DirectoryPath WorkingDirectory { get; set; }

        /// <summary>
        /// Gets a special path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A <see cref="DirectoryPath"/> to the special path.</returns>
        DirectoryPath GetSpecialPath(SpecialPath path);

        /// <summary>
        /// Gets the application root path.
        /// </summary>
        /// <returns>The application root path.</returns>
        DirectoryPath GetApplicationRoot();
    }
}