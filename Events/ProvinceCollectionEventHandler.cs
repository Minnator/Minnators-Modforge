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
   }
}