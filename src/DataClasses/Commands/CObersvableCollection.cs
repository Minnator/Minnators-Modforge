using Editor.DataClasses.ObservableObjects;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public class CObersvableCollection<T>(Saveable saveable, ref ObservableICollection<T> collection, ICollection<T> items, bool add) : CSaveableCommand(saveable) 
   {
      private readonly ObservableICollection<T> _collection = collection;

      public override void Execute()
      {
         base.Execute();
         if (add)
            foreach (var item in items)
               _collection.Add(item);
         else
            foreach (var item in items)
               _collection.Remove(item);

      }

      public override void Undo()
      {
         base.Undo();
         if (add)
            foreach (var item in items)
               _collection.Remove(item);
         else
            foreach (var item in items)
               _collection.Add(item);

      }

      public override string GetDescription()
      {
         if (add)
            return "Add " + items.Count + " items to collection";
         return "Remove " + items.Count + " items from collection";
      }

      public override string GetDebugInformation(int indent)
      {
         return new string(' ', indent) + "Items: " + items.Count;
      }
   }
}