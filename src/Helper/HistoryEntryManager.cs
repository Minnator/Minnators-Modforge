using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;
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



   // If a same token is found, replace the token otherwise add to the End unless it is in the forbidden tokens
   /// <summary>
   /// -1 Do nothing\n-2 Error\n-3 Append to end
   /// </summary>
   /// <param name="entry"></param>
   /// <param name="token"></param>
   /// <returns></returns>
   public static int MergeTokenToEntry(this ProvinceHistoryEntry entry, IToken token)
   {
      switch (token.GetTokenName())
      {
         case "add_core":
         case "remove_core":
         case "add_claim":
         case "remove_claim":
         case "add_permanent_claim":
         case "remove_permanent_claim":
         case "discovered_by":
         case "remove_discovered_by":
         case "add_building":
         case "remove_permanent_province_modifier":
         case "remove_province_modifier":
         case "remove_province_triggered_modifier":
         case "add_permanent_province_modifier":
         case "add_province_modifier":
         case "add_province_triggered_modifier":
         case "add_to_trade_company":
         case "add_trade_company_investment":

            for (var i = 0; i < entry.Effects.Count; i++)
            {
               if (entry.Effects[i].GetTokenName().Equals(token.GetTokenName()))
                  if (entry.Effects[i].Equals(token))
                     return -1;
            }
            return -3;
         // we need to merge
         default:
            for (var i = 0; i < entry.Effects.Count; i++)
            {
               if (entry.Effects[i].GetTokenName().Equals(token.GetTokenName()))
               {
                  if (entry.Effects[i].Equals(token))
                     return -1;
                  return i;
               }
            }
            return -3;
      }
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
      RemoveEntry(province, dummyEntry, HEAddingResult.Appended);
   }

   public static void RemoveEntry(Province province, ProvinceHistoryEntry entry, HEAddingResult ar = HEAddingResult.New)
   {
      var index = province.History.BinarySearch(entry);
      if (index < 0)
         index = ~index;

      RemoveEntry(province, index, ar);
   }

   public static void RemoveEntry(Province province, int index, HEAddingResult ar = HEAddingResult.New)
   {
      var entry = province.History[index];
      // We need to make sure that we find the correct one if we have several with the same date.
      var searchIndex = index;
      while (searchIndex < province.History.Count && province.History[searchIndex].Date == entry.Date)
      {
         if (province.History[searchIndex].PHEId == entry.PHEId)
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
         if (province.History[searchIndex - 1].PHEId == entry.PHEId)
         {
            if (ar == HEAddingResult.New)
               province.History.RemoveAt(searchIndex);
            else
               province.History[searchIndex].Effects.RemoveAll(entry.Effects.Contains);
            return;
         }
         searchIndex--;
      }
      throw new EvilActions($"Could not find specified 'ProvinceHistoryEntry' to remove: {entry.PHEId}");
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