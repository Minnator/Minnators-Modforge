using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class DebugMapMode : MapMode
   {
#if DEBUG
      public override MapModeType MapModeType { get; } = MapModeType.DEBUG;
#else
      public override MapModeType MapModeType { get; } = MapModeType.Area;
#endif

      public override void RenderMapMode()
      {
         var centers = new List<Point>();
         var lines = new HashSet<Point>();
         foreach (var prov in Globals.Provinces)
         {
            var center = prov.Center;
            AddCirclePoints(center, 2, centers);
             if(!Globals.AdjacentProvinces.TryGetValue(prov, out var adjacs))
               continue;
            foreach (var adjac in adjacs)
            {
               AddLinePoints(center, adjac.Center, 1, lines, Globals.MapWidth, Globals.MapHeight, 1);
            }
         }

         MapDrawing.DrawOnMap(new(lines.ToArray()),unchecked((int)0xFFFFFFFF),Globals.ZoomControl);
         MapDrawing.DrawOnMap(new(centers.ToArray()), unchecked((int)0xFFFF0000), Globals.ZoomControl);
         //MapDrawing.DrawAllBorders(Color.Black.ToArgb(), Globals.ZoomControl);
         //Selection.RePaintSelection();
         Globals.ZoomControl.Invalidate();
         MapModeManager.PreviousLandOnly = IsLandOnly;
      }

      private static void AddCirclePoints(Point center, int radius, List<Point> pixelPoints)
      {
         for (int y = -radius; y <= radius; y++)
         {
            var yCoord = center.Y + y;
            if (!ClampYValue(yCoord))
               continue;
            for (int x = -radius; x <= radius; x++)
            {
               var xCoord = ClampXValue(center.X + x);
               pixelPoints.Add(new Point(xCoord, yCoord));
            }
         }
      }

      private static int ClampXValue(int value)
      {
         if (value < 0)
            return Globals.MapWidth + value;
         else if (value >= Globals.MapWidth)
            return value - Globals.MapWidth;
         return value;
      }

      private static bool ClampYValue(int value)
      {
         return value >= 0 && value < Globals.MapHeight;
      }

      private static void DrawLine(Point start, Point end, int width, HashSet<Point> pixelPoints)
      {
         int dxLine = Math.Abs(end.X - start.X), dyLine = Math.Abs(end.Y - start.Y);
         int sx = start.X < end.X ? 1 : -1, sy = start.Y < end.Y ? 1 : -1;
         int err = dxLine - dyLine, e2;

         int x = start.X, y = start.Y;

         while (true)
         {
            // Add a rectangle of width `width` around each point in the line
            for (int i = -width / 2; i <= width / 2; i++)
            {
               var xCoord = x + i;
               if (xCoord < 0 || xCoord >= Globals.MapWidth)
                  continue;
               for (int j = -width / 2; j <= width / 2; j++)
               {
                  var yCoord = y + j;
                  if (yCoord < 0 || yCoord >= Globals.MapHeight)
                     continue;
                  pixelPoints.Add(new Point(xCoord, yCoord));
               }
            }

            if (x == end.X && y == end.Y) break;
            e2 = 2 * err;
            if (e2 > -dyLine)
            {
               err -= dyLine;
               x += sx;
            }
            if (e2 < dxLine)
            {
               err += dxLine;
               y += sy;
            }
         }

      }

      private static void DrawLineWithOffset(Point start, Point end, int width, int offset, HashSet<Point> pixelPoints)
      {
         double dx = end.X - start.X;
         double dy = end.Y - start.Y;

         // Normalize direction vector
         double length = Math.Sqrt(dx * dx + dy * dy);
         if (length == 0) return; // Avoid division by zero
         double unitDx = dx / length;
         double unitDy = dy / length;

         // Perpendicular offset vector (rotated by 90 degrees)
         int offsetDx = (int)Math.Round(-unitDy * offset);
         int offsetDy = (int)Math.Round(unitDx * offset);

         start.X += offsetDx;
         start.Y += offsetDy;
         end.X += offsetDx;
         end.Y += offsetDy;

         DrawLine(start, end, width, pixelPoints);
      }

      private static void AddLinePoints(Point start, Point end, int width, HashSet<Point> pixelPoints, int mapWidth, int mapHeight, int offset)
      {

         var xDir = end.X - start.X;

         if (xDir > Globals.MapWidth / 2)
         {
            end.X -= Globals.MapWidth;
            DrawLineWithOffset(start, end, width, offset, pixelPoints);
            end.X += Globals.MapWidth;
            start.X += Globals.MapWidth;
            DrawLineWithOffset(start, end, width, offset, pixelPoints);
         }
         else if(xDir < - Globals.MapWidth / 2)
         {
            end.X += Globals.MapWidth;
            DrawLineWithOffset(start, end, width, offset, pixelPoints);
            end.X -= Globals.MapWidth;
            start.X -= Globals.MapWidth;
            DrawLineWithOffset(start, end, width, offset, pixelPoints);
         }
         else
         {
            DrawLineWithOffset(start, end, width, offset, pixelPoints);
         }

      }

      private static int WrapCoordinate(int value, int max)
      {
         return (value + max) % max;
      }

      private static int ShortestDelta(int start, int end, int max)
      {
         int delta = end - start;
         if (delta > max / 2)
            delta -= max;
         else if (delta < -max / 2)
            delta += max;
         return delta;
      }

      public override int GetProvinceColor(Province id)
      {
         return id.Color.ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         return "Debug Map Mode";
      }

   }
}