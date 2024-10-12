using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Events
{
   public class ProvinceCollectionEventArgs : EventArgs
   {
      public string GroupKey { get; }
      public List<Province> Ids { get; }

      public ProvinceCollectionEventArgs(string groupKey, List<Province> ids)
      {
         GroupKey = groupKey;
         Ids = ids;
      }

   }


   public static class ProvinceCollectionEvents
   {
      public static void OnTradeCompanyProvinceAdded(object? sender, ProvinceCollectionEventArgs e)
      {
         if (!Globals.AllowEditing || e == null!)
            return;
         Globals.HistoryManager.AddCommand(new CTradeCompanyProvinceChanged(e.GroupKey, true, true, e.Ids));
      }

      public static void OnTradeCompanyProvinceRemoved(object? sender, ProvinceCollectionEventArgs e)
      {
         if (!Globals.AllowEditing || e == null!)
            return;
         Globals.HistoryManager.AddCommand(new CTradeCompanyProvinceChanged(e.GroupKey, false, true, e.Ids));
      }
   }
}