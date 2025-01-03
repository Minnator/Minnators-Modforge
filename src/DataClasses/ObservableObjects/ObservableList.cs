using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Editor.DataClasses.ObservableObjects
{
   [DebuggerDisplay("Count = {Count}")]
   public class ObservableList<T> : IList<T>
   {
      private readonly List<T> _list;

      // Events for modifications
      public event Action<T>? OnAdd;
      public event Action<IEnumerable<T>>? OnAddRange;
      public event Action<T>? OnRemove;
      public event Action<IEnumerable<T>>? OnRemoveRange;
      public event Action<int, T>? OnInsert;
      public event Action<int, IEnumerable<T>>? OnInsertRange;
      public event Action<int, T, T>? OnReplace;
      public event Action? OnClear;

      public ObservableList()
      {
         _list = new List<T>();
      }

      public ObservableList(IEnumerable<T> collection)
      {
         _list = new List<T>(collection);
      }

      public ObservableList(int capacity)
      {
         _list = new List<T>(capacity);
      }

      public void Add(T item)
      {
         _list.Add(item);
         OnAdd?.Invoke(item);
      }

      public void AddRange(IEnumerable<T> items)
      {
         var addedItems = new List<T>(items);
         _list.AddRange(addedItems);
         OnAddRange?.Invoke(addedItems);
      }

      public bool Remove(T item)
      {
         if (_list.Remove(item))
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
            if (_list.Remove(item))
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

      public void Insert(int index, T item)
      {
         _list.Insert(index, item);
         OnInsert?.Invoke(index, item);
      }

      public void InsertRange(int index, IEnumerable<T> items)
      {
         var addedItems = new List<T>(items);
         _list.InsertRange(index, addedItems);
         OnInsertRange?.Invoke(index, addedItems);
      }

      public void Replace(int index, T newItem)
      {
         var oldItem = _list[index];
         _list[index] = newItem;
         OnReplace?.Invoke(index, oldItem, newItem);
      }

      public void Clear()
      {
         if (_list.Count > 0)
         {
            _list.Clear();
            OnClear?.Invoke();
         }
      }

      #region IList<T> Implementation
      public int Count => _list.Count;

      public bool IsReadOnly => false;

      public T this[int index]
      {
         get => _list[index];
         set => Replace(index, value);
      }

      public int IndexOf(T item) => _list.IndexOf(item);

      public bool Contains(T item) => _list.Contains(item);

      public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

      public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

      public void RemoveAt(int index)
      {
         var item = _list[index];
         _list.RemoveAt(index);
         OnRemove?.Invoke(item);
      }
      #endregion
   }
}
