using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Editor.Controls;

public sealed class PannablePictureBox : PictureBox
{
   private Point _startingPoint = Point.Empty;
   private Point _movingPoint = Point.Empty;
   private bool _panning = false;
   public event EventHandler ImageChanged = null!;
   private Image _image = null!;
   private Image _overlay = null!;
   private readonly Panel _parentPanel;
   public bool AllowPanning { get; set; } = true;

   public PannablePictureBox(string path, ref Panel parentPanel)
   {
      this.MouseDown += PictureBox_MouseDown;
      this.MouseMove += PictureBox1_MouseMove;
      this.MouseUp += PictureBox1_MouseUp;

      Image = Image.FromFile(path);
      _parentPanel = parentPanel;
   }

   public new Image Image
   {
      get => _image;
      set
      {
         if (_image != value)
         {
            _image = value;
            OnImageChanged(EventArgs.Empty);
         }
      }
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
      if (!AllowPanning)
         return;
      if (_panning)
      {
         if (_panning)
         {
            _movingPoint = new Point(-(_parentPanel.AutoScrollPosition.X + e.X - _startingPoint.X),
               -(_parentPanel.AutoScrollPosition.Y + e.Y - _startingPoint.Y));
            _parentPanel.AutoScrollPosition = _movingPoint;
         }
         Invalidate();
      }
   }

   protected override void OnPaint(PaintEventArgs pe)
   {
      base.OnPaint(pe);
      pe.Graphics.DrawImage(Image, 0, 0, Image.Width, Image.Height);
   }
}
