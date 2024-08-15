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
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.owner, ProvAttrSetr.owner));
      }

      public static void OnControllerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.controller, ProvAttrSetr.controller));
      }

      public static void OnReligionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.religion, ProvAttrSetr.religion));
      }

      public static void OnCultureChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.culture, ProvAttrSetr.culture));
      }

      public static void OnCapitalNameChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not TextBox tb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, tb.Text, ProvAttr.capital, ProvAttrSetr.capital));
      }

      public static void OnTextBaseTaxChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttr.base_tax, ProvAttrSetr.base_tax));
      }

      public static void OnTextBaseProductionChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttr.base_production, ProvAttrSetr.base_production));
      }

      public static void OnTextBaseManpowerChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttr.base_manpower, ProvAttrSetr.base_manpower));
      }

      public static void OnIsCityChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttr.is_city, ProvAttrSetr.is_city));
      }

      public static void OnIsHreChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttr.hre, ProvAttrSetr.hre));
      }

      public static void OnIsSeatInParliamentChanged (object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttr.seat_in_parliament, ProvAttrSetr.seat_in_parliament));
      }

      public static void OnHasRevoltChanged(object? sender, EventArgs e)
      {
         if (!AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Globals.Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttr.revolt, ProvAttrSetr.revolt));
      }

      public static void OnTradeGoodsChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.trade_good, ProvAttrSetr.trade_goods));
      }
      
      public static void OnCoreAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttr.cores, ProvAttrSetr.add_core, ProvAttrSetr.remove_core, true));
      }

      public static void OnCoreRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute (e.Provinces, e.Value.ToString()!, ProvAttr.cores, ProvAttrSetr.remove_core, ProvAttrSetr.add_core, false));
      }

      public static void OnClaimAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttr.claims, ProvAttrSetr.add_claim, ProvAttrSetr.remove_claim, false));
      }

      public static void OnClaimRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttr.claims, ProvAttrSetr.remove_claim, ProvAttrSetr.add_claim, true));
      }

      public static void OnBuildingAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         //TODO own command
         Globals.HistoryManager.AddCommand(new CAddBuilding(e.Provinces, true, e.Value.ToString()!));
      }

      public static void OnBuildingRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddBuilding(e.Provinces, false, e.Value.ToString()!));
      }

      public static void OnDiscoveredByAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttr.discovered_by, ProvAttrSetr.discovered_by));
      }

      public static void OnDiscoveredByRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, "no", ProvAttr.discovered_by, ProvAttrSetr.remove_discovered_by));
      }

      public static void OnUpBaseTaxChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 1, true));
      }

      public static void OnUpTaxButtonButtonPressedMedium(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 5, true));
      }

      public static void OnUpBaseTaxButtonPressedLarge(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 10, true));
      }

      public static void OnDownBaseTaxChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 1, false));
      }

      public static void OnDownBaseTaxButtonPressedMedium(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 5, false));
      }

      public static void OnDownBaseTaxButtonPressedLarge(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_tax, ProvAttrSetr.base_tax, 10, false));
      }

      public static void OnUpBaseProductionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 1, true));
      }

      public static void OnUpButtonPressedMediumProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 5, true));
      }

      public static void OnUpButtonPressedLargeProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 10, true));
      }

      public static void OnDownBaseProductionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 1, false));
      }

      public static void OnDownButtonPressedMediumProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 5, false));
      }

      public static void OnDownButtonPressedLargeProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_production, ProvAttrSetr.base_production, 10, false));
      }

      public static void OnUpBaseManpowerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 1, true));
      }

      public static void OnUpButtonPressedMediumManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 5, true));
      }

      public static void OnUpButtonPressedLargeManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 10, true));
      }

      public static void OnDownBaseManpowerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 1, false));
      }

      public static void OnDownButtonPressedMediumManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 5, false));
      }

      public static void OnDownButtonPressedLargeManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces,ProvAttr.base_manpower, ProvAttrSetr.base_manpower, 10, false));
      }

   }
}