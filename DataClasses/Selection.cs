using Editor.Controls;
using Editor.Helper;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Editor.Commands;

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
   
   public void MarkAllInRectangle(Point point)
   {
      if (RectanglePoint == Point.Empty)
         return;

      // Remove the last selection rectangle
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);

      // Precompute the bounds
      var currentRectBounds = MathHelper.GetBounds([RectanglePoint, point]);
      var lastRectBounds = MathHelper.GetBounds([RectanglePoint, _lastRectPoint]);
      var intersection = MathHelper.GetIntersection(currentRectBounds, lastRectBounds);

      var provPtrAdd = MathHelper.GetProvincesInRectangle(currentRectBounds);
      var provPtrRemove = new List<int>();
      
      // Provinces which are in the previous selection but not in the current intersection
      foreach (var province in SelectedProvPtr)
      {
         if (!MathHelper.RectanglesIntercept(Data.Provinces[province].Bounds, intersection)) 
            provPtrRemove.Add(province);
      }

      AddRange(provPtrAdd, false);
      RemoveRange(provPtrRemove);

      // Draw the new selection rectangle
      DrawRectangle(point, Color.Red, pannablePictureBox.Overlay);
   }


   // Draws a rectangle on the map in reference to the RectanglePoint
   private void DrawRectangle(Point refPoint, Color rectColor, Bitmap bmp)
   {
      pannablePictureBox.IsPainting = true;
      var rect = MathHelper.GetBounds([RectanglePoint, refPoint]);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawOnMap(rect, MathHelper.GetRectanglePoints(rect), rectColor,
         bmp));
      _lastRectPoint = refPoint;
      pannablePictureBox.IsPainting = false;
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
      var rect = MathHelper.GetBounds([RectanglePoint, _lastRectPoint]);
      Data.HistoryManager.AddCommand(new CRectangleSelection(rect, pannablePictureBox), HistoryType.ComplexSelection);
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);
      RectanglePoint = Point.Empty;
      IsInRectSelection = false;
   }
}