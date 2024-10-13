using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;

namespace Editor.Helper
{
   public static class GuiDrawing
   {

      private static List<Province> _visibleProvinces = [];

      private class MapModePaintEventArgs : EventArgs
      {

         public Graphics Graphics { get; set; }
         public Rectangle ClipRectangle { get; set; }
         public ICollection<Province> VisibleProvinces { get; set; }
         public MapModePaintEventArgs(Graphics graphics, Rectangle clipRectangle, ICollection<Province> visibleProvinces)
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
         {
            _mapModePaint += OnMapModePaintTradeRoutes;
         }
      }

      private static void RemoveEvents(GuiElements elements)
      {
         if (elements.HasFlag(GuiElements.TradeRoutes))
         {
            _mapModePaint -= OnMapModePaintTradeRoutes;
         }
      }

      private static void GuiPaint(object? _, PaintEventArgs e)
      {
         // TODO Add Calculation
         MapModePaintEventArgs eventArgs = new(e.Graphics, Globals.ZoomControl.MapRectangle, _visibleProvinces);
         _mapModePaint.Invoke(Globals.ZoomControl, eventArgs);
      }

      private static void OnImagePositionChanged(object? sender, ImagePositionEventArgs e)
      {
         _visibleProvinces = Geometry.GetProvincesInRectangle(e.NewRectangle);
      }

      private static void OnMapModePaintTradeRoutes(object _, MapModePaintEventArgs e)
      {
         foreach (var node in Globals.TradeNodes.Values)
         {
            foreach (var outgoing in node.Outgoing)
            {
               if (outgoing.Control.Count < 3)
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

   }
}