using Editor.Controls;
using Editor.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Editor.Commands;

namespace Editor.DataClasses;

public enum SelectionState
{
   Single,
   Rectangle,
   Lasso
}
public class Selection(PannablePictureBox pannablePictureBox)
{
   // Settable color of the selection
   public Color Color = Color.Red;

   // ------------------- Rectangle Selection -------------------
   private Point _rectanglePoint = Point.Empty; // Point which defines the starting point of the rectangle selection
   public SelectionState State = SelectionState.Single; // State of the selection
   private Point _lastRectPoint = Point.Empty; // Last point of the rectangle selection used to clear the rectangle
   
   // ------------------- Draw Selection -------------------
   public List<Point> LassoSelection = []; // List of points which define the selection polygon
   private bool _clearPolygonSelection;

   // List of selected provinces
   public List<int> SelectedProvPtr { get; set; } = [];

   // Setting the clearPolygonSelection to false will clear the polygon selection
   public bool ClearPolygonSelection
   {
      get => _clearPolygonSelection;
      set
      {
         if (!value)
         {
            Debug.WriteLine($"Lasso had {LassoSelection.Count} points");
            LassoSelection = [];
         }
         _clearPolygonSelection = value;
      }
   }

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
         pannablePictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(provPtr, Color, pannablePictureBox.SelectionOverlay));
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

   public void LassoSelect(int[] ids)
   {
      if (LassoSelection.Count < 2)
         return;
      AddRange(ids, false);
   }

   public void MarkAllInRectangle(Point point)
   {
      if (_rectanglePoint == Point.Empty)
         return;

      // Remove the last selection rectangle
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);

      // Precompute the bounds
      var currentRectBounds = Geometry.GetBounds([_rectanglePoint, point]);
      var lastRectBounds = Geometry.GetBounds([_rectanglePoint, _lastRectPoint]);
      var intersection = Geometry.GetIntersection(currentRectBounds, lastRectBounds);

      var provPtrAdd = Geometry.GetProvinceIdsInRectangle(currentRectBounds);
      var provPtrRemove = new List<int>();
      
      // Provinces which are in the previous selection but not in the current intersection
      foreach (var province in SelectedProvPtr)
      {
         if (!Geometry.RectanglesIntercept(Data.Provinces[province].Bounds, intersection)) 
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
      var rect = Geometry.GetBounds([_rectanglePoint, refPoint]);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawOnMap(rect, Geometry.GetRectanglePoints(rect), rectColor,
         bmp));
      _lastRectPoint = refPoint;
      pannablePictureBox.IsPainting = false;
   }

   // Enters the rectangle selection and sets the starting point
   public void EnterRectangleSelection(Point startPoint)
   {
      _rectanglePoint = startPoint;
      State = SelectionState.Rectangle;
   }

   // Exits the rectangle selection and redraws the Selection ones to remove the red rectangle
   public void ExitRectangleSelection()
   {
      var rect = Geometry.GetBounds([_rectanglePoint, _lastRectPoint]);
      Data.HistoryManager.AddCommand(new CRectangleSelection(rect, pannablePictureBox), HistoryType.ComplexSelection);
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);
      _rectanglePoint = Point.Empty;
      State = SelectionState.Single;
   }
}