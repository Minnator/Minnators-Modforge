using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Editor.DataClasses.Misc;

public class BindingDictionary<TKey, TValue> : BindingList<TKey>, IDictionary<TKey, TValue> where TKey : notnull
{
   private readonly Dictionary<TKey, TValue> _internalDictionary = [];
   private readonly List<ComboBox> _boundControls = [];
   private readonly List<TKey> _internalList;
   private readonly IComparer<TKey> _keyComparer;

   private BindingDictionary(List<TKey> list, IComparer<TKey>? comparer) : base(list) 
   {
      _keyComparer = comparer ?? Comparer<TKey>.Default;
      _internalList = list;
   }
   
   public BindingDictionary(IComparer<TKey>? comparer = null) : this([], comparer) {  }

   public void Sort()
   {
      _internalList.Sort();
   }

   // Override InsertItem to maintain sorting if necessary
   public void InsertItemSorted(TKey item)
   {
      var index = _internalList.BinarySearch(item, _keyComparer);
      index = index >= 0 ? index : ~index;
      base.InsertItem(index, item);
   }

   public void AddControl(ComboBox control)
   {
      _boundControls.Add(control);
   }

   public void RemoveControl(ComboBox control)
   {
      _boundControls.Remove(control);
   }

   private void BeginUpdate()
   {
      foreach (var control in _boundControls)
         control.BeginUpdate();
   }

   private void EndUpdate()
   {
      foreach (var control in _boundControls)
         control.EndUpdate();
   }

   public void Add(TKey key, TValue value)
   {
      BeginUpdate();
      _internalDictionary.Add(key, value);
      InsertItemSorted(key);
      EndUpdate();
   }
   
   public new bool Remove(TKey key)
   {
      BeginUpdate();
      if (_internalDictionary.Remove(key))
         if (!base.Remove(key))
            throw new InvalidOperationException("Failed to remove key from base list");
      EndUpdate();
      return true;

   }
   
   public bool ContainsKey(TKey key)
   {
      return _internalDictionary.ContainsKey(key);
   }

   public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
   {
      return _internalDictionary.TryGetValue(key, out value);
   }

   public TValue this[TKey key]
   {
      get => _internalDictionary[key];
      set => _internalDictionary[key] = value;
   }

   public ICollection<TKey> Keys => _internalDictionary.Keys;
   public ICollection<TValue> Values => _internalDictionary.Values;

   public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
   {
      return _internalDictionary.GetEnumerator();
   }

   public void Add(KeyValuePair<TKey, TValue> item)
   {
      Add(item.Key, item.Value);
   }

   public bool Contains(KeyValuePair<TKey, TValue> item)
   {
      return _internalDictionary.Contains(item);
   }

   public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
   {
      _internalDictionary.ToArray().CopyTo(array, arrayIndex);
   }

   public bool Remove(KeyValuePair<TKey, TValue> item)
   {
      return Remove(item.Key);
   }

   public bool IsReadOnly => false;
}