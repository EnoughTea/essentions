using System.Collections.Generic;
using JetBrains.Annotations;

namespace Essentions
{
    /// <summary> Extension methods for <see cref="ICollection{T}" />. </summary>
    public static class CollectionExtensions
    {
        /// <summary> Adds the specified item to a list fluently, so you can chain adds. </summary>
        /// <typeparam name="T">Type of list element.</typeparam>
        /// <param name="collection">The list to add to.</param>
        /// <param name="item">The item to add.</param>
        /// <returns>List with added item.</returns>
        [NotNull]
        public static ICollection<T> Add<T>([NotNull] this ICollection<T> collection, T item)
        {
            Check.NotNull(collection);

            collection.Add(item);
            return collection;
        }

        /// <summary> Adds all specified elements to the collection. </summary>
        /// <typeparam name="T">The type of the elements of the collections.</typeparam>
        /// <param name="collection">The collection to add elements to.</param>
        /// <param name="other">The elements to add.</param>
        /// <param name="checkUnicity">
        ///     If set to <c>true</c>, will add only elements which don't exist in collection.
        /// </param>
        /// <returns>The same collection for fluent constructions.</returns>
        [NotNull]
        public static ICollection<T> AddRange<T>(
            [NotNull] this ICollection<T> collection,
            [NotNull] IEnumerable<T> other,
            bool checkUnicity = false)
        {
            Check.NotNull(collection)
                 .NotNull(other);

            if (checkUnicity) {
                foreach (var item in other) {
                    collection.AddUnique(item);
                }
            }
            else {
                foreach (var item in other) {
                    collection.Add(item);
                }
            }

            return collection;
        }

        /// <summary> Adds specified item to the collection, if the collection doesn't already contain such item.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the collections.</typeparam>
        /// <param name="collection">The collection to add to.</param>
        /// <param name="item">The element to add.</param>
        /// <returns>
        ///     <c>true</c> if element was added; <c>false</c> if such element already exists in the collection.
        /// </returns>
        public static bool AddUnique<T>([NotNull] this ICollection<T> collection, T item)
        {
            Check.NotNull(collection);

            var added = false;
            if (!collection.Contains(item)) {
                collection.Add(item);
                added = true;
            }

            return added;
        }

        /// <summary> Adds specified items to the collection, if the collection doesn't already contain such items.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the collections.</typeparam>
        /// <param name="collection">The collection to add to.</param>
        /// <param name="items">The elements to add.</param>
        public static void AddUnique<T>([NotNull] this ICollection<T> collection, [NotNull] IEnumerable<T> items)
        {
            Check.NotNull(collection);

            foreach (var item in items) {
                collection.AddUnique(item);
            }
        }

        /// <summary> Removes all specified elements from the collection. </summary>
        /// <typeparam name="T">The type of the elements of the collections.</typeparam>
        /// <param name="collection">The collection to remove elements from.</param>
        /// <param name="other">The elements to remove.</param>
        /// <returns>The same collection for fluent constructions.</returns>
        [NotNull]
        public static ICollection<T> RemoveRange<T>(
            [NotNull] this ICollection<T> collection,
            [CanBeNull] IEnumerable<T> other)
        {
            Check.NotNull(collection);

            if (other != null) {
                foreach (var item in other) collection.Remove(item);
            }

            return collection;
        }
    }
}