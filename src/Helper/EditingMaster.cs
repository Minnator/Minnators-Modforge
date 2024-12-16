
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;

namespace Editor.Helper
{
   public enum EditingTarget
   {
      None = 0,
      Province,
      Country,
      EventModifier
   }

   public static class EditingMaster
   {
      /// <summary>
      /// If true, the program will create history entries for the changes instead of just changing the base data.
      /// </summary>
      public static bool CreateHistoryEntries { get; set; } = false;
      /// <summary>
      /// The current type of object which will be edited. 
      /// </summary>
      public static EditingTarget EditTarget { get; set; } = EditingTarget.None;

      // We need to add a way to track which control from the gui caused a new command to be created to only update that control.

      /// <summary>
      /// Every Gui interaction should call this method to Modify the object properties.
      /// This Method decides whether to create a history entry of the base values.
      /// This method can also add a control to the last command in the history tree for the current branch
      /// </summary>
      /// <param name="objects"></param>
      /// <param name="propName"></param>
      /// <param name="value"></param>
      /// <param name="control"></param>
      public static void ModifyObjectProperties<T>(ICollection<IHistoryProvider<T>> objects, string propName, string value, Control control) where T : HistoryEntry
      {
         if (CreateHistoryEntries)
            ModifyByHistoryEntry(objects, propName, value);
         else
            ModifyByBaseValues(objects, propName, value);

         // Add control to last command in history tree
      }

      private static void ModifyByHistoryEntry<T>(ICollection<IHistoryProvider<T>> objects, string propName, string value) where T : HistoryEntry
      {

      }

      private static void ModifyByBaseValues<T>(ICollection<IHistoryProvider<T>> objects, string propName, string value) where T : HistoryEntry
      {

      }
   }
}