using Editor.Controls;

namespace Editor.Helper;
using System;
using System.Runtime.InteropServices;
using Editor.DataClasses.GameDataClasses;
using static GDIHelper;

public enum PixelsOrBorders
{
   Pixels,
   Borders,
   Both
}


public class MapDrawing
{
   public static void DrawOnMap(Point[] points, int color, ZoomControl zoomControl)
   {
      if (points.Length > 4000)
         DrawPixelsParallel(points, color, zoomControl);
      else
         DrawPixels(points, color, zoomControl);
   }

   public static void DrawOnMap(ICollection<Province> provinces, Func<Province, int> func, ZoomControl zoomControl, PixelsOrBorders pixelOrBorders)
   {
      if (provinces.Count == 0)
         return;

      if (provinces.Count > Environment.ProcessorCount)
         DrawProvincesParallel(provinces, func, zoomControl, pixelOrBorders);
      else
         foreach (var province in provinces)
            switch (pixelOrBorders)
            {
               case PixelsOrBorders.Pixels:
                  DrawPixels(province.Pixels, func(province), zoomControl);
                  break;
               case PixelsOrBorders.Borders:
                  DrawPixels(province.Borders, func(province), zoomControl);
                  break;
               case PixelsOrBorders.Both:
                  DrawPixels(province.Pixels, func(province), zoomControl);
                  DrawPixels(province.Borders, func(province), zoomControl);
                  break;
            }
   }

   public static void DrawOnMap(ICollection<Province> provinces, int color, ZoomControl zoomControl, PixelsOrBorders pixelOrBorders)
   {
      if (provinces.Count == 0)
         return;

      if (provinces.Count > Environment.ProcessorCount)
         DrawProvincesParallel(provinces, color, zoomControl, pixelOrBorders);
      else
         foreach (var province in provinces)
            switch (pixelOrBorders)
            {
               case PixelsOrBorders.Pixels:
                  DrawPixels(province.Pixels, color, zoomControl);
                  break;
               case PixelsOrBorders.Borders:
                  DrawPixels(province.Borders, color, zoomControl);
                  break;
               case PixelsOrBorders.Both:
                  DrawPixels(province.Pixels, color, zoomControl);
                  DrawPixels(province.Borders, color, zoomControl);
                  break;
            }
   }

   public static void DrawOnMap(Province province, int color, ZoomControl zoomControl, PixelsOrBorders pixelOrBorders)
   {
      switch (pixelOrBorders)
      {
         case PixelsOrBorders.Pixels:
            DrawPixels(province.Pixels, color, zoomControl);
            break;
         case PixelsOrBorders.Borders:
            DrawPixels(province.Borders, color, zoomControl);
            break;
         case PixelsOrBorders.Both:
            DrawPixels(province.Pixels, color, zoomControl);
            DrawPixels(province.Borders, color, zoomControl);
            break;
      }
   }


   /// <summary>
   /// BGRA fromat
   /// </summary>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   public static void DrawAllBorders(int color, ZoomControl zoomControl)
   {
      DrawPixelsParallel(Globals.BorderPixels, color, zoomControl);
   }

   // Invalidation rects needs to bet taken care of
   private static void DrawProvincesParallel(ICollection<Province> provinces, Func<Province, int> func, ZoomControl zoomControl, PixelsOrBorders pixelsOrBorders)
   {
      Parallel.ForEach(provinces, province => // Cpu core affinity?
      {
         switch (pixelsOrBorders)
         {
            case PixelsOrBorders.Pixels:
               DrawPixels(province.Pixels, func.Invoke(province), zoomControl);
               break;
            case PixelsOrBorders.Borders:
               DrawPixels(province.Borders, func.Invoke(province), zoomControl);
               break;
            case PixelsOrBorders.Both:
               DrawPixels(province.Pixels, func.Invoke(province), zoomControl);
               DrawPixels(province.Borders, func.Invoke(province), zoomControl);
               break;
         }
      });
   }
   // Invalidation rects needs to bet taken care of
   private static void DrawProvincesParallel(ICollection<Province> provinces, int color, ZoomControl zoomControl, PixelsOrBorders pixelsOrBorders)
   {
      Parallel.ForEach(provinces, province => // Cpu core affinity?
      {
         switch (pixelsOrBorders)
         {
            case PixelsOrBorders.Pixels:
               DrawPixels(province.Pixels, color, zoomControl);
               break;
            case PixelsOrBorders.Borders:
               DrawPixels(province.Borders, color, zoomControl);
               break;
            case PixelsOrBorders.Both:
               DrawPixels(province.Pixels, color, zoomControl);
               DrawPixels(province.Borders, color, zoomControl);
               break;
         }
      });
   }


   /// <summary>
   /// 32 bpp
   /// </summary>
   /// <param name="points"></param>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   private static void DrawPixels(Point[] points, int color, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      // Calculate necessary values
      var stride = bmp.bmWidthBytes / 4;
      var tempHeight = (bmp.bmHeight - 1) * stride;

      unsafe
      {
         var scan0 = (int*)bmp.bmBits;

         foreach (var point in points)
         {
            // Calculate pixel address in the bitmap
            var pixelAddress = (scan0 + tempHeight - point.Y * stride + point.X);
            // Write the full color int (ARGB) directly to the pixel address
            *pixelAddress = color;
         }
      }
   }

   /// <summary>
   /// 32 bpp
   /// </summary>
   /// <param name="points"></param>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   private static void DrawPixelsParallel(Point[] points, int color, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      // Calculate necessary values
      var stride = bmp.bmWidthBytes / 4;
      var tempHeight = (bmp.bmHeight - 1) * stride;

      unsafe
      {
         var scan0 = (int*)bmp.bmBits;

         Parallel.ForEach(points, point =>
         {
            // Calculate pixel address in the bitmap
            var pixelAddress = (scan0 + tempHeight - point.Y * stride + point.X);
            // Write the full color int (ARGB) directly to the pixel address
            *pixelAddress = color;
         });
      }
   }

   public static void Clear(ZoomControl zoomControl, Color color)
   {
      DrawOnMap(Globals.Provinces, color.ToArgb(), zoomControl, PixelsOrBorders.Both);
   }

   // --------------- Additional methods --------------- \\

   public static void DrawCapitals(List<Province> ids)
   {
   }
   public static void DrawAllCapitals()
   {
   }

   public static void DrawAllCapitals(int color)
   {
   }

   public static void DrawStripes(int color, List<Province> provinces, ZoomControl zoomControl) //Point[] stripes
   {
      foreach (var province in provinces)
      {
         Geometry.GetStripesArray(province, out var points);
         DrawOnMap(points, color, zoomControl);
      }
   }

   private static void DrawCapital(ref Graphics g, Point provinceCenter)
   {
      g.DrawRectangle(new(Color.Black, 1), provinceCenter.X - 2, provinceCenter.Y - 2, 4, 4);
      g.DrawRectangle(Pens.Yellow, provinceCenter.X - 1, provinceCenter.Y - 1, 2, 2);
   }

   public static void DrawOccupations(bool rebelsOnly, ZoomControl zoomControl)
   {
      foreach (var province in Globals.LandProvinces)
      {
         if (rebelsOnly && !province.HasRevolt) // Has no rebels but we only want to show rebels
            continue;
         if (!rebelsOnly && province is { IsNonRebelOccupied: false, HasRevolt: false }) // has neither rebels nor occupation, but we want to show some
            continue;

         if (!Geometry.GetIfHasStripePixels(province, rebelsOnly, out var stripePixels))
            continue;

         DrawOnMap(stripePixels, province.GetOccupantColor, zoomControl);
      }
   }

   public static void WriteOnProvince(Func<Province, string> textProvider)
   {

   }
}