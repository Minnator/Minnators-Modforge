using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.Helper;

public static class HistoryEntryManager
{
   public static int PHEIndex;
   public enum HEAddingResult
   {
      New,
      Appended,
   }

   public static bool AddEntry(Province province, Date date, IToken newEffect, bool warnIfNotLandProvince = true)
   {
      if (warnIfNotLandProvince && !Globals.LandProvinces.Contains(province))
      {
         MessageBox.Show("You can only add history entries to land provinces!", "Illegal Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
         return false;
      }

      var entry = new ProvinceHistoryEntry(date, PHEIndex++);
      entry.Effects.Add(newEffect);
      InsertEntry(province, entry);
      return true;
   }

   public static (HEAddingResult heAddingResult, int PHEId) InsertEntry(Province province, ProvinceHistoryEntry entry)
   {
      var index = province.History.BinarySearch(entry);
      if (index < 0)
         index = ~index;

      while (index > 0 && province.History[index - 1].Date == entry.Date) 
         index--;
      province.History.Insert(index, entry);
      return (HEAddingResult.New, entry.PHEId);
   }

   public static void RemoveEntry(Province province, List<IToken> effects, int hEIndex, Date date)
   {
      var dummyEntry = new ProvinceHistoryEntry(date, hEIndex);
      dummyEntry.Effects.AddRange(effects);
      RemoveEntry(province, dummyEntry, hEIndex, HEAddingResult.Appended);
   }

   public static void RemoveEntry(Province province, ProvinceHistoryEntry entry, int hEIndex, HEAddingResult ar)
   {
      var index = province.History.BinarySearch(entry);
      if (index < 0)
         index = ~index;

      // We need to make sure that we find the correct one if we have several with the same date.
      var searchIndex = index;
      while (searchIndex < province.History.Count && province.History[searchIndex].Date == entry.Date)
      {
         if (province.History[searchIndex].PHEId == hEIndex)
         {
            if (ar == HEAddingResult.New) 
               province.History.RemoveAt(searchIndex);
            else
               province.History[searchIndex].Effects.RemoveAll(entry.Effects.Contains);
            return;
         }
         searchIndex++;
      }
      searchIndex = index;
      while (searchIndex > 0 && province.History[searchIndex - 1].Date == entry.Date)
      {
         if (province.History[searchIndex - 1].PHEId == hEIndex)
         {
            if (ar == HEAddingResult.New)
               province.History.RemoveAt(searchIndex);
            else
               province.History[searchIndex].Effects.RemoveAll(entry.Effects.Contains);
            return;
         }
         searchIndex--;
      }
      throw new EvilActions($"Could not find specified 'ProvinceHistoryEntry' to remove: {hEIndex}");
   }

   public static bool MergeEntries(List<Province> provinces) => MergeEntries(provinces, Date.MinValue, Date.MaxValue);
   public static bool MergeEntries(List<Province> provinces, Date from, Date to) => provinces.All(x => MergeEntries(x, from, to));
   public static bool MergeEntries(Province province) => MergeEntries(province, Date.MinValue, Date.MaxValue);
   public static bool MergeEntries(Province province, Date from, Date to)
   {
      Debug.Assert(from < to, "from < to");
      if (from >= to)
      {
         MessageBox.Show("'From' must always be smaller than 'to' to merge history entries!", "Illegal Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
         return false;
      }

      var history = province.History;
      if (history.Count < 2)
         return true;

      List<ProvinceHistoryEntry> priorEntries = [];
      List<ProvinceHistoryEntry> entriesInRange = [];
      List<ProvinceHistoryEntry> laterEntries = [];

      if (from == Date.MinValue && to == Date.MaxValue)
         entriesInRange = history;
      else
      {
         foreach (var entry in history)
         {
            if (entry.Date < from)
               priorEntries.Add(entry);
            else if (entry.Date > to)
               laterEntries.Add(entry);
            else
               entriesInRange.Add(entry);
         }
      }

      if (entriesInRange.Count < 2)
         return true;

      Dictionary<int, ProvinceHistoryEntry> perDate = new(entriesInRange.Count);
      var hadToMerge = false;

      foreach (var entry in entriesInRange)
      {
         // We have at least two entries on this date so we merge them by joining their effects
         if (!perDate.TryAdd(entry.Date.TimeStamp, entry))
         {
            perDate[entry.Date.TimeStamp].Effects.AddRange(entry.Effects);
            hadToMerge = true;
         }
      }

      if (!hadToMerge) // No entries were merged so we don't need to do anything
         return true;

      var restructuredEntries = perDate.Values.ToList();
      restructuredEntries.Sort();

      province.History.Clear();
      province.History.AddRange(priorEntries);
      province.History.AddRange(restructuredEntries);
      province.History.AddRange(laterEntries);

      return true;
   }
}