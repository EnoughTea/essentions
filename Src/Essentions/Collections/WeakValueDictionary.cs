using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Essentions.Collections
{
    /// <summary>
    ///     Dictionary with weak-referenced values. Null values are supported.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [DataContract(Name = "weakDict", IsReference = true, Namespace = "")]
    [KnownType(nameof(GetKnownTypesForSerializer))]
    public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>Represents null value in dictionary.</summary>
        private static readonly NullObject _NullObject = new NullObject();

        /// <summary>Initializes a new instance of the <see cref="WeakValueDictionary{TKey,TValue}" /> class.</summary>
        public WeakValueDictionary()
            : this(32)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WeakValueDictionary{TKey,TValue}" /> class.</summary>
        /// <param name="capacity">Initial capacity.</param>
        public WeakValueDictionary(int capacity)
        {
            _pool = new RequestReturnPool<WeakReference>(capacity, () => new WeakReference(null));
            _inner = new Dictionary<TKey, WeakReference>(capacity);
            _garbageCollectionSentinel = new WeakReference(new object());
            _deadKeys = new Queue<KeyValuePair<TKey, WeakReference>>();
        }

        /// <summary>Contains dead keys to remove from dictionary.</summary>
        [DataMember(Name = "dead")]
        private readonly Queue<KeyValuePair<TKey, WeakReference>> _deadKeys;

        [DataMember(Name = "data")]
        private readonly Dictionary<TKey, WeakReference> _inner;

        /// <summary>
        ///     Serves as a "GC Monitor" that indicates whether cleanup is needed.
        ///     If its <c>IsAlive</c> property is false, GC has occurred and we should perform cleanup.
        ///     Non-serialized, so needs to be recreated on access.
        /// </summary>
        private WeakReference _garbageCollectionSentinel;

        // Non-serialized, so needs to be recreated on access.
        private RequestReturnPool<WeakReference> _pool;

        /// <summary>Returns a <see cref="System.String" /> that represents this instance.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return "Count: " + Count;
        }

        [UsedImplicitly]
        internal static IEnumerable<Type> GetKnownTypesForSerializer()
        {
            yield return typeof(NullObject);
            yield return typeof(WeakReference);
        }

        [CanBeNull]
        private static TObject DecodeNullObject<TObject>(object innerValue)
        {
            return _NullObject.Equals(innerValue) ? default(TObject) : (TObject)innerValue;
        }

        private static object EncodeNullObject(object value)
        {
            return value ?? _NullObject;
        }

        /// <summary>Remove keys whose values were garbage-collected.</summary>
        private void CleanAbandonedItems()
        {
            foreach (var kvp in _inner.Where(kvp => kvp.Value.Target == null)) _deadKeys.Enqueue(kvp);

            while (_deadKeys.Count > 0) {
                var kvp = _deadKeys.Dequeue();
                GetPool().Return(kvp.Value);
                _inner.Remove(kvp.Key);
            }
        }

        /// <summary> Performs cleanup if GC occurred. </summary>
        private void CleanIfNeeded()
        {
            if (_garbageCollectionSentinel == null) {
                _garbageCollectionSentinel = new WeakReference(new object());
            }
            else if (_garbageCollectionSentinel.Target == null) {
                CleanAbandonedItems();
                _garbageCollectionSentinel.Target = new object();
            }
        }

        private RequestReturnPool<WeakReference> GetPool()
        {
            return _pool ?? (_pool = new RequestReturnPool<WeakReference>(_inner.Count, () => new WeakReference(null)));
        }

        /// <summary> Gets the number of items in the dictionary. </summary>
        /// <remarks>
        ///     Since the items in the dictionary are held by weak reference, the count value
        ///     cannot be relied upon to guarantee the number of objects that would be discovered via
        ///     enumeration. Treat the Count as an estimate only.  This property also has the side effect
        ///     of clearing out any GC'd refs.
        /// </remarks>
        public int Count
        {
            get {
                CleanAbandonedItems();
                return _inner.Count;
            }
        }

        /// <summary> Retrieves a value from the dictionary. </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>The value in the dictionary.</returns>
        /// <exception cref="KeyNotFoundException">
        ///     Thrown when the key does exist in the dictionary.
        ///     Since the dictionary contains weak references, the key may have been removed by the
        ///     garbage collection of the object.
        /// </exception>
        [CanBeNull]
        public TValue this[TKey key]
        {
            get {
                Check.NotNull(key);

                TValue result;
                if (TryGetValue(key, out result)) return result;

                throw new KeyNotFoundException();
            }

            set {
                Check.NotNull(key);

                CleanIfNeeded();

                var wr = GetPool().Request();
                wr.Target = EncodeNullObject(value);
                _inner[key] = wr;
            }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />. Not a static copy, refers to original keys.
        /// </summary>
        public ICollection<TKey> Keys => _inner.Keys;

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />. Returns a copy of all non-null values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get {
                return _inner.Values
                             .Select(weakRef => DecodeNullObject<TValue>(weakRef.Target))
                             .Where(value => !ReferenceEquals(value, null))
                             .ToArray();
            }
        }

        /// <summary> Adds a new item to the dictionary. </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">Key is already present in this dictionary.</exception>
        public void Add([NotNull] TKey key, [CanBeNull] TValue value)
        {
            Check.NotNull(key);

            CleanIfNeeded();

            TValue dummy;
            if (TryGetValue(key, out dummy)) throw new ArgumentException("Key is already present in this dictionary.");

            var wr = GetPool().Request();
            wr.Target = EncodeNullObject(value);
            _inner.Add(key, wr);
        }

        /// <summary>Removes all keys and values from the dictionary.</summary>
        public void Clear()
        {
            foreach (var wr in _inner.Values) GetPool().Return(wr);

            _inner.Clear();
        }

        /// <summary> Determines if the dictionary contains the specified key. </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key exists in the dictionary; false otherwise.</returns>
        public bool ContainsKey([NotNull] TKey key)
        {
            Check.NotNull(key);

            TValue dummy;
            return TryGetValue(key, out dummy);
        }

        /// <summary> Gets an enumerator over the values in the dictionary. </summary>
        /// <returns>The enumerator.</returns>
        /// <remarks>
        ///     As objects are discovered and returned from the enumerator, a strong reference
        ///     is temporarily held on the object so that it will continue to exist for the duration of
        ///     the enumeration. Once the enumeration of that object is over, the strong reference is
        ///     removed. If you wish to keep values alive for use after enumeration, to ensure that they
        ///     stay alive, you should store strong references to them during enumeration.
        /// </remarks>
        [NotNull]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in _inner) {
                var innerValue = kvp.Value.Target;

                if (innerValue != null)
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, DecodeNullObject<TValue>(innerValue));
            }
        }

        /// <summary> Removes an item from the dictionary. </summary>
        /// <param name="key">The key of the item to be removed.</param>
        /// <returns>Returns true if the key was in the dictionary; return false otherwise.</returns>
        public bool Remove([NotNull] TKey key)
        {
            Check.NotNull(key);

            return _inner.Remove(key);
        }

        /// <summary> Attempts to get a value from the dictionary. </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Returns true if the value was present; false otherwise.</returns>
        public bool TryGetValue([NotNull] TKey key, [CanBeNull] out TValue value)
        {
            Check.NotNull(key);

            value = default(TValue);
            WeakReference wr;
            if (!_inner.TryGetValue(key, out wr)) return false;

            var result = wr.Target;
            if (result == null) {
                _inner.Remove(key);
                GetPool().Return(wr);
                return false;
            }

            value = DecodeNullObject<TValue>(result);
            return true;
        }

        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
        ///     the collection.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region NullObject

        /// <summary>Represents null in the weak dictionary.</summary>
        [DataContract(Name = "null", IsReference = true, Namespace = "")]
        private class NullObject : IEquatable<NullObject>
        {
            public override bool Equals(object obj)
            {
                return (obj == null) || obj is NullObject;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public static bool operator ==(NullObject left, NullObject right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(NullObject left, NullObject right)
            {
                return !Equals(left, right);
            }

            public bool Equals(NullObject other)
            {
                return true;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     is read-only.
        /// </summary>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Check.NotNull(item.Key);

            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     contains a specific value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            Check.NotNull(item.Key);

            TValue value;
            return TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this) {
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </param>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        ///     This method also returns false if <paramref name="item" /> is not found in the original
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
                return Remove(item.Key);

            return false;
        }

        #endregion
    }
}