using System.Data;
using System.Diagnostics;
using Editor.Saving;
using System.Reflection;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.Events;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Date = Editor.DataClasses.Misc.Date;

namespace Editor.DataClasses.Commands
{
   public class PrvHistoryEntryCommand : SaveableCommandBasic
   {
      private readonly (Province province, bool created, KeyValuePair<int, IToken>[] tokenLocations)[] _tokens;
      private readonly Date _date;

      // Tokens
      // GUI Tokens
      // Command needs to check how the entry should be changed and if some of the tokens need to be replaced
      public PrvHistoryEntryCommand(List<Province> provinces, List<IToken> tokens, Date date, out bool changed, bool executeOnInit = true)
      {
         _date = new(date.TimeStamp);
         _tokens = new (Province, bool, KeyValuePair<int, IToken>[])[provinces.Count];
         changed = false;

         for (var i = provinces.Count - 1; i >= 0; i--)
         {
            var province = provinces[i];
            var entryIndex = ProvinceHistoryManager.BinarySearchDateExact(province.History, date);
            var tokenLocations = new List<KeyValuePair<int, IToken>>(tokens.Count);

            if (entryIndex == -1)
            {
               for (var j = 0; j < tokens.Count; j++)
               {
                  tokenLocations.Add(new(j, tokens[j]));
               }

               _tokens[i] = (province, true, tokenLocations.ToArray());
               changed = true;
            }
            else
            {
               var endPos = province.History[entryIndex].Effects.Count;
               for (var j = 0; j < tokens.Count; j++)
               {
                  var mergedIndex = province.History[entryIndex].MergeTokenToEntry(tokens[j]);
                  switch (mergedIndex)
                  {
                     case -1:
                        continue;
                     case -3:
                        tokenLocations.Add(new(endPos++, tokens[j]));
                        break;
                     default:
                        tokenLocations.Add(new(mergedIndex, tokens[j]));
                        break;
                  }

                  changed = true;
               }

               if (changed)
               {
                  _tokens[i] = (province, false, tokenLocations.ToArray());
               }
            }
         }

         if (executeOnInit && changed)
         {
            Execute();
         }
      }

      public sealed override void Execute()
      {
         base.Execute(_tokens.Select(Saveable (x) => x.province).ToList());
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var (province, created, tokenLocations) in _tokens)
         {
            if (created)
            {
               var newEntry = new ProvinceHistoryEntry(new(_date.TimeStamp), HistoryEntryManager.PHEIndex++);
               newEntry.Effects.AddRange(tokenLocations.Select(x => x.Value));
               HistoryEntryManager.InsertEntry(province, newEntry);
            }
            else
            {
               var oldIndex = ProvinceHistoryManager.BinarySearchDateExact(province.History, _date);
               Debug.Assert(oldIndex >= 0 && oldIndex < province.History.Count);
               var oldEntry = province.History[oldIndex];

               for (var i = 0; i < tokenLocations.Length; i++)
               {
                  var (location, token) = tokenLocations[i];
                  var oldEffect = IToken.Empty;
                  if (location >= oldEntry.Effects.Count)
                  {
                     Debug.Assert(oldEntry.Effects.Count == location);
                     oldEntry.Effects.Add(token);
                  }
                  else
                  {
                     oldEffect = oldEntry.Effects[location];
                     oldEntry.Effects[location] = token;
                  }

                  tokenLocations[i] = new(location, oldEffect);
               }
            }

            ProvinceHistoryManager.ReloadDate(province, true);
         }

         MapModeManager.RenderCurrent();
         LoadGuiEvents.ReloadHistoryProvinces();
      }

