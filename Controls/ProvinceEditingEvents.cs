using Editor.Commands;
using Editor.DataClasses.Commands;
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
      private static bool AllowEditing
      {
         get
         {
            return Globals.EditingStatus == EditingStatus.Idle;
         }
      }

      public static void OnOwnerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeOwner(e.Provinces, e.Value.ToString()!));
      }

      public static void OnControllerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeController(e.Provinces, e.Value.ToString()!));
      }

      public static void OnReligionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeReligion(e.Provinces, e.Value.ToString()!));
      }

      public static void OnCultureChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CChangeCulture(e.Provinces, e.Value.ToString()!));
      }

      public static void OnCapitalNameChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not TextBox tb)
            return;
         Globals.HistoryManager.AddCommand(new CChangeCapitalName(Globals.Selection.GetSelectedProvinces, tb.Text));
      }

      public static void OnBaseTaxChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CSetDevelopment(Globals.Selection.GetSelectedProvinces, (int)nup.Value, 0));
      }

      public static void OnBaseProductionChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CSetDevelopment(Globals.Selection.GetSelectedProvinces, (int)nup.Value, 1));
      }

      public static void OnBaseManpowerChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CSetDevelopment(Globals.Selection.GetSelectedProvinces, (int)nup.Value, 2));
      }
   }
}