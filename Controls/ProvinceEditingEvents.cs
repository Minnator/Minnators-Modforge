using Editor.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Controls
{
   public class ProvinceEditedEventArgs(List<Province> provinces, object value) : EventArgs
   {
      public List<Province> Provinces { get; set; } = provinces;
      public object Value { get; set; } = value;
   }

   public static class ProvinceEditingEvents
   {
      public static void OnOwnerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (Globals.EditingStatus != EditingStatus.Idle || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeOwner(e.Provinces, e.Value.ToString()!));
      }

      public static void OnControllerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (Globals.EditingStatus != EditingStatus.Idle || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeController(e.Provinces, e.Value.ToString()!));
      }
   }
}