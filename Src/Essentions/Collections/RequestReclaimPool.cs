using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Essentions.Collections
{
    /// <summary>
    ///     Pool of objects which tracks provided instances and returns them back to pool when they become 'unused'.
    ///     When you need an object, you <see cref="Request" />, and pool will use provided "is in use?" predicate every
    ///     call to <see cref="Reclaim" /> to check if this object is still used. If object became 'unused' since
    ///     the last reclaim, it is added back to the collection of available instances.
    /// </summary>
    /// <typeparam name="T">The type of stored object.</typeparam>
    [DataContract(Name = "rcp", IsReference = true, Namespace = "")]
    public class RequestReclaimPool<T> : IEnumerable<T>
        where T : class
    {
        /// <summary> Creates a new pool with a specific starting size. </summary>
        /// <param name="isInstanceStillInUse">
        ///     A predicate used to determine if a given object is still used and
        ///     cannot be returned to the pool.
        /// </param>
        /// <param name="initialSize">The initial size of the pool.</param>
        /// <param name="createObject">A function used to allocate an instance for the pool.</param>
        /// <exception cref="InvalidOperationException">
        ///     Throws when no allocate function has been provided and
        ///     type of pool's item doesn't have a parameterless constructor.
        /// </exception>
        public RequestReclaimPool(
            [CanBeNull] Predicate<T> isInstanceStillInUse,
            int initialSize = 32,
            [CanBeNull] Func<T> createObject = null)
        {
            Check.Positive(initialSize);

            _items = new T[initialSize];
            UnusedCount = _items.Length;
            IsInstanceStillInUse = isInstanceStillInUse;
            Spawner = createObject ?? Activator.CreateInstance<T>;
            for (var i = 0; i < initialSize; i++) {
                var item = Spawner.Call();
                _items[i] = item;
                if (item == null) {
                    throw new InvalidOperationException("Spawner should not return a null object reference.");
                }
            }
        }

        [DataMember(Name = "p", EmitDefaultValue = false)]
        private T[] _items;

        /// <summary> Gets the number of unused (available for requesting) objects in the pool. </summary>
        [DataMember(Name = "ic", EmitDefaultValue = false)]
        public int UnusedCount { get; private set; }

        /// <summary> Gets or sets a delegate used for checking if a given object is still in use. </summary>
        [CanBeNull]
        public Predicate<T> IsInstanceStillInUse { get; set; }

        /// <summary>
        ///     Function used to create new object on request when the pool is empty.
        ///     If null, <see cref="Activator.CreateInstance{T}" /> will be used.
        /// </summary>
        [CanBeNull]
        public Func<T> Spawner { get; set; }

        /// <summary>
        ///     Gets the number of used (requested earlier, but not returned to the pool yet) objects in the
        ///     pool.
        /// </summary>
        public int UsedCount => _items.Length - UnusedCount;

        /// <summary>
        ///     Returns a used object at the given index.
        ///     The index must fall in the range of [0, <see cref="UsedCount" />].
        /// </summary>
        /// <param name="index">The index of the valid object to get</param>
        /// <returns>A valid object found at the index</returns>
        /// <exception cref="IndexOutOfRangeException" accessor="get">
        ///     Index must be &lt;= ValidCount
        /// </exception>
        public T this[int index]
        {
            get {
                index += UnusedCount;
                if ((index < UnusedCount) || (index >= _items.Length))
                    throw new IndexOutOfRangeException($"Index must be <= {nameof(UsedCount)} ({UsedCount})");

                return _items[index];
            }
        }

        /// <summary> Occurs before an object is returned from <see cref="Request" />. </summary>
        public event Action<T> ItemGot;

        /// <summary> Occurs when an object is marked as available for <see cref="Request" />. </summary>
        public event Action<T> ItemReclaimed;

        /// <summary>
        ///     Returns a new or existing instance from the pool. Note that reclaimed instances are reused,
        ///     so previously requesteds instances which became unused will be returned again after
        ///     <see cref="Reclaim" />.
        /// </summary>
        /// <returns>Next unused instance, or newly allocated instance if all instances are in use.</returns>
        /// <exception cref="InvalidOperationException">
        ///     The pool's allocate method returned a null object reference.
        /// </exception>
        public T Request()
        {
            // if we're out of invalid instances, resize to fit some more
            if (UnusedCount == 0) {
                IncreaseSize((int)(_items.Length * 1.5 - _items.Length));
            }

            // get the next item in the list
            UnusedCount--;
            var item = _items[UnusedCount];

            // if the item is null, we need to allocate a new instance
            if (ReferenceEquals(item, null)) {
                var creator = Spawner ?? Activator.CreateInstance<T>;
                item = creator.Call();

                if (item == null) {
                    throw new InvalidOperationException("Spawner should not return a null object reference.");
                }

                _items[UnusedCount] = item;
            }

            ItemGot.Call(item);
            return item;
        }

        /// <summary>
        ///     Checks each valid object to ensure it is still actually valid;
        ///     makes invalid objects available for <see cref="Request" />.
        /// </summary>
        /// <param name="reclaimUsed">
        ///     if set to <c>true</c>, reclaims up every object in the pool, both used and unused.
        /// </param>
        public void Reclaim(bool reclaimUsed = false)
        {
            for (var i = UnusedCount; i < _items.Length; i++) {
                var item = _items[i];

                // if it's still valid, keep going
                if (!reclaimUsed && IsInstanceStillInUse.Call(item)) continue;

                // otherwise if we're not at the start of the invalid objects, we have to move
                // the object to the invalid object section of the array
                if (i != UnusedCount) {
                    _items[i] = _items[UnusedCount];
                    _items[UnusedCount] = item;
                }

                ItemReclaimed.Call(item);
                UnusedCount++;
            }
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{UsedCount} instances used for '{typeof(T).Name}', with {UnusedCount} unused";
        }

        private void IncreaseSize(int amount)
        {
            var newSize = amount > 0 ? _items.Length + amount : (int)(_items.Length * 1.5);
            if ((newSize <= 0) || (newSize == _items.Length)) {
                throw new OverflowException(
                    $"{nameof(RequestReclaimPool<T>)} should not contain more than Int32.MaxValue items.");
            }

            // create a new array with some more slots and copy over the existing items
            var newItems = new T[newSize];
            for (var i = _items.Length - 1; i >= 0; i--) newItems[i + amount] = _items[i];

            _items = newItems;
            UnusedCount += amount; // move the invalid count based on our resize amount
        }

        /// <summary> Returns an enumerator that iterates through all used instances in the pool. </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through all used instances in the pool.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < UsedCount; i++) {
                yield return _items[i + UnusedCount];
            }
        }

        /// <summary> Returns an enumerator that iterates through all used instances in the pool. </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through all used instances in the pool.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}