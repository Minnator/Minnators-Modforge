using Editor.Saving;

namespace Editor.DataClasses.Misc
{
   public class ObservableHashSet<T>(Saveable saveable) : HashSet<T>
   {

      public new bool Add(T item)
      {
         //TODO implement the command reverseable fun
         if (Globals.State == State.Running)
            saveable.EditingStatus = ObjEditingStatus.Modified;
         return base.Add(item);
      }

      public new bool Remove(T item)
      {
         if (Globals.State == State.Running)
            saveable.EditingStatus = ObjEditingStatus.Modified;
         return base.Remove(item);
      }

   }
}