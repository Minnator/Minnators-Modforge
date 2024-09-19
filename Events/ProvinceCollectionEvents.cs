using Editor.DataClasses.Commands;

namespace Editor.Events
{
   public class ProvinceCollectionEventArgs : EventArgs
   {
      public string GroupKey { get; } = string.Empty;
      public int[] Ids { get; }

      public ProvinceCollectionEventArgs(string groupKey, params int[] ids)
      {
         GroupKey = groupKey;
         Ids = ids;
      }

      public ProvinceCollectionEventArgs(ProvinceEditedEventArgs e)
      {
         if (e == null!)
         {
            GroupKey = string.Empty;
            Ids = [];
            return;
         }

         var ids = new List<int>();
         foreach (var province in e.Provinces)
            ids.Add(province.Id);

         Ids = [..ids];
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