      public override void Undo()
      {
         base.Undo();
         Debug.Assert(_tokens.Length > 0, "_tokens.Length > 0");
         foreach (var (province, created, tokenLocations) in _tokens)
         {
            var oldEntryIndex = ProvinceHistoryManager.BinarySearchDateExact(province.History, _date);
            Debug.Assert(oldEntryIndex >= 0 && oldEntryIndex < province.History.Count);
            if (created)
            {
               HistoryEntryManager.RemoveEntry(province, oldEntryIndex);
            }
            else
            {
               var oldEntry = province.History[oldEntryIndex];
               Debug.Assert(tokenLocations.Length > 0, "tokenLocations.Length > 0");
               for (var i = tokenLocations.Length - 1; i >= 0; i--)
               {
                  var (location, token) = tokenLocations[i];
                  var oldEffect = oldEntry.Effects[location];
                  if (token.Equals(IToken.Empty))
                  {
                     Debug.Assert(location == oldEntry.Effects.Count - 1);
                     oldEntry.Effects.RemoveAt(location);
                  }
                  else
                  {
                     oldEntry.Effects[location] = token;
                  }

                  tokenLocations[i] = new(location, oldEffect);
               }
            }

            ProvinceHistoryManager.ReloadDate(province, true);
         }

         MapModeManager.RenderCurrent();
         LoadGuiEvents.ReloadHistoryProvinces();
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override List<int> GetTargetHash()
      {
         return [.. _tokens.Select(x => x.province.GetHashCode())];
      }

      public override string GetDescription()
      {
         return $"Added {_tokens[0].tokenLocations.Length} tokens to {_tokens.Length} provinces";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"{new(' ', indent)}PrvHistoryEntryCommand");
         sb.AppendLine($"{new(' ', indent + 2)}Date: {_date}");
         foreach (var (province, _, tokenLocations) in _tokens)
         {
            sb.AppendLine($"{new(' ', indent + 2)}Province: {province}");
            foreach (var (index, token) in tokenLocations)
            {
               sb.AppendLine($"{new(' ', indent + 4)}Index: {index}, Token: {token}");
            }
         }

         return sb.ToString();
      }
   }

   public class PrvHistoryListCommand<T> : SaveableCommandBasic where T : notnull
   {
      private readonly struct DataStruct(
         ProvinceHistoryEntry entry,
         IToken[] created,
         KeyValuePair<int, IToken>[] tokenLocations,
         Province province,
         bool isNewEntry)
      {
         public ProvinceHistoryEntry Entry { get; } = entry;
         public IToken[] Created { get; } = created;
         public KeyValuePair<int, IToken>[] TokenLocations { get; } = tokenLocations;
         public Province Province { get; } = province;
         public bool IsNewEntry { get; } = isNewEntry;
      }

      private readonly List<DataStruct> _tokens;

      public PrvHistoryListCommand(Func<T, IToken> creator, List<T> items, string addName, string removeName, List<Province> provinces, Date date, bool isAdd,
                                   out bool changed, bool executeOnInit = true)
      {
         _tokens = new(items.Count);
         changed = false;
         foreach (var province in provinces)
         {
            var entryIndex = ProvinceHistoryManager.BinarySearchDateExact(province.History, date);

            if (entryIndex == -1)
            {
               var historyEntry = new ProvinceHistoryEntry(new(date.TimeStamp), HistoryEntryManager.PHEIndex++)
               {
                  Effects = items.Select(creator).ToList()
               };
               changed = true;
               _tokens.Add(new(historyEntry, [], [], province, true));
               continue;
            }

            var created = new List<IToken>();
            var entry = province.History[entryIndex];

            List<KeyValuePair<int, IToken>> removals = [];

            var found = new bool[items.Count];

            for (var i = 0; i < entry.Effects.Count; i++)
            {
               if (entry.Effects[i] is not SimpleEffect<T> effect)
               {
                  continue;
               }

               var action = effect.GetTokenName();
               var value = effect._value.Val;

               Debug.Assert(value != null, "value != null");

               for (var j = 0; j < items.Count; j++)
               {
                  if (!value.Equals(items[j]))
                  {
                     continue;
                  }

                  if (action == addName)
                  {
                     if (!isAdd || found[j])
                     {
                        removals.Add(new(i, entry.Effects[i]));
                     }
                     else
                     {
                        found[j] = true;
                     }
                  }
                  else if (action == removeName)
                  {
                     if (isAdd || found[j])
                     {
                        removals.Add(new(i, entry.Effects[i]));
                     }
                     else
                     {
                        found[j] = true;
                     }
                  }
               }
            }

            for (var j = 0; j < items.Count; j++)
            {
               if (!found[j])
               {
                  created.Add(creator(items[j]));
               }
            }

            if (removals.Count == 0 && created.Count == 0)
            {
               continue;
            }

            _tokens.Add(new(entry, created.ToArray(), removals.ToArray(), province, false));
            changed = true;
         }

         if (executeOnInit)
         {
            Execute();
         }
      }


      public override void Execute()
      {
         base.Execute(_tokens.Select(Saveable (x) => x.Province).ToList());
         InternalExecute();
      }

      private void InternalExecute()
      {
         foreach (var data in _tokens)
         {
            if (data.IsNewEntry)
            {
               HistoryEntryManager.InsertEntry(data.Province, data.Entry);
            }
            else
            {
               for (var i = data.TokenLocations.Length - 1; i >= 0; i--)
               {
                  data.Entry.Effects.RemoveAt(data.TokenLocations[i].Key);
               }

               foreach (var token in data.Created)
               {
                  data.Entry.Effects.Add(token);
               }
            }

            ProvinceHistoryManager.ReloadDate(data.Province, true);
         }

         MapModeManager.RenderCurrent();
         LoadGuiEvents.ReloadHistoryProvinces();
      }

      public override void Undo()
      {
         base.Undo();
         foreach (var data in _tokens)
         {
            if (data.IsNewEntry)
            {
               HistoryEntryManager.RemoveEntry(data.Province, data.Entry);
            }
            else
            {
               var effs = data.Entry.Effects;
               for (var i = 0; i < data.TokenLocations.Length; i++)
               {
                  effs.Insert(data.TokenLocations[i].Key, data.TokenLocations[i].Value);
               }

               effs.RemoveRange(effs.Count - data.Created.Length, data.Created.Length);
            }

            ProvinceHistoryManager.ReloadDate(data.Province, true);
         }

         MapModeManager.RenderCurrent();
         LoadGuiEvents.ReloadHistoryProvinces();
      }

      public override void Redo()
      {
         base.Redo();
         InternalExecute();
      }

      public override List<int> GetTargetHash()
      {
         return [.. _tokens.Select(x => x.Province.GetHashCode())];
      }

      public override string GetDescription()
      {
         var numbers = _tokens.Count;
         var objType = "province" + (numbers > 1 ? "s" : "");
         return $"Modified {numbers} {objType} with {_tokens.Sum(x => x.TokenLocations.Length)} tokens";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"{new(' ', indent)}PrvHistoryListCommand");
         sb.AppendLine($"{new(' ', indent + 2)}Provinces: {_tokens.Count}");
         foreach (var data in _tokens)
         {
            sb.AppendLine($"{new(' ', indent + 2)}Entry: {data.Entry}");
            sb.AppendLine($"{new(' ', indent + 4)}Created Tokens: {data.Created.Length}");
            foreach (var (index, token) in data.TokenLocations)
            {
               sb.AppendLine($"{new(' ', indent + 4)}Index: {index}, Token: {token}");
            }
         }

         return sb.ToString();
      }
   }
}