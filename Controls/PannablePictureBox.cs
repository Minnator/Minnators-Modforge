using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Controls;

public sealed class PannablePictureBox : PictureBox
{
   public bool IsPainting; // Prevents double locking of bitmaps when painting
   public event EventHandler ImageChanged = null!;

   // ------------------------------ Province Selection ------------------------------
   public Selection Selection; // Contains the selected provinces public to retrieve the selected provinces
   private int _lastInvalidatedProvince = -1; // Contains the invalidated province from the last MouseMove event to clear it

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
            Overlay = new Bitmap(_image.Width, _image.Height, PixelFormat.Format32bppArgb);
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
   private bool _panning = false;
   private bool AllowPanning { get; set; } = true; // If true, the user can pan the map by holding the middle mouse button

   public PannablePictureBox(string path, ref Panel parentPanel, MapWindow mapWindow)
   {
      MouseDown += PictureBox_MouseDown;
      MouseMove += PictureBox1_MouseMove;
      MouseUp += PictureBox1_MouseUp;
      MouseClick += OnMouseClick_Click;

      Selection = new Selection(this); // Initialize the selection class which manages the province selection
      Image = new Bitmap(path);
      _parentPanel = parentPanel;
      _mapWindow = mapWindow;
   }
   private void OnImageChanged(EventArgs e)
   {
      ImageChanged?.Invoke(this, e);
      Width = Image.Width;
      Height = Image.Height;
   }

   private void OnMouseClick_Click (object sender, MouseEventArgs e)
   {
      // ------------------------------ Panning ------------------------------
      // We don't want to mess with the selection if the user is panning
      if (e.Button == MouseButtons.Middle)
         return;

      // ------------------------------ Province Selection ------------------------------
      if (!Data.ColorToProvId.TryGetValue(Color.FromArgb(Image.GetPixel(e.X, e.Y).ToArgb()), out var ptr)) return;
      //check if ctrl is pressed
      if (ModifierKeys == Keys.Control) 
         Selection.Add(ptr);
      else if (!Selection.IsInRectSelection && ModifierKeys != Keys.Shift)
         Selection.MarkNext(ptr);

      _mapWindow.SetSelectedProvinceSum(Selection.SelectedProvPtr.Count);
   }
   
   private void PictureBox_MouseDown(object sender, MouseEventArgs e)
   {
      // ------------------------------ Province Selection ------------------------------
      if (ModifierKeys == Keys.Shift)
      {
         Selection.EnterRectangleSelection(e.Location);
         return;
      }
      _mapWindow.SetSelectedProvinceSum(Selection.SelectedProvPtr.Count);

      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
         if (e.Button == MouseButtons.Middle)
         {
            _panning = true;
            _startingPoint = e.Location;
            Cursor = Cursors.Hand; // Optional: change cursor to hand while panning
         }

   }

   private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
   { 
      // ------------------------------ Province Selection ------------------------------
      if (Selection.IsInRectSelection)
      {
         Selection.ExitRectangleSelection();
         return;
      }
      _mapWindow.SetSelectedProvinceSum(Selection.SelectedProvPtr.Count);

      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
         if (e.Button == MouseButtons.Middle)
         {
            _panning = false;
            Cursor = Cursors.Default; // Optional: revert cursor back to default
         }
   }

   private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
   {
      // ------------------------------ Out of Bounds Check ------------------------------
      if (e.X < 0 || e.Y < 0 || e.X >= Image.Width || e.Y >= Image.Height)
         return;

      // ------------------------------ Province Selection ------------------------------
      if (ModifierKeys == Keys.Shift && Selection.IsInRectSelection)
         Selection.MarkAllInRectangle(e.Location);
      _mapWindow.SetSelectedProvinceSum(Selection.SelectedProvPtr.Count);

      // ------------------------------ Province Highlighting ------------------------------
      if (Data.ColorToProvId.TryGetValue(Color.FromArgb(Image.GetPixel(e.X, e.Y).ToArgb()), out var ptr))
      {
         if (_lastInvalidatedProvince != -1) 
            Invalidate(MapDrawHelper.DrawProvinceBorder(_lastInvalidatedProvince, Color.Transparent, Overlay));
         Invalidate(MapDrawHelper.DrawProvinceBorder(ptr, Color.Aqua, Overlay));
         _lastInvalidatedProvince = ptr;
      }

      // ------------------------------ Panning ------------------------------
      if (AllowPanning)
         if (_panning)
         {
            _movingPoint = new Point(-(_parentPanel.AutoScrollPosition.X + e.X - _startingPoint.X),
               -(_parentPanel.AutoScrollPosition.Y + e.Y - _startingPoint.Y));
            _parentPanel.AutoScrollPosition = _movingPoint;
         }
   }

   protected override void OnPaint(PaintEventArgs pe)
   {
      base.OnPaint(pe);
      if (IsPainting)
         return;
      //IsPainting = true;
      pe.Graphics.DrawImage(Image, 0, 0, Image.Width, Image.Height);
      pe.Graphics.DrawImage(SelectionOverlay, 0, 0, Image.Width, Image.Height);
      pe.Graphics.DrawImage(Overlay, 0, 0, Image.Width, Image.Height);
      //IsPainting = false;
   }
}
