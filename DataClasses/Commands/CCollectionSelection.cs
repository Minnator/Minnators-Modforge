using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.Commands
{
   public class CCollectionSelection : ICommand
   {
      private readonly List<Province> _selectionDelta;
      private readonly bool _deselect;

      public CCollectionSelection(ICollection<Province> toSelect, bool deselect = false, bool executeOnInit = true)
      {
         _selectionDelta = toSelect.Except(Selection.SelectionPreview).ToList();
         _deselect = deselect;
         if (executeOnInit)
            Execute();
      }

      public CCollectionSelection(ProvinceCollection<Province> collection, bool deselect = false, bool executeOnInit = true)
      {
         _selectionDelta = collection.GetProvinces().ToList();
         _deselect = deselect;
         if (executeOnInit)
            Execute();
      }

      public void Execute()
      {
         if (_deselect)
            Selection.RemoveProvincesFromSelection(_selectionDelta);
         else
            Selection.AddProvincesToSelection(_selectionDelta);
      }

      public void Undo()
      {
         if (_deselect)
            Selection.AddProvincesToSelection(_selectionDelta);
         else
            Selection.RemoveProvincesFromSelection(_selectionDelta);
      }

      public void Redo()
      {
         if (_deselect)
            Execute();
         else
            Undo();
      }

      public string GetDescription()
      {
         return $"Selected [{_selectionDelta.Count}] provinces";
      }
   }
}