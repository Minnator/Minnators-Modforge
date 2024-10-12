using System.Collections.Generic;
using System.Linq;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Commands;

public class CLassoSelection : ICommand
{
   private readonly List<Province> _selectionDelta;

   public CLassoSelection(bool executeOnInit = true)
   {
      // TODO rework selection commands
      //var selection = Geometry.GetProvincesInPolygon(Selection.LassoSelection);
      //_selectionDelta = selection.Except(Selection.SelectionPreview).ToList();
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
      return $"Lasso selected [{_selectionDelta.Count}] provinces";
   }
}