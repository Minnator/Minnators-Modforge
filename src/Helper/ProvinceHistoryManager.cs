using System.Diagnostics;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      private static Date LastDate = Date.MinValue;

      public static void LoadDate(Date date, bool render = true)
      {
         if (date != LastDate)
         {
            Globals.State = State.Loading;
            if (date < LastDate)
               foreach (var province in Globals.Provinces) 
                  province.ResetHistory();
            foreach (var province in Globals.Provinces) 
               province.LoadHistoryForDate(date);
            LastDate.SetDateSilent(date);
            Globals.State = State.Running;
         }
         if (render)
            MapModeManager.RenderCurrent();
      }

      public static void LoadDate(Date date, Province province, bool render = true)
      {
         if (date != LastDate)
         {
            Globals.State = State.Loading;
            if (date < LastDate)
               province.ResetHistory();
            province.LoadHistoryForDate(date);
            LastDate.SetDateSilent(date);
            Globals.State = State.Running;
         }
         if (render)
            MapModeManager.RenderCurrent();
      }

      public static void LoadDate(Date date, ICollection<Province> provinces, bool render = true)
      {
         if (date != LastDate)
         {
            Globals.State = State.Loading;
            if (date < LastDate)
               foreach (var province in provinces) 
                  province.ResetHistory();
            foreach (var province in provinces) 
               province.LoadHistoryForDate(date);
            LastDate.SetDateSilent(date);
            Globals.State = State.Running;
         }
         if (render)
            MapModeManager.RenderCurrent();
      }
   }
}