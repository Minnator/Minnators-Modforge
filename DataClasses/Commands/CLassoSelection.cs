using System.Collections.Generic;
using System.Linq;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CLassoSelection : ICommand
{
   private readonly PannablePictureBox _pb;
   private readonly List<int> _selectionDelta;

   public CLassoSelection(PannablePictureBox pb, bool executeOnInit = true)
   {
      _pb = pb;
      var selection = Geometry.GetProvincesInPolygon(pb.Selection.LassoSelection);
      _selectionDelta = selection.Except(pb.Selection.SelectedProvinces).ToList();
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
      return $"Lasso selected [{_selectionDelta.Count}] provinces";
   }
}