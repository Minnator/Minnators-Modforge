using System.Collections.Concurrent;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using System.Runtime.InteropServices;
using static Editor.Controls.GDIHelper;
using Windows.Media.Devices;
using Editor.DataClasses.Settings;
using ZoomControl = Editor.Controls.ZoomControl;

namespace Editor.Helper;

public enum PixelsOrBorders
{
   Pixels,
   Borders,
   Both
}


public static class MapDrawing
{

   public static void DrawOnMap(Memory<Point> points, int color, int testColor, ZoomControl zoomControl)
   {
      if (points.Length > 4000)
         DrawPixelsParallel(points, color, testColor, zoomControl);
      else
         DrawPixels(points, color, testColor, zoomControl);
   }

   public static void DrawOnMap(Memory<Point> points, int color, ZoomControl zoomControl)
   {
      if (points.Length > 4000)
         DrawPixelsParallel(points, color, zoomControl);
      else
         DrawPixels(points, color, zoomControl);
   }

   public static void DrawOnMap(ICollection<Province> provinces, ZoomControl zoomControl, PixelsOrBorders pixelOrBorders)
   {
      if (provinces.Count == 0)
         return;

      if (provinces.Count > Environment.ProcessorCount)
         DrawProvincesParallel(provinces, zoomControl, pixelOrBorders);
      else
         foreach (var province in provinces)
            switch (pixelOrBorders)
            {
               case PixelsOrBorders.Pixels:
                  DrawPixels(province.Pixels, MapModeManager.ColorCache[province], zoomControl);
                  break;
               case PixelsOrBorders.Borders:
                  if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                     DrawPixels(province.Borders, MapModeManager.ColorCache[province], zoomControl);
                  else
                     DrawPixelsMerged(province, MapModeManager.ColorCache[province], zoomControl);
                  break;
               case PixelsOrBorders.Both:
                  DrawPixels(province.Pixels, MapModeManager.ColorCache[province], zoomControl);
                  DrawPixels(province.Borders, MapModeManager.ColorCache[province], zoomControl);
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
                  if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                     DrawPixels(province.Borders, color, zoomControl);
                  else
                     DrawPixelsMerged(province, color, zoomControl);
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
            if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
               DrawPixels(province.Borders, color, zoomControl);
            else
               DrawPixelsMerged(province, color, zoomControl);
            break;
         case PixelsOrBorders.Both:
            DrawPixels(province.Pixels, color, zoomControl);
            DrawPixels(province.Borders, color, zoomControl);
            break;
      }
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
                  DrawPixels(province.Pixels, func.Invoke(province), zoomControl);
                  break;
               case PixelsOrBorders.Borders:
                  if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                     DrawPixels(province.Borders, func.Invoke(province), zoomControl);
                  else
                     DrawPixelsMerged(province, func.Invoke(province), zoomControl);
                  break;
               case PixelsOrBorders.Both:
                  DrawPixels(province.Pixels, func.Invoke(province), zoomControl);
                  DrawPixels(province.Borders, func.Invoke(province), zoomControl);
                  break;
            }
   }

   public static void DrawBordersWithoutMerge(Province province, int color, ZoomControl zoomControl)
   {
      DrawPixels(province.Borders, color, zoomControl);
   }

   public static void DrawBordersWithoutMerge(Province province, int color, int testColor, ZoomControl zoomControl)
   {
      DrawPixels(province.Borders, color, testColor, zoomControl);
   }


   public static void DrawBordersWithoutMerge(ICollection<Province> provinces, int color, ZoomControl zoomControl)
   {
      if (provinces.Count > Environment.ProcessorCount)
         Parallel.ForEach(provinces, province => // Cpu core affinity?
         {
            DrawPixels(province.Borders, color, zoomControl);
         });
      else
         foreach(var province in provinces)
            DrawPixels(province.Borders, color, zoomControl);
   }

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
               if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                  DrawPixels(province.Borders, func.Invoke(province), zoomControl);
               else
                  DrawPixelsMerged(province, func.Invoke(province), zoomControl);
               break;
            case PixelsOrBorders.Both:
               DrawPixels(province.Pixels, func.Invoke(province), zoomControl);
               DrawPixels(province.Borders, func.Invoke(province), zoomControl);
               break;
         }
      });
   }

   /// <summary>
   /// BGRA fromat
   /// </summary>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   public static void DrawAllBorders(int color, ZoomControl zoomControl)
   {
      Parallel.ForEach(Globals.Provinces, province =>
      {
         DrawPixelsMerged(province, color, zoomControl);
      });
   }

   // Invalidation rects needs to bet taken care of
   private static void DrawProvincesParallel(ICollection<Province> provinces, ZoomControl zoomControl, PixelsOrBorders pixelsOrBorders)
   {
      Parallel.ForEach(provinces, province => // Cpu core affinity?
      {
         switch (pixelsOrBorders)
         {
            case PixelsOrBorders.Pixels:
               DrawPixels(province.Pixels, MapModeManager.ColorCache[province], zoomControl);
               break;
            case PixelsOrBorders.Borders:
               if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                  DrawPixels(province.Borders, MapModeManager.ColorCache[province], zoomControl);
               else
                  DrawPixelsMerged(province, MapModeManager.ColorCache[province], zoomControl);
               break;
            case PixelsOrBorders.Both:
               DrawPixels(province.Pixels, MapModeManager.ColorCache[province], zoomControl);
               DrawPixels(province.Borders, MapModeManager.ColorCache[province], zoomControl);
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
               if (Globals.Settings.Rendering.Map.MergeBorders == RenderingSettings.BorderMergeType.None)
                  DrawPixels(province.Borders, color, zoomControl);
               else
                  DrawPixelsMerged(province, color, zoomControl);
               break;
            case PixelsOrBorders.Both:
               DrawPixels(province.Pixels, color, zoomControl);
               DrawPixels(province.Borders, color, zoomControl);
               break;
         }
      });
   }

   /// EXPERIMENTAL
   private static void DrawPixelsMerged(Province province, int color, ZoomControl zoomControl)
   {
      var provColor = MapModeManager.ColorCache[province];
      foreach (var neighbours in province.ProvinceBorders)
         if (provColor != MapModeManager.ColorCache[neighbours.Key])
            DrawPixels(neighbours.Value, color, zoomControl);
         else
            DrawPixels(neighbours.Value, provColor, zoomControl);
   }

   private static void DrawPixels(Memory<Point> points, int color, int testColor, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      // Calculate necessary values
      var stride = bmp.bmWidthBytes / 4;

      unsafe
      {
         var bmpBmHeight = (int*)bmp.bmBits + (bmp.bmHeight - 1) * stride;
         foreach (var point in points.Span)
         {
            var ptr = (bmpBmHeight - point.Y * stride + point.X);
            if (*ptr != testColor)
               *ptr = color;
         }
      }
   }

   private static void DrawPixelsParallel(Memory<Point> points, int color, int testColor, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      var stride = bmp.bmWidthBytes / 4;
      var tempHeight = (bmp.bmHeight - 1) * stride;

      unsafe
      {
         var scan0 = (int*)bmp.bmBits;
         var scan0Height = scan0 + tempHeight;
         Parallel.ForEach(
            Partitioner.Create(0, points.Length),
            range =>
            {
               var sliceSpan = points.Slice(range.Item1, range.Item2 - range.Item1).Span;
               for (var i = 0; i < sliceSpan.Length; i++)
               {
                  var ptr = (scan0Height - sliceSpan[i].Y * stride + sliceSpan[i].X);
                  if (*ptr != testColor)
                     *ptr = color;
               }

            }
         );
      }
   }

   /// <summary>
   /// 32 bpp
   /// </summary>
   /// <param name="points"></param>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   private static void DrawPixels(Memory<Point> points, int color, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      // Calculate necessary values
      var stride = bmp.bmWidthBytes / 4;

      unsafe
      {
         var bmpBmHeight = (int*)bmp.bmBits + (bmp.bmHeight - 1) * stride;
         foreach (var point in points.Span) 
            *(bmpBmHeight - point.Y * stride + point.X) = color;
      }
   }

   /// <summary>
   /// 32 bpp
   /// </summary>
   /// <param name="points"></param>
   /// <param name="color"></param>
   /// <param name="zoomControl"></param>
   private static void DrawPixelsParallel(Memory<Point> points, int color, ZoomControl zoomControl)
   {
      // Check if the bitmap is still in RAM at the same location. Can maybe be removed?
      BITMAP bmp = new();
      GetObject(zoomControl.HBitmap, Marshal.SizeOf(bmp), ref bmp);

      var stride = bmp.bmWidthBytes / 4;
      var tempHeight = (bmp.bmHeight - 1) * stride;

      unsafe
      {
         var scan0 = (int*)bmp.bmBits;
         var scan0Height = scan0 + tempHeight;
         Parallel.ForEach(
            Partitioner.Create(0, points.Length),  
            range =>
            {
               var sliceSpan = points.Slice(range.Item1, range.Item2 - range.Item1).Span; 
               for (var i = 0; i < sliceSpan.Length; i++) 
                  *(scan0Height - sliceSpan[i].Y * stride + sliceSpan[i].X) = color;
            }
         );
      }
   }

   public static void Clear(ZoomControl zoomControl, Color color)
   {
      DrawOnMap(Globals.Provinces, color.ToArgb(), zoomControl, PixelsOrBorders.Both);
   }

   public static bool ShouldNotMerge(Province province1, Province province2) => false;
   public static bool ShouldMerge(Province province1, Province province2) => true;

   // --------------- Additional methods --------------- \\


   public static void DrawStripes(int color, List<Province> provinces, ZoomControl zoomControl) //Point[] stripes
   {
      foreach (var province in provinces)
      {
         Geometry.GetStripesArray(province, out var points);
         DrawOnMap(points, color, zoomControl);
      }
   }

   public static void DrawRivers()
   {
      foreach (var river in Globals.Rivers) 
         DrawOnMap(river.Value, river.Key, Globals.ZoomControl);
   }

   public static void DrawOccupations(bool rebelsOnly, ZoomControl zoomControl)
   {
      foreach (var province in Globals.LandProvinces)
         DrawOccupation(province, rebelsOnly, zoomControl);
   }

   public static void DrawOccupation(Province province, bool rebelsOnly, ZoomControl zoomControl)
   {
      if (rebelsOnly && !province.HasRevolt) // Has no rebels but we only want to show rebels
         return;
      if (!rebelsOnly && province is { IsNonRebelOccupied: false, HasRevolt: false }) // has neither rebels nor occupation, but we want to show some
         return;

      if (!Geometry.GetIfHasStripePixels(province, rebelsOnly, out var stripePixels))
         return;

      DrawOnMap(stripePixels, province.OccupantColor, zoomControl);
   }

}