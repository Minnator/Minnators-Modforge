using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Saveables;
using Editor.Events;
using Date = Editor.DataClasses.Misc.Date;

namespace Editor.Helper
{
   public static class ProvinceHistoryManager
   {
      public static Date CurrentLoadedDate = Date.MinValue;
      public static bool IsLoading { get; private set; } = false;

      public static void ResetProvinceHistory()
      {
         foreach (var province in Globals.Provinces)
            province.ResetHistory();
      }
      public static void LoadDate(Date date, bool render = true)
      {
         var state = Globals.State;
         Globals.State = State.Loading;
         if (date != CurrentLoadedDate)
         {
            IsLoading = true;
            if (date < CurrentLoadedDate)
            {
               ResetProvinceHistory();
               CurrentLoadedDate.SetDateSilent(Date.MinValue);
            }
            foreach (var province in Globals.Provinces) 
               province.LoadHistoryForDate(date);
            CurrentLoadedDate.SetDate(date);
            IsLoading = false;
         }
         if (render)
            MapModeManager.RenderCurrent();
         if (Selection.GetSelectedProvinces.Count > 0)
            LoadGuiEvents.ProvHistoryLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);

         Globals.State = state;
         Globals.MapWindow.DateControl.SetDate(CurrentLoadedDate);
      }

      public static void ReloadDate(Province province, bool force = false)
      {
         IsLoading = true;
         var state = Globals.State;
         Globals.State = State.Loading;
         province.ResetHistory();
         var date = CurrentLoadedDate.Copy();
         if (date < CurrentLoadedDate || force)
            CurrentLoadedDate.SetDateSilent(Date.MinValue);
         province.LoadHistoryForDate(date);
         CurrentLoadedDate.SetDateSilent(date);
         Globals.State = state;
         IsLoading = false;
      }

      public static IEnumerable<ProvinceHistoryEntry> EnumerateFromToDate(List<ProvinceHistoryEntry> entries, Date date, Date endDate)
      {
         var start = BinarySearchDate(entries, date);
         for (; start < entries.Count; start++)
         {
            var entry = entries[start];
            if (entry.Date > endDate)
               break;
            yield return entry;
         }
      }


      public static int BinarySearchDateExact(List<ProvinceHistoryEntry> entries, Date date, int startIndex = 0)
      {
         var low = startIndex;
         var high = entries.Count - 1;
         while (low <= high)
         {
            var index = low + (high - low) / 2;
            if (entries[index].Date < date)
               low = index + 1;
            else if (entries[index].Date > date)
               high = index - 1;
            else
               return index;
         }
         return -1;
      }

      public static (ProvinceHistoryEntry?[] indexes, bool all) BinarySearchDateExactMultiple(List<List<ProvinceHistoryEntry>> entries, Date date, int startIndex = 0)
      {
         var result = new ProvinceHistoryEntry?[entries.Count];
         var all = true;
         for (var i = 0; i < entries.Count; i++)
         {
            var resIndex = BinarySearchDateExact(entries[i], date, startIndex);

            if (resIndex == -1)
            {
               all = false;
               result[i] = null;
               continue;
            }
            result[i]= entries[i][resIndex];
         }
         return (result, all);
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
            else
               high = index - 1;
         }
         return low;
      }
   }
}