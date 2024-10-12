using Editor.Controls;

namespace Editor.Helper;
using System;
using System.Runtime.InteropServices;
using Editor.DataClasses.GameDataClasses;
using static GDIHelper;

public enum PixelsOrBorders
{
   Pixels,
   Borders
}


public class MapDrawing
{
   public static void DrawOnMap(Point[] points, int color)
   {
      if (points.Length > 4000)
         DrawPixelsParallel(points, color);
      else
         DrawPixels(points, color);
   }

   public static void DrawOnMap(ICollection<Province> provinces, Func<Province, int> func, PixelsOrBorders pixelOrBorders)
   {
      if (provinces.Count == 0)
         return;

      if (provinces.Count > Environment.ProcessorCount)
         DrawProvincesParallel(provinces, func, pixelOrBorders);
      else
         foreach (var province in provinces)
            if (pixelOrBorders == PixelsOrBorders.Pixels)
               DrawPixels(province.Pixels, func(province));
            else
               DrawPixels(province.Borders, func(province));
   }

   public static void DrawOnMap(ICollection<Province> provinces, int color, PixelsOrBorders pixelOrBorders)
   {
      if (provinces.Count == 0)
         return;

      if (provinces.Count > Environment.ProcessorCount)
         DrawProvincesParallel(provinces, color, pixelOrBorders);
      else
         foreach (var province in provinces)
            if (pixelOrBorders == PixelsOrBorders.Pixels)
               DrawPixels(province.Pixels, color);
            else
               DrawPixels(province.Borders, color);
   }

   public static void DrawOnMap(Province province, int color, PixelsOrBorders pixelOrBorders)
   {
      if (pixelOrBorders == PixelsOrBorders.Pixels)
         DrawPixels(province.Pixels, color);
      else
         DrawPixels(province.Borders, color);
   }


   /// <summary>
   /// BGRA fromat
   /// </summary>
   /// <param name="color"></param>
   public static void DrawAllBorders(int color)
   {
      DrawPixelsParallel(Globals.BorderPixels, color);
   }

   // Invalidation rects needs to bet taken care of
   private static void DrawProvincesParallel(ICollection<Province> provinces, Func<Province, int> func, PixelsOrBorders pixelsOrBorders)
   {
      Parallel.ForEach(provinces, province => // Cpu core affinity?
      {
         if (pixelsOrBorders == PixelsOrBorders.Pixels)
            DrawPixels(province.Pixels, func.Invoke(province));
         else
            DrawPixels(province.Borders, func.Invoke(province));
      });
   }
   // Invalidation rects needs to bet taken care of
   private static void DrawProvincesParallel(ICollection<Province> provinces, int color, PixelsOrBorders pixelsOrBorders)
   {
      Parallel.ForEach(provinces, province => // Cpu core affinity?
      {
         if (pixelsOrBorders == PixelsOrBorders.Pixels)
            DrawPixels(province.Pixels, color);
         else
            DrawPixels(province.Borders, color);
      });
   }

   
   /// <summary>
   /// 32 bpp
   /// </summary>
   /// <param name="points"></param>
   /// <param name="color"></param>
   private static void DrawPixels(Point[] points, int color)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(Globals.ZoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

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
   private static void DrawPixelsParallel(Point[] points, int color)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(Globals.ZoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

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

   public static void DrawCapitals(List<Province> ids)
   {

   }
   public static void DrawAllCapitals()
   {

   }

   public static void DrawAllCapitals(int color)
   {
      //TODO: Implement
   }

   public static void DrawStripes(int color, List<Province> provinces) //Point[] stripes
   {
      //TODO: Implement
   }

   public static void DrawOccupations(bool rebelsOnly)
   {

   }

   public static void WriteOnProvince(Func<Province, string> method)
   {

   }
}