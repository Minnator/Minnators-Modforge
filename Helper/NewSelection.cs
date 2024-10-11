using Editor.DataClasses.GameDataClasses;
using System.Diagnostics;
using System.Linq;

namespace Editor.Helper;


public enum SelectionType
{
   Lasso,     // Using the lasso tool to select provinces
   Rectangle, // Using the rectangle tool to select provinces
   MagicWand, // Using the Magic Wand tool to select provinces using flood fill
   Single,    // Single province selection
   Collection // Collection of provinces
}

public static class NewSelection
{
   // TODO MustHaves
   // [ ] Must have a way to draw between provinces for e.g. Straits

   //--- Getters ---\\
   // [x] Get Selected Provinces
   // [x] Get Selected Province Ids
   // [x] Get Selection Count

   //--- Events ---\\
   // [x] OnProvinceSelection Changed
   // [x] OnCountrySelection Changed

   //--- Selection Types ---\\
   // [x] Lasso Selection
   // [x] Rectangle Selection
   // [ ] Magic Wand Selection

   //--- Selection Modes ---\\
   // [x] Add Selection (n provinces) // Adds the provinces to the current selection and has bool to remove if it is already selected
   // [x] Subtract Selection (n provinces)
   // [x] Clear Selection (Clears the current selection)
   // [x] Invert Selection (Inverts the current selection)
   // [-] Group Selection (Selects all provinces which return true for a func<int> bool) // e.g. Select all provinces with owner = Tag.Empty or in area.Provinces

   //--- Selection Preview ---\\
   // [x] Preview Selection (Shows the provinces that will be selected)
   // [x] Preview Selection Color (Color of the provinces that will be selected)

   //--- Other Methods ---\\
   // [x] Focus Selection (Focuses on the selected provinces bounds center) Animation?
   // [x] Highlight Provinces (Highlights the given collection provinces)
   // [x] Unhighlight Provinces (DeHighlights the given collection of provinces)
   // [x] Highlight Country (Highlights the given country)
   // [x] Unhighlight Country (DeHighlights the given country)

   // LMB down ==> Select only one province or deselect if already selected
   // STRG + LMB down ==> Add to selection or remove if already added
   // SHIFT + LMB down ==> Rectangle Selection
   // ALT + LMB down ==> Lasso Selection

   // MMB down ==> Pan
   // MMB scroll ==> Zoom

   // STRG + RMB down ==> MarcoSelection interface

   // ALT ==> IProvinceCollection Preview
   // ALT LMB Click ==> IProvinceCollection Selection
   // ALT + STRG + LMB Click ==> IProvinceCollection add or remove from selection
   // ALT + RMB ==> Magic Wand Selection

   //( ALT + SHIFT ==> LoadProvince to GUI) LEAST PRIORITY

   // Province Selection Variables
   private static HashSet<Province> _selectedProvinces = [];
   private static HashSet<Province> _selectionPreview = [];
   private static HashSet<Province> _highlightedProvinces = [];

   private static Province _lastHoveredProvince = Province.Empty;
   private static Province _lastSelectedProvince = Province.Empty;

   // Rectangle Selection Variables
   private static Point _rectangleStartPoint = Point.Empty;
   private static Point _lastRectPoint = Point.Empty;
   private static Rectangle _rectangleSelectionRectangle = Rectangle.Empty;

   // Lasso Selection Variables
   private static List<Point> _lassoTruePoints = [];
   private static List<Point> _lassoConvertedPoints = [];
   private static Triangle _lastLassoTriangle = Triangle.Empty;
   private static HashSet<Province> _remainingProvinces = [];

   // Magic Wand Selection Variables

   // Country Selection Variables
   private static HashSet<Country> _selectedCountries = []; // Use later when multiple countries can be selected and edited at once
   private static Country _lastSelectedCountry = Country.Empty;

   // Events
   public static event EventHandler<List<Province>> OnProvinceGroupSelected = delegate { };
   public static event EventHandler<List<Province>> OnProvinceGroupDeselected = delegate { };

   public static event EventHandler<List<Country>> OnCountryGroupSelected = delegate { };
   public static event EventHandler<List<Country>> OnCountryGroupDeselected = delegate { };

   // Colors
   private static int _hoverColor = Color.FromArgb(255, 0, 255, 255).ToArgb();
   private static int _selectedColor = Color.FromArgb(255, 185, 235, 235).ToArgb();
   public static int _borderColor = Color.FromArgb(255, 0, 0, 0).ToArgb();
   private static int _previewSelectionColor = Color.FromArgb(255, 255, 0, 0).ToArgb();
   private static int _highlightColor = Color.FromArgb(255, 0, 255, 0).ToArgb();

   // Selection State
   private static SelectionType _selectionType = SelectionType.Single;

   // ------------ Getters ------------ \\
   public static HashSet<Province> GetSelectedProvinces() => _selectedProvinces;
   public static HashSet<int> GetSelectedProvinceIds() => _selectedProvinces.Select(p => p.Id).ToHashSet();
   public static int GetSelectionCount() => _selectedProvinces.Count;
   public static SelectionType GetSelectionType() => _selectionType;

   public static void Initialize()
   {
      Globals.ZoomControl.MouseDown += ZoomControl_MouseDown;
      Globals.ZoomControl.MouseMove += ZoomControl_MouseMove;
      Globals.ZoomControl.MouseUp += ZoomControl_MouseUp;
      Globals.ZoomControl.Paint += ZoomControlPaint;
   }

   // ------------ Province Selection Methods ------------ \\

   #region Selection Methods

   public static void InvertSelection()
   {
      var toAdd = Globals.Provinces.Values.Except(_selectedProvinces).ToList();
      ClearSelection();
      AddProvincesToSelection(toAdd);
   }

   public static void RedrawSelection(Province province)
   {
      MapDrawing.DrawOnMap(province, _selectedColor, PixelsOrBorders.Borders);
   }

   public static void ClearSelection()
   {
      MapDrawing.DrawOnMap(_selectedProvinces, _borderColor, PixelsOrBorders.Borders);
      OnProvinceGroupDeselected(Globals.ZoomControl, [.. _selectedProvinces]);
      _selectedProvinces.Clear();
   }

   public static void RemoveProvincesFromSelection(List<Province> provinces)
   {
      _selectedProvinces.ExceptWith(provinces);
      MapDrawing.DrawOnMap(provinces, _borderColor, PixelsOrBorders.Borders);
      OnProvinceGroupDeselected(Globals.ZoomControl, provinces);
   }

   public static void AddProvincesToSelection(List<Province> provinces, bool deselectSelected = false)
   {
      if (deselectSelected)
      {
         foreach (var province in provinces)
            AddProvinceToSelection(province, deselectSelected, false);
      }
      else
      {
         _selectedProvinces.UnionWith(provinces);
         MapDrawing.DrawOnMap(provinces, _selectedColor, PixelsOrBorders.Borders);
      }

      OnProvinceGroupSelected(Globals.ZoomControl, provinces.ToList());
   }

   public static void RemoveProvinceFromSelection(Province province, bool fireEvent = true)
   {
      InternalRemoveProvinceFromSelection(province, fireEvent);
   }

   public static void AddProvinceToSelection(Province province, bool deselectSelected = false, bool fireEvent = true)
   {
      if (deselectSelected && _selectedProvinces.Contains(province))
         InternalRemoveProvinceFromSelection(province, fireEvent);
      else
         InternalAddProvinceToSelection(province, fireEvent);
   }

   private static void InternalAddProvinceToSelection(Province province, bool fireEvent)
   {
      _selectedProvinces.Add(province);
      MapDrawing.DrawOnMap(province.Borders, _selectedColor);
      if (fireEvent)
         OnProvinceGroupSelected(Globals.ZoomControl, [province]);
   }

   private static void InternalRemoveProvinceFromSelection(Province province, bool fireEvent)
   {
      _selectedProvinces.Remove(province);
      MapDrawing.DrawOnMap(province.Borders, _borderColor);
      if (fireEvent)
         OnProvinceGroupSelected(Globals.ZoomControl, [province]);
   }

   #endregion

   // ------------ Province Highlighting Methods ------------ \\

   #region Highlighting Methods

   public static void HighlightProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _highlightColor, PixelsOrBorders.Borders);
      _highlightedProvinces.UnionWith(provinces);
   }

   public static void UnhighlightProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _borderColor, PixelsOrBorders.Borders);
      _highlightedProvinces.ExceptWith(provinces);
   }

   public static void HighlightProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _highlightColor, PixelsOrBorders.Borders);
      _highlightedProvinces.Add(province);
   }

   public static void UnhighlightProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _borderColor, PixelsOrBorders.Borders);
      _highlightedProvinces.Remove(province);
   }

   public static void ClearHighlightedProvinces()
   {
      MapDrawing.DrawOnMap(_highlightedProvinces, _borderColor, PixelsOrBorders.Borders);
      _highlightedProvinces.Clear();
   }

   #endregion

   // ------------ Selection Preview Methods ------------ \\

   #region Selection Preview Methods

   public static void PreviewProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _previewSelectionColor, PixelsOrBorders.Borders);
      _selectionPreview.UnionWith(provinces);
   }

   public static void UnPreviewProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _borderColor, PixelsOrBorders.Borders);
      _selectionPreview.ExceptWith(provinces);
   }

   public static void PreviewProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _previewSelectionColor, PixelsOrBorders.Borders);
      _selectionPreview.Add(province);
   }

   public static void UnPreviewProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _borderColor, PixelsOrBorders.Borders);
      _selectionPreview.Remove(province);
   }

   public static void ClearPreview()
   {
      MapDrawing.DrawOnMap(_selectionPreview, _borderColor, PixelsOrBorders.Borders);
      _selectionPreview.Clear();
   }

   #endregion

   // ------------ Province Hovering Methods ------------ \\

   #region Hovering Methods

   /// <summary>
   /// "Unhovers" the last hovered province and highlights the new hovered province
   /// </summary>
   /// <param name="province"></param>
   public static void HoverProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _hoverColor, PixelsOrBorders.Borders);
      _lastHoveredProvince = province;
   }

   public static void ClearHover()
   {
      if (_lastHoveredProvince == Province.Empty)
         return;
      // If the province is already selected, mark it as such again
      if (_selectedProvinces.Contains(_lastHoveredProvince))
         RedrawSelection(_lastHoveredProvince);
      else
         MapDrawing.DrawOnMap(_lastHoveredProvince, _borderColor, PixelsOrBorders.Borders);

      _lastHoveredProvince = Province.Empty;
   }

   #endregion

   // ------------ Getting Data from Map Methods ------------ \\

   #region Map Data Interactions

   public static bool GetProvinceFromMap(Point point, out Province province)
   {
      province = default!;
      return !(!Globals.ColorToProvId.TryGetValue(Globals.ZoomControl.GetColor(point), out var id) ||
             !Globals.Provinces.TryGetValue(id, out province!));

   }

   public static bool GetIProvinceCollection(Province province, out List<Province> provinces)
   {
      // TODO Implement
      throw new NotImplementedException();
   }

   #endregion



   // ------------ Event Handlers for the ZoomControl ------------ \\


   private static void ZoomControlPaint(object? sender, PaintEventArgs e)
   {
      if (_rectangleSelectionRectangle != Rectangle.Empty && _selectionType == SelectionType.Rectangle)
         e.Graphics.DrawRectangle(new(Color.White), _rectangleSelectionRectangle);
      else if (_selectionType == SelectionType.Lasso && _lassoTruePoints.Count > 2)
      {
         e.Graphics.DrawPolygon(new(Color.White), _lassoTruePoints.ToArray());
      }
   }


   private static void ZoomControl_MouseDown(object? sender, MouseEventArgs e)
   {
      switch (e.Button)
      {
         case MouseButtons.Left:
            switch (Control.ModifierKeys)
            {
               case Keys.Control | Keys.Alt:
                  // IProvinceCollection Add / rmv Selection
                  break;
               case Keys.Shift:
                  EnterRectangleSelection(e.Location);
                  break;
               case Keys.Control:
                  // Add | rmv Selection
                  AddRmvProvinceFromSelection(e.Location);
                  break;
               case Keys.Alt:
                  // Lasso Selection
                  EnterLassoSelection(e.Location);
                  break;
               default:
                  // Single Selection
                  MarkNextProvinceAsSelected(e.Location);
                  break;
            }
            break;
         case MouseButtons.Right:
            switch (Control.ModifierKeys)
            {
               case Keys.Alt:
                  // Magic Wand Selection
                  break;
               case Keys.Control:
                  // Marco Selection
                  break;
               default:
                  break;
            }
            break;
         default:
            return;
      }
   }

   private static void ZoomControl_MouseUp(object? sender, MouseEventArgs e)
   {
      if (e.Button == MouseButtons.Left)
      {
         switch (_selectionType)
         {
            case SelectionType.Rectangle:
               ExitRectangleSelection();
               break;
            case SelectionType.Lasso:
               ExitLassoSelection();
               break;
         }
      }
   }

   private static void ZoomControl_MouseMove(object? sender, MouseEventArgs e)
   {
      switch (_selectionType)
      {
         case SelectionType.Rectangle:
            RectangleMouseMove(e.Location);
            ClearHover();
            break;
         case SelectionType.Lasso:
            if (Globals.ZoomControl.isPanning)
            {
               var delta = _lassoTruePoints[^1].Subtract(e.Location);
               for (var i = 0; i < _lassoTruePoints.Count; i++)
               {
                  _lassoTruePoints[i] = _lassoTruePoints[i].Subtract(ref delta);
               }
            }
            else
            {
               LassoSelectionMove(e.Location);
               ClearHover();
            }
            break;
         default:
            HoverProvinceMove(e.Location);
            break;
      }
   }


   private static void AddRmvProvinceFromSelection(Point point)
   {
      if (!GetProvinceFromMap(point, out var prov))
         return;

      if (_selectedProvinces.Contains(prov))
         RemoveProvinceFromSelection(prov);
      else
         AddProvinceToSelection(prov);
      Globals.ZoomControl.Invalidate();
   }

   private static void MarkNextProvinceAsSelected(Point point)
   {
      if (!GetProvinceFromMap(point, out var prov))
         return;

      if (_selectedProvinces.Contains(prov) && _selectedProvinces.Count == 1)
         RemoveProvinceFromSelection(prov);
      else
      {
         ClearSelection();
         AddProvinceToSelection(prov);
      }
      Globals.ZoomControl.Invalidate();
   }

   private static void EnterRectangleSelection(Point point)
   {
      Globals.ZoomControl.CanZoom = false;
      if (GetProvinceFromMap(point, out var prov))
         PreviewProvince(prov);
      _selectionType = SelectionType.Rectangle;
      Globals.ZoomControl.Invalidate();

      _rectangleStartPoint = Globals.ZoomControl.ConvertCoordinates(point, out _);
   }

   private static void ExitRectangleSelection()
   {
      //_selectionPreview.Clear(); // No need to clear as all will be real selection
      //ClearPreview();
      AddProvincesToSelection(_selectionPreview.ToList());
      Globals.ZoomControl.Invalidate();
      _selectionPreview.Clear();
      _rectangleStartPoint = Point.Empty;
      _rectangleSelectionRectangle = Rectangle.Empty;
      _selectionType = SelectionType.Single;
      Globals.ZoomControl.CanZoom = true;
   }

   private static void RectangleMouseMove(Point eventPoint)
   {
      if (_rectangleStartPoint == Point.Empty)
         return;

      // Calculate on Paint and OnZoom
      _rectangleSelectionRectangle = Geometry.GetBounds([Globals.ZoomControl.ReverseCoordinate(_rectangleStartPoint), eventPoint]);
      var point = Globals.ZoomControl.ConvertCoordinates(eventPoint, out _);
      // Precompute the bounds
      var currentRect = Geometry.GetBounds([_rectangleStartPoint, point]);
      var lastRectBounds = Geometry.GetBounds([_rectangleStartPoint, _lastRectPoint]);
      var (intersection, toAddRect, toRemoveRect) = Geometry.GetDeltaSelectionRectangle(currentRect, lastRectBounds);
      var toRemove = Geometry.GetProvincesInRectanglesWithPixelCheck(toRemoveRect);
      var toAdd = Geometry.GetProvincesInRectanglesWithPixelCheck(toAddRect);

      _lastRectPoint = point;

      var intersectionProvinces = Geometry.GetProvincesInRectangleWithPixelCheck(intersection, toRemove);
      toRemove = toRemove.Except(intersectionProvinces).ToList();

      foreach (var prov in toRemove)
      {
         RemoveProvinceFromPreview(prov);
      }

      foreach (var prov in toAdd)
      {
         PreviewProvince(prov);
      }

      Globals.ZoomControl.Invalidate();
   }

   private static void HoverProvinceMove(Point point)
   {
      var valid = GetProvinceFromMap(point, out var province);
      if (valid && _lastHoveredProvince == province)
         return;
      ClearHover();
      if (!valid)
      {
         Globals.ZoomControl.Invalidate();
         return;
      }

      // Update the hovered province and redraw with the new selection.
      HoverProvince(province);

      Globals.ZoomControl.Invalidate();
   }
   private static void LassoSelectionMove(Point point)
   {
      var sw = new Stopwatch();
      sw.Start();
      var convertedPoint = Globals.ZoomControl.ConvertCoordinates(point, out _);

      if (_lassoTruePoints.Count > 3)
         _lastLassoTriangle.RollPoints(convertedPoint);
      else if (_lassoTruePoints.Count == 3)
         _lastLassoTriangle = new(_lassoConvertedPoints[0], _lassoConvertedPoints[1], _lassoConvertedPoints[2]);
      else
      {
         _lassoTruePoints.Add(point);
         _lassoConvertedPoints.Add(convertedPoint);
         Globals.ZoomControl.Invalidate();
         return;
      }
      _lassoTruePoints.Add(point);
      _lassoConvertedPoints.Add(convertedPoint);

      var provs = Geometry.testLineWithPixel(_lassoConvertedPoints[^2], _lassoConvertedPoints[^1], _remainingProvinces);

      foreach (var prov in provs)
      {
         PreviewProvince(prov);
         _remainingProvinces.Remove(prov);
      }

      provs = Geometry.testLineWithPixel(_lassoConvertedPoints[^1], _lassoConvertedPoints[0], _remainingProvinces);

      foreach (var prov in provs)
         PreviewProvince(prov);

      provs = Geometry.testLineWithPixel(_lassoConvertedPoints[^2], _lassoConvertedPoints[0], _remainingProvinces.Except(provs).ToList());

      foreach (var prov in provs)
      {
         if (Geometry.IsPointInPolygon(prov.Borders[0], _lassoConvertedPoints))
            PreviewProvince(prov);
         else
            RemoveProvinceFromPreview(prov);
      }

      provs = Geometry.GetFullyContainedProvinces(ref _lastLassoTriangle, _remainingProvinces);

      foreach (var prov in provs)
      {
         if (_selectionPreview.Contains(prov))
            PreviewProvince(prov);
         else
            RemoveProvinceFromPreview(prov);
      }

      /*
      var (fully, intersects) = Geometry.GetProvincesInTriangleWithPixelCheck(ref _lastLassoTriangle, [..Globals.Provinces.Values]);


      //Debug.WriteLine($"Intersects: {intersects.Count} | Fully: {fully.Count}\n");

      foreach (var prov in fully)
      {
         if (_selectionPreview.Contains(prov))
            RemoveProvinceFromPreview(prov);
         else
            PreviewProvince(prov);
      }
      foreach (var prov in intersects)
      {
         if (_selectionPreview.Contains(prov))
         {
            if (Geometry.IsRectangleInPolygon(_lassoConvertedPoints, prov.Bounds) &&
              Geometry.IsPointInPolygon(_lassoConvertedPoints, prov.Borders))
            {
               RemoveProvinceFromPreview(prov);
            }
            else
            {
               Globals.ZoomControl.Invalidate();
               return;
            }
         }
         else
            PreviewProvince(prov);
      }*/
      Globals.ZoomControl.Invalidate();
      sw.Stop();
      Debug.WriteLine($"CHecks: {sw.ElapsedTicks} nano seconds");
   }
   private static void ExitLassoSelection()
   {
      AddProvincesToSelection([.. _selectionPreview]);
      _selectionType = SelectionType.Single;
      _selectionPreview.Clear();
      _lassoTruePoints.Clear();
      _lassoConvertedPoints.Clear();
      Globals.ZoomControl.CanZoom = true;
      _lastLassoTriangle = Triangle.Empty;
   }

   private static void EnterLassoSelection(Point point)
   {
      Globals.ZoomControl.CanZoom = false;
      _remainingProvinces = [.. Globals.Provinces.Values];
      _selectionType = SelectionType.Lasso;
      _lassoTruePoints.Add(point);
      _lassoConvertedPoints.Add(Globals.ZoomControl.ConvertCoordinates(point, out _));
   }


   public static void RemoveProvinceFromPreview(Province province)
   {
      if (_selectedProvinces.Contains(province))
      {
         _selectionPreview.Remove(province);
         RedrawSelection(province); // If the province is already selected, mark it as such again
      }
      else
      {
         UnPreviewProvince(province);
      }
   }
}