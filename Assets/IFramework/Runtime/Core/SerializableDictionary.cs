using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
namespace IFramework
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> :
        
        ISerializationCallbackReceiver,
        IDictionary<TKey, TValue>
    {
        [SerializeField] private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

        [Serializable]
        private struct SerializableKeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public SerializableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private Dictionary<TKey, int> KeyPositions => _keyPositions;
        private Dictionary<TKey, int> _keyPositions;
        public SerializableDictionary()
        {
            _keyPositions = new Dictionary<TKey, int>();
        }


        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _keyPositions = new Dictionary<TKey, int>();
            for (var i = 0; i < list.Count; i++)
            {
                _keyPositions[list[i].Key] = i;
            }
        }

        #region IDictionary<TKey, TValue>

        public TValue this[TKey key]
        {
            get => list[KeyPositions[key]].Value;
            set
            {
                var pair = new SerializableKeyValuePair(key, value);
                if (KeyPositions.ContainsKey(key))
                {
                    list[KeyPositions[key]] = pair; 


                }
                else
                {
                    KeyPositions[key] = list.Count;
                    list.Add(pair);
                }
            }
        }

        public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
        public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

        public void Add(TKey key, TValue value)
        {
            if (KeyPositions.ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
            else
            {
                KeyPositions[key] = list.Count;
                list.Add(new SerializableKeyValuePair(key, value));
            }
        }

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                KeyPositions.Remove(key);

                list.RemoveAt(index);
                for (var i = index; i < list.Count; i++)
                    KeyPositions[list[i].Key] = i;

                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                value = list[index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        #endregion

        #region ICollection <KeyValuePair<TKey, TValue>>
        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

        public void Clear() {
            list.Clear();
            _keyPositions.Clear();
        }
        public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositions.ContainsKey(kvp.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var numKeys = list.Count;
            if (array.Length - arrayIndex < numKeys)
                throw new ArgumentException("arrayIndex");
            for (var i = 0; i < numKeys; i++, arrayIndex++)
            {
                var entry = list[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);

        #endregion

        #region IEnumerable <KeyValuePair<TKey, TValue>>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return list.Select(ToKeyValuePair).GetEnumerator();


        }
        static KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp)
        {
            return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void Add(string localizationType, object getDefault)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
