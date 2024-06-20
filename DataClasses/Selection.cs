using Editor.Controls;
using Editor.Helper;
using System.Collections.Generic;
using System.Drawing;

namespace Editor.DataClasses;

public class Selection(PannablePictureBox pannablePictureBox)
{
   // Settable color of the selection
   public Color color = Color.DarkOrange;

   // ------------------- Rectangle Selection -------------------
   private Point RectanglePoint = Point.Empty; // Point which defines the starting point of the rectangle selection
   public bool IsInRectSelection; // True if the user is currently in the rectangle selection
   private Point _lastRectPoint = Point.Empty; // Last point of the rectangle selection used to clear the rectangle
   
   // List of selected provinces
   public List<int> SelectedProvPtr { get; set; } = [];

   public void MarkNext(int provPtr)
   {
      Clear();
      Add(provPtr);
   }

   // Adds a province to the selection and redraws the border, allows for deselecting
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

   public void Clear() => RemoveRange(SelectedProvPtr);

   public bool Contains(int provPtr) => SelectedProvPtr.Contains(provPtr);

   // Marks all provinces in the rectangle which is defined by the RectanglePoint and the given point
   public void MarkAllInRectangle(Point point)
   {
      if (RectanglePoint == Point.Empty) 
         return;

      DrawRectangle(_lastRectPoint, Color.Transparent);
      List<int> provincePtr = [];
      foreach (var province in Data.Provinces)
         if (MathHelper.RectanglesIntercept(province.Bounds, MathHelper.GetBoundingRectangle([RectanglePoint, point])))
            provincePtr.Add(province.SelfPtr);

      Clear();
      AddRange(provincePtr, false);
      DrawRectangle(point, Color.Red);
   }

   // Draws a rectangle on the map in reference to the RectanglePoint
   private void DrawRectangle(Point refPoint, Color rectColor)
   {
      var rect = MathHelper.GetBoundingRectangle([RectanglePoint, refPoint]);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawOnMap(rect, MathHelper.GetRectanglePoints(rect), rectColor,
         pannablePictureBox.SelectionOverlay));
      _lastRectPoint = refPoint;
   }

   // Enters the rectangle selection and sets the starting point
   public void EnterRectangleSelection(Point startPoint)
   {
      RectanglePoint = startPoint;
      IsInRectSelection = true;
   }

   // Exits the rectangle selection and redraws the Selection ones to remove the red rectangle
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