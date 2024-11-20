namespace Editor.Controls;

using System.ComponentModel;
using System.Runtime.InteropServices;
using static GDIHelper;
using Rectangle = Rectangle;

public class ImagePositionEventArgs(Rectangle oldRect, Rectangle newRect) : EventArgs
{
   public Rectangle OldRectangle { get; set; } = oldRect;
   public Rectangle NewRectangle { get; set; } = newRect;
}

public sealed class ZoomControl : Control, IDisposable
{
   private Bitmap _map = null!;
   private Graphics _bmpGfx = null!;
   public IntPtr HBitmap;

   public float ZoomFactor = 1.0f;
   private Point _panOffset;
   private Point _lastMousePosition;
   internal bool IsPanning;

   // ---------- Zoom limits ----------
   private float _zoomLimitUpper;
   private float _zoomLimitLower;
   private float _limitX;
   private float _limitY;
   private float _limitYHeight;
   private float _limitXWidth;
   private float _widthCorrected;
   private float _heightCorrected;

   private Rectangle _oldRectange = Rectangle.Empty;

   public bool CanZoom = true;
   private bool _border = true;

   private int _minVisiblePixels = 80;
   public int MinVisiblePixels
   {
      get => _minVisiblePixels;
      set => _minVisiblePixels = Math.Max(10, value);
   }

   public bool Border
   {
      get => _border;
      set
      {
         _border = value;
         Invalidate();
      }
   }
   private int _borderWidth = 2;
   public int BorderWidth
   {
      get => _borderWidth;
      set
      {
         _borderWidth = value;
         Invalidate();
      }
   }
   private Color _borderColor = Color.Black;
   public Color BorderColor
   {
      get => _borderColor;
      set
      {
         _borderColor = value;
         Invalidate();
      }
   }

   public EventHandler<ImagePositionEventArgs> ImagePositionChange = delegate { }; 

   public ZoomControl(Bitmap bmp)
   {
      Map = bmp;
      DoubleBuffered = true;
      // Attach event handlers for panning and zooming
      MouseWheel += PictureBox_MouseWheel!;
      MouseDown += PictureBox_MouseDown!;
      MouseMove += PictureBox_MouseMove!;
      MouseUp += PictureBox_MouseUp!;
      Resize += ZoomingControl_Resize!;
      Paint += ZoomOnPaint!;

      BackColor = Color.DimGray;
      MinimumSize = new (10, 10);
      Dock = DockStyle.Fill;
   }

   public new void Dispose()
   {
      if (_bmpGfx != null!)
         _bmpGfx.Dispose();
      if (HBitmap != IntPtr.Zero)
         DeleteObject(HBitmap);
      if (_map != null!)
         _map.Dispose();
   }
   
   // ---------- Properties ----------

   public Bitmap Map
   {
      get
      {
         return Image.FromHbitmap(HBitmap);
      }
      private set
      {
         _map = value;
         //dispose/delete any previous caches
         if (_bmpGfx != null!)
            _bmpGfx.Dispose();
         if (HBitmap != IntPtr.Zero)
            DeleteObject(HBitmap);
         if (value == null!)
            return;
         //cache the new HBitmap and Graphics.
         _bmpGfx = Graphics.FromImage(_map);
         HBitmap = _map.GetHbitmap();
         _panOffset = new((int)(_map.Width / 2), (int)(_map.Height / 2));
      }
   }

   public Rectangle MapRectangle => new(_panOffset.X, _panOffset.Y, (int)_widthCorrected, (int)_heightCorrected);

   // ---------- Pixel Frame Conversions ----------

   public PointF ConvertCoordinatesFloat(Point inPoint, out bool inBounds)
   {
      PointF outPoint = new((inPoint.X / ZoomFactor + _panOffset.X), (inPoint.Y / ZoomFactor + _panOffset.Y));

      if (outPoint.X < 0 || outPoint.Y < 0 || outPoint.X >= _map.Width || outPoint.Y >= _map.Height)
         inBounds = false;
      else
         inBounds = true;
      return outPoint;
   }

