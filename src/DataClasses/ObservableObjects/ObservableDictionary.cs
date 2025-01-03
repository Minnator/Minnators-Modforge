using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Editor.DataClasses.ObservableObjects
{
   [DebuggerDisplay("Count = {Count}")]
   public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
   {
      private readonly Dictionary<TKey, TValue> _dictionary;

      // Events for modifications
      public event Action<TKey, TValue>? OnAdd;
      public event Action<IEnumerable<KeyValuePair<TKey, TValue>>>? OnAddRange;
      public event Action<TKey, TValue>? OnRemove;
      public event Action<IEnumerable<TKey>>? OnRemoveRange;
      public event Action? OnClear;
      public event Action<TKey, TValue, TValue>? OnUpdate;

      public ObservableDictionary()
      {
         _dictionary = new();
      }

      public ObservableDictionary(IEqualityComparer<TKey> comparer)
      {
         _dictionary = new(comparer);
      }

      public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
      {
         _dictionary = new(dictionary);
      }

      public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
      {
         _dictionary = new(dictionary, comparer);
      }

      public void Add(TKey key, TValue value)
      {
         _dictionary.Add(key, value);
         OnAdd?.Invoke(key, value);
      }

      public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
      {
         var addedItems = new List<KeyValuePair<TKey, TValue>>();
         foreach (var item in items)
         {
            if (_dictionary.TryAdd(item.Key, item.Value))
            {
               addedItems.Add(item);
               OnAdd?.Invoke(item.Key, item.Value);
            }
         }

         if (addedItems.Count > 0)
         {
            OnAddRange?.Invoke(addedItems);
         }
      }

      public bool Remove(TKey key)
      {
         if (_dictionary.Remove(key, out var value))
         {
            OnRemove?.Invoke(key, value);
            return true;
         }
         return false;
      }

      public void RemoveRange(IEnumerable<TKey> keys)
      {
         var removedKeys = new List<TKey>();
         foreach (var key in keys)
         {
            if (_dictionary.Remove(key, out var value))
            {
               removedKeys.Add(key);
               OnRemove?.Invoke(key, value);
            }
         }

         if (removedKeys.Count > 0)
         {
            OnRemoveRange?.Invoke(removedKeys);
         }
      }

      public void Clear()
      {
         if (_dictionary.Count > 0)
         {
            _dictionary.Clear();
            OnClear?.Invoke();
         }
      }

      public TValue this[TKey key]
      {
         get => _dictionary[key];
         set
         {
            if (_dictionary.ContainsKey(key))
            {
               var oldValue = _dictionary[key];
               _dictionary[key] = value;
               OnUpdate?.Invoke(key, oldValue, value);
            }
            else
            {
               Add(key, value);
            }
         }
      }

      #region IDictionary<TKey, TValue> Implementation
      public int Count => _dictionary.Count;

      public bool IsReadOnly => false;

      public ICollection<TKey> Keys => _dictionary.Keys;

      public ICollection<TValue> Values => _dictionary.Values;

      public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

      public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

      public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
          ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);

      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

      void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

      bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
          ((IDictionary<TKey, TValue>)_dictionary).Contains(item);

      bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) =>
          Remove(item.Key);
      #endregion
   }
}
