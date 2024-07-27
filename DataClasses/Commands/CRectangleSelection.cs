using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CRectangleSelection :ICommand
{
   private readonly PannablePictureBox _pb;
   private readonly List<int> _selectionDelta;
   public CRectangleSelection(Rectangle rectangle, PannablePictureBox pb, bool executeOnInit = true)
   {
      _pb = pb;
      _selectionDelta = Geometry.GetProvinceIdsInRectangle(rectangle).Except(Globals.Selection.SelectedProvinces).ToList();

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
      Globals.Selection.AddRange(_selectionDelta);
   }

   public string GetDescription()
   {
      return $"Select {_selectionDelta.Count} provinces in rectangle";
   }
}