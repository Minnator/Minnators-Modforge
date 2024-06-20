using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Editor.Controls;
using Editor.Helper;

namespace Editor.DataClasses;

public class Selection(PannablePictureBox pannablePictureBox)
{
   public Color color = Color.Aquamarine;
   public Point RectanglePoint = Point.Empty;
   public bool IsInRectSelection;

   private Point _lastRectPoint = Point.Empty;

   public List<int> SelectedProvPtr { get; set; } = [];

   public void MarkNext(int provPtr)
   {
      Clear();
      Add(provPtr);
   }

   public void Add(int provPtr, bool allowDeselect = true)
   {
      if (!SelectedProvPtr.Contains(provPtr))
      {
         SelectedProvPtr.Add(provPtr);
         pannablePictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(provPtr, color, pannablePictureBox.SelectionOverlay));
      }
      else if (allowDeselect)
         Remove(provPtr);
   }

   public void AddRange(IEnumerable<int> provPtrs, bool allowDeselect = true)
   {
      foreach (var provPtr in provPtrs)
         Add(provPtr, allowDeselect);
   }

   public void Remove(int provPtr)
   {
      if (SelectedProvPtr.Contains(provPtr))
      {
         SelectedProvPtr.Remove(provPtr);
         pannablePictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(provPtr, Color.Transparent, pannablePictureBox.SelectionOverlay));
      }
   }

   public void RemoveRange(List<int> provPtrs)
   {
      for (var i = provPtrs.Count - 1; i >= 0; i--)
         Remove(provPtrs[i]);
   }

   public void Clear()
   {
      RemoveRange(SelectedProvPtr);
   }

   public bool Contains(int provPtr) => SelectedProvPtr.Contains(provPtr);

   public void MarkAllInRectangle(Point point)
   {
      if (RectanglePoint != Point.Empty)
      {
         DrawRectangle(_lastRectPoint, Color.Transparent);
         List<int> provincePtr = [];
         foreach (var province in Data.Provinces)
            if (MathHelper.RectanglesIntercept(province.Bounds, MathHelper.GetBoundingBox([RectanglePoint, point])))
               provincePtr.Add(province.SelfPtr);
         Clear();
         AddRange(provincePtr, false);

         DrawRectangle(point, Color.Red);
      }
   }

   public void DrawRectangle(Point refPoint, Color rectColor)
   {
      var rect = MathHelper.GetBoundingBox([RectanglePoint, refPoint]);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawOnMap(rect, MathHelper.GetRectanglePoints(rect), rectColor,
         pannablePictureBox.SelectionOverlay));
      _lastRectPoint = refPoint;
   }

   public void EnterRectangleSelection(Point startPoint)
   {
      RectanglePoint = startPoint;
      IsInRectSelection = true;
   }

   public void ExitRectangleSelection()
   {
      DrawRectangle(_lastRectPoint, Color.Transparent);
      RectanglePoint = Point.Empty;
      IsInRectSelection = false;
      var proPtrs = new List<int>(SelectedProvPtr);
      Clear();
      AddRange(proPtrs);
   }
}