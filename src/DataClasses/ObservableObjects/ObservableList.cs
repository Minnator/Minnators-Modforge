using System.Diagnostics;
using Editor.Saving;

namespace Editor.DataClasses.ObservableObjects
{
   [DebuggerDisplay("Count = {Count}")]
   public class ObservableList<T>(Saveable saveable) : List<T>
   {
      public new void Add(T item)
      {
         //TODO implement the command reverseable fun
         if (Globals.State == State.Running)
            saveable.EditingStatus = ObjEditingStatus.Modified;
         base.Add(item);
      }

      public new bool Remove(T item)
      {
         if (Globals.State == State.Running)
            saveable.EditingStatus = ObjEditingStatus.Modified;
         return base.Remove(item);
      }
   }
}