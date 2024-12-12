using System.Diagnostics;
using Editor.Saving;

namespace Editor.DataClasses.ObservableObjects
{
   [DebuggerDisplay("Count = {Count}")]
   public class ObservableHashSet<T>(Saveable saveable) : HashSet<T>, ObservableICollection<T> 
   {


      public bool AddNotifyCommand(T item)
      {
         return Add(item);
      }

      public bool AddRangeNotifyCommand(T item)
      {
         throw new NotImplementedException();
      }

      public bool RemoveNotifyCommand(T item)
      {
         return Remove(item);
      }

      public bool RemoveRangeNotifyCommand(T item)
      {
         throw new NotImplementedException();
      }
   }

}