   public Point ConvertCoordinates(Point inPoint, out bool inBounds)
   {
      var xCord = (inPoint.X / ZoomFactor + _panOffset.X) % _map.Width;
      if (xCord < 0)
         xCord += _map.Width;
      Point outPoint = new((int)xCord, (int)(inPoint.Y / ZoomFactor + _panOffset.Y));

      if (outPoint.X < 0 || outPoint.Y < 0 || outPoint.X >= _map.Width || outPoint.Y >= _map.Height)
         inBounds = false;
      else
         inBounds = true;
      return outPoint;
   }

   public Point ReverseCoordinate(Point inPoint)
   {
      return new((int)(ZoomFactor * (inPoint.X - _panOffset.X)), (int)(ZoomFactor * (inPoint.Y - _panOffset.Y)));
   }

   public PointF ReverseCoordinateFloat(PointF inPoint, bool withOverRoll = false)
   {
      if (withOverRoll)
      {
         if (inPoint.X < MapRectangle.Right - _map.Width)
            inPoint.X += _map.Width;
         else if (inPoint.X > _map.Width + MapRectangle.Left)
            inPoint.X -= _map.Width;
      }

      return new((ZoomFactor * (inPoint.X - _panOffset.X)), (ZoomFactor * (inPoint.Y - _panOffset.Y)));
   }

   // ---------- Focusing and Zooming on Points ----------

   public void FocusOn(Rectangle rect)
   {
      var mid = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
      if (rect.Width < 100)
         rect.Width = 100;
      if (rect.Height < 100)
         rect.Height = 100;
      var zoom = MathF.Min(Width / (1.1f * rect.Width), Height / (1.1f * rect.Height));
      FocusOn(mid, zoom);
   }

   public void FocusOn(Point p)
   {
      _panOffset.X = p.X - (int)(Width / (ZoomFactor * 2));
      _panOffset.Y = p.Y - (int)(Height / (ZoomFactor * 2));
      LimitPan();
      Invalidate();
   }

   public void FocusOn(Point p, float zoom)
   {
      ZoomFactor = zoom;
      LimitZoom();
      FocusOn(p);
   }

   public void SetZoomFully(Point point, float newZoom)
   {
      var oldZoom = ZoomFactor;
      ZoomFactor = newZoom;
      SetZoom(point, oldZoom);
   }

   public void SetZoom(Point point, float oldZoom)
   {
      LimitZoom();
      if (oldZoom.Equals(ZoomFactor))
         return;

      var halfHeight = Height / 2;
      var halfWidth = Width / 2;
      var tempDelta = ZoomFactor - oldZoom;
      var zoomFactorFactor = tempDelta / (oldZoom * ZoomFactor);
      var zoomFactorSqr = tempDelta / (ZoomFactor * ZoomFactor);
      
      _panOffset.X += (int)(halfWidth * zoomFactorFactor + zoomFactorSqr * (point.X - halfWidth));
      _panOffset.Y += (int)(halfHeight * zoomFactorFactor + zoomFactorSqr * (point.Y - halfHeight));

      LimitPan();
      // Redraw the image with the updated zoom
      Invalidate();
   }

   public Color GetColor(Point inPoint)
   {
      var point = ConvertCoordinates(inPoint, out bool inBounds);
      if (!inBounds)
         return Color.Empty;
      return _map.GetPixel(point.X, point.Y); 
   }


   private void LimitZoom()
   {
      ZoomFactor = Math.Max(Math.Min(ZoomFactor, _zoomLimitLower), _zoomLimitUpper);
      var logzoom = (int)MathF.Round(MathF.Log(ZoomFactor) / MathF.Log(1.1f));

      ZoomFactor = MathF.Pow(1.1f, logzoom);

      _widthCorrected = Width / ZoomFactor;
      _heightCorrected = Height / ZoomFactor;

      _limitX = -0.1f * _widthCorrected;
      _limitY = -0.1f * _heightCorrected;
      _limitYHeight = _map.Height + 9 * _limitY;
      _limitXWidth = _map.Width + 9 * _limitX;

   }

