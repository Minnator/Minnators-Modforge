using System.Diagnostics;
using Windows.UI.ApplicationSettings;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Forms;
using Editor.Forms.AdvancedSelections;
using Newtonsoft.Json.Linq;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Helper;

// Could be expanded further with more features
public enum SelectionType
{
   Province,
   Area,
   Region,
   Country
}
public enum SelectionToolType
{
   Lasso,     // Using the lasso tool to select provinces
   Rectangle, // Using the rectangle tool to select provinces
   MagicWand, // Using the Magic Wand tool to select provinces using flood fill
   Single,    // Single province selection
   Collection // Collection of provinces
}

public static class Selection
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

   // [x] LMB down ==> Select only one province or deselect if already selected
   // [x] STRG + LMB down ==> Add to selection or remove if already added
   // [x] SHIFT + LMB down ==> Rectangle Selection
   // [x] ALT + LMB down ==> Lasso Selection

   // [x] MMB down ==> Pan
   // [x] MMB scroll ==> Zoom

   // [x] STRG + RMB down ==> MarcoSelection interface

   // [x] STRG + ALT ==> IProvinceCollection Preview
   // [x] ALT + STRG + LMB Click ==> IProvinceCollection add or remove from selection
   // [x] ALT + SHIFT + LMB ==> Magic Wand Selection

   //( ALT + SHIFT ==> LoadProvince to GUI) LEAST PRIORITY

   // Province Selection Variables
   private static HashSet<Province> _selectedProvinces = [];
   private static HashSet<Province> _selectionPreview = [];
   private static HashSet<Province> _highlightedProvinces = [];

   public static Province LastHoveredProvince = Province.Empty;
   private static HashSet<Province> _hoveredCollection = [];

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

   // Tooltips
   public static bool ShowToolTip = true;
   private static ToolTip MapToolTip = new();

   // Country Selection Variables
   // Selected Country TODO ADD THIS FEATURE
   private static HashSet<Country> _selectedCountries = []; // Use later when multiple countries can be selected and edited at once
   private static Country _selectedCountry = Country.Empty; 
   public static Country SelectedCountry
   {
      get => _selectedCountry;
      private set => _selectedCountry = value;
   }
   public static List<Province> SelectionCoresAndClaims { get; set; } = [];

   // Events
   public static event EventHandler<List<Province>> OnProvinceGroupSelected = delegate { };
   public static event EventHandler<List<Province>> OnProvinceGroupDeselected = delegate { };

   public static event EventHandler<Country> OnCountrySelected = delegate { };
   public static event EventHandler<Country> OnCountryDeselected = delegate { };

   // Colors
   private static int _hoverColor = Color.FromArgb(255, 0, 255, 255).ToArgb();
   private static int _selectedColor = Color.FromArgb(255, 185, 235, 235).ToArgb();
   public static int _borderColor = Color.FromArgb(255, 0, 0, 0).ToArgb();
   private static int _previewSelectionColor = Color.FromArgb(255, 255, 0, 0).ToArgb();
   private static int _highlightColor = Color.FromArgb(255, 0, 255, 0).ToArgb();

   // Selection State
   private static SelectionToolType _selectionToolType = SelectionToolType.Single;
   private static SelectionType _selectionType = SelectionType.Province;
   private static ProvAttrGet _mwComparisionAttribute = ProvAttrGet.id;
   private static ProvAttrType _mwAttributeType = ProvAttrType.String;

   // ------------ Getters ------------ \\
   public static List<Province> GetSelectedProvinces => _selectedProvinces.ToList();
   public static int[] GetSelectedProvincesIds => _selectedProvinces.Select(p => p.Id).ToArray();
   public static List<Province> SelectionPreview => [.._selectionPreview];
   public static int Count => _selectedProvinces.Count;
   public static SelectionToolType GetSelectionType() => _selectionToolType;

   public static void Initialize()
   {
      Globals.ZoomControl.MouseDown += ZoomControl_MouseDown;
      Globals.ZoomControl.MouseMove += ZoomControl_MouseMove;
      Globals.ZoomControl.MouseUp += ZoomControl_MouseUp;
      Globals.ZoomControl.MouseUp += AdditionalFeatures_MouseUp;
      Globals.ZoomControl.Paint += ZoomControlPaint;

      Globals.ZoomControl.ContextMenuStrip = SelectionMenuBuilder.GetSelectionMenu();
      Globals.MapWindow.SelectionTypeBox.SelectedIndexChanged += SelectionTypeBox_SelectedIndexChanged;

      Globals.MapWindow.MWAttirbuteCombobox.SelectedIndexChanged += OnSelectedAttributeIndexChanged;
   }

   // ------------ Province Selection Methods ------------ \\

   #region Selection Methods

   public static void InvertSelection()
   {
      var toAdd = Globals.Provinces.Except(_selectedProvinces).ToList();
      ClearSelection();
      AddProvincesToSelection(toAdd);
   }

   public static void RePaintSelection()
   {
      foreach (var province in _selectedProvinces)
         RedrawSelection(province);
   }

   public static void RedrawSelection(Province province)
   {
      MapDrawing.DrawOnMap(province, _selectedColor, Globals.ZoomControl, PixelsOrBorders.Borders);
   }

   public static void ClearSelection()
   {
      MapDrawing.DrawOnMap(_selectedProvinces, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      OnProvinceGroupDeselected(Globals.ZoomControl, [.. _selectedProvinces]);
      _selectedProvinces.Clear();
   }

   public static void RemoveProvincesFromSelection(ICollection<Province> provinces)
   {
      _selectedProvinces.ExceptWith(provinces);
      MapDrawing.DrawOnMap(provinces, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      OnProvinceGroupDeselected(Globals.ZoomControl, provinces.ToList());
   }
   
   public static void AddProvincesToSelection(ICollection<Province> provinces, bool deselectSelected = false)
   {
      if (deselectSelected)
      {
         foreach (var province in provinces)
            AddProvinceToSelection(province, deselectSelected, false);
      }
      else
      {
         _selectedProvinces.UnionWith(provinces);
         MapDrawing.DrawOnMap(provinces, _selectedColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      }

      OnProvinceGroupSelected(Globals.ZoomControl, provinces.ToList());
   }

   public static void AddOrRemoveAllFromSelection(ICollection<Province> provinces)
   {
      var containsAny = false;
      foreach (var province in provinces)
      {
         containsAny = _selectedProvinces.Contains(province);
         if (containsAny)
            break;
      }

      if (containsAny)
         RemoveProvincesFromSelection(provinces);
      else
         AddProvincesToSelection(provinces);
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
      MapDrawing.DrawOnMap(province.Borders, _selectedColor, Globals.ZoomControl);
      if (fireEvent)
         OnProvinceGroupSelected(Globals.ZoomControl, [province]);
   }

   private static void InternalRemoveProvinceFromSelection(Province province, bool fireEvent)
   {
      _selectedProvinces.Remove(province);
      MapDrawing.DrawOnMap(province.Borders, _borderColor, Globals.ZoomControl);
      if (fireEvent)
         OnProvinceGroupSelected(Globals.ZoomControl, [province]);
   }

   public static void FocusSelection()
   {
      Globals.ZoomControl.FocusOn(Geometry.GetBounds(_selectedProvinces.ToList()));
   }

   private static void SelectCountry(Province province)
   {
      if (province == Province.Empty || !Globals.Countries.TryGetValue(province.Owner, out var country) || SelectedCountry == country)
         return;

      if (Globals.MapModeManager.CurrentMapMode is DiplomaticMapMode mapMode)
      {
         // Set flag to remove the old claims and cores
         mapMode.ClearPreviousCoresClaims = true;
         mapMode.Update([.. SelectionCoresAndClaims]);
         mapMode.ClearPreviousCoresClaims = false;
      }
      SelectedCountry = country;
      if (SelectedCountry != Country.Empty && Count == 1)
      {
         if (Globals.MapModeManager.CurrentMapMode is DiplomaticMapMode)
            Globals.MapModeManager.CurrentMapMode.Update(province);
         OnCountrySelected?.Invoke(Globals.ZoomControl, SelectedCountry);
      }
   }

   private static void RemoveCountrySelection()
   {
      SelectedCountry = Country.Empty;
      OnCountryDeselected?.Invoke(Globals.ZoomControl, SelectedCountry);
   }

   #endregion

   // ------------ Province Highlighting Methods ------------ \\

   #region Highlighting Methods

   public static void HighlightProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _highlightColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _highlightedProvinces.UnionWith(provinces);
   }

   public static void UnhighlightProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _highlightedProvinces.ExceptWith(provinces);
   }

   public static void HighlightProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _highlightColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _highlightedProvinces.Add(province);
   }

   public static void UnhighlightProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _highlightedProvinces.Remove(province);
   }

   public static void ClearHighlightedProvinces()
   {
      MapDrawing.DrawOnMap(_highlightedProvinces, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _highlightedProvinces.Clear();
   }

   #endregion

   // ------------ Selection Preview Methods ------------ \\

   #region Selection Preview Methods

   public static void PreviewProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _previewSelectionColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _selectionPreview.UnionWith(provinces);
   }

   public static void UnPreviewProvinces(List<Province> provinces)
   {
      MapDrawing.DrawOnMap(provinces, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _selectionPreview.ExceptWith(provinces);
   }

   public static void PreviewProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _previewSelectionColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _selectionPreview.Add(province);
   }

   public static void UnPreviewProvince(Province province)
   {
      MapDrawing.DrawOnMap(province, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _selectionPreview.Remove(province);
   }

   public static void ClearPreview()
   {
      MapDrawing.DrawOnMap(_selectionPreview, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
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
      MapDrawing.DrawOnMap(province, _hoverColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      LastHoveredProvince = province;
   }

   public static void HoverCollection(ICollection<Province> provinces, Province province)
   {
      if (provinces.Count == 0)
         return;

      ClearHoverCollection();
      MapDrawing.DrawOnMap(provinces, _hoverColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _hoveredCollection.UnionWith(provinces);
      LastHoveredProvince = province;
   }

   private static void ClearHoverCollection()
   {
      if (_hoveredCollection.Count == 0)
         return;

      MapDrawing.DrawOnMap(_hoveredCollection, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      RePaintSelection();
      _hoveredCollection.Clear();
   }

   public static void ClearHover()
   {
      if (LastHoveredProvince == Province.Empty)
         return;
      // If the province is already selected, mark it as such again
      if (_selectedProvinces.Contains(LastHoveredProvince))
         RedrawSelection(LastHoveredProvince);
      else
         MapDrawing.DrawOnMap(LastHoveredProvince, _borderColor, Globals.ZoomControl, PixelsOrBorders.Borders);

      LastHoveredProvince = Province.Empty;
      if (_hoveredCollection.Count > 0)
         ClearHoverCollection();
   }

   public static void HoverProvinces(ICollection<Province> provinces)
   {
      if (provinces.Count == 0)
         return;

      ClearHover();
      MapDrawing.DrawOnMap(provinces, _hoverColor, Globals.ZoomControl, PixelsOrBorders.Borders);
      _hoveredCollection.UnionWith(provinces);
   }

   #endregion

   // ------------ Getting Data from Map Methods ------------ \\

   #region Map Data Interactions

   public static bool GetProvinceFromMap(Point point, out Province province)
   {
      // TODO improve Performance
      province = Province.Empty;
      var coords = Globals.ZoomControl.ConvertCoordinates(point, out var isValid);
      if (!isValid)
         return false;

      if (Globals.ColorToProvId.TryGetValue(Globals.MapModeManager.IdMapMode.Bitmap.GetPixel(coords.X, coords.Y).ToArgb(), out province!) 
            && province != Province.Empty)
      {
         return true;
      }

      return false;

   }

   #endregion
   
   // ------------ Event Handlers for the ZoomControl ------------ \\

   #region Event Handlers
   private static void ZoomControlPaint(object? sender, PaintEventArgs e)
   {
      if (_rectangleSelectionRectangle != Rectangle.Empty && _selectionToolType == SelectionToolType.Rectangle)
         e.Graphics.DrawRectangle(new(Color.White), _rectangleSelectionRectangle);
      else if (_selectionToolType == SelectionToolType.Lasso && _lassoTruePoints.Count > 2)
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
                  SelectCollection(e.Location);
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
         if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift && (Control.ModifierKeys & Keys.Alt) != 0)
         {
            OnMagicWandSelection(e.Location);
            goto SetCount;
         }
         switch (_selectionToolType)
         {
            case SelectionToolType.Rectangle:
               ExitRectangleSelection();
               break;
            case SelectionToolType.Lasso:
               ExitLassoSelection();
               break;
         }
      }
      SetCount:
      Globals.MapWindow.SetSelectedProvinceSum(Count);
      // ------------------------------ Province Idle Loading ------------------------------
      if (Globals.ProvinceEditingStatus == ProvinceEditingStatus.Selection
          || Globals.ProvinceEditingStatus == ProvinceEditingStatus.PreviewUntilSelection && Count > 1)
      {
         Globals.MapWindow.ProvinceClick();
      }
   }

   private static void ZoomControl_MouseMove(object? sender, MouseEventArgs e)
   {
      if ((Control.ModifierKeys & Keys.Control) != 0 && (Control.ModifierKeys & Keys.Alt) != 0)
         HoverCollection(e.Location);
      if ((Control.ModifierKeys & Keys.Alt) != 0 && (Control.ModifierKeys & Keys.Shift) != 0)
      {
         MagicWandSelectionMove(e.Location);
         return;
      }

      switch (_selectionToolType)
      {
         case SelectionToolType.Rectangle:
            RectangleMouseMove(e.Location);
            ClearHover();
            break;
         case SelectionToolType.Lasso:
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

   #endregion

   #region SingleSelection

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
      {
         RemoveProvinceFromSelection(prov);
         RemoveCountrySelection();
      }
      else
      {
         ClearSelection();
         AddProvinceToSelection(prov);
         SelectCountry(prov);
      }
      Globals.ZoomControl.Invalidate();
   }

   #endregion

   #region Rectangle Selection

   private static void EnterRectangleSelection(Point point)
   {
      Globals.ZoomControl.CanZoom = false;
      if (GetProvinceFromMap(point, out var prov))
         PreviewProvince(prov);
      _selectionToolType = SelectionToolType.Rectangle;
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
      _selectionToolType = SelectionToolType.Single;
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



   #endregion

   #region Lasso Selection

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

      Globals.ZoomControl.Invalidate();
      sw.Stop();
      Debug.WriteLine($"CHecks: {sw.ElapsedTicks} nano seconds");
   }
   private static void ExitLassoSelection()
   {
      AddProvincesToSelection([.. _selectionPreview]);
      _selectionToolType = SelectionToolType.Single;
      _selectionPreview.Clear();
      _lassoTruePoints.Clear();
      _lassoConvertedPoints.Clear();
      Globals.ZoomControl.CanZoom = true;
      _lastLassoTriangle = Triangle.Empty;
   }

   private static void EnterLassoSelection(Point point)
   {
      Globals.ZoomControl.CanZoom = false;
      _remainingProvinces = [.. Globals.Provinces];
      _selectionToolType = SelectionToolType.Lasso;
      _lassoTruePoints.Add(point);
      _lassoConvertedPoints.Add(Globals.ZoomControl.ConvertCoordinates(point, out _));
   }


   #endregion

   #region Collection Selection
   public static void SelectCollection(Point point)
   {
      if (!GetProvinceFromMap(point, out var province))
         return;

      AddOrRemoveAllFromSelection(GetProvincesFromCollection(province, _selectionType));

      Globals.ZoomControl.Invalidate();
   }

   public static void HoverCollection(Point point)
   {
      if (_selectionType == SelectionType.Province || !GetProvinceFromMap(point, out var province))
         return;

      HoverCollection(GetProvincesFromCollection(province, _selectionType), province);
      Globals.ZoomControl.Invalidate();
   }

   private static ICollection<Province> GetProvincesFromCollection(Province province, SelectionType selectionType)
   {
      switch (selectionType)
      {
         case SelectionType.Province:
            return [province];
         case SelectionType.Area:
            if (Globals.Areas.TryGetValue(province.Area, out var area))
               return area.Provinces;
            break;
         case SelectionType.Region:
            if (Globals.Areas.TryGetValue(province.Area, out var area2))
               if (Globals.Regions.TryGetValue(area2.Region, out var region))
                  return region.GetProvinces();
            break;
         case SelectionType.Country:
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return country.GetProvinces();
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }

      return [];
   }

   #endregion

   #region MagicWand Selection

   private static void OnMagicWandSelection(Point point)
   {
      if (!GetProvinceFromMap(point, out _))
         return;

      AddProvincesToSelection(_hoveredCollection);
      _hoveredCollection.Clear();
      Globals.ZoomControl.Invalidate();
   }

   private static void MagicWandSelectionMove(Point point)
   {
      if (!GetProvinceFromMap(point, out var province) || LastHoveredProvince == province)
         return;

      ClearHighlightedProvinces();
      var provinces = GetProvincesForMagicWand(province);
      HoverProvinces(provinces);
      LastHoveredProvince = province;

      Globals.ZoomControl.Invalidate();
   }

   private static List<Province> GetProvincesForMagicWand(Province province)
   {
      
      var provinces = new List<Province>();
      var queue = new Queue<Province>();
      var visited = new HashSet<Province>(); // Track visited provinces
      queue.Enqueue(province);
      visited.Add(province);
      
      var originalValue = province.GetAttribute(_mwComparisionAttribute);
      var tolerance = Globals.MapWindow.MagicWandTolerance.Value;

      while (queue.Count > 0)
      {
         var current = queue.Dequeue();
         provinces.Add(current);

         foreach (var neighbour in Globals.AdjacentProvinces[current])
         {
            if (visited.Contains(neighbour) || Globals.NonLandProvinces.Contains(neighbour))
               continue;

            var neighborValue = neighbour.GetAttribute(_mwComparisionAttribute);
            bool shouldEnqueue = false;

            switch (_mwAttributeType)
            {
               case ProvAttrType.Int:
                  shouldEnqueue = Math.Abs((int)originalValue - (int)neighborValue) <= (int)tolerance;
                  break;
               case ProvAttrType.Float:
                  shouldEnqueue = Math.Abs((float)originalValue - (float)neighborValue) <= (float)tolerance;
                  break;
               case ProvAttrType.String:
               case ProvAttrType.Bool:
               case ProvAttrType.Tag:
                  shouldEnqueue = originalValue.Equals(neighborValue);
                  break;
               case ProvAttrType.List:
                  break;
            }

            if (shouldEnqueue)
            {
               queue.Enqueue(neighbour);
               visited.Add(neighbour);
            }
         }
      }

      return provinces;
   }



   private static void OnSelectedAttributeIndexChanged(object? sender, EventArgs e)
   {
      if (sender is not ComboBox comboBox || !Enum.TryParse(comboBox.SelectedItem?.ToString(), out _mwComparisionAttribute))
         return;

      // Enable the tolerance if the type is float or int
      _mwAttributeType = GetAttributeType(_mwComparisionAttribute);
      Globals.MapWindow.MagicWandTolerance.Enabled = _mwAttributeType is ProvAttrType.Float or ProvAttrType.Int;
   }

   #endregion

   private static void HoverProvinceMove(Point point)
   {
      var valid = GetProvinceFromMap(point, out var province);
      if (valid && LastHoveredProvince == province)
         return;
      ClearHover();
      if (!valid)
      {
         Globals.ZoomControl.Invalidate();
         return;
      }

      // Update the hovered province and redraw with the new selection.
      HoverProvince(province);

      if (ShowToolTip)
         MapToolTip.SetToolTip(Globals.ZoomControl, ToolTipBuilder.BuildToolTip(Globals.ToolTipText, LastHoveredProvince));
      Globals.MapWindow.UpdateHoveredInfo(province);
      Globals.MapWindow.SetEditingMode();
      Globals.ZoomControl.Invalidate();
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


   // ----------- TODO OVERWORK ------------ \\
   /// <summary>
   /// Returns true if the attribute is the same across all selected provinces
   /// </summary>
   /// <param name="attribute"></param>
   /// <param name="result"></param>
   /// <returns></returns>
   public static bool GetSharedAttribute(ProvAttrGet attribute, out object? result)
   {
      result = null;

      foreach (var province in _selectedProvinces)
      {
         var value = province.GetAttribute(attribute);
         if (result == null)
            result = value!;
         else if (!result.Equals(value))
         {
            result = null;
            return false;
         }
      }
      return result != null;
   }

   // ----------------- Additional Features ----------------- \\

   private static void AdditionalFeatures_MouseUp(object? sender, MouseEventArgs e)
   {
      if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Control)
      {
         if (Globals.AdvancedSelectionsForm == null || Globals.AdvancedSelectionsForm.IsDisposed)
         {
            Globals.AdvancedSelectionsForm = new();
            Globals.AdvancedSelectionsForm.Show();
         }
         else
            Globals.AdvancedSelectionsForm.BringToFront();

         Globals.AdvancedSelectionsForm.Location = Globals.ZoomControl.PointToScreen(e.Location);
      }
   }

   // SelectionTypeBox SelectedIndexChanged
   public static void SelectionTypeBox_SelectedIndexChanged(object? sender, EventArgs e)
   {
      if (sender is ComboBox comboBox)
      {
         if (Enum.TryParse(comboBox.SelectedItem?.ToString(), out SelectionType selectionType))
         {
            switch (selectionType)
            {
               case SelectionType.Province:
                  _selectionToolType = SelectionToolType.Single;
                  break;
               case SelectionType.Area:
                  _selectionToolType = SelectionToolType.Collection;
                  break;
               case SelectionType.Region:
                  _selectionToolType = SelectionToolType.Collection;
                  break;
               case SelectionType.Country:
                  _selectionToolType = SelectionToolType.Collection;
                  break;
            }

            _selectionType = selectionType;
            ClearHoverCollection();
         }
      }
   }

}