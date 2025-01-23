using Editor.Saving;

namespace Editor.DataClasses.Commands;

public class CInitial : ICommand
{
   public void Execute() {  }
   public void Undo() {  }
   public void Redo() {  }
   public List<Saveable> GetTargets() => [];
   public string GetDescription() => "Initial Command";
   public string GetDebugInformation(int indent)
   {
      return "Cheese.";
   }
}