   private void LimitPan()
   {
      if (_limitYHeight < _limitY)
         _panOffset.Y = -(int)((Height / ZoomFactor - _map.Height) / 2);
      else
         _panOffset.Y = (int)Math.Max(_limitY, Math.Min(_limitYHeight, _panOffset.Y));

      //if (_limitXWidth < _limitX)
      //   panOffset.X = -(int)((Width / zoomFactor - map.Width) / 2);
      //else
      //panOffset.X = (int)Math.Max(_limitX, Math.Min(_limitXWidth, panOffset.X));

      var center = MapRectangle.X + MapRectangle.Width / 2;

      if(center > _map.Width)
         _panOffset.X -= _map.Width;
      else if(center < 0)
        _panOffset.X += _map.Width;
      
      var newRect = MapRectangle;
      ImagePositionChange.Invoke(this, new (_oldRectange, newRect));
      _oldRectange = newRect;
   }
   public static void DrawStretch(IntPtr hBitmap, Graphics srcGfx, Graphics destGfx, Rectangle srcRect, Rectangle destRect)
   {
      var pTarget = destGfx.GetHdc();
      var pSource = CreateCompatibleDC(pTarget);
      var pOrig = SelectObject(pSource, hBitmap);
      if (!StretchBlt(pTarget, destRect.X, destRect.Y, destRect.Width, destRect.Height,
             pSource, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
             TernaryRasterOperations.SRCCOPY))
         throw new Win32Exception(Marshal.GetLastWin32Error());

      SelectObject(pSource, pOrig);
      DeleteDC(pSource);
      destGfx.ReleaseHdc(pTarget);
   }


   // ---------- Event Handlers ----------

   private void PictureBox_MouseDown(object sender, MouseEventArgs e)
   {
      if (e.Button == MouseButtons.Middle)
      {
         IsPanning = true;
         _lastMousePosition = e.Location;
         Cursor = Cursors.Hand; // Change cursor to hand during panning
      }
   }
   private void PictureBox_MouseUp(object sender, MouseEventArgs e)
   {
      // --------- Panning ---------
      if (e.Button == MouseButtons.Middle)
      {
         IsPanning = false;
         Cursor = Cursors.Default; // Restore default cursor after panning
      }
   }

   private void PictureBox_MouseMove(object sender, MouseEventArgs e)
   {
      if (IsPanning)
      {
         var oldOffset = _panOffset;
         _panOffset.X += (int)(_lastMousePosition.X / ZoomFactor) - (int)(e.X / ZoomFactor);
         _panOffset.Y += (int)(_lastMousePosition.Y / ZoomFactor) - (int)(e.Y / ZoomFactor);
         LimitPan();

         if (oldOffset == _panOffset)
            return;

         _lastMousePosition = e.Location;
         Invalidate();
      }
   }

   public void ZoomingControl_Resize(object sender, EventArgs e)
   {
      _zoomLimitUpper = Math.Min((float)Width / _map.Width, (float)Height / _map.Height) / 1.1f;
      _zoomLimitLower = Math.Min((float)Width / MinVisiblePixels, (float)Height / MinVisiblePixels);

      LimitZoom();
      LimitPan();
      Invalidate();
   }

   private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
   {
      if (!CanZoom)
         return;

      var converted = ConvertCoordinates(e.Location, out _);

      // Adjust the zoom factor based on the mouse wheel direction
      if (e.Delta > 0)
         ZoomFactor *= 1.1f; // Zoom in
      else if (e.Delta < 0)
         ZoomFactor /= 1.1f; // Zoom out
      LimitZoom();
      var converted2 = ConvertCoordinates(e.Location, out _);
      _panOffset.X -= converted2.X - converted.X;
      _panOffset.Y -= converted2.Y - converted.Y;
      LimitPan();
      Invalidate();
   }

