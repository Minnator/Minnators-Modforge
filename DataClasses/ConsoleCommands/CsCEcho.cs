using System.Windows.Forms;
using Editor.Controls;

namespace Editor.DataClasses.ConsoleCommands;

public class CsCEcho: ConsoleCommand
{
   public override string Command { get; } = "echo";
   public override string Description { get; } = "Echoes the input back to the console.";
   public override string Help { get; } = "Usage: echo <text>";
   public override string[] Aliases { get; } = ["print", "out"];
   public override void Execute(string[] args)
   {
      if (args is ["help"])
      {
         Output.WriteLine(Help);
      }
      else if (args.Length == 0)
      {
         Output.WriteLine(Description);
      }
      else
      {
         var text = string.Join(" ", args);
         text = text.Replace("echo", "");
         Output.WriteLine(text);         
      }
   }
}