using System;
using System.Collections.Generic;
using System.Linq;
using Essentions.IO.Globbing;

namespace Essentions.IO
{
    /// <summary>
    /// The file system globber.
    /// </summary>
    public sealed class Globber : IGlobber
    {
        private readonly GlobParser _parser;
        private readonly GlobVisitor _visitor;
        private readonly PathComparer _comparer;
        private readonly IFileSystemEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="Globber"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        /// <exception cref="ArgumentNullException"><paramref name="env"/> is <see langword="null"/></exception>
        public Globber(IFileSystemEnvironment env)
        {
            if (env == null) {
                throw new ArgumentNullException(nameof(env));
            }

            _env = env;
            _parser = new GlobParser(env);
            _visitor = new GlobVisitor(env);
            _comparer = new PathComparer(env.IsUnix());
        }

        /// <summary>
        /// Returns <see cref="Path" /> instances matching the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="predicate">The predicate used to filter directories based on file system information.</param>
        /// <returns>
        ///   <see cref="Path" /> instances matching the specified pattern.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is <see langword="null"/></exception>
        public IEnumerable<Path> Match(string pattern, Func<IDirectory, bool> predicate)
        {
            if (pattern == null) {
                throw new ArgumentNullException(nameof(pattern));
            }
            if (string.IsNullOrWhiteSpace(pattern)) {
                return Enumerable.Empty<Path>();
            }

            // Parse the pattern into an AST.
            var root = _parser.Parse(pattern, _env.IsUnix());

            // Visit all nodes in the parsed patterns and filter the result.
            return _visitor.Walk(root, predicate)
                           .Select(x => x.Path)
                           .Distinct(_comparer);
        }
    }
}