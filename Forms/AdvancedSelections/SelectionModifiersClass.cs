using Editor.Commands;
using Editor.Controls;

namespace Editor.Forms.AdvancedSelections;

public class Deselection(string attr, object value, PannablePictureBox pb) : ISelectionModifier
{
   public string Name { get; set; } = "Deselection";

   public void Execute()
   {
      Globals.HistoryManager.AddCommand(new CDeselectCommand(attr, value, pb));
   }
}