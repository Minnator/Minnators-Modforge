using Editor.DataClasses.Misc;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date LastDate = Date.MinValue;

      public static void LoadDate(Date date)
      {
         if (date < LastDate)
            foreach (var province in Globals.Provinces) 
               province.ResetHistory();
         foreach (var province in Globals.Provinces) 
            province.LoadHistoryForDate(date);
         LastDate.Copy(date);
      }
   }
}