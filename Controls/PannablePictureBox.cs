using System.Diagnostics;
using System.Drawing.Imaging;
using Editor.Commands;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms;
using Editor.Forms.AdvancedSelections;
using Editor.Helper;

namespace Editor.Controls;
#nullable enable


public sealed class PannablePictureBox : PictureBox
{
   public bool IsPainting; // Prevents double locking of bitmaps when painting
   public event EventHandler? ImageChanged;
   private AdvancedSelectionsForm? _advancedSelectionsForm;

   // ------------------------------ ToolTip ------------------------------
   public ToolTip MapToolTip = new(); // Contains the tooltip for the map
   public bool ShowToolTip { get; set; } = true; // If true, the tooltip will be shown


   // ------------------------------ Province Selection ------------------------------
   public int LastInvalidatedProvince = -1; // Contains the invalidated province from the last MouseMove event to clear it

   // ------------------------------ Image Layers ------------------------------
   //Bitmaps from bottom to top render order: Image, Overlay, SelectionOverlay
   public new Bitmap Image
   {
      get => _image;
      set
      {
         if (_image != value)
         {
            _image = value;
            // Crate transparent overlay and selection bitmaps
            if (_image == null!) 
               return;
            Overlay = new (_image.Width, _image.Height, PixelFormat.Format32bppArgb);
            SelectionOverlay = new Bitmap(_image.Width, _image.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(Overlay);
            g.Clear(Color.Transparent);
            using var g2 = Graphics.FromImage(SelectionOverlay);
            g2.Clear(Color.Transparent);
            OnImageChanged(EventArgs.Empty);
         }
      }
   }
   private Bitmap _image = null!;
   public Bitmap Overlay = null!;
   public Bitmap SelectionOverlay = null!;

   // ------------------------------ Panning ------------------------------
   private readonly Panel _parentPanel; // Reference to the panel which contains the PictureBox to scroll it
   private readonly MapWindow _mapWindow; // Reference to the main window to update the selected province count
   private Point _startingPoint = Point.Empty;
   private Point _movingPoint = Point.Empty;
   private bool _panning;
   private bool AllowPanning { get; set; } = true; // If true, the user can pan the map by holding the middle mouse button

   public PannablePictureBox(ref Panel parentPanel, MapWindow mapWindow)
   {
      MouseDown += PictureBox_MouseDown!;
      MouseMove += PictureBox1_MouseMove!;
      MouseUp += PictureBox1_MouseUp!;
      MouseClick += OnMouseClick_Click!;

      _parentPanel = parentPanel;
      _mapWindow = mapWindow;


      ContextMenuStrip = SelectionMenuBuilder.GetSelectionMenu();
   }
   private void OnImageChanged(EventArgs e)
   {
      ImageChanged?.Invoke(this, e);
      Width = Image.Width;
      Height = Image.Height;
   }

   public void FocusOn(Point point)
   {
      _parentPanel.AutoScrollPosition = new (point.X - _parentPanel.Width / 2, point.Y - _parentPanel.Height / 2);
   }

   public void FocusOn(List<int> ids)
   {
      List<Province> provinces = [];
      foreach (var id in ids)
         if (Globals.Provinces.TryGetValue(id, out var province))
            provinces.Add(province);
      FocusOn(provinces);
   }

   public void FocusOn(List<Province> provinces)
   {
      List<Point> centers = [];
      foreach (var province in provinces)
         centers.Add(province.Center);

      var bounds = Geometry.GetBounds([..centers]);
      FocusOn(new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2));
   }

   private void OnMouseClick_Click (object sender, MouseEventArgs e)
   {
      // ------------------------------ Panning ------------------------------
      // We don't want to mess with the selection if the user is panning
      if (e.Button is MouseButtons.Middle)
         return;


      // ------------------------------ Province Selection ------------------------------
      if (!Globals.MapModeManager.GetProvince(e.Location, out var province)) 
         return;
      
      //check if ctrl is pressed
      if (ModifierKeys == Keys.Control && Globals.Selection.State == SelectionState.Single) 
         Globals.HistoryManager.AddCommand(new CAddSingleSelection(province.Id), CommandHistoryType.SimpleSelection);
      else if (ModifierKeys != Keys.Shift && Globals.Selection.State == SelectionState.Single && e.Button is not MouseButtons.Right)
         Globals.HistoryManager.AddCommand(new CSelectionMarkNext(province.Id, this), CommandHistoryType.SimpleSelection);

      if (Globals.Selection.State == SelectionState.MagicWand)
      {
         // TODO create command for magic wand and add it to the history manager HERE
      }
      
      _mapWindow.SetSelectedProvinceSum(Globals.Selection.SelectedProvinces.Count);
   }
   
   private void PictureBox_MouseDown(object sender, MouseEventArgs e)
   {
      // ------------------------------ Province Selection ------------------------------
      switch (ModifierKeys)
      {
         case Keys.Alt:
            Globals.Selection.State = SelectionState.Lasso;
            return;
         case Keys.Shift:
            Globals.Selection.EnterRectangleSelection(e.Location);
            return;
      }

      _mapWindow.SetSelectedProvinceSum(Globals.Selection.SelectedProvinces.Count);
      
      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
      {
         if (e.Button == MouseButtons.Middle)
         {
            _panning = true;
            _startingPoint = e.Location;
            Cursor = Cursors.Hand; // Optional: change cursor to hand while panning
         }
      }
   }

   private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
   { 
      // ------------------------------ Province Selection ------------------------------
      
      if (ModifierKeys == Keys.Alt)
      {
         Globals.Selection.ExitLassoSelection();
         Globals.HistoryManager.AddCommand(new CLassoSelection(), CommandHistoryType.ComplexSelection);
         return;
      }

      if (Globals.Selection.State == SelectionState.Rectangle)
      {
         Globals.Selection.ExitRectangleSelection();
      }
      
      // ------------------------------ Advanced Selections Menu ------------------------------
      if (e.Button == MouseButtons.Right && ModifierKeys == Keys.Control)
      {
         if (_advancedSelectionsForm == null || _advancedSelectionsForm.IsDisposed)
         {
            _advancedSelectionsForm = new (this);
            _advancedSelectionsForm.Show();
         }
         else
            _advancedSelectionsForm.BringToFront();
         _advancedSelectionsForm.Location = new Point(MousePosition.X, MousePosition.Y);
      }

      _mapWindow.SetSelectedProvinceSum(Globals.Selection.SelectedProvinces.Count);

      // ------------------------------ Province Idle Loading ------------------------------
      if (Globals.ProvinceEditingStatus == ProvinceEditingStatus.Selection
         || Globals.ProvinceEditingStatus == ProvinceEditingStatus.PreviewUntilSelection && Globals.Selection.SelectedProvinces.Count > 1)
      {
         Globals.MapWindow.ProvinceClick();
      }

      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
      {
         if (e.Button == MouseButtons.Middle)
         {
            _panning = false;
            Cursor = Globals.Selection.State switch
            {
               SelectionState.MagicWand => Cursors.Cross,
               _ => Cursors.Default
            };
         }
      }
   }

   private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
   {
      if (Globals.State == State.Loading || Globals.State == State.Initializing)
         return;
      // ------------------------------ Out of Bounds Check ------------------------------
      if (Image == null! || e.X < 0 || e.Y < 0 || e.X >= Image.Width || e.Y >= Image.Height)
         return;
      
      // ------------------------------ Province Selection ------------------------------
      if (ModifierKeys == Keys.Alt && Globals.Selection.State == SelectionState.Lasso)
      {
         Globals.Selection.LassoSelection.Add(e.Location);
         Globals.Selection.PreviewAllInPolygon();
         Invalidate(Geometry.GetBounds([.. Globals.Selection.LassoSelection]));
      }

      if (ModifierKeys == Keys.Shift && Globals.Selection.State == SelectionState.Rectangle)
         Globals.Selection.PreviewAllInRectangle(e.Location);

      _mapWindow.SetSelectedProvinceSum(Globals.Selection.SelectedProvinces.Count);

      // ------------------------------ Province Highlighting ------------------------------
         var sw = Stopwatch.StartNew();
      if (Globals.MapModeManager.GetProvince(e.Location, out var province) && province.Id != LastInvalidatedProvince)
      {
         if (LastInvalidatedProvince != -1) 
            Invalidate(MapDrawHelper.DrawProvinceBorder(LastInvalidatedProvince, Color.Transparent, Overlay));
         Invalidate(MapDrawHelper.DrawProvinceBorder(province.Id, Color.Aqua, Overlay));

         Debug.WriteLine($"Highlighting took {sw.ElapsedTicks}ticks");

         if (((ModifierKeys & Keys.Control) != 0 && (ModifierKeys & Keys.Alt) != 0) 
             || Globals.ProvinceEditingStatus == ProvinceEditingStatus.PreviewOnly
             || Globals.ProvinceEditingStatus == ProvinceEditingStatus.PreviewUntilSelection && Globals.Selection.SelectedProvinces.Count <= 1)
         {
            if (LastInvalidatedProvince != province.Id)
               province.LoadToGui();
         }

         _mapWindow.SetEditingMode();

         LastInvalidatedProvince = province.Id;

         // Update the tooltip
         if (ShowToolTip)
            MapToolTip.SetToolTip(this, ToolTipBuilder.BuildToolTip(Globals.ToolTipText, province.Id));

         Globals.MapWindow.UpdateHoveredInfo(province);
      }

      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
      {
         if (_panning)
         {
            _movingPoint = new Point(-(_parentPanel.AutoScrollPosition.X + e.X - _startingPoint.X),
               -(_parentPanel.AutoScrollPosition.Y + e.Y - _startingPoint.Y));
            _parentPanel.AutoScrollPosition = _movingPoint;
         }
      }
   }

   protected override void OnPaint(PaintEventArgs pe)
   {
      base.OnPaint(pe);
      if (IsPainting)
         return;
      IsPainting = true;
      // Draw the layers to the screen
      if (Image != null!) 
         pe.Graphics.DrawImage(Image, 0, 0, Image.Width, Image.Height);
      pe.Graphics.DrawImage(SelectionOverlay, 0, 0, SelectionOverlay.Width, SelectionOverlay.Height);
      pe.Graphics.DrawImage(Overlay, 0, 0, Overlay.Width, Overlay.Height);

      // Draw the selection lasso
      if (Globals.Selection is { State: SelectionState.Lasso, LassoSelection.Count: > 2 })
      {
         pe.Graphics.DrawPolygon(new(Globals.Selection.SelectionOutlineColor, 1), Globals.Selection.LassoSelection.ToArray());
      }
      if (Globals.Selection is { ClearPolygonSelection: true, LassoSelection.Count: > 2 })
      {
         Invalidate(Geometry.GetBounds([.. Globals.Selection.LassoSelection]));
         Globals.Selection.ClearPolygonSelection = false;
      }
      IsPainting = false;
   }
}
