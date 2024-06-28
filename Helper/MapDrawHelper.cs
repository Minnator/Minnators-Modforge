using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Editor.DataClasses;

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
   
   // Draws the border of the given province on the given Bitmap with the given Color
   public static Rectangle DrawProvinceBorder(int provincePtr, Color color, Bitmap bmp)
   {
      var province = Globals.Provinces[provincePtr];
      var points = new Point[province.BorderCnt];
      Array.Copy(Globals.BorderPixels, province.BorderPtr, points, 0, province.BorderCnt);
      return DrawOnMap(province.Bounds, points, color, bmp);
   }

   public static Rectangle DrawProvince(int provincePtr, Color color, Bitmap bmp)
   {
      var province = Globals.Provinces[provincePtr];
      var points = new Point[province.PixelCnt];
      Array.Copy(Globals.Pixels, province.PixelPtr, points, 0, province.PixelCnt);
      return DrawOnMap(province.Bounds, points, color, bmp);
   }

   public static void DrawAllProvinceBorders(Bitmap bmp, Color color)
   {
      DrawOnMap(new Rectangle(0, 0, bmp.Width, bmp.Height), Globals.BorderPixels, color, bmp);
   }

   public static Rectangle DrawProvinceCollection (int[] provinceIds, Color color, Bitmap bmp, bool forceSingleDraw)
   {
      if (forceSingleDraw || provinceIds.Length > 20)
         return DrawProvinceCollection(provinceIds, color, bmp);
      List<Rectangle> rects = [];
      foreach (var provinceId in provinceIds)
      {
         DrawProvince(provinceId, Globals.Provinces[provinceId].Color, bmp);
         rects.Add(Globals.Provinces[provinceId].Bounds);
      }
      return Geometry.GetBounds(rects);
   }
   
   public static Rectangle DrawBorderCollection (int[] provinceIds, Color color, Bitmap bmp, bool forceSingleDraw)
   {
      if (forceSingleDraw || provinceIds.Length > 20)
         return DrawBorderCollection(provinceIds, color, bmp);
      List<Rectangle> rects = [];
      foreach (var provinceId in provinceIds)
      {
         DrawProvinceBorder(provinceId, color, bmp);
         rects.Add(Globals.Provinces[provinceId].Bounds);
      }
      return Geometry.GetBounds(rects);
   }

   private static Rectangle DrawBorderCollection (int[] provinceIds, Color color, Bitmap bmp)
   {
     var rects = provinceIds.Select(ptr => Globals.Provinces[ptr].Bounds).ToList();
      return DrawOnMap(Geometry.GetBounds(rects), GetAllBorderPoints(provinceIds), color, bmp);
   }

   private static Rectangle DrawProvinceCollection (int[] provinceIds, Color color, Bitmap bmp)
   {
      var rects = provinceIds.Select(ptr => Globals.Provinces[ptr].Bounds).ToList();
      GetAllPixelPoints(provinceIds, out var points);
      return DrawOnMap(Geometry.GetBounds(rects), points, color, bmp);
   }

   public static void GetAllPixelPoints (int[] provinceIds, out Point[] points)
   {
      var cnt = 0;
      foreach (var p in provinceIds)
      {
         cnt += Globals.Provinces[p].PixelCnt;
      }
      points = new Point[cnt];
      var index = 0;
      foreach (var p in provinceIds)
      {
         var prov = Globals.Provinces[p];
         Array.Copy(Globals.Pixels, prov.PixelPtr, points, index, prov.PixelCnt);
         index += prov.PixelCnt;
      }
   }

   public static void GetAllPixelPtrs(int[] ids, out int[,] ptrs)
   {
      ptrs = new int[ids.Length, 2];
      for (var i = 0; i < ids.Length; i++)
      {
         var prov = Globals.Provinces[ids[i]];
         ptrs[i, 0] = prov.PixelPtr;
         ptrs[i, 1] = prov.PixelCnt;
      }
   }

   public static Point[] GetAllBorderPoints (int[] provinceIds)
   {
      var cnt = 0;
      foreach (var p in provinceIds)
      {
         cnt += Globals.Provinces[p].BorderCnt;
      }
      var points = new Point[cnt];
      var index = 0;
      foreach (var p in provinceIds)
      {
         var prov = Globals.Provinces[p];
         Array.Copy(Globals.BorderPixels, prov.BorderPtr, points, index, prov.BorderCnt);
         index += prov.BorderCnt;
      }
      return points;
   }

   // ------------------------------ 24bpp ------------------------------
   #region 24bpp Drawing
   // !!24bpp!! Draws the given Array of Points on the given Bitmap with the given Color in parallel
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

}
