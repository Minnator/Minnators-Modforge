using static Editor.Events.ProvinceEventHandler;

namespace Editor.Events
{
   public static class ProvinceCollectionEventHandler
   {
      public static event EventHandler<ProvinceCollectionEventArgs>? OnTradeCompanyChanged = delegate { };
      public static void RaiseOnTradeCompanyChanged(object? sender, ProvinceCollectionEventArgs? e)
      {
         if (!Globals.AllowEditing || e == null)
            return;
         OnTradeCompanyChanged?.Invoke(sender, e);
      }

      public static event EventHandler<ProvinceCollectionEventArgs>? OnTradeCompanyInvestmentChanged = delegate { };
      public static void RaiseOnTradeCompanyInvestmentChanged(object? sender, ProvinceCollectionEventArgs? e)
      {
         if (!Globals.AllowEditing || e == null)
            return;
         OnTradeCompanyInvestmentChanged?.Invoke(sender, e);
      }
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
   }
}