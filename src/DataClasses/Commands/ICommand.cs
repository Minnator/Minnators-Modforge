using Editor.Saving;

namespace Editor.DataClasses.Commands;

public interface ICommand
{
   public void Execute();
   public void Undo();
   public void Redo();
   public List<Saveable> GetTargets();
   public string GetDescription();
   public string GetDebugInformation(int indent);
}
