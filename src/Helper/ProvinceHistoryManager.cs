using System.Diagnostics;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      public static Date CurrentLoadedDate = Date.MinValue;

      public static void ResetProvinceHistory()
      {
         foreach (var province in Globals.Provinces)
            province.ResetHistory();
      }
      public static void LoadDate(Date date, bool render = true)
      {
         if (date != CurrentLoadedDate)
         {
            Globals.State = State.Loading;
            if (date < CurrentLoadedDate)
               ResetProvinceHistory();
            foreach (var province in Globals.Provinces) 
               province.LoadHistoryForDate(date);
            CurrentLoadedDate.SetDateSilent(date);
            Globals.State = State.Running;
         }
         if (render)
            MapModeManager.RenderCurrent();
         if (Selection.GetSelectedProvinces.Count > 0)
            LoadGuiEvents.ProvHistoryLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);
      }

      public static void LoadDate(Date date, Province province, bool render = true)
      {
         if (date != CurrentLoadedDate)
         {
            Globals.State = State.Loading;
            if (date < CurrentLoadedDate)
               province.ResetHistory();
            province.LoadHistoryForDate(date);
            CurrentLoadedDate.SetDateSilent(date);
            Globals.State = State.Running;
         }
         if (render)
            MapModeManager.RenderCurrent();
         if (Selection.GetSelectedProvinces.Count > 0)
            LoadGuiEvents.ProvHistoryLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);
      }
   }
}