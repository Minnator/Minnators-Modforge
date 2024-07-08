using System.Collections.Generic;
using Editor.DataClasses.ConsoleCommands;

namespace Editor.Forms;

public static class ConsoleCommandParser
{
   private static Dictionary<string, ConsoleCommand> _commands = new()
   {
      { "echo", new CsCEcho() }
   };

   public static void ParseCommand(string command)
   {
      var args = command.Split(' ');
      var cmd = args[0];
      args = args[1..];

      if (_commands.ContainsKey(cmd))
      {
         _commands[cmd].Execute(args);
      }
   }
}