using Editor.DataClasses.ObservableObjects;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CObersveableCollection<T> : ICommand
   {
      private readonly ObservableICollection<T> _collection;

      private readonly SaveableCommandHelper _collectionOwner;
      private readonly bool _add;
      private readonly ICollection<T> _items;

      public CObersveableCollection(Saveable saveable, ref ObservableICollection<T> collection, ICollection<T> items, bool add)
      {
         _collection = collection;
         _collectionOwner = new (saveable);
         _items = items;
         _add = add;
      }

      public void Execute()
      {
         _collectionOwner.Execute();
         if (_add)
            foreach (var item in _items)
               _collection.Add(item);
         else
            foreach (var item in _items)
               _collection.Remove(item);

      }

      public void Undo()
      {
         _collectionOwner.Undo();
         if (_add)
            foreach (var item in _items)
               _collection.Remove(item);
         else
            foreach (var item in _items)
               _collection.Add(item);
      }

      public void Redo()
      {
         _collectionOwner.Redo();
      }

      public string GetDescription()
      {
         return _add ? "Add " : "Remove " + _items.Count + " items";
      }

      public string GetDebugInformation(int indent)
      {
         return new string(' ', indent) + GetDescription();
      }
   }
}