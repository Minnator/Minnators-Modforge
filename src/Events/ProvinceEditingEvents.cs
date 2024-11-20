using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Events
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
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.owner, ProvAttrSet.owner));
      }

      public static void OnControllerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.controller, ProvAttrSet.controller));
      }

      public static void OnReligionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.religion, ProvAttrSet.religion));
      }

      public static void OnCultureChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.culture, ProvAttrSet.culture));
      }

      public static void OnCapitalNameChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not TextBox tb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, tb.Text, ProvAttrGet.capital, ProvAttrSet.capital));
      }

      public static void OnTextBaseTaxChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttrGet.base_tax, ProvAttrSet.base_tax));
      }

      public static void OnTextBaseProductionChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttrGet.base_production, ProvAttrSet.base_production));
      }

      public static void OnTextBaseManpowerChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not NumericUpDown nup)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, ((int)nup.Value).ToString(), ProvAttrGet.base_manpower, ProvAttrSet.base_manpower));
      }

      public static void OnIsCityChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttrGet.is_city, ProvAttrSet.is_city));
      }

      public static void OnIsHreChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttrGet.hre, ProvAttrSet.hre));
      }

      public static void OnIsSeatInParliamentChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttrGet.seat_in_parliament, ProvAttrSet.seat_in_parliament));
      }

      public static void OnHasRevoltChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not CheckBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.Checked ? "yes" : "no", ProvAttrGet.revolt, ProvAttrSet.revolt));
      }

      public static void OnTradeGoodsChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.trade_good, ProvAttrSet.trade_goods));
      }

      public static void OnCoreAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.cores, ProvAttrSet.add_core, ProvAttrSet.remove_core, true));
      }

      public static void OnCoreRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.cores, ProvAttrSet.remove_core, ProvAttrSet.add_core, false));
      }

      public static void OnClaimAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.claims, ProvAttrSet.add_claim, ProvAttrSet.remove_claim, false));
      }

      public static void OnClaimRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.claims, ProvAttrSet.remove_claim, ProvAttrSet.add_claim, true));
      }

      public static void OnBuildingAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddBuilding(e.Provinces, true, e.Value.ToString()!));
      }

      public static void OnBuildingRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddBuilding(e.Provinces, false, e.Value.ToString()!));
      }

      public static void OnDiscoveredByAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.discovered_by, ProvAttrSet.discovered_by));
      }

      public static void OnDiscoveredByRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, "no", ProvAttrGet.discovered_by, ProvAttrSet.remove_discovered_by));
      }

      public static void OnUpBaseTaxChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 1, true));
      }

      public static void OnUpTaxButtonButtonPressedMedium(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 5, true));
      }

      public static void OnUpBaseTaxButtonPressedLarge(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 10, true));
      }

      public static void OnDownBaseTaxChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 1, false));
      }

      public static void OnDownBaseTaxButtonPressedMedium(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 5, false));
      }

      public static void OnDownBaseTaxButtonPressedLarge(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_tax, ProvAttrSet.base_tax, 10, false));
      }

      public static void OnUpBaseProductionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 1, true));
      }

      public static void OnUpButtonPressedMediumProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 5, true));
      }

      public static void OnUpButtonPressedLargeProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 10, true));
      }

      public static void OnDownBaseProductionChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 1, false));
      }

      public static void OnDownButtonPressedMediumProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 5, false));
      }

      public static void OnDownButtonPressedLargeProduction(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_production, ProvAttrSet.base_production, 10, false));
      }

      public static void OnUpBaseManpowerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 1, true));
      }

      public static void OnUpButtonPressedMediumManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 5, true));
      }

      public static void OnUpButtonPressedLargeManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 10, true));
      }

      public static void OnDownBaseManpowerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 1, false));
      }

      public static void OnDownButtonPressedMediumManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 5, false));
      }

      public static void OnDownButtonPressedLargeManpower(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.base_manpower, ProvAttrSet.base_manpower, 10, false));
      }

      public static void OnTextLocalAutonomyChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy));
      }


      public static void OnUpLocalAutonomyChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 1, true));
      }

      public static void OnUpButtonPressedMediumLocalAutonomy(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 5, true));
      }

      public static void OnUpButtonPressedLargeLocalAutonomy(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 10, true));
      }

      public static void OnDownLocalAutonomyChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 1, false));
      }

      public static void OnDownButtonPressedMediumLocalAutonomy(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 5, false));
      }

      public static void OnDownButtonPressedLargeLocalAutonomy(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.local_autonomy, ProvAttrSet.add_local_autonomy, 10, false));
      }

      public static void OnTextDevastationChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.devastation, ProvAttrSet.devastation));
      }

      public static void OnUpDevastationChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 1, true));
      }

      public static void OnUpButtonPressedMediumDevastation(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 5, true));
      }

      public static void OnUpButtonPressedLargeDevastation(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 10, true));
      }

      public static void OnDownDevastationChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 1, false));
      }

      public static void OnDownButtonPressedMediumDevastation(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 5, false));
      }

      public static void OnDownButtonPressedLargeDevastation(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.devastation, ProvAttrSet.devastation, 10, false));
      }

      public static void OnTextProsperityChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.prosperity, ProvAttrSet.prosperity));
      }

      public static void OnUpProsperityChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 1, true));
      }

      public static void OnUpButtonPressedMediumProsperity(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 5, true));
      }

      public static void OnUpButtonPressedLargeProsperity(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 10, true));
      }

      public static void OnDownProsperityChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 1, false));
      }

      public static void OnDownButtonPressedMediumProsperity(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 5, false));
      }

      public static void OnDownButtonPressedLargeProsperity(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.prosperity, ProvAttrSet.prosperity, 10, false));
      }


      public static void OnTextExtraCostChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost));
      }

      public static void OnUpExtraCostChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 1, true));
      }

      public static void OnUpButtonPressedMediumExtraCost(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 5, true));
      }

      public static void OnUpButtonPressedLargeExtraCost(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 10, true));
      }

      public static void OnDownExtraCostChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 1, false));
      }

      public static void OnDownButtonPressedMediumExtraCost(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 5, false));
      }

      public static void OnDownButtonPressedLargeExtraCost(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CModifyValue(e.Provinces, ProvAttrGet.extra_cost, ProvAttrSet.extra_cost, 10, false));
      }


      public static void OnPermanentClaimAdded(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.permanent_claims, ProvAttrSet.add_permanent_claim, ProvAttrSet.remove_permanent_claim, true));
      }

      public static void OnPermanentClaimRemoved(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CAddRemoveProvinceAttribute(e.Provinces, e.Value.ToString()!, ProvAttrGet.permanent_claims, ProvAttrSet.remove_permanent_claim, ProvAttrSet.add_permanent_claim, false));
      }

      public static void OnTradeCenterChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not ComboBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.SelectedItem?.ToString()!, ProvAttrGet.center_of_trade, ProvAttrSet.center_of_trade));
      }

      public static void OnTradeGoodChanged(object? sender, EventArgs e)
      {
         if (!Globals.AllowEditing || sender is not ComboBox cb)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(Selection.GetSelectedProvinces, cb.SelectedItem?.ToString()!, ProvAttrGet.trade_good, ProvAttrSet.trade_goods));
      }

      public static void OnTribalOwnerChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.tribal_owner, ProvAttrSet.tribal_owner));
      }

      public static void OnNativeFerocityChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.native_ferocity, ProvAttrSet.native_ferocity));
      }

      public static void OnNativeHostilityChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.native_hostileness, ProvAttrSet.native_hostileness));
      }

      public static void OnNativeSizeChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.native_size, ProvAttrSet.native_size));
      }

      public static void OnTradeCompanyInvestmentChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;
         Globals.HistoryManager.AddCommand(new CProvinceAttributeChange(e.Provinces, e.Value.ToString()!, ProvAttrGet.trade_company_investment, ProvAttrSet.add_trade_company_investment));
      }


      // Add Modifier to Province 
      public static void OnModifierAdded(ModifierAbstract modifier, ModifierType type)
      {
         if (!Globals.AllowEditing)
            return;
         Globals.HistoryManager.AddCommand(new CAddRmvModifier(Selection.GetSelectedProvinces,modifier ,type));
      }

      public static void OnModifierRemoved( ModifierAbstract modifier, ModifierType type)
      {
         if (!Globals.AllowEditing)
            return;
         Globals.HistoryManager.AddCommand(new CAddRmvModifier(Selection.GetSelectedProvinces, modifier,
            type, false));
      }

      public static void OnTerrainChanged(object? sender, ProvinceEditedEventArgs e)
      {
         if (!Globals.AllowEditing || e?.Value == null)
            return;

      }
   }
}