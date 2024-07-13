namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {

      public static void LoadDate(DateTime date)
      {
         foreach (var province in Globals.Provinces.Values)
         {
            province.LoadHistoryForDate(date);
         }
      }



   }
}