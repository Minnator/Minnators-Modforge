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
         _date = new (date.TimeStamp);
         _tokens = new (Province, bool, KeyValuePair<int, IToken>[])[provinces.Count];
         changed = false;

         for (var i = provinces.Count - 1; i >= 0; i--)
         {
            var province = provinces[i];
            var entryIndex = ProvinceHistoryManager.BinarySearchDateExact(province.History, date);
            var tokenLocations = new List<KeyValuePair<int, IToken>> (tokens.Count);
            
            if (entryIndex == -1)
            {
               for (var j = 0; j < tokens.Count; j++)
                  tokenLocations.Add(new(j, tokens[j]));
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
               if(changed)
                  _tokens[i] = (province, false, tokenLocations.ToArray());
            }
         }

         if (executeOnInit && changed)
            Execute();
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

      public override List<int> GetTargetHash() => [.. _tokens.Select(x => x.province.GetHashCode())];

      public override string GetDescription()
      {
         return $"Added {_tokens[0].tokenLocations.Length} tokens to {_tokens.Length} provinces";
      }

      public override string GetDebugInformation(int indent)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"{new string(' ', indent)}PrvHistoryEntryCommand");
         sb.AppendLine($"{new string(' ', indent + 2)}Date: {_date}");
         foreach (var (province, _, tokenLocations) in _tokens)
         {
            sb.AppendLine($"{new string(' ', indent + 2)}Province: {province}");
            foreach (var (index, token) in tokenLocations)
            {
               sb.AppendLine($"{new string(' ', indent + 4)}Index: {index}, Token: {token}");
            }
         }
         return sb.ToString();
      }
   }
}