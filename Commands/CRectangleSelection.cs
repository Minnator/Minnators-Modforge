using System.Drawing;
using Editor.Controls;
using Editor.Helper;

namespace Editor.Commands;

public class CRectangleSelection :ICommand
{
   private readonly Rectangle _rectangle;
   private readonly PannablePictureBox _pb;
   public CRectangleSelection(Rectangle rectangle, PannablePictureBox pb, bool executeOnInit = true)
   {
      _rectangle = rectangle;
      _pb = pb;

      if (executeOnInit)
         Execute();

   }

   public void Execute()
   {
      // We do not execute code here as the command is added without the need to do it.
   }

   public void Undo()
   {
      var ids = Geometry.GetProvinceIdsInRectangle(_rectangle);
      _pb.Selection.RemoveRange(ids);
   }

   public void Redo()
   {
      var ids = Geometry.GetProvinceIdsInRectangle(_rectangle);
      _pb.Selection.AddRange(ids);
   }

   public string GetDescription()
   {
      var count = Geometry.GetProvinceIdsInRectangle(_rectangle).Count;
      return $"Select {count} provinces in rectangle";
   }
}