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

      public static void OnIsCityChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", "is_city"));
      }

      public static void OnIsHreChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", "hre"));
      }

      public static void OnIsSeatInParliamentChanged (object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", "seat_in_parliament"));
      }

      public static void OnHasRevoltChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : string.Empty, "revolt"));
      }

      public static void OnTradeGoodsChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "trade_goods"));
      }


      public static void OnCoreAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "add_core"));
      }

      public static void OnCoreRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "remove_core"));
      }

      public static void OnClaimAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "add_claim"));
      }

      public static void OnClaimRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "remove_claim"));
      }


      public static void OnBuildingAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, "yes", e.Value.ToString()!));
      }

      public static void OnBuildingRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, "no", e.Value.ToString()!));
      }

      public static void OnDiscoveredByAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "discovered_by"));
      }

      public static void OnDiscoveredByRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, "remove_discovered_by"));
      }
   }
}