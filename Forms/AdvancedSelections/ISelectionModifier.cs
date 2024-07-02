using Editor.Commands;

namespace Editor.Forms.AdvancedSelections;

public interface ISelectionModifier
{
   public string Name { get; set; }
   public void Execute();
}