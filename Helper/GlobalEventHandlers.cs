using static Editor.Helper.ProvinceEventHandler;
using System;

namespace Editor.Helper;

public static class GlobalEventHandlers
{
   #region ProvinceGroups

   // Region
   public static event EventHandler<ProvinceDataChangedEventArgs> OnRegionChanged = delegate { };
   public static void RaiseRegionChanged(int id, object value, object oldValue, string propertyName)
   {
      OnRegionChanged.Invoke(id, new(value, propertyName));
   }

   // SuperRegion
   public static event EventHandler<ProvinceDataChangedEventArgs> OnSuperRegionChanged = delegate { };
   public static void RaiseSuperRegionChanged(int id, object value, object oldValue, string propertyName)
   {
      OnSuperRegionChanged.Invoke(id, new(value, propertyName));
   }

   // Area
   public static event EventHandler<ProvinceDataChangedEventArgs> OnAreaChanged = delegate { };
   public static void RaiseAreaChanged(int id, object value, object oldValue, string propertyName)
   {
      OnAreaChanged.Invoke(id, new(value, propertyName));
   }

   // Continent
   public static event EventHandler<ProvinceDataChangedEventArgs> OnContinentChanged = delegate { };
   public static void RaiseContinentChanged(int id, object value, object oldValue, string propertyName)
   {
      OnContinentChanged.Invoke(id, new(value, propertyName));
   }

   #endregion

   public static event EventHandler<EventArgs> OnCountryListChanged = delegate { };
   public static void RaiseCountryListChanged()
   {
      OnCountryListChanged.Invoke(null, EventArgs.Empty);
   }

}

