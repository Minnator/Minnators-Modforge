using Editor.Controls;
using Editor.Interfaces;

namespace Editor.Commands
{
   public class CCollectionSelection : ICommand
   {
      private readonly List<int> _selectionDelta;

      public CCollectionSelection(PannablePictureBox pb, List<int> toSelect, bool executeOnInit = true)
      {
         _selectionDelta = toSelect.Except(Globals.Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public CCollectionSelection(PannablePictureBox pb, IProvinceCollection collection, bool executeOnInit = true)
      {
         _selectionDelta = collection.GetProvinceIds().Except(Globals.Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         Globals.Selection.AddRange(_selectionDelta, false);
      }

      public void Undo()
      {
         Globals.Selection.RemoveRange(_selectionDelta);
      }

      public void Redo()
      {
         Execute();
      }

      public string GetDescription()
      {
         return $"Selected [{_selectionDelta.Count}] provinces";
      }
   }
}