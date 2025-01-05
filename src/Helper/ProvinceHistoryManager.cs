using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date LastDate = Date.MinValue;

      /* TODO REWORK
      public static void LoadDate(Date date)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         if (date < LastDate)
            foreach (var province in Globals.Provinces) 
               province.ResetHistory();
         foreach (var province in Globals.Provinces) 
            province.LoadHistoryForDate(date);
         LastDate = LastDate.Copy(date);
         Globals.EditingStatus = EditingStatus.Idle;
      }*/
   }
}