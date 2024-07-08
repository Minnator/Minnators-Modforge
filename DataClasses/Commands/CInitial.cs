namespace Editor.Commands;

public class CInitial : ICommand
{
   public void Execute() {  }
   public void Undo() {  }
   public void Redo() {  }
   public string GetDescription() => "Initial Command";
}