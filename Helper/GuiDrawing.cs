using System.Diagnostics;
using System.Drawing.Drawing2D;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.Helper
{
   public static class GuiDrawing
   {
      private static Pen _straitPen = new(Color.Red, 2) { DashStyle = DashStyle.Dash };
      public static HashSet<Province> VisibleProvinces = [];

      private class MapModePaintEventArgs : EventArgs
      {

         public Graphics Graphics { get; set; }
         public Rectangle ClipRectangle { get; set; }
         public HashSet<Province> VisibleProvinces { get; set; }
         public MapModePaintEventArgs(Graphics graphics, Rectangle clipRectangle, HashSet<Province> visibleProvinces)
         {
            Graphics = graphics;
            ClipRectangle = clipRectangle;
            VisibleProvinces = visibleProvinces;
         }
      }

      private static EventHandler<MapModePaintEventArgs> _mapModePaint = delegate { };

      [Flags]
      public enum GuiElements
      {
         None = 0,
         TradeRoutes = 0b1,
         Captitals = 0b10,
         Straits = 0b100
      }

      private static GuiElements _currentElements = GuiElements.None;

      public static GuiElements CurrentElements
      {
         get => _currentElements;
         set 
         {
            var toRemove = _currentElements &~ value;
            var toAdd = ~_currentElements & value;

            AddEvents(toAdd);
            RemoveEvents(toRemove);

            _currentElements = value;
         }
      }

      public static void Initialize()
      {
         Globals.ZoomControl.Paint += GuiPaint;
         Globals.ZoomControl.ImagePositionChange += OnImagePositionChanged;
      }

      public static void AddOrRemoveGuiElement(GuiElements elements, bool add)
      {
         if (add)
            AddGuiElements(elements);
         else
            RemoveGuiElements(elements);
      }

      public static void AddGuiElements(GuiElements elements)
      {
         CurrentElements |= elements;
      }

      public static void RemoveGuiElements(GuiElements elements)
      {
         CurrentElements &= ~elements;
      }

      private static void AddEvents(GuiElements elements)
      {
         if (elements.HasFlag(GuiElements.TradeRoutes)) 
            _mapModePaint += OnMapModePaintTradeRoutes;
         if (elements.HasFlag(GuiElements.Captitals))
            _mapModePaint += OnMapModePaintCapitals;
         if (elements.HasFlag(GuiElements.Straits))
            _mapModePaint += OnMapModePaintStraits;
      }

      private static void RemoveEvents(GuiElements elements)
      {
         if (elements.HasFlag(GuiElements.TradeRoutes)) 
            _mapModePaint -= OnMapModePaintTradeRoutes;
         if (elements.HasFlag(GuiElements.Captitals)) 
            _mapModePaint -= OnMapModePaintCapitals;
         if (elements.HasFlag(GuiElements.Straits)) 
            _mapModePaint -= OnMapModePaintStraits;
      }

      private static void GuiPaint(object? _, PaintEventArgs e)
      {
         // TODO Add Calculation
         MapModePaintEventArgs eventArgs = new(e.Graphics, Globals.ZoomControl.MapRectangle, VisibleProvinces);
         _mapModePaint.Invoke(Globals.ZoomControl, eventArgs);
      }

      private static void OnImagePositionChanged(object? sender, ImagePositionEventArgs e)
      {
         VisibleProvinces = Geometry.GetProvincesInRectangle(e.NewRectangle);
      }

      private static void OnMapModePaintTradeRoutes(object _, MapModePaintEventArgs e)
      {
         foreach (var node in Globals.TradeNodes.Values)
         {
            foreach (var outgoing in node.Outgoing)
            {
               if (outgoing.Control.Count < 3 || !Geometry.RectanglesIntercept(e.ClipRectangle, outgoing.Bounds))
                  continue;
               var points = new PointF[outgoing.Control.Count];
               for (var i = 0; i < outgoing.Control.Count; i++)
               {
                  var point = outgoing.Control[i];
                  //point = point with { Y = Globals.MapHeight - point.Y};
                  points[i] = (Globals.ZoomControl.ReverseCoordinateFloat(point));
               }

               e.Graphics.DrawCurve(Pens.BlanchedAlmond, points);
            }
         }
      }

      private static void OnMapModePaintCapitals(object _, MapModePaintEventArgs e)
      {
         var capitals = Geometry.GetVisibleCapitals(e.VisibleProvinces);
         foreach (var capital in capitals)
         {
            var provinceCenter = Globals.ZoomControl.ReverseCoordinate(capital.Center);
            e.Graphics.DrawRectangle(new(Color.Black, 1), provinceCenter.X - 2, provinceCenter.Y - 2, 4, 4);
            e.Graphics.DrawRectangle(Pens.Yellow, provinceCenter.X - 1, provinceCenter.Y - 1, 2, 2);
         }
      }

      private static void OnMapModePaintStraits(object _, MapModePaintEventArgs e)
      {
         foreach (var strait in Globals.Straits)
         {
            if (!e.VisibleProvinces.Contains(strait.From) || !e.VisibleProvinces.Contains(strait.To))
               continue;

            Point start;
            if (strait.Start.X == -1 || strait.Start.Y == -1) 
               start = Globals.ZoomControl.ReverseCoordinate(strait.From.Center);
            else
               start = Globals.ZoomControl.ReverseCoordinate(strait.Start);

            Point end;
            if (strait.End.X == -1 || strait.End.Y == -1) 
               end = Globals.ZoomControl.ReverseCoordinate(strait.To.Center);
            else
               end = Globals.ZoomControl.ReverseCoordinate(strait.End);

            e.Graphics.DrawLine(_straitPen, start, end);
         }
      }
   }
}