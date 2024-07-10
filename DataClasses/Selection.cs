using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Editor.Commands;
using Editor.Controls;
using Editor.Helper;

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
   public Color SelectionColor = Color.Orange;
   public Color SelectionOutlineColor = Color.Red;

   // ------------------- Rectangle Selection -------------------
   private Point _rectanglePoint = Point.Empty; // Point which defines the starting point of the rectangle selection
   public SelectionState State = SelectionState.Single; // State of the selection
   private Point _lastRectPoint = Point.Empty; // Last point of the rectangle selection used to clear the rectangle
   
   // ------------------- Draw Selection -------------------
   public List<Point> LassoSelection = []; // List of points which define the selection polygon
   private bool _clearPolygonSelection;

   // List of selected provinces
   public List<int> SelectedProvinces { get; set; } = [];
   public HashSet<int> SelectionPreview { get; set; } = [];

   // Setting the clearPolygonSelection to false will clear the polygon selection
   public bool ClearPolygonSelection
   {
      get => _clearPolygonSelection;
      set
      {
         if (!value)
         {
            LassoSelection = [];
         }
         _clearPolygonSelection = value;
      }
   }

   public void MarkNext(int provPtr)
   {
      if (SelectedProvinces.Count == 1 && SelectedProvinces.Contains(provPtr))
      {
         Clear();
      }
      else
      {
         Clear();
         Add(provPtr);
      }
   }

   // Adds a province to the selection and redraws the border, allows for deselecting
   public void Add(int provId, bool allowDeselect = true, bool isPreview = false)
   {
      if (isPreview)
      {
         if (!SelectionPreview.Add(provId))
            return;
      }
      else
      {
         if (allowDeselect && SelectedProvinces.Contains(provId))
         {
            Remove(provId);
            return;
         }
         SelectedProvinces.Add(provId);
      }
      
      pannablePictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(provId, SelectionColor, pannablePictureBox.SelectionOverlay));
   }

   public void AddRange(IEnumerable<int> provIds, bool allowDeselect = true, bool isPreview = false)
   {
      foreach (var provPtr in provIds)
         Add(provPtr, allowDeselect, isPreview);
   }

   public void Remove(int provId, bool isPreview = false)
   {
      if (isPreview)
         SelectionPreview.Remove(provId);
      else
         SelectedProvinces.Remove(provId);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawProvinceBorder(provId, Color.Transparent, pannablePictureBox.SelectionOverlay));
   }

   public void RemoveRange(List<int> provIds, bool isPreview = false)
   {
      for (var i = provIds.Count - 1; i >= 0; i--)
         Remove(provIds[i], isPreview);
   }

   public void Clear() => RemoveRange(SelectedProvinces);

   public bool Contains(int provPtr) => SelectedProvinces.Contains(provPtr);

   public void LassoSelect(int[] ids)
   {
      if (LassoSelection.Count < 2)
         return;
      AddRange(ids, false);
   }

   public void PreviewAllInPolygon()
   {
      if (LassoSelection.Count < 3)
         return;

      var polyDiff = Geometry.GetPolygonDiffLastPoint(LassoSelection);
      var provDiff = Geometry.GetProvincesInPolygon(polyDiff); 
      
      if (provDiff.Count == 0)
         return;

      if (SelectionPreview.Contains(provDiff[0]))
         RemoveRange(provDiff, true);
      else
         AddRange(provDiff, false, true);
   }

   public void PreviewAllInRectangle(Point point)
   {
      if (_rectanglePoint == Point.Empty)
         return;
      // Remove the last selection rectangle
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);

      // Precompute the bounds
      var currentRectBounds = Geometry.GetBounds([_rectanglePoint, point]);
      var lastRectBounds = Geometry.GetBounds([_rectanglePoint, _lastRectPoint]);
      var intersection = Geometry.GetIntersection(currentRectBounds, lastRectBounds);

      _lastRectPoint = point;
      var provPtrAdd = Geometry.GetProvinceIdsInRectangle(currentRectBounds);
      List<int> provPtrRemove = [];

      // Provinces which are in the previous selection but not in the current intersection
      foreach (var province in SelectionPreview)
      {
         if (!Geometry.RectanglesIntercept(Globals.Provinces[province].Bounds, intersection)) 
            provPtrRemove.Add(province);
      }

      AddRange(provPtrAdd, false, true);
      RemoveRange(provPtrRemove, true);

      // Draw the new selection rectangle
      DrawRectangle(point, SelectionOutlineColor, pannablePictureBox.Overlay);
   }


   // Draws a rectangle on the map in reference to the RectanglePoint
   private void DrawRectangle(Point refPoint, Color rectColor, Bitmap bmp)
   {
      pannablePictureBox.IsPainting = true;
      var rect = Geometry.GetBounds([_rectanglePoint, refPoint]);
      pannablePictureBox.Invalidate(MapDrawHelper.DrawOnMap(rect, Geometry.GetRectanglePoints(rect), rectColor, bmp));
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
      Globals.HistoryManager.AddCommand(new CRectangleSelection(rect, pannablePictureBox), CommandHistoryType.ComplexSelection);
      DrawRectangle(_lastRectPoint, Color.Transparent, pannablePictureBox.Overlay);
      _rectanglePoint = Point.Empty;
      SelectionPreview = [];
      State = SelectionState.Single;
   }

   public void ExitLassoSelection()
   {
      ClearPolygonSelection = true;
      State = SelectionState.Single;
      RemoveRange(SelectionPreview.ToList(), true);
      SelectionPreview = [];
   }
}