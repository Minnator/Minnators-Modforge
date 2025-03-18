using System.Diagnostics;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date LastDate = Date.MinValue;

      public static void LoadDate(Date date)
      {
         var sw = Stopwatch.StartNew();
         Globals.State = State.Loading;
         if (date < LastDate)
            foreach (var province in Globals.Provinces) 
               province.ResetHistory();
         foreach (var province in Globals.Provinces) 
            province.LoadHistoryForDate(date);
         LastDate.SetDateSilent(date);
         Globals.State = State.Running;
         sw.Stop();
      }
   }
}