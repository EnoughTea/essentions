using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="Assembly" />. </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Finds types derived from the specified base type. Works with raw generics such as SomeType&lt;&gt;
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <param name="baseType">Base type.</param>
        /// <param name="classOnly">if set to <c>true</c>, will omit structs and search classes only.</param>
        /// <returns>Derived types in the current assembly.</returns>
        [NotNull]
        public static IEnumerable<Type> FindDerived(
            [NotNull] this Assembly assembly,
            [NotNull] Type baseType,
            bool classOnly = true)
        {
            Check.NotNull(assembly)
                 .NotNull(baseType);

            foreach (var type in assembly.ExportedTypes) {
                if (classOnly && !type.GetTypeInfo().IsClass) continue;

                if (type.DerivedFrom(baseType)) yield return type;
            }
        }
    }
}