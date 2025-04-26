using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation;

namespace Editor.Helper;

public static class HistoryEntryManager
{
   public enum HECreationType
   {
      Snap, // We add it to the nearest history entry (Median distance for several entries)
      Exact, // We add it to the exact date
   }

   public static bool AddEntry(Province province, Date date, IToken newEffect, HECreationType type, bool warnIfNotLandProvince = true)
   {
      if (warnIfNotLandProvince && !Globals.LandProvinces.Contains(province))
      {
         MessageBox.Show("You can only add history entries to land provinces!", "Illegal Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
         return false;
      }

      var entry = new ProvinceHistoryEntry(date);
      entry.Effects.Add(newEffect);
      InsertEntry(province, entry, type);
      return true;
   }

   public static void AddEntry(List<Province> provinces, Date date, IToken newEffect, HECreationType type)
   {
      var hadNonLandProvince = true;
      foreach (var province in provinces)
         if (!AddEntry(province, date, newEffect, type, hadNonLandProvince))
            hadNonLandProvince = false;
   }
   
   public static void InsertEntry(Province province, ProvinceHistoryEntry entry, HECreationType type)
   {
      var index = province.History.BinarySearch(entry);
      if (index < 0)
         index = ~index;

      switch (type)
      {
         case HECreationType.Snap:
            if (province.History.Count == 0)
            {
               province.History.Add(entry);
               break;
            }

            var prevDist = int.MaxValue;
            var nextDist = int.MaxValue;

            if (index > 0)
               prevDist = province.History[index - 1].Date.TimeStamp - entry.Date.TimeStamp;
            if (index < province.History.Count)
               nextDist = province.History[index].Date.TimeStamp - entry.Date.TimeStamp;

            if (prevDist < nextDist || nextDist == int.MaxValue)
               province.History[index - 1].Effects.AddRange(entry.Effects);
            else // if they are equal we add it to the next entry
               province.History[index].Effects.AddRange(entry.Effects);
            break;
         case HECreationType.Exact:
            if (index > 0) // we check if the previous entry is the same date
            {
               if (province.History[index - 1].Date == entry.Date)
                  province.History[index - 1].Effects.AddRange(entry.Effects);
            }
            else
               province.History.Insert(index, entry);

            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
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