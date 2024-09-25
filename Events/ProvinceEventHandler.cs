using System;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Events;

public static class ProvinceEventHandler
{
   // Contains all the events for changes in the province data which can be subscribed to by e.g. the map modes

   public class ProvinceDataChangedEventArgs(object value, string propertyName)
      : EventArgs
   {
      public object Value = value;
      public string PropertyName = propertyName;

      public override string ToString()
      {
         return $"{PropertyName}: {Value}";
      }
   }

   // Will notify of any changes in the province data
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDataChanged = delegate { };
   public static void RaiseProvinceDataChanged(int id, object value, string propertyName)
   {
      Globals.Provinces[id].ProvinceDataChanged(null, new(value, propertyName)); // Update the province status
      OnProvinceDataChanged.Invoke(id, new(value, propertyName));
   }

   // Claims
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceClaimsChanged = delegate { };
   public static void RaiseProvinceClaimsChanged(int id, object value, string propertyName)
   {
      OnProvinceClaimsChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvincePermanentClaimsChanged = delegate { };
   public static void RaiseProvincePermanentClaimsChanged(int id, object value, string propertyName)
   {
      OnProvincePermanentClaimsChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Cores
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCoresChanged = delegate { };
   public static void RaiseProvinceCoresChanged(int id, object value, string propertyName)
   {
      OnProvinceCoresChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }
   // Controller
   // EVERY MAPMODE SHOWING OCCUPATION SHOULD SUBSCRIBE TO THIS EVENT
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceControllerChanged = delegate { };
   public static void RaiseProvinceControllerChanged(int id, object value, string propertyName)
   {
      OnProvinceControllerChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Owner
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceOwnerChanged = delegate { };
   public static void RaiseProvinceOwnerChanged(int id, object value, string propertyName)
   {
      OnProvinceOwnerChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // TribalOwner
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTribalOwnerChanged = delegate { };
   public static void RaiseProvinceTribalOwnerChanged(int id, object value, string propertyName)
   {
      OnProvinceTribalOwnerChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // BaseManpower
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseManpowerChanged = delegate { };
   public static void RaiseProvinceBaseManpowerChanged(int id, object value, string propertyName)
   {
      OnProvinceBaseManpowerChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // BaseTax
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseTaxChanged = delegate { };
   public static void RaiseProvinceBaseTaxChanged(int id, object value, string propertyName)
   {
      OnProvinceBaseTaxChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // BaseProduction
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseProductionChanged = delegate { };
   public static void RaiseProvinceBaseProductionChanged(int id, object value, string propertyName)
   {
      OnProvinceBaseProductionChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // CenterOfTradeLevel
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCenterOfTradeLevelChanged = delegate { };
   public static void RaiseProvinceCenterOfTradeLevelChanged(int id, object value, string propertyName)
   {
      OnProvinceCenterOfTradeLevelChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // ExtraCost
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceExtraCostChanged = delegate { };
   public static void RaiseProvinceExtraCostChanged(int id, object value, string propertyName)
   {
      OnProvinceExtraCostChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // NativeFerocity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeFerocityChanged = delegate { };
   public static void RaiseProvinceNativeFerocityChanged(int id, object value, string propertyName)
   {
      OnProvinceNativeFerocityChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // NativeHostileness
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeHostilenessChanged = delegate { };
   public static void RaiseProvinceNativeHostilenessChanged(int id, object value, string propertyName)
   {
      OnProvinceNativeHostilenessChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // NativeSize
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeSizeChanged = delegate { };
   public static void RaiseProvinceNativeSizeChanged(int id, object value, string propertyName)
   {
      OnProvinceNativeSizeChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // RevoltRisk 
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceRevoltRiskChanged = delegate { };
   public static void RaiseProvinceRevoltRiskChanged(int id, object value, string propertyName)
   {
      OnProvinceRevoltRiskChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // CitySize
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCitySizeChanged = delegate { };
   public static void RaiseProvinceCitySizeChanged(int id, object value, string propertyName)
   {
      OnProvinceCitySizeChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // LocalAutonomy
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceLocalAutonomyChanged = delegate { };
   public static void RaiseProvinceLocalAutonomyChanged(int id, object value, string propertyName)
   {
      OnProvinceLocalAutonomyChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Nationalism
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNationalismChanged = delegate { };
   public static void RaiseProvinceNationalismChanged(int id, object value, string propertyName)
   {
      OnProvinceNationalismChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // DiscoveredBy
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDiscoveredByChanged = delegate { };
   public static void RaiseProvinceDiscoveredByChanged(int id, object value, string propertyName)
   {
      OnProvinceDiscoveredByChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Capital
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCapitalChanged = delegate { };
   public static void RaiseProvinceCapitalChanged(int id, object value, string propertyName)
   {
      OnProvinceCapitalChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Culture
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCultureChanged = delegate { };
   public static void RaiseProvinceCultureChanged(int id, object value, string propertyName)
   {
      OnProvinceCultureChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Religion
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceReligionChanged = delegate { };
   public static void RaiseProvinceReligionChanged(int id, object value, string propertyName)
   {
      OnProvinceReligionChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Buildings
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBuildingsChanged = delegate { };
   public static void RaiseProvinceBuildingsChanged(int id, object value, string propertyName)
   {
      OnProvinceBuildingsChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // IsHre
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsHreChanged = delegate { };
   public static void RaiseProvinceIsHreChanged(int id, object value, string propertyName)
   {
      OnProvinceIsHreChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // IsCity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsCityChanged = delegate { };
   public static void RaiseProvinceIsCityChanged(int id, object value, string propertyName)
   {
      OnProvinceIsCityChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // IsSeatInParliament
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsSeatInParliamentChanged = delegate { };
   public static void RaiseProvinceIsSeatInParliamentChanged(int id, object value, string propertyName)
   {
      OnProvinceIsSeatInParliamentChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // TradeGood
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeGoodChanged = delegate { };
   public static void RaiseProvinceTradeGoodChanged(int id, object value, string propertyName)
   {
      OnProvinceTradeGoodChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Area
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceAreaChanged = delegate { };
   public static void RaiseProvinceAreaChanged(int id, object value, string propertyName)
   {
      OnProvinceAreaChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Continent
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceContinentChanged = delegate { };
   public static void RaiseProvinceContinentChanged(int id, object value, string propertyName)
   {
      OnProvinceContinentChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // History
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceHistoryChanged = delegate { };
   public static void RaiseProvinceHistoryChanged(int id, object value, string propertyName)
   {
      OnProvinceHistoryChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // latentTradeGood
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceLatentTradeGoodChanged = delegate { };
   public static void RaiseProvinceLatentTradeGoodChanged(int id, object value, string propertyName)
   {
      OnProvinceLatentTradeGoodChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // HasRevolt
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceHasRevoltChanged = delegate { };
   public static void RaiseProvinceHasRevoltChanged(int id, object value, string propertyName)
   {
      OnProvinceHasRevoltChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // TradeCompany
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeCompanyChanged = delegate { };
   public static void RaiseProvinceTradeCompanyChanged(int id, object value, string propertyName)
   {
      OnProvinceTradeCompanyChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // TradeCompanyInvestment
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeCompanyInvestmentChanged = delegate { };
   public static void RaiseProvinceTradeCompanyInvestmentChanged(int id, object value, string propertyName)
   {
      OnProvinceTradeCompanyInvestmentChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // ProvinceModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProvinceModifiersChanged = delegate { };
   public static void RaiseProvinceProvinceModifiersChanged(int id, object value, string propertyName)
   {
      OnProvinceProvinceModifiersChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // ProvinceTriggeredModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProvinceTriggeredModifiersChanged = delegate { };
   public static void RaiseProvinceProvinceTriggeredModifiersChanged(int id, object value, string propertyName)
   {
      OnProvinceProvinceTriggeredModifiersChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // ProvinceTradeModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProvinceTradeModifiersChanged = delegate { };
   public static void RaiseProvinceTradeModifiersChanged(int id, object value, string propertyName)
   {
      OnProvinceProvinceTradeModifiersChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Effects
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceScriptedEffectsChanged = delegate { };
   public static void RaiseProvinceScriptedEffectsChanged(int id, object value, string propertyName)
   {
      OnProvinceScriptedEffectsChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // ReformationCenter
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceReformationCenterChanged = delegate { };
   public static void RaiseProvinceReformationCenterChanged(int id, object value, string propertyName)
   {
      OnProvinceReformationCenterChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // PermanentProvinceModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvincePermanentProvinceModifiersChanged = delegate { };
   public static void RaiseProvincePermanentProvinceModifiersChanged(int id, object value, string propertyName)
   {
      OnProvincePermanentProvinceModifiersChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Devastation 
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDevastationChanged = delegate { };
   public static void RaiseProvinceDevastationChanged(int id, object value, string propertyName)
   {
      OnProvinceDevastationChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Prosperity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProsperityChanged = delegate { };
   public static void RaiseProvinceProsperityChanged(int id, object value, string propertyName)
   {
      OnProvinceProsperityChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }

   // Region Areas set
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceRegionAreasChanged = delegate { };
   public static void RaiseProvinceRegionAreasChanged(int id, object value, string propertyName)
   {
      OnProvinceRegionAreasChanged.Invoke(id, new(value, propertyName));
      RaiseProvinceDataChanged(id, value, propertyName);
   }
}