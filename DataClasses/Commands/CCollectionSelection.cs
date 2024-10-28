using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.Commands
{
   public class CCollectionSelection : ICommand
   {
      private readonly List<Province> _selectionDelta;

      public CCollectionSelection(ICollection<Province> toSelect, bool executeOnInit = true)
      {
         _selectionDelta = toSelect.Except(Selection.SelectionPreview).ToList();
         if (executeOnInit)
            Execute();
      }

      public CCollectionSelection(ProvinceCollection<Province> collection, bool executeOnInit = true)
      {
         _selectionDelta = collection.GetProvinces().Except(Selection.SelectionPreview).ToList();
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