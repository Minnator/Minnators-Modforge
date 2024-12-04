using Editor.DataClasses.Misc;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date _lastDate = Date.MinValue;

      private static Date LastDate
      {
         get => _lastDate;
         set => _lastDate = value;
      }

      public static void LoadDate(Date date)
      {
         if (date < LastDate)
            foreach (var province in Globals.Provinces) 
               province.ResetHistory();
         foreach (var province in Globals.Provinces) 
            province.LoadHistoryForDate(date);
         LastDate.CopyDate(date);
      }
   }
}