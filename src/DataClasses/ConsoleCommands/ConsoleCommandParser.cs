using Editor.Controls;
using Editor.Helper;

namespace Editor.DataClasses.ConsoleCommands;

public static class ConsoleCommandParser
{
   public static List<string> PastCommands = [];
   public static int PastCommandIndex = 0;
   public static string[] CommandNames { get; set; }
   private static readonly Dictionary<string, ConsoleCommand> _commands = new()
   {
      { "echo", new CsCEcho() },
      { "analyze", new CsCAnalyzeProvince() },
      { "dump", new Dump()}
   };

   static ConsoleCommandParser()
   {
      foreach (var cmd in _commands.Values.ToList())
      {
         foreach (var alias in cmd.Aliases)
         {
            _commands.Add(alias, cmd);
         }
      }

      CommandNames = [.._commands.Keys];
   }

   public static void ParseCommand(string command)
   {
      PastCommands.Add(command);
      PastCommandIndex = PastCommands.Count - 1;
      var args = command.Split(' ');

      if (!_commands.TryGetValue(args[0], out var cmd))
      {
         // Print red error message to Output
         Globals.ConsoleForm!.Output.AppendText($"Command '{args[0]}' not found.\n", Color.Red);
         return;
      }

      switch (args.Length)
      {
         case 1:
            cmd.ExecuteDescription();
            break;
         case 2:
            switch (args[1])
            {
               case "help":
                  cmd.ExecuteHelp();
                  break;
               case "alias":
                  cmd.ExecuteAliases();
                  break;
               default:
                  cmd.Execute(args);
                  break;
            }
            break;
         default:
            cmd.Execute(args);
            break;
      }

   }

   public static List<ConsoleCommand> GetCommandCompletion (string command)
   {
      var args = command.Split(' ');
      var cmd = args[0];
      List<string> strs = [.. _commands.Keys.Where(c => c.StartsWith(cmd))];
      return strs.Select(s => _commands[s]).ToList();
   }

   public static List<ConsoleCommand> GetCommandCompletionsLevinstein (string command)
   {
      var args = command.Split(' ');
      var cmdName = args[0];

      var completions = new List<ConsoleCommand>();
      foreach (var cmd in _commands.Values)
      {
         if (Levinstein.GetLevinsteinDistance(CommandNames, cmdName) < 6)
         {
            completions.Add(cmd);
         }
      }

      return completions;
   }

   public static string GetPreviousCommand ()
   {
      if (PastCommands.Count == 0) 
         return string.Empty;
      if (PastCommandIndex == 0) 
         return PastCommands[PastCommandIndex];
      return PastCommands[PastCommandIndex--];
   }

   public static string GetNextCommand ()
   {
      if (PastCommands.Count == 0) 
         return string.Empty;
      if (PastCommandIndex == PastCommands.Count - 1) 
         return PastCommands[PastCommandIndex];
      return PastCommands[PastCommandIndex++];
   }

   
}