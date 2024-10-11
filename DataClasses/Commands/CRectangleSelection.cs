using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CRectangleSelection :ICommand
{
   private readonly List<int> _selectionDelta;
   public CRectangleSelection(Rectangle rectangle, bool executeOnInit = true)
   {
      _selectionDelta = Geometry.GetProvinceIdsInRectangle(rectangle).Except(Selection.GetSelectedProvincesIds.ToList()).ToList();

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
      Selection.AddProvincesToSelection(_selectionDelta, false);
   }

   public string GetDescription()
   {
      return $"Select {_selectionDelta.Count} provinces in rectangle";
   }
}