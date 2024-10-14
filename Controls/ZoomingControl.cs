using System.Diagnostics;

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
   private Bitmap map;
   public Graphics BmpGfx;
   public IntPtr HBitmap;

   public float zoomFactor = 1.0f;
   private Point panOffset;
   private Point lastMousePosition;
   internal bool isPanning;

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

   public void Dispose()
   {
      if (HBitmap != null)
         DeleteObject(HBitmap);
      if (BmpGfx != null)
         BmpGfx.Dispose();
      if (map != null)
         map.Dispose();
   }
   
   // ---------- Properties ----------

   public Bitmap Map
   {
      get
      {
         return Image.FromHbitmap(HBitmap);
      }
      set
      {
         map = value;
         //dispose/delete any previous caches
         if (BmpGfx != null)
            BmpGfx.Dispose();
         if (HBitmap != null)
            DeleteObject(HBitmap);
         if (value == null)
            return;
         //cache the new HBitmap and Graphics.
         BmpGfx = Graphics.FromImage(map);
         HBitmap = map.GetHbitmap();
         panOffset = new((int)(map.Width / 2), (int)(map.Height / 2));
      }
   }

   public Rectangle MapRectangle => new(panOffset.X, panOffset.Y, (int)_widthCorrected, (int)_heightCorrected);

   // ---------- Pixel Frame Conversions ----------

   public PointF ConvertCoordinatesFloat(Point inPoint, out bool inBounds)
   {
      PointF outPoint = new((inPoint.X / zoomFactor + panOffset.X), (inPoint.Y / zoomFactor + panOffset.Y));

      if (outPoint.X < 0 || outPoint.Y < 0 || outPoint.X >= map.Width || outPoint.Y >= map.Height)
         inBounds = false;
      else
         inBounds = true;
      return outPoint;
   }

   public Point ConvertCoordinates(Point inPoint, out bool inBounds)
   {
      var xCord = (inPoint.X / zoomFactor + panOffset.X) % map.Width;
      if (xCord < 0)
         xCord += map.Width;
      Point outPoint = new((int)xCord, (int)(inPoint.Y / zoomFactor + panOffset.Y));

      if (outPoint.X < 0 || outPoint.Y < 0 || outPoint.X >= map.Width || outPoint.Y >= map.Height)
         inBounds = false;
      else
         inBounds = true;
      return outPoint;
   }

   public Point ReverseCoordinate(Point inPoint)
   {
      return new((int)(zoomFactor * (inPoint.X - panOffset.X)), (int)(zoomFactor * (inPoint.Y - panOffset.Y)));
   }

   public PointF ReverseCoordinateFloat(PointF inPoint, bool withOverRoll = false)
   {
      if (withOverRoll)
      {
         if (inPoint.X < MapRectangle.Right - map.Width)
            inPoint.X += map.Width;
         else if (inPoint.X > map.Width + MapRectangle.Left)
            inPoint.X -= map.Width;
      }

      return new((zoomFactor * (inPoint.X - panOffset.X)), (zoomFactor * (inPoint.Y - panOffset.Y)));
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
      panOffset.X = p.X - (int)(Width / (zoomFactor * 2));
      panOffset.Y = p.Y - (int)(Height / (zoomFactor * 2));
      LimitPan();
      Invalidate();
   }

   public void FocusOn(Point p, float zoom)
   {
      zoomFactor = zoom;
      LimitZoom();
      FocusOn(p);
   }

   public void SetZoomFully(Point point, float newZoom)
   {
      var oldZoom = zoomFactor;
      zoomFactor = newZoom;
      SetZoom(point, oldZoom);
   }

   public void SetZoom(Point point, float oldZoom)
   {
      LimitZoom();
      if (oldZoom == zoomFactor)
         return;

      var halfHeight = Height / 2;
      var halfWidth = Width / 2;
      var tempdelta = zoomFactor - oldZoom;
      var zoomFactorFactor = tempdelta / (oldZoom * zoomFactor);
      var zoomFactorSqr = tempdelta / (zoomFactor * zoomFactor);
      
      panOffset.X += (int)(halfWidth * zoomFactorFactor + zoomFactorSqr * (point.X - halfWidth));
      panOffset.Y += (int)(halfHeight * zoomFactorFactor + zoomFactorSqr * (point.Y - halfHeight));

      LimitPan();
      // Redraw the image with the updated zoom
      Invalidate();
   }

   public Color GetColor(Point inPoint)
   {
      var point = ConvertCoordinates(inPoint, out bool inBounds);
      if (!inBounds)
         return Color.Empty;
      return map.GetPixel(point.X, point.Y); // TODO maybe find faster way using Hmap? calculate pointer using stride and height
   }


   private void LimitZoom()
   {
      zoomFactor = Math.Max(Math.Min(zoomFactor, _zoomLimitLower), _zoomLimitUpper);
      var logzoom = (int)MathF.Round(MathF.Log(zoomFactor) / MathF.Log(1.1f));

      zoomFactor = MathF.Pow(1.1f, logzoom);

      _widthCorrected = Width / zoomFactor;
      _heightCorrected = Height / zoomFactor;

      _limitX = -0.1f * _widthCorrected;
      _limitY = -0.1f * _heightCorrected;
      _limitYHeight = map.Height + 9 * _limitY;
      _limitXWidth = map.Width + 9 * _limitX;

   }

   private void LimitPan()
   {
      if (_limitYHeight < _limitY)
         panOffset.Y = -(int)((Height / zoomFactor - map.Height) / 2);
      else
         panOffset.Y = (int)Math.Max(_limitY, Math.Min(_limitYHeight, panOffset.Y));

      //if (_limitXWidth < _limitX)
      //   panOffset.X = -(int)((Width / zoomFactor - map.Width) / 2);
      //else
      //panOffset.X = (int)Math.Max(_limitX, Math.Min(_limitXWidth, panOffset.X));

      var center = MapRectangle.X + MapRectangle.Width / 2;

      if(center > map.Width)
         panOffset.X -= map.Width;
      else if(center < 0)
        panOffset.X += map.Width;
      
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
         isPanning = true;
         lastMousePosition = e.Location;
         Cursor = Cursors.Hand; // Change cursor to hand during panning
      }
   }
   private void PictureBox_MouseUp(object sender, MouseEventArgs e)
   {
      // --------- Panning ---------
      if (e.Button == MouseButtons.Middle)
      {
         isPanning = false;
         Cursor = Cursors.Default; // Restore default cursor after panning
      }
   }

   private void PictureBox_MouseMove(object sender, MouseEventArgs e)
   {
      if (isPanning)
      {
         var oldOffset = panOffset;
         panOffset.X += (int)(lastMousePosition.X / zoomFactor) - (int)(e.X / zoomFactor);
         panOffset.Y += (int)(lastMousePosition.Y / zoomFactor) - (int)(e.Y / zoomFactor);
         LimitPan();

         if (oldOffset == panOffset)
            return;

         lastMousePosition = e.Location;
         Invalidate();
      }
   }

   private void ZoomingControl_Resize(object sender, EventArgs e)
   {
      _zoomLimitUpper = Math.Min((float)Width / map.Width, (float)Height / map.Height) / 1.1f;
      _zoomLimitLower = Math.Min((float)Width / 80, (float)Height / 80);

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
         zoomFactor *= 1.1f; // Zoom in
      else if (e.Delta < 0)
         zoomFactor /= 1.1f; // Zoom out
      LimitZoom();
      var converted2 = ConvertCoordinates(e.Location, out _);
      panOffset.X -= converted2.X - converted.X;
      panOffset.Y -= converted2.Y - converted.Y;
      LimitPan();
      Invalidate();
   }

   private void ZoomOnPaint2(object? sender, PaintEventArgs? e)
   {
      if (map == null! || e == null) return;

      Rectangle thisRect = new(0, 0, Width, Height);


      // Get the width of the bitmap and the rectangle
      int rectEnd = MapRectangle.Right;

      // Calculate how much we need to offset the drawing on the X axis
      int offsetX = rectEnd % map.Width - MapRectangle.Width;

      if (offsetX < 0) offsetX += map.Width;

      // Draw the bitmap multiple times to repeat on the X-axis
      for (int x = offsetX; x < MapRectangle.X; x += map.Width)
      {
         // Create a new rectangle to draw the bitmap in the correct position
         Rectangle destRect = new Rectangle(x, 0, Width, Height);

         // Adjust the source rectangle in case we are drawing only part of the bitmap
         //Rectangle srcRect = new Rectangle(0, 0, bitmapWidth, map.Height);

         // Stretch the bitmap to the destination rectangle
         DrawStretch(HBitmap, BmpGfx, e.Graphics, MapRectangle, destRect);
      }
   }

   private void ZoomOnPaint(object? sender, PaintEventArgs? e)
   {
      if (map == null! || e == null) return;
      
      Rectangle thisRect = new(0, 0, Width, Height);

      DrawStretch(HBitmap, BmpGfx, e.Graphics, MapRectangle, thisRect);

      Rectangle test = MapRectangle;

      int deltax = MapRectangle.Right - map.Width;
      if (deltax > 0)
      {
         var rightPoint = ReverseCoordinate(new(map.Width, map.Height)).X;

         thisRect = thisRect with { Width = Width - rightPoint, X = rightPoint};

         test = test with { Width = MapRectangle.Right - map.Width, X = 0};
         DrawStretch(HBitmap, BmpGfx, e.Graphics, test, thisRect);
      }

      if (MapRectangle.X >= 0)
         return;
      //var point = ReverseCoordinate(MapRectangle.Location);

      var zeroPoint = ReverseCoordinateFloat(new(1f,0));

      test = test with { Width = - MapRectangle.X, X = map.Width + MapRectangle.X }; //map.Width + MapRectangle.X

      thisRect.X = 0;

      thisRect = thisRect with { Width = (int)Math.Ceiling(zeroPoint.X)};
      DrawStretch(HBitmap, BmpGfx, e.Graphics, test, thisRect);
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