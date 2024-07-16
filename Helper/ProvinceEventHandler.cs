using System;

namespace Editor.Helper;

public static class ProvinceEventHandler
{
   // Contains all the events for changes in the province data which can be subscribed to by e.g. the map modes
   
   public class ProvinceDataChangedEventArgs(object value, object oldValue, string propertyName)
      : EventArgs
   {
      public object Value = value;
      public object OldValue = oldValue;
      public string PropertyName = propertyName;
   }

   // Will notify of any changes in the province data
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDataChanged = delegate { };
   public static void RaiseProvinceDataChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceDataChanged.Invoke(id, new (value, oldValue, propertyName));
   }

   // Claims
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceClaimsChanged = delegate { };
   public static void RaiseProvinceClaimsChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceClaimsChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Cores
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCoresChanged = delegate { };
   public static void RaiseProvinceCoresChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceCoresChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }
   // Controller
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceControllerChanged = delegate { };
   public static void RaiseProvinceControllerChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceControllerChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Owner
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceOwnerChanged = delegate { };
   public static void RaiseProvinceOwnerChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceOwnerChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // TribalOwner
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTribalOwnerChanged = delegate { };
   public static void RaiseProvinceTribalOwnerChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceTribalOwnerChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // BaseManpower
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseManpowerChanged = delegate { };
   public static void RaiseProvinceBaseManpowerChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceBaseManpowerChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // BaseTax
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseTaxChanged = delegate { };
   public static void RaiseProvinceBaseTaxChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceBaseTaxChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // BaseProduction
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBaseProductionChanged = delegate { };
   public static void RaiseProvinceBaseProductionChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceBaseProductionChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // CenterOfTradeLevel
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCenterOfTradeLevelChanged = delegate { };
   public static void RaiseProvinceCenterOfTradeLevelChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceCenterOfTradeLevelChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // ExtraCost
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceExtraCostChanged = delegate { };
   public static void RaiseProvinceExtraCostChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceExtraCostChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // NativeFerocity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeFerocityChanged = delegate { };
   public static void RaiseProvinceNativeFerocityChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceNativeFerocityChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // NativeHostileness
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeHostilenessChanged = delegate { };
   public static void RaiseProvinceNativeHostilenessChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceNativeHostilenessChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // NativeSize
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNativeSizeChanged = delegate { };
   public static void RaiseProvinceNativeSizeChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceNativeSizeChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // RevoltRisk 
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceRevoltRiskChanged = delegate { };
   public static void RaiseProvinceRevoltRiskChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceRevoltRiskChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // LocalAutonomy
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceLocalAutonomyChanged = delegate { };
   public static void RaiseProvinceLocalAutonomyChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceLocalAutonomyChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Nationalism
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceNationalismChanged = delegate { };
   public static void RaiseProvinceNationalismChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceNationalismChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }
   
   // DiscoveredBy
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDiscoveredByChanged = delegate { };
   public static void RaiseProvinceDiscoveredByChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceDiscoveredByChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Capital
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCapitalChanged = delegate { };
   public static void RaiseProvinceCapitalChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceCapitalChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Culture
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceCultureChanged = delegate { };
   public static void RaiseProvinceCultureChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceCultureChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Religion
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceReligionChanged = delegate { };
   public static void RaiseProvinceReligionChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceReligionChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Buildings
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceBuildingsChanged = delegate { };
   public static void RaiseProvinceBuildingsChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceBuildingsChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // IsHre
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsHreChanged = delegate { };
   public static void RaiseProvinceIsHreChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceIsHreChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // IsCity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsCityChanged = delegate { };
   public static void RaiseProvinceIsCityChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceIsCityChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // IsSeatInParliament
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceIsSeatInParliamentChanged = delegate { };
   public static void RaiseProvinceIsSeatInParliamentChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceIsSeatInParliamentChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // TradeGood
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeGoodChanged = delegate { };
   public static void RaiseProvinceTradeGoodChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceTradeGoodChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Area
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceAreaChanged = delegate { };
   public static void RaiseProvinceAreaChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceAreaChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Continent
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceContinentChanged = delegate { };
   public static void RaiseProvinceContinentChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceContinentChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // History
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceHistoryChanged = delegate { };
   public static void RaiseProvinceHistoryChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceHistoryChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // latentTradeGood
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceLatentTradeGoodChanged = delegate { };
   public static void RaiseProvinceLatentTradeGoodChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceLatentTradeGoodChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // HasRevolt
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceHasRevoltChanged = delegate { };
   public static void RaiseProvinceHasRevoltChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceHasRevoltChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // TradeCompany
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeCompanyChanged = delegate { };
   public static void RaiseProvinceTradeCompanyChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceTradeCompanyChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // TradeCompanyInvestment
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceTradeCompanyInvestmentChanged = delegate { };
   public static void RaiseProvinceTradeCompanyInvestmentChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceTradeCompanyInvestmentChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // ProvinceModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProvinceModifiersChanged = delegate { };
   public static void RaiseProvinceProvinceModifiersChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceProvinceModifiersChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // ProvinceTriggeredModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProvinceTriggeredModifiersChanged = delegate { };
   public static void RaiseProvinceProvinceTriggeredModifiersChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceProvinceTriggeredModifiersChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // ReformationCenter
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceReformationCenterChanged = delegate { };
   public static void RaiseProvinceReformationCenterChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceReformationCenterChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // PermanentProvinceModifiers
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvincePermanentProvinceModifiersChanged = delegate { };
   public static void RaiseProvincePermanentProvinceModifiersChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvincePermanentProvinceModifiersChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Devastation 
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceDevastationChanged = delegate { };
   public static void RaiseProvinceDevastationChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceDevastationChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }

   // Prosperity
   public static event EventHandler<ProvinceDataChangedEventArgs> OnProvinceProsperityChanged = delegate { };
   public static void RaiseProvinceProsperityChanged(int id, object value, object oldValue, string propertyName)
   {
      OnProvinceProsperityChanged.Invoke(id, new (value, oldValue, propertyName));
      RaiseProvinceDataChanged(id, value, oldValue, propertyName);
   }
}