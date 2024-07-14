namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static DateTime _lastDate = DateTime.MinValue;
      public static void LoadDate(DateTime date)
      {
         if (date == _lastDate)
            return;
         if (date < _lastDate)
         {
            foreach (var province in Globals.Provinces.Values)
            {
               province.ResetHistory();
            }
         }
         foreach (var province in Globals.Provinces.Values)
         {
            province.LoadHistoryForDate(date);
         }
         _lastDate = date;
      }



   }
}