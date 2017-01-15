using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;

namespace Essentions.Collections
{
    /// <summary>
    ///     Represents a strongly typed list of objects that can be accessed by index. Provides event handlers
    ///     for all list-changing events, methods to search, sort, and manipulate lists.
    /// </summary>
    /// <remarks>
    ///     This is needed because <see cref="System.Collections.ObjectModel.ObservableCollection{T}" />
    ///     does not provide modified elements for a 'big collection change' such as clearing.
    /// </remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DataContract(Name = "evLst", IsReference = true, Namespace = "")]
    [DebuggerDisplay("Count = {Count}")]
    internal class ListWithEvents<T> : IList<T>, IList
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ListWithEvents{T}" /> class that is empty and has
        ///     the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public ListWithEvents(int capacity = 32)
        {
            Check.Positive(capacity);

            _list = new List<T>(capacity);
            EventsEnabled = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListWithEvents{T}" /> class that contains elements
        ///     copied from the specified collection and has sufficient capacity to accommodate the number of elements
        ///     copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public ListWithEvents([NotNull] IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        [DataMember(Name = "l", Order = 10)]
        private readonly List<T> _list;

        /// <summary> Gets or sets a value indicating whether the events are enabled. </summary>
        [DataMember(Name = "ee", Order = 0)]
        public bool EventsEnabled { get; set; }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        [CanBeNull]
        public T this[int index]
        {
            get { return _list[index]; }

            set {
                var replaced = _list[index];

                var success = EventsEnabled ? BeforeReplacing.Call(this, index, replaced, value, true) : true;
                if (success) {
                    _list[index] = value;
                    if (EventsEnabled && !EqualityComparer<T>.Default.Equals(replaced, value))
                        Replaced.Call(this, index, replaced, value);
                }
            }
        }

        /// <summary>
        ///     Occurs after a new element was added to the list. Provides added element index and
        ///     the added element itself.
        /// </summary>
        public event Action<ListWithEvents<T>, int, T> Added;

        /// <summary>
        ///     Occurs before a new element will be added to the list. Provides list index where object will be
        ///     added and the object which will be added. If <c>false</c> is returned, will cancel adding.
        /// </summary>
        public event Func<ListWithEvents<T>, int, T, bool> BeforeAdding;

        /// <summary>Occurs before the list will be cleared. If <c>false</c> is returned, will cancel clearing.</summary>
        public event Func<ListWithEvents<T>, bool> BeforeClearing;

        /// <summary>
        ///     Occurs before a range of objects will be added to the list. Provides index where
        ///     range will be added and the range items which will be inserted. If <c>false</c> is returned,
        ///     will cancel addition. Note that <see cref="BeforeAdding" /> won't be called for every element when
        ///     <see cref="AddRange" /> is used.
        /// </summary>
        public event Func<ListWithEvents<T>, int, ICollection<T>, bool> BeforeRangeAddition;

        /// <summary>
        ///     Occurs before a range of existing elements will be removed from the list.
        ///     Provides index where removed range starts and range length. If <c>false</c> is returned,
        ///     will cancel removal. Note that <see cref="BeforeRemoval" /> won't be called for every element when
        ///     <see cref="RemoveRange" /> is used.
        /// </summary>
        public event Func<ListWithEvents<T>, int, int, bool> BeforeRangeRemoval;

        /// <summary>
        ///     Occurs before an element will be removed from the list. Provides index at which removal
        ///     will occur, and the element which will be removed. If <c>false</c> is returned, will cancel removal.
        /// </summary>
        public event Func<ListWithEvents<T>, int, T, bool> BeforeRemoval;

        /// <summary>
        ///     Occurs before a value replaces some element via indexed property. Provides index where
        ///     replacement will occur, replaced element and replacee.
        ///     If <c>false</c> is returned, will cancel replacing.
        /// </summary>
        public event Func<ListWithEvents<T>, int, T, T, bool> BeforeReplacing;

        /// <summary>Occurs after the list was cleared.</summary>
        public event Action<ListWithEvents<T>> Cleared;

        /// <summary>
        ///     Occurs after a range of objects was added to the list. Provides list index where
        ///     range started and range length. Note that <see cref="Added" /> won't be called for every element when
        ///     <see cref="AddRange" /> is used.
        /// </summary>
        public event Action<ListWithEvents<T>, int, int> RangeAdded;

        /// <summary>
        ///     Occurs after a range of existing elements was removed from the list. Provides removed elements.
        ///     Note that <see cref="Removed" /> won't be called for every element when <see cref="RemoveRange" /> is used.
        /// </summary>
        public event Action<ListWithEvents<T>, ICollection<T>> RangeRemoved;

        /// <summary>
        ///     Occurs after existing element was removed from the list. Provides index which
        ///     the removed element had, and the removed element itself.
        /// </summary>
        public event Action<ListWithEvents<T>, int, T> Removed;

        /// <summary>
        ///     Occurs after a value replaces some element via indexed property. Provides index where
        ///     replacement occurred, replaced element and replacee.
        /// </summary>
        public event Action<ListWithEvents<T>, int, T, T> Replaced;

        /// <summary>Adds an object to the end of the list.</summary>
        /// <param name="item">
        ///     The object to be added to the end of the list. The value can be null for
        ///     reference types.
        /// </param>
        public void Add([CanBeNull] T item)
        {
            var index = Count;
            var success = EventsEnabled ? BeforeAdding.Call(this, index, item, true) : true;
            if (success) {
                _list.Add(item);
                if (EventsEnabled) Added.Call(this, index, item);
            }
        }

        /// <summary>
        ///     Adds the elements of the specified collection to the end of the list. Note that
        ///     <see cref="Added" /> won't be called for every element when <see cref="AddRange" /> is used.
        ///     Use <see cref="RangeAdded" /> instead.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the list.
        ///     The collection itself cannot be null, but it can contain elements that are null,
        ///     if type <typeparamref name="T" /> is a reference type.
        /// </param>
        public void AddRange([NotNull] IEnumerable<T> collection)
        {
            Check.NotNull(collection);

            InsertRange(Count, collection);
        }

        /// <summary>
        ///     Searches the entire sorted list for an element using the default comparer and returns the
        ///     zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        ///     The zero-based index of item in the sorted list, if item is found; otherwise,
        ///     a negative number that is the bitwise complement of the index of the next element that is larger
        ///     than item or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        public int BinarySearch([CanBeNull] T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        /// <summary>
        ///     Searches the entire sorted list for an element using the specified comparer and returns the
        ///     zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">
        ///     The IComparer{} implementation to use when comparing elements.
        ///     -or-null to use the default comparer.
        /// </param>
        /// <returns>
        ///     The zero-based index of item in the sorted list, if item is found; otherwise,
        ///     a negative number that is the bitwise complement of the index of the next element that is larger
        ///     than item or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        public int BinarySearch([CanBeNull] T item, [CanBeNull] IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        /// <summary>
        ///     Searches a range of elements in the sorted list for an element using the specified comparer and
        ///     returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">
        ///     The IComparer{} implementation to use when comparing elements.
        ///     -or-null to use the default comparer.
        /// </param>
        /// <returns>
        ///     The zero-based index of item in the sorted list, if item is found; otherwise,
        ///     a negative number that is the bitwise complement of the index of the next element that is larger
        ///     than item or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        public int BinarySearch(int index, int count, [CanBeNull] T item, [CanBeNull] IComparer<T> comparer)
        {
            return _list.BinarySearch(index, count, item, comparer);
        }

        /// <summary>Determines whether an element is in the list.</summary>
        /// <param name="item">The object to locate in the list. The value can be null for reference types.</param>
        /// <returns>ttrue if item is found in the list; otherwise, false.</returns>
        public bool Contains([CanBeNull] T item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        ///     Copies the entire list to a compatible one-dimensional array, starting at the beginning of
        ///     the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from list.
        ///     The array must have zero-based indexing.
        /// </param>
        public void CopyTo([NotNull] T[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        ///     Copies the entire list to a compatible one-dimensional array, starting at the specified index of
        ///     the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from list.
        ///     The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo([NotNull] T[] array, int arrayIndex)
        {
            Check.NotNull(array);

            CopyTo(0, array, arrayIndex, Count);
        }

        /// <summary>
        ///     Copies a range of elements from the list to a compatible one-dimensional array,
        ///     starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source list at which copying begins.</param>
        /// <param name="array">
        ///     The one-dimensional array that is the destination of the elements copied from list.
        ///     The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(int index, [NotNull] T[] array, int arrayIndex, int count)
        {
            Check.NotNull(array);

            _list.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        ///     Determines whether the list contains elements that match the conditions defined by
        ///     the specified predicate.
        /// </summary>
        /// <param name="match">
        ///     The Predicate{} delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///     true if the list contains one or more elements that match the conditions defined by
        ///     the specified predicate; otherwise, false.
        /// </returns>
        public bool Exists([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return _list.Exists(match);
        }

        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified predicate,
        ///     and returns the first occurrence within the entire list.
        /// </summary>
        /// <param name="match">
        ///     The Predicate{} delegate that defines the conditions of the element to search for.
        /// </param>
        /// <returns>
        ///     The first element that matches the conditions defined by the specified predicate, if found;
        ///     otherwise, the default value for type <typeparamref name="T" />.
        /// </returns>
        public T Find([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return _list.Find(match);
        }

        /// <summary>Retrieves all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">
        ///     The Predicate{} delegate that defines the conditions of the elements to search for.
        /// </param>
        /// <returns>
        ///     A list containing all the elements that match the conditions defined by the specified predicate,
        ///     if found; otherwise, an empty list.
        /// </returns>
        [NotNull]
        public ListWithEvents<T> FindAll([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            var list = new ListWithEvents<T>();
            for (var i = 0; i < Count; i++) if (match(this[i])) list.Add(this[i]);

            return list;
        }

        public int FindIndex([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return FindIndex(0, Count, match);
        }

        public int FindIndex(int startIndex, [NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return FindIndex(startIndex, Count - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, [NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return _list.FindIndex(startIndex, count, match);
        }

        public T FindLast([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return _list.FindLast(match);
        }

        public int FindLastIndex([NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return FindLastIndex(Count - 1, Count, match);
        }

        public int FindLastIndex(int startIndex, [NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, [NotNull] Predicate<T> match)
        {
            Check.NotNull(match);

            return _list.FindLastIndex(startIndex, count, match);
        }

        /// <summary>Performs the specified action on each element of the list.</summary>
        /// <param name="action">The Action{} delegate to perform on each element of the list.</param>
        public void ForEach([NotNull] Action<T> action)
        {
            Check.NotNull(action);

            for (var i = 0; i < _list.Count; i++) action(_list[i]);
        }

        /// <summary>Creates a shallow copy of a range of elements in the source list.</summary>
        /// <param name="index">The zero-based list index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source list.</returns>
        public ListWithEvents<T> GetRange(int index, int count)
        {
            return new ListWithEvents<T>(_list.GetRange(index, count));
        }

        public int IndexOf([CanBeNull] T item)
        {
            return IndexOf(item, 0, Count);
        }

        public int IndexOf([CanBeNull] T item, int index)
        {
            return IndexOf(item, index, Count - index);
        }

        public int IndexOf([CanBeNull] T item, int index, int count)
        {
            return _list.IndexOf(item, index, count);
        }

        /// <summary>Inserts an element into the list at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        public void Insert(int index, [CanBeNull] T item)
        {
            var success = EventsEnabled ? BeforeAdding.Call(this, index, item, true) : true;
            if (success) {
                _list.Insert(index, item);
                if (EventsEnabled) Added.Call(this, index, item);
            }
        }

        /// <summary>Gets the number of elements contained in the list </summary>
        public int Count => _list.Count;

        /// <summary>Removes all elements from the list.</summary>
        public void Clear()
        {
            var success = EventsEnabled ? BeforeClearing.Call(this, true) : true;
            if (success) {
                for (var index = Count - 1; index >= 0; index--) RemoveAt(index);

                if (EventsEnabled) Cleared.Call(this);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>Inserts the elements of a collection into the list at the specified index.</summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="elements">
        ///     The collection whose elements should be inserted into the list.
        ///     The collection itself cannot be null, but it can contain elements that are null,
        ///     if type <typeparamref name="T" /> is a reference type.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0 -or- index is greater than Count.</exception>
        public void InsertRange(int index, [NotNull] IEnumerable<T> elements)
        {
            Check.NotNull(elements)
                 .InRange(index, 0, Count, "index is less than 0 -or- index is greater than Count.",
                          nameof(index));

            // First call the pre-event if needed, enumerating sequence in process:
            var success = true;
            IEnumerable<T> fixedElements = null;
            var beforeRangeAdditionHandler = Volatile.Read(ref BeforeRangeAddition);
            if (EventsEnabled && (beforeRangeAdditionHandler != null)) {
                // ReSharper disable PossibleMultipleEnumeration
                fixedElements = elements as ICollection<T> ?? elements.ToArray();
                success = beforeRangeAdditionHandler(this, index, (ICollection<T>)fixedElements);
            }

            if (success) {
                // Enumerated sequence will be null if pre-event was not called, since no enumeration was done.
                if (fixedElements == null) fixedElements = elements;

                // ReSharper enable PossibleMultipleEnumeration
                var countBefore = Count;
                _list.InsertRange(index, fixedElements);

                var addedTotal = Count - countBefore;
                if (EventsEnabled && (addedTotal > 0)) RangeAdded.Call(this, index, addedTotal);
            }
        }

        public int LastIndexOf([CanBeNull] T item)
        {
            return LastIndexOf(item, Count - 1);
        }

        public int LastIndexOf([CanBeNull] T item, int index)
        {
            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf([CanBeNull] T item, int index, int count)
        {
            return _list.LastIndexOf(item, index, count);
        }

        /// <summary>Removes the first occurrence of a specific object from the list. </summary>
        /// <param name="item">TThe object to remove from the list. The value can be null for reference types.</param>
        /// <returns>
        ///     true if item is successfully removed; otherwise, false.
        ///     This method also returns false if item was not found in the list.
        /// </returns>
        public bool Remove([CanBeNull] T item)
        {
            var removed = false;
            var index = IndexOf(item);
            if (index >= 0) {
                RemoveAt(index);
                removed = true;
            }

            return removed;
        }

        /// <summary>Removes all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="match">The Predicate{} delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the list.</returns>
        public int RemoveAll([CanBeNull] Predicate<T> match)
        {
            if (match == null) return -1;

            var removed = 0;
            for (var index = _list.Count - 1; index >= 0; index--) {
                var item = _list[index];
                if (match(item)) {
                    RemoveAt(index);
                    removed++;
                }
            }

            return removed;
        }

        /// <summary>Removes the element at the specified index of the list.</summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is less than 0 -or-
        ///     index is equal to or greater than Count.
        /// </exception>
        public void RemoveAt(int index)
        {
            Check.InRange(index, 0, Count, "index is less than 0 -or- index is greater than Count.",
                          nameof(index));

            var removed = _list[index];
            var success = EventsEnabled ? BeforeRemoval.Call(this, index, removed, true) : true;
            if (success) {
                _list.RemoveAt(index);
                if (EventsEnabled) Removed.Call(this, index, removed);
            }
        }

        /// <summary>
        ///     Removes a range of elements from the list. Note that <see cref="Removed" /> won't be called for
        ///     every element when <see cref="RemoveRange" /> is used, use <see cref="RangeRemoved" /> instead.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is less than 0 -or- count is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     index and count do not denote a valid range of elements in
        ///     the List.
        /// </exception>
        public void RemoveRange(int index, int count)
        {
            Check.InRange(index, 0, Count, "index is less than 0 -or- index is greater than Count.", nameof(index))
                 .GreaterOrEqual(Count - index, count,
                                 "index and count do not denote a valid range of elements in the list.");

            if (count <= 0) return;

            var allowRemoval = EventsEnabled ? BeforeRangeRemoval.Call(this, index, count, true) : true;
            if (allowRemoval) {
                var rangeRemovedHandler = Volatile.Read(ref RangeRemoved);
                // If event exist, we need to copy soon to be removed elements for it:
                if (EventsEnabled && (rangeRemovedHandler != null)) {
                    var removed = new T[count];
                    _list.CopyTo(index, removed, 0, count);
                    _list.RemoveRange(index, count);
                    rangeRemovedHandler(this, removed);
                }
                else {
                    _list.RemoveRange(index, count);
                }
            }
        }

        /// <summary>Reverses the order of the elements in the entire list.</summary>
        public void Reverse()
        {
            Reverse(0, Count);
        }

        /// <summary>Reverses the order of the elements in the specified range.</summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        public void Reverse(int index, int count)
        {
            _list.Reverse(index, count);
        }

        /// <summary>Sorts the elements in the entire list using the default comparer.</summary>
        public void Sort()
        {
            Sort(0, Count, null);
        }

        /// <summary>Sorts the elements in the entire list using the specified comparer.</summary>
        /// <param name="comparer">
        ///     The IComparer{} implementation to use when comparing elements, or null to use
        ///     the default comparer.
        /// </param>
        public void Sort([CanBeNull] IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        /// <summary>Sorts the elements in a range of elements in list using the specified comparer.</summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">
        ///     The IComparer{} implementation to use when comparing elements, or null to use
        ///     the default comparer.
        /// </param>
        public void Sort(int index, int count, [CanBeNull] IComparer<T> comparer)
        {
            _list.Sort(index, count, comparer);
        }

        /// <summary>Sorts the elements in the entire list using the specified Comparison{}.</summary>
        /// <param name="comparison">The Comparison{} to use when comparing elements.</param>
        public void Sort([NotNull] Comparison<T> comparison)
        {
            _list.Sort(comparison);
        }

        /// <summary>Copies the elements of the list to a new array.</summary>
        /// <returns>An array containing copies of the elements of the list.</returns>
        [NotNull]
        public T[] ToArray()
        {
            return _list.ToArray();
        }

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "Count: " + Count;
        }

        /// <summary>
        ///     Sets the capacity to the actual number of elements in the list, if that number
        ///     is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            _list.TrimExcess();
        }

        /// <summary>
        ///     Determines whether every element in the list matches the conditions defined by the
        ///     specified predicate.
        /// </summary>
        /// <param name="match">
        ///     The predicate delegate that defines the conditions to check against the
        ///     elements.
        /// </param>
        /// <returns>
        ///     true if every element in the list matches the conditions defined by the specified predicate;
        ///     otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        public bool TrueForAll([NotNull] Predicate<T> match)
        {
            return _list.TrueForAll(match);
        }

        #region IList Members

        [CanBeNull]
        object IList.this[int index]
        {
            get { return this[index]; }

            set {
                Check.True(IsElementTypeCompatible(value),
                           "Wrong value type: '" + value?.GetType() + "'.", nameof(value));

                this[index] = (T)value;
            }
        }

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        int IList.Add([CanBeNull] object item)
        {
            Add((T)item);
            return Count - 1;
        }

        bool IList.Contains([CanBeNull] object item)
        {
            if (IsElementTypeCompatible(item)) return Contains((T)item);

            return false;
        }

        int IList.IndexOf([CanBeNull] object item)
        {
            if (IsElementTypeCompatible(item)) return IndexOf((T)item);

            return -1;
        }

        void IList.Insert(int index, [CanBeNull] object item)
        {
            Insert(index, (T)item);
        }

        void IList.Remove([CanBeNull] object item)
        {
            if (IsElementTypeCompatible(item)) Remove((T)item);
        }

        private static bool IsElementTypeCompatible([CanBeNull] object element)
        {
            // Null is valid for reference types and invalid for value types.
            return element is T || ((element == null) && !typeof(T).GetTypeInfo().IsValueType);
        }

        #endregion

        #region ICollection Members

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        void ICollection.CopyTo([NotNull] Array array, int arrayIndex)
        {
            ((ICollection)_list).CopyTo(array, arrayIndex);
        }

        #endregion

        #region IList<T> Members

        int IList<T>.IndexOf(T item)
        {
            return IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        [CanBeNull]
        T IList<T>.this[int index]
        {
            get { return this[index]; }

            set { this[index] = value; }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add([CanBeNull] T item)
        {
            Add(item);
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        bool ICollection<T>.Contains([CanBeNull] T item)
        {
            return Contains(item);
        }

        void ICollection<T>.CopyTo([NotNull] T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count => Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        #endregion
    }
}