using Editor.Commands;
using Editor.Controls;

namespace Editor.Forms.AdvancedSelections;

public class Deselection(string attr, object value) : ISelectionModifier
{
   public string Name { get; set; } = "Deselection";

   public void Execute()
   {
      Globals.HistoryManager.AddCommand(new CDeselectCommand(attr, value));
   }
}