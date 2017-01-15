using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Essentions.Collections
{
    /// <summary>
    ///     Simple pool of objects used to remove GC pressure. When you need an object,
    ///     you <see cref="Request" />, and when you don't need it anymore, you <see cref="Return" /> it back.
    /// </summary>
    /// <typeparam name="T">Pool object type.</typeparam>
    [DataContract(Name = "rrp", IsReference = true, Namespace = "")]
    public class RequestReturnPool<T> : IEnumerable<T>
        where T : class
    {
        /// <summary> Initializes a new instance of the <see cref="RequestReturnPool{T}" /> class. </summary>
        /// <param name="spawnObject">
        ///     Object spawner. If null, <see cref="Activator.CreateInstance{T}" /> will be used.
        /// </param>
        /// <param name="initialCapacity">Initial pool capacity.</param>
        /// <exception cref="InvalidOperationException">
        ///     Throws when no allocate function has been provided and
        ///     type of pool's item doesn't have a parameterless constructor.
        /// </exception>
        public RequestReturnPool(int initialCapacity = 32, [CanBeNull] Func<T> spawnObject = null)
        {
            _items = new Stack<T>();
            Spawner = spawnObject ?? Activator.CreateInstance<T>;
            for (var i = 0; i < initialCapacity; i++) {
                var item = Spawner.Call();
                _items.Push(item);
                if (item == null)
                    throw new InvalidOperationException("RequestReturnPool spawner returned a null object reference.");
            }
        }

        [DataMember(Name = "i")]
        private readonly Stack<T> _items;

        /// <summary> Gets the available instances count for this pool. </summary>
        public int UnusedCount => _items.Count;

        /// <summary>
        ///     Function used to create new object on request when the pool is empty.
        ///     If null, <see cref="Activator.CreateInstance{T}" /> will be used.
        /// </summary>
        [CanBeNull]
        public Func<T> Spawner { get; set; }

        /// <summary> Occurs when object is requested from the pool. </summary>
        public event Action<T> ItemRequested;

        /// <summary> Occurs when object is returned to the pool. </summary>
        public event Action<T> ItemReturned;

        /// <summary> Retrieves pool object or creates a new one. Call this when you need an object. </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Pool spawner returned null instead of new object.</exception>
        [NotNull]
        public T Request()
        {
            T item;
            if (_items.Count == 0) {
                var creator = Spawner ?? Activator.CreateInstance<T>;
                item = creator.Call();
                if (item == null)
                    throw new InvalidOperationException("RequestReturnPool spawner returned a null object reference.");
            }
            else {
                item = _items.Pop();
            }

            ItemRequested.Call(item);
            return item;
        }

        /// <summary> Adds object to the pool. Call this when you don't need it anymore. </summary>
        public void Return([NotNull] T item)
        {
            ItemReturned.Call(item);
            _items.Push(item);
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Available instances for '{typeof(T).Name}': {UnusedCount}";
        }

        /// <summary> Returns an enumerator that iterates through available pool instances. </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate
        ///     through available pool instances.
        /// </returns>
        [NotNull]
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary> Returns an enumerator that iterates through available pool instances. </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate
        ///     through available pool instances.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}