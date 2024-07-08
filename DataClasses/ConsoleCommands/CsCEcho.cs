using System.Windows.Forms;

namespace Editor.DataClasses.ConsoleCommands;

public class CsCEcho: ConsoleCommand
{
   public override string Command { get; } = "echo";
   public override string Description { get; } = "Echoes the input back to the console.";
   public override string Help { get; } = "Usage: echo <text>";
   public override string[] Aliases { get; } = { "print" };
   public override void Execute(string[] args)
   {
      var text = string.Join(" ", args);
      text = text.Replace("echo", "");
      Output.Text += text + "\n";
   }
}