   private void ZoomOnPaint2(object? sender, PaintEventArgs? e)
   {
      if (_map == null! || e == null) return;

      Rectangle thisRect = new(0, 0, Width, Height);


      // Get the width of the bitmap and the rectangle
      int rectEnd = MapRectangle.Right;

      // Calculate how much we need to offset the drawing on the X axis
      int offsetX = rectEnd % _map.Width - MapRectangle.Width;

      if (offsetX < 0) offsetX += _map.Width;

      // Draw the bitmap multiple times to repeat on the X-axis
      for (int x = offsetX; x < MapRectangle.X; x += _map.Width)
      {
         // Create a new rectangle to draw the bitmap in the correct position
         Rectangle destRect = new Rectangle(x, 0, Width, Height);

         // Adjust the source rectangle in case we are drawing only part of the bitmap
         //Rectangle srcRect = new Rectangle(0, 0, bitmapWidth, map.Height);

         // Stretch the bitmap to the destination rectangle
         DrawStretch(HBitmap, _bmpGfx, e.Graphics, MapRectangle, destRect);
      }
   }

   private void ZoomOnPaint(object? sender, PaintEventArgs? e)
   {
      if (_map == null! || e == null)
      {
         if (e != null)
            DrawBorder(e.Graphics);
         return;
      }
      
      Rectangle thisRect = new(0, 0, Width, Height);

      DrawStretch(HBitmap, _bmpGfx, e.Graphics, MapRectangle, thisRect);

      var test = MapRectangle;

      if (MapRectangle.Right - _map.Width > 0)
      {
         var rightPoint = ReverseCoordinate(new(_map.Width, _map.Height)).X;

         thisRect = thisRect with { Width = Width - rightPoint, X = rightPoint};

         test = test with { Width = MapRectangle.Right - _map.Width, X = 0};
         DrawStretch(HBitmap, _bmpGfx, e.Graphics, test, thisRect);
      }

      if (MapRectangle.X >= 0)
      {
         DrawBorder(e.Graphics);
         return;
      }
      //var point = ReverseCoordinate(MapRectangle.Location);

      var zeroPoint = ReverseCoordinateFloat(new(0f,0));

      test = test with { Width = - MapRectangle.X, X = _map.Width + MapRectangle.X }; //map.Width + MapRectangle.X

      thisRect.X = 0;

      thisRect = thisRect with { Width = (int)Math.Ceiling(zeroPoint.X + 1.5)};
      DrawStretch(HBitmap, _bmpGfx, e.Graphics, test, thisRect);
      DrawBorder(e.Graphics);

   }

   private void DrawBorder(Graphics g)
   {
      if (!Border)
         return;
      var halfBorderWidth = BorderWidth / 2;
      g.DrawRectangle(new(BorderColor, BorderWidth), halfBorderWidth, halfBorderWidth, Width - BorderWidth, Height - BorderWidth);
   }

}

public static class GDIHelper
{
   [StructLayout(LayoutKind.Sequential)]
   public struct BITMAP
   {
      public int bmType;
      public int bmWidth;
      public int bmHeight;
      public int bmWidthBytes;
      public ushort bmPlanes;
      public ushort bmBitsPixel;
      public IntPtr bmBits;
   }

   [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
   public static extern IntPtr SelectObject(
      [In] IntPtr hdc,
      [In] IntPtr h);

   [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
   public static extern bool DeleteDC(IntPtr hdc);

   [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
   [return: MarshalAs(UnmanagedType.Bool)]
   public static extern bool DeleteObject(
      [In] IntPtr ho);

   [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
   public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

   [DllImport("gdi32.dll")]
   public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
      int nWidthDest, int nHeightDest,
      IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
      TernaryRasterOperations dwRop);

   [DllImport("gdi32.dll", SetLastError = true)]
   public static extern IntPtr CreateSolidBrush(uint color);

   [DllImport("gdi32.dll", SetLastError = true)]
   public static extern bool Rectangle(IntPtr hdc, int left, int top, int right, int bottom);

   [DllImport("user32.dll")]
   public static extern IntPtr GetDC(IntPtr hwnd);

   [DllImport("user32.dll")]
   public static extern bool ReleaseDC(IntPtr hwnd, IntPtr hdc);

   [DllImport("gdi32.dll")]
   public static extern int GetObject(IntPtr hObject, int nCount, ref BITMAP lpObject);

   public enum TernaryRasterOperations : uint
   {
      SRCCOPY = 0x00CC0020
      //there are many others but we don't need them for this purpose, omitted for brevity
   }
}