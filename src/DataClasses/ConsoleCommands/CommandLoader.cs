﻿using System.Diagnostics;
using System.Reflection.Metadata;

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
            var names = _handler.GetCommands();
            names.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            var list = WriteFormattedColumns(names.Select(x => x.Name).ToArray(), names.Select(x => x.Usage).ToArray());
            return ["Available Commands:", .. list];
         }, ClearanceLevel.User));

         // execution directory
         _handler.RegisterCommand(new("directory", "", args =>
         {
            return ["Executing directory:", Directory.GetCurrentDirectory()];
         }, ClearanceLevel.User, "dir", "pwd"));


         _handler.RegisterCommand(new("history", "Usage: history [-c]", args =>
         {
            if (args is ["-c"])
            {
               _handler.History.Clear();
               return ["Command history cleared"];
            }

            return _handler.History.Count > 0 ? ["History: ", .. _handler.History.ToArray()] : ["No commands in history"];
         }, ClearanceLevel.User));

         _handler.RegisterCommand(new("table", "Usage: table <val1,val2,val3...> <column2> ...", args =>
         {
            if (args.Length < 2)
               return ["Usage: table <column1> <column2> ..."];

            var columns = args.Select(x => CommandHandler.SplitStringQuotes(x, ',')).ToArray();
            return DrawTable('|', columns);
         }, ClearanceLevel.User));
      }


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