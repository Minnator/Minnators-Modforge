using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using ABI.Windows.Foundation;
using Editor.DataClasses.GameDataClasses;
using Point = System.Drawing.Point;

namespace Editor.Helper;

public static class MapDrawHelper
{
   // ================================ Public Methods ================================
   // Draws the given Array of Points on the given Bitmap with the given Color
   public static Rectangle DrawOnMap(Rectangle rect, Point[] points, Color color, Bitmap bmp)
   {
      if (points.Length == 0)
         return rect;
      switch (bmp.PixelFormat)
      {
         case PixelFormat.Format24bppRgb when points.Length > 4000:
            DrawPixels24BppParallel(rect, points, color, bmp);
            break;
         case PixelFormat.Format24bppRgb:
            DrawPixels24Bpp(rect, points, color, bmp);
            break;
         case PixelFormat.Format32bppArgb when points.Length > 4000:
            DrawPixels32BppParallel(rect, points, color, bmp);
            break;
         case PixelFormat.Format32bppArgb:
            DrawPixels32Bpp(rect, points, color, bmp);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(bmp.PixelFormat), "Unknown Bitmap format.");
      }
      return rect;
   }

   public static Rectangle DrawOnMap(ICollection<Province> ids, Func<Province, Color> func, Bitmap bmp)
   {
      if (ids.Count == 0)
         return Rectangle.Empty;

      List<Rectangle> rects;

      switch (bmp.PixelFormat)
      {
         case PixelFormat.Format24bppRgb when ids.Count > Environment.ProcessorCount:
            DrawPixels24BppParallel(ids, func, bmp, out rects);
            break;
         case PixelFormat.Format24bppRgb:
            DrawPixels24Bpp(ids, func, bmp, out rects);
            break;
         case PixelFormat.Format32bppArgb when ids.Count > Environment.ProcessorCount:
            DrawPixels32BppParallel(ids, func, bmp, out rects);
            break;
         case PixelFormat.Format32bppArgb:
            DrawPixels32Bpp(ids, func, bmp, out rects);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(bmp.PixelFormat), "Unknown Bitmap format.");
      }
      return Geometry.GetBounds(rects);
   }

   // Draws the border of the given province on the given Bitmap with the given Color
   public static Rectangle DrawProvinceBorder(Province province, Color color, Bitmap bmp)
   {
      var points = new Point[province.BorderCnt];
      Array.Copy(Globals.BorderPixels, province.BorderPtr, points, 0, province.BorderCnt);
      return DrawOnMap(province.Bounds, points, color, bmp);
   }

   public static Rectangle DrawProvince(Province province, Color color, Bitmap bmp)
   {
      var points = new Point[province.PixelCnt];
      Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);
      return DrawOnMap(province.Bounds, points, color, bmp);
   }

   public static void DrawAllProvinceBorders(Bitmap bmp, Color color)
   {
      DrawOnMap(new (0, 0, bmp.Width, bmp.Height), Globals.BorderPixels, color, bmp);
   }

   // ------------------------------ 24bpp ------------------------------
   #region 24bpp Drawing
   // !!24bpp!! Draws the given Array of Points on the given Bitmap with the given Color in parallel

   private static void DrawPixels24BppParallel(ICollection<Province> ids, Func<Province, Color> func, Bitmap bmp, out List<Rectangle> rects)
   {
      rects = [];
      foreach (var id in ids)
      {
         var points = new Point[id.PixelCnt];
         Array.Copy(Globals.Pixels, id.PixelPtr, points, 0, id.PixelCnt);
         DrawPixels24BppParallel(id.Bounds, points, func(id), bmp);
         rects.Add(id.Bounds);
      }
   }

   private static void DrawPixels24Bpp (ICollection<Province> ids, Func<Province, Color> func, Bitmap bmp, out List<Rectangle> rects)
   {
      rects = [];
      foreach (var id in ids)
      {
         var points = new Point[id.PixelCnt];
         Array.Copy(Globals.Pixels, id.PixelPtr, points, 0, id.PixelCnt);
         DrawPixels24Bpp(id.Bounds, points, func(id), bmp);
         rects.Add(id.Bounds);
      }
   }
   
   private static void DrawPixels24BppParallel(Rectangle rect, Point[] points, Color color, Bitmap bmp)
   {
      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

      unsafe
      {
         var r = color.R;
         var g = color.G;
         var b = color.B;
         var scan0 = (byte*)bmpData.Scan0.ToPointer();

         Parallel.ForEach(points, point =>
         {
            var index = (point.Y - rect.Y) * bmpData.Stride + (point.X - rect.X) * 3;

            scan0[index]     = b;       // Blue component
            scan0[index + 1] = g;   // Green component
            scan0[index + 2] = r;   // Red component
         });
      }

      bmp.UnlockBits(bmpData);
   }
   // !!24bpp!! Draws the given Array of Points on the given Bitmap with the given Color
   private static void DrawPixels24Bpp(Rectangle rect, Point[] points, Color color, Bitmap bmp)
   {
      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

      unsafe
      {
         var r = color.R;
         var g = color.G;
         var b = color.B;
         var scan0 = (byte*)bmpData.Scan0.ToPointer();



         foreach (var point in points)
         {
            var index = (point.Y - rect.Y) * bmpData.Stride + (point.X - rect.X) * 3;

            scan0[index] = b;       // Blue component
            scan0[index + 1] = g;   // Green component
            scan0[index + 2] = r;   // Red component
         }
      }

      bmp.UnlockBits(bmpData);
   }
   #endregion

   // ------------------------------ 32bpp ------------------------------
   #region 32bpp Drawing
   // !!32bpp!! Draws the given Array of Points on the given Bitmap with the given Color
   
   private static void DrawPixels32Bpp(ICollection<Province> ids, Func<Province, Color> func, Bitmap bmp, out List<Rectangle> rects)
   {
      rects = [];
      foreach (var province in ids)
      {
         var points = new Point[province.PixelCnt];
         Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);
         DrawPixels32Bpp(province.Bounds, points, func(province), bmp);
         rects.Add(province.Bounds);
      }
   }

   private static void DrawPixels32BppParallel(ICollection<Province> ids, Func<Province, Color> func, Bitmap bmp, out List<Rectangle> rects)
   {
      rects = [];
      foreach (var id in ids)
      {
         var points = new Point[id.PixelCnt];
         Array.Copy(Globals.Pixels, id.PixelPtr, points, 0, id.PixelCnt);
         DrawPixels32BppParallel(id.Bounds, points, func(id), bmp);
         rects.Add(id.Bounds);
      }
   }

   private static void DrawPixels32Bpp(Rectangle rect, Point[] points, Color color, Bitmap bmp)
   {
      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

      unsafe
      {
         var a = color.A;
         var r = color.R;
         var g = color.G;
         var b = color.B;
         var scan0 = (byte*)bmpData.Scan0.ToPointer();

         foreach (var point in points)
         {
            var index = (point.Y - rect.Y) * bmpData.Stride + (point.X - rect.X) * 4;

            scan0[index] = b;       // Blue component
            scan0[index + 1] = g;   // Green component
            scan0[index + 2] = r;   // Red component
            scan0[index + 3] = a;   // Alpha component
         }
      }

      bmp.UnlockBits(bmpData);
   }
   // !!32bpp!! Draws the given Array of Points on the given Bitmap with the given Color in parallel
   private static void DrawPixels32BppParallel(Rectangle rect, Point[] points, Color color, Bitmap bmp)
   {
      var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

      try
      {
         unsafe
         {
            var a = color.A;
            var r = color.R;
            var g = color.G;
            var b = color.B;
            var scan0 = (byte*)bmpData.Scan0.ToPointer();

            Parallel.ForEach(points, point =>
            {
               var index = (point.Y - rect.Y) * bmpData.Stride + (point.X - rect.X) * 4;

               scan0[index] = b; // Blue component
               scan0[index + 1] = g; // Green component
               scan0[index + 2] = r; // Red component
               scan0[index + 3] = a; // Alpha component
            });
         }
      }
      finally 
      {
         bmp.UnlockBits(bmpData);
      }
   }
   #endregion

   public static Rectangle DrawStripes(Color color, List<Province> ids, Bitmap bmp)
   {
      var rects = new Rectangle[ids.Count];
      for (var index = 0; index < ids.Count; index++)
      {
         Geometry.GetStripesArray(ids[index], out var stripePixels);
         rects[index] = DrawOnMap(ids[index].Bounds, stripePixels, color, bmp);
      }

      return Geometry.GetBounds([..rects]);
   }

   public static void DrawAllCapitals(Bitmap bmp)
   {
      List<Point> capitals = [];
      foreach (var id in Globals.Capitals)
         capitals.Add(id.Center);
      DrawCapitals(bmp, capitals);
   }

   private static void DrawCapitals(Bitmap bmp, List<Point> capitals)
   {
      var g = Graphics.FromImage(bmp);
      foreach (var capital in capitals)
         DrawCapital(ref g, capital);
      g.Dispose();
   }

   private static void DrawCapital(ref Graphics g, Point provinceCenter)
   {
      g.DrawRectangle(new(Color.Black, 1), provinceCenter.X - 2, provinceCenter.Y - 2, 4, 4);
      g.DrawRectangle(Pens.Yellow, provinceCenter.X - 1, provinceCenter.Y - 1, 2, 2);
   }

   public static void RedrawCapitals(Bitmap bmp, List<Province> provinceIds)
   {
      List<Point> redrawList = [];
      foreach (var id in provinceIds)
         if (Globals.Capitals.TryGetValue(id, out var prov))
            redrawList.Add(prov.Center);
      if (redrawList.Count > 0)
         DrawCapitals(bmp, redrawList);
   }
}
