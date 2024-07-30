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
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "owner"));
      }

      public static void OnControllerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "controller"));
      }

      public static void OnReligionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "religion"));
      }

      public static void OnCultureChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "culture"));
      }

      public static void OnCapitalNameChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not TextBox tb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, tb.Text, "capital"));
      }

      public static void OnTextBaseTaxChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), "base_tax"));
      }

      public static void OnTextBaseProductionChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), "base_production"));
      }

      public static void OnTextBaseManpowerChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), "base_manpower"));
      }
   }
}