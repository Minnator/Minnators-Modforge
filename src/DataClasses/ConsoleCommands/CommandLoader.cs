using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using Windows.Media.Devices;
using Editor.DataClasses.Achievements;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using ScriptedEffect = Editor.Loading.Enhanced.ScriptedEffect;

namespace Editor.DataClasses.ConsoleCommands
{
   public class CommandLoader
   {
      private readonly CommandHandler _handler;
      private readonly RichTextBox _outputBox;
      public CommandLoader(CommandHandler handler, RichTextBox outputBox)
      {
         _handler = handler;
         _outputBox = outputBox;
      }

      public void Load()
      {
         SetUtilityCommands();
         SetProvinceCommands();
         RunFiles();
#if DEBUG
         DebugCommands();
#endif
      }

#if DEBUG
      private void DebugCommands()
      {
         _handler.RegisterCommand(new("ach_popup", "Usage: ach_popup", args =>
         {
            var achievement = AchievementManager.GetAchievement(AchievementId.UseForTheFirstTime);
            if (achievement is null)
               return ["Achievement not found"];
            AchievementPopup.Show(achievement);
            return [$"Showing achievement popup for {achievement.Name}"];
         }, ClearanceLevel.Debug));

         // get num of historyEntries for the given day
         var historyCountUsage = "Usage: history_count <date>";
         _handler.RegisterCommand(new("history_count", historyCountUsage, args =>
         {
            if (args.Length != 1)
               return [historyCountUsage];

            if (Date.TryParse(args[0], out var date).Ignore())
            {
               var count = Globals.Provinces.Sum(x => x.History.Count(y => y.Date.Equals(date)));
               return [$"History entries on {date}: {count}"];
            }

            return ["Invalid province id"];
         }, ClearanceLevel.Debug));

         // Scripted Effect parsgin
         var scriptedEffectUsage = "Usage: parse_scripted_effect <file>";
         _handler.RegisterCommand(new("parse_scripted_effect", scriptedEffectUsage, args =>
         {
            if (args.Length != 1)
               return [scriptedEffectUsage];

            var path = Path.Combine(Globals.AppDirectory, args[0]);
            if (!File.Exists(path))
               return [$"File '{path}' not found"];

            var effs = ScriptedEffectLoading.LoadParseFile(path);
            var sb = new StringBuilder($"Parsed scripted effects from '{args[0]}':");
            sb.AppendLine();
            foreach (var eff in effs)
            {
               sb.AppendLine(eff.Name);
               SavingUtil.AddElements(1, eff.EnhancedElements, ref sb);
               sb.AppendLine();
            }

            return sb.ToString().Split("\r\n");
         }, ClearanceLevel.Debug, "scr_eff"));

         // dump history of province
         var dumpHistoryUsage = "Usage: dump_history <provinceId>";
         _handler.RegisterCommand(new("dump_history", dumpHistoryUsage, args =>
         {
            if (args.Length != 1)
               return [dumpHistoryUsage];

            if (Province.TryParse(args[0], out var province).Ignore())
            {
               var history = province.History;
               if (history.Count == 0)
                  return ["No history entries found"];

               var output = new string[history.Count];
               for (var i = 0; i < history.Count; i++)
                  output[i] = $"{history[i].Date} - {history[i].ToString()}";

               return ["History: ", .. output];
            }

            return ["Invalid province id"];
         }, ClearanceLevel.Debug));

         // To Test dates
         var dateUsage = "Usage: date <date> [--m, --d, --y]";
         _handler.RegisterCommand(new("date", dateUsage, args =>
         {
            if (args.Length == 1)
            {
               if (int.TryParse(args[0], out var stamp))
                  return [$"In: {stamp}", $"Date: {new Date(stamp)}"];
               if (Date.TryParse(args[0], out var date).Ignore())
                  return [$"In: {args[0]}", $"Date: {date}"];
               return ["Invalid date format"];
            }
            if (args.Length == 3)
            {
               Date date;
               if (Date.TryParse(args[0], out date).Ignore())
               {
                  if (!int.TryParse(args[2], out var amount))
                     return [$"Invalid amount for {args[1]}"];

                  switch (args[1])
                  {
                     case "--d":
                        date.AddDays(amount);
                        break;
                     case "--m":
                        date.AddMonths(amount);
                        break;
                     case "--y":
                        date.AddYears(amount);
                        break;
                  }
                  return [$"Date: {date}"];
               }
            }

            return [dateUsage];
         }, ClearanceLevel.Debug));

         // achievement debug command 
         var achievementUsage = "Usage: achievement <id> [-set, -reset, -get, --add <value>]";
         _handler.RegisterCommand(new("achievement", achievementUsage, args =>
         {
            if (args.Length == 0)
               return [achievementUsage];

            var achievement = AchievementManager.GetAchievementFromIdName(args[0]);
            if (achievement is null)
               return [$"Achievement <{args[0]}> not found"];

            if (args.Length == 1)
               return [achievement.Name, achievement.Description];

            if (args.Length == 2)
            {
               if (args[1].Equals("-set"))
               {
                  achievement.SetAchieved();
                  return [$"Achievement '{achievement.Name}' completed."];
               }
               if (args[1].Equals("-reset"))
               {
                  achievement.Reset();
                  return [$"Achievement '{achievement.Name}' reset."];
               }
               if (args[1].Equals("-get"))
                  if (achievement.Condition is ProgressCondition condition)
                     return [$"Achievement '{achievement.Name}' progress: {condition.GetProgress()}/{condition.Goal} ({GetPercentage(condition.CurrentProgress, condition.Goal):0.00}%)"];
                  else
                     return [$"Achievement '{achievement.Name}' progress: {achievement.Condition.GetProgress()}"];
            }

            if (args.Length == 3)
            {
               if (args[1].Equals("--add") && float.TryParse(args[2], out var value))
               {
                  achievement.Condition.IncreaseProgress(value);
                  return [$"Achievement '{achievement.Name}' progress increased by {value}"];
               }
            }

            return [achievementUsage];
         }, ClearanceLevel.Debug));

         // achievements clear and list commands
         var achievementClearUsage = "Usage: achievements [-c] [-l]";
         _handler.RegisterCommand(new("achievements", achievementClearUsage, args =>
         {
            if (args is ["-c"])
            {
               AchievementManager.ResetAchievements();
               return ["Achievements cleared"];
            }

            if (args is ["-l"])
            {
               return ["Achievements: ", .. Enum.GetNames<AchievementId>()];
            }

            return [achievementClearUsage];
         }, ClearanceLevel.Debug));
      }


#endif
      
      private void RunFiles()
      {
         var runFileUsage = "Usage: run <file1(relative to .exe)> --p <provinceId>|--c <TAG>";
         _handler.RegisterCommand(new("run", runFileUsage, args =>
         {
            if (args.Length != 3)
               return [runFileUsage];

            if (args[1].Equals("--c")) // Execute on Country Scope
            {
               var path = args[0];
               path = Path.Combine(Globals.AppDirectory, path);
               if (!File.Exists(path))
                  return [$"File '{path}' not found"];

               var errObj = Country.GeneralParse(args[2], out var country);

               var errorDesc = "";
               if (errObj.Then(o => { errorDesc = o.GetDescription(); }))
               {
                  Executor.ExecuteFile(path, (ITarget)country);
                  return [$"Executed file \'{Path.GetFileName(path)}\' in country scope"];
               }

               return [errorDesc];
            }

            if (args[1].Equals("--p")) // Execute on Province Scope
            {
               var path = args[0];
               path = Path.Combine(Globals.AppDirectory, path);

               if (!File.Exists(path))
                  return [$"File '{path}' not found"];

               var errObj = Province.GeneralParse(args[2], out var province);

               var errorDesc = "";
               if (errObj.Then(o => {errorDesc = o.GetDescription();}))
               {
                  Executor.ExecuteFile(path, (ITarget)province);
                  return [$"Executed file \'{Path.GetFileName(path)}\' in province scope"];
               }

               return [errorDesc];
            }
            return [];
         }, ClearanceLevel.User));
      }

      private void SetProvinceCommands()
      {
         var provinceIdsUsage = "Usage: province_ids";
         _handler.RegisterCommand(new("province_ids", provinceIdsUsage, args =>
         {
            if (args.Length > 0) 
               return [provinceIdsUsage];

            var provinces = Globals.Provinces.ToList();
            provinces.Sort((a, b) => a.Id.CompareTo(b.Id));
            List<string> output = [];

            var gapStart = -1;
            var lastId = 0;
            for (var i = 0; i < provinces.Count; i++)
            {
               if (lastId + 1 == provinces[i].Id)
               {
                  if (gapStart != -1)
                  {
                     var gapEnd = i - 1;
                     output.Add($"Unused ids from {provinces[gapStart].Id} to {provinces[gapEnd].Id}");
                  }
                  lastId = provinces[i].Id;
                  gapStart = -1;
                  continue;
               }
               gapStart = i;
            }
            if (gapStart != -1)
            {
               var gapEnd = provinces[^1].Id;
               output.Add($"Unused ids from {provinces[gapStart].Id} to {gapEnd}");
            }
            return [.. output];
         }, ClearanceLevel.User));
      }

      private void SetUtilityCommands()
      {
         // echo
         var echoUsage = "Usage: echo <message>";
         _handler.RegisterCommand(new("echo", echoUsage, args =>
         {
            if (args.Length == 0)
               return [echoUsage]; // Use the captured variable
            return [string.Join(" ", args)];
         }, ClearanceLevel.User, "say"));
         // help
         _handler.RegisterCommand(new("help", "", args =>
         {
            var cmds = _handler.GetCommandNames();
            cmds.Sort();
            return [.. cmds];
         }, ClearanceLevel.User));
         // Aliases
         _handler.RegisterCommand(new("aliases", "", args =>
         {
            var cmds = _handler.GetCommandAliases();
            cmds.Sort();
            return [.. cmds];
         }, ClearanceLevel.User));
         // alias
         var aliasUsage = "Usage: alias <name> <command> [-r]";
         _handler.RegisterCommand(new("alias", aliasUsage, args =>
         {
            if (args.Length == 2)
            {
               var aliasName = args[0].ToLower();
               var command = string.Join(" ", args.Skip(1));

               if (command.Equals("-r"))
               {
                  if (_handler.RemoveAlias(aliasName))
                     return [$"Alias '{aliasName}' removed"];
                  return [$"Alias '{aliasName}' not found"];
               }

               if (_handler.SetAlias(aliasName, command))
                  return [$"Alias '{aliasName}' created for '{command}'"];
               return [$"Command '{command}' not found"];
            }
            return [aliasUsage];

         }, ClearanceLevel.User));
         // macro
         var macroUsage = "Macro: macro [-l, -c] <name> \"<value>\" [-r, -a]";
         _handler.RegisterCommand(new("macro", macroUsage, args =>
         {
            if (args.Length == 1)
            {
               if (args[0].Equals("-l"))
               {
                  var macros = CommandHandler.GetMacros(_handler.Identifier);
                  return macros.Count > 0 ? ["Macros: ", .. macros.Select(x => $"{x.Key} - {x.Value}").ToArray()] : ["No macros"];
               }
               if (args[0].Equals("-c"))
               {
                  _handler.ClearMacros();
                  return ["Macros cleared"];
               }
               if (_handler.RunMacro(args[0], out var value))
                  return value;
               return [$"Macro '{args[0]}' not found"];
            }
            if (args.Length == 2)
            {
               var macroName = args[0].ToLower();
               var value = args[1];

               if (value.Equals("-r"))
               {
                  if (_handler.RemoveMacro(macroName))
                     return [$"Macro '{macroName}' removed"];
                  return [$"Macro '{macroName}' not found"];
               }
            }
            if (args.Length == 3)
            {
               var macroName = args[0].ToLower();
               var value = args[1];
               if (!(value.StartsWith('\"') && value.EndsWith('\"')) && value.Length < 3)
                  return [macroUsage];
               value = value[1..^1];
               if (args[2].Equals("-a"))
               {
                  _handler.AddMacro(macroName, value);
                  return [$"Macro '{macroName}' created for '{value}'"];
               }
            }
            return [macroUsage];
         }, ClearanceLevel.User));
         // clear
         var clearUsage = "Usage: clear [--l <lines>]";
         _handler.RegisterCommand(new("clear", clearUsage, args =>
         {
            if (args is ["--l", _] && int.TryParse(args[1], out var linesToRemove))
            {
               RemoveLastLines(_outputBox, linesToRemove);
               return [$"Removed last {linesToRemove} lines"];
            }

            if (args.Length > 0) return [clearUsage];
            _outputBox.Clear();
            return [];
         }, ClearanceLevel.User));
         // list
         _handler.RegisterCommand(new("list", "", args =>
         {
            var names = _handler.GetCommands().Where(x => x.Clearance <= _handler.Clearance).ToList();
            names.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            var list = WriteFormattedColumns(names.Select(x => x.Name).ToArray(), names.Select(x => x.Usage).ToArray());

            var clearanceArray = names.Select(x =>
            {
               return x.Clearance switch
               {
                  ClearanceLevel.Debug => "[D]",
                  ClearanceLevel.User => "[U]",
                  ClearanceLevel.Admin => "[A]",
                  _ => "[-]"
               };
            }).ToList();

            for (var i = 0; i < list.Length; i++)
               list[i] = $"{clearanceArray[i]} {list[i]}";

            return ["Available Commands:", .. list];
         }, ClearanceLevel.User));
         // execution directory
         _handler.RegisterCommand(new("directory", "", args =>
         {
            return ["Executing directory:", Directory.GetCurrentDirectory()];
         }, ClearanceLevel.User, "dir", "pwd"));
         // history
         _handler.RegisterCommand(new("history", "Usage: history [-c]", args =>
         {
            if (args is ["-c"])
            {
               _handler.History.Clear();
               return ["Command history cleared"];
            }

            return _handler.History.Count > 0 ? ["History: ", .. _handler.History.ToArray()] : ["No commands in history"];
         }, ClearanceLevel.User));
         // table
         _handler.RegisterCommand(new("table", "Usage: table <val1,val2,val3...> <column2> ...", args =>
         {
            if (args.Length < 2)
               return ["Usage: table <column1> <column2> ..."];

            var columns = args.Select(x => CommandHandler.SplitStringQuotes(x, ',')).ToArray();
            return DrawTable('|', columns);
         }, ClearanceLevel.User));
      }

      private float GetPercentage(float value, float max) => Math.Clamp(value / max * 100, 0, 100);

      private static void RemoveLastLines(RichTextBox outputBox, int lineCount)
      {
         if (lineCount <= 0)
            return;

         var lines = outputBox.Lines.ToList();
         if (lines.Count == 0) return;

         var removeCount = Math.Min(lineCount, lines.Count);
         lines.RemoveRange(lines.Count - removeCount, removeCount);

         if (lines.Count == 0)
            lines.Add("> ");
         else
            lines[^1] = "> ";
         outputBox.Lines = lines.ToArray();
      }

      private static string[] WriteFormattedColumns(IReadOnlyList<string> col1, IReadOnlyList<string> col2, char separator = '-')
      {
         Debug.Assert(col1.Count == col2.Count, "Both lists must be of even length!");

         var output = new string[col1.Count];
         var col1MaxWidth = col1.Max(x => x.Length);

         for (var i = 0; i < col1.Count; i++)
            output[i] = $"{col1[i].PadRight(col1MaxWidth)} {separator} {col2[i]}";

         return output;
      }

      private static string[] DrawTable(char separator = '|', params string[][] columns)
      {
         Debug.Assert(columns.Length > 0, "At least one column must be provided.");
         var rowCount = columns[0].Length;

         Debug.Assert(columns.All(col => col.Length == rowCount), "All columns must have the same number of rows.");

         var colWidths = columns.Select(col => col.Max(cell => cell.Length)).ToArray();
         var output = new string[rowCount];
         
         for (var row = 0; row < rowCount; row++) 
            output[row] = string.Join($" {separator} ", columns.Select((col, i) => col[row].PadRight(colWidths[i])));

         return output;
      }

   }
}