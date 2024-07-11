using Editor.Controls;
using Editor.Interfaces;

namespace Editor.Commands
{
   public class CCollectionSelection : ICommand
   {
      private readonly PannablePictureBox _pb;
      private readonly List<int> _selectionDelta;

      public CCollectionSelection(PannablePictureBox pb, List<int> toSelect, bool executeOnInit = true)
      {
         _pb = pb;
         _selectionDelta = toSelect.Except(pb.Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public CCollectionSelection(PannablePictureBox pb, IProvinceCollection collection, bool executeOnInit = true)
      {
         _pb = pb;
         _selectionDelta = collection.GetProvinceIds().Except(pb.Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         _pb.Selection.AddRange(_selectionDelta, false);
      }

      public void Undo()
      {
         _pb.Selection.RemoveRange(_selectionDelta);
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