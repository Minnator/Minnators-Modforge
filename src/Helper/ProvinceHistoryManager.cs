using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
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
            var state = Globals.State;
            Globals.State = State.Loading;
            if (date < CurrentLoadedDate)
               ResetProvinceHistory();

            CurrentLoadedDate.SetDateSilent(Date.MinValue);
            foreach (var province in Globals.Provinces) 
               province.LoadHistoryForDate(date);
            CurrentLoadedDate.SetDateSilent(date);
            Globals.State = state;
         }
         if (render)
            MapModeManager.RenderCurrent();
         if (Selection.GetSelectedProvinces.Count > 0)
            LoadGuiEvents.ProvHistoryLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);
      }

      public static void ReloadDate(Province province)
      {
         var state = Globals.State;
         Globals.State = State.Loading;
         province.ResetHistory();
         var date = CurrentLoadedDate.Copy();
         CurrentLoadedDate.SetDateSilent(Date.MinValue);
         province.LoadHistoryForDate(date);
         CurrentLoadedDate.SetDateSilent(date);
         Globals.State = state;

      }

      public static IEnumerable<ProvinceHistoryEntry> EnumerateFromToDate(List<ProvinceHistoryEntry> entries, Date date, Date endDate)
      {
         var start = BinarySearchDate(entries, date);
         for (; start < entries.Count - 1; start++)
         {
            var entry = entries[start];
            if (entry.Date > endDate)
               break;
            yield return entry;
         }
      }

      private static int BinarySearchDate(List<ProvinceHistoryEntry> entries, Date date)
      {
         var low = 0;
         var high = entries.Count - 1;
         while (low <= high)
         {
            var index = low + (high - low) / 2;
            if (entries[index].Date <= date)
               low = index + 1;
            else if (entries[index].Date > date)
               high = index - 1;
         }
         return low;
      }
   }
}