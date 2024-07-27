using System.Collections.Generic;
using System.Linq;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CLassoSelection : ICommand
{
   private readonly List<int> _selectionDelta;

   public CLassoSelection(bool executeOnInit = true)
   {
      var selection = Geometry.GetProvincesInPolygon(Globals.Selection.LassoSelection);
      _selectionDelta = selection.Except(Globals.Selection.SelectedProvinces).ToList();
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
      return $"Lasso selected [{_selectionDelta.Count}] provinces";
   }
}