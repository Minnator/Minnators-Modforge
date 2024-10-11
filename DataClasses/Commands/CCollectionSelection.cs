using Editor.Controls;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.Commands
{
   public class CCollectionSelection : ICommand
   {
      private readonly List<int> _selectionDelta;

      public CCollectionSelection(List<int> toSelect, bool executeOnInit = true)
      {
         _selectionDelta = toSelect.Except(Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public CCollectionSelection(IProvinceCollection collection, bool executeOnInit = true)
      {
         _selectionDelta = collection.GetProvinceIds().Except(Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         Selection.AddProvincesToSelection(_selectionDelta, false);
      }

      public void Undo()
      {
         Selection.RemoveProvincesFromSelection(_selectionDelta);
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