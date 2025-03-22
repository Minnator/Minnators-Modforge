using Editor.Saving;

namespace Editor.DataClasses.Commands;

public interface ICommand
{
   public void Execute();
   public void Undo();
   public void Redo();
   /// <summary>
   /// The hash is needed to determine commands which target the same objects in history compaction
   /// </summary>
   /// <returns></returns>
   public List<int> GetTargetHash();
   public string GetDescription();
   public string GetDebugInformation(int indent);
}
