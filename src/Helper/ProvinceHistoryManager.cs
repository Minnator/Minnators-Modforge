using Editor.DataClasses.Misc;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date _lastDate = Date.MinValue;
      public static void LoadDate(Date date)
      {
         if (date == _lastDate)
            return;
         if (date < _lastDate)
         {
            foreach (var province in Globals.Provinces)
            {
               province.ResetHistory();
            }
         }
         foreach (var province in Globals.Provinces)
         {
            province.LoadHistoryForDate(date);
         }
         _lastDate = date;
      }



   }
}