using System.Collections;
using System.Diagnostics;
using Editor.Saving;

namespace Editor.DataClasses.ObservableObjects
{
   [DebuggerDisplay("Count = {Count}")]
   public class ObservableHashSet<T> : ICollection<T>
   {
      private readonly HashSet<T> _hashSet;

      // Events for modifications
      public event Action<T>? OnAdd;
      public event Action<IEnumerable<T>>? OnAddRange;
      public event Action<T>? OnRemove;
      public event Action<IEnumerable<T>>? OnRemoveRange;
      public event Action? OnClear;

      public ObservableHashSet()
      {
         _hashSet = [];
      }

      public ObservableHashSet(IEqualityComparer<T> comparer)
      {
         _hashSet = new(comparer);
      }

      public ObservableHashSet(IEnumerable<T> collection)
      {
         _hashSet = [.. collection];
      }

      public ObservableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
      {
         _hashSet = new(collection, comparer);
      }

      public bool Add(T item)
      {
         if (_hashSet.Add(item))
         {
            OnAdd?.Invoke(item);
            return true;
         }
         return false;
      }

      public void AddRange(IEnumerable<T> items)
      {
         var addedItems = new List<T>();
         foreach (var item in items)
         {
            if (_hashSet.Add(item))
            {
               addedItems.Add(item);
               OnAdd?.Invoke(item);
            }
         }

         if (addedItems.Count > 0)
         {
            OnAddRange?.Invoke(addedItems);
         }
      }

      public bool Remove(T item)
      {
         if (_hashSet.Remove(item))
         {
            OnRemove?.Invoke(item);
            return true;
         }
         return false;
      }

      public void RemoveRange(IEnumerable<T> items)
      {
         var removedItems = new List<T>();
         foreach (var item in items)
         {
            if (_hashSet.Remove(item))
            {
               removedItems.Add(item);
               OnRemove?.Invoke(item);
            }
         }

         if (removedItems.Count > 0)
         {
            OnRemoveRange?.Invoke(removedItems);
         }
      }

      public void Clear()
      {
         if (_hashSet.Count > 0)
         {
            _hashSet.Clear();
            OnClear?.Invoke();
         }
      }

      #region ICollection<T> Implementation
      public int Count => _hashSet.Count;

      public bool IsReadOnly => false;

      public void CopyTo(T[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);

      public bool Contains(T item) => _hashSet.Contains(item);

      public IEnumerator<T> GetEnumerator() => _hashSet.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => _hashSet.GetEnumerator();

      void ICollection<T>.Add(T item) => Add(item);
      #endregion
   }

}