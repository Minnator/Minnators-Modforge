using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Editor.Helper;

namespace Editor.Controls;

public sealed class PannablePictureBox : PictureBox
{
   private Point _startingPoint = Point.Empty;
   private Point _movingPoint = Point.Empty;
   private bool _panning = false;
   public event EventHandler ImageChanged = null!;

   //Bitmaps from bottom to top render order: Image, Overlay, Selection
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
            Selection = new Bitmap(_image.Width, _image.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(Overlay);
            g.Clear(Color.Transparent);
            using var g2 = Graphics.FromImage(Selection);
            g2.Clear(Color.Transparent);
            OnImageChanged(EventArgs.Empty);
         }
      }
   }
   private Bitmap _image = null!;
   public Bitmap Overlay = null!;
   public Bitmap Selection = null!;

   private readonly Panel _parentPanel;
   private int _lastInvalidatedProvince = -1; // Contains the invalidated provice from the last MouseMove event to clear it
   public bool AllowPanning { get; set; } = true;

   public PannablePictureBox(string path, ref Panel parentPanel)
   {
      MouseDown += PictureBox_MouseDown;
      MouseMove += PictureBox1_MouseMove;
      MouseUp += PictureBox1_MouseUp;

      Image = new Bitmap(path);
      _parentPanel = parentPanel;
   }


   private void OnImageChanged(EventArgs e)
   {
      ImageChanged?.Invoke(this, e);
      Width = Image.Width;
      Height = Image.Height;
   }

   private void PictureBox_MouseDown(object sender, MouseEventArgs e)
   {
      if (!AllowPanning)
         return;
      if (e.Button == MouseButtons.Left)
      {
         _panning = true;
         _startingPoint = e.Location;
         Cursor = Cursors.Hand; // Optional: change cursor to hand while panning
      }
   }

   private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
   {
      if (!AllowPanning)
         return;
      if (e.Button == MouseButtons.Left)
      {
         _panning = false;
         Cursor = Cursors.Default; // Optional: revert cursor back to default
      }
   }

   private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
   {
      // ------------------------------ Province Highlighting ------------------------------
      if (Data.ColorToProvPtr.TryGetValue(Color.FromArgb(Image.GetPixel(e.X, e.Y).ToArgb()), out var ptr))
      {
         if (_lastInvalidatedProvince != -1) 
            Invalidate(MapHelper.DrawProvinceBorder(_lastInvalidatedProvince, Color.Transparent, Overlay));
         Invalidate(MapHelper.DrawProvinceBorder(ptr, Color.Aqua, Overlay));
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
      pe.Graphics.DrawImage(Image, 0, 0, Image.Width, Image.Height);
      pe.Graphics.DrawImage(Overlay, 0, 0, Image.Width, Image.Height);
      pe.Graphics.DrawImage(Selection, 0, 0, Image.Width, Image.Height);
   }
}
