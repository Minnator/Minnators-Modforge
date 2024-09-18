namespace Editor.Events
{
   public static class ProvinceCollectionEvents
   {
      public class ProvinceGroupEventArgs(string groupKey, params int[] ids) : EventArgs
      {
         public string GroupKey { get; } = groupKey;
         public int[] Ids { get; } = ids;
      }
      

      public static event EventHandler<ProvinceGroupEventArgs>? OnTradeCompanyChanged = delegate { };
      public static void RaiseOnTradeCompanyChanged(object? sender, ProvinceGroupEventArgs? e)
      {
         if (!Globals.AllowEditing || e == null)
            return;
         OnTradeCompanyChanged?.Invoke(sender, e);
      }
   }
}