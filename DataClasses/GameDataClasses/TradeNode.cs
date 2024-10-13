using Editor.Helper;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeNode(string name, Province location)
   {
      public string Name { get; } = name;
      public Province Location { get; set; } = location;
      public Color Color { get; set; } = Color.Empty;
      public bool IsInland { get; set; } = false;
      public HashSet<Province> Members { get; set; } = [];
      public List<string> Incoming { get; set; } = [];
      public List<Outgoing> Outgoing { get; set; } = [];
      public static TradeNode Empty => new ("", Province.Empty);

      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TradeNode node)
            return Name == node.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }

   public class Outgoing(string target)
   {
      public string Target { get; set; } = target;
      public List<int> Path { get; set; } = [];
      public List<PointF> Control { get; set; } = [];
      public int SplitIndex = -1;
      public Rectangle Bounds { get; set; } = Rectangle.Empty;

      public void CalculateSplitIndex()
      {
      
      }

      public (List<PointF>, List<PointF>) GetSplitRoutes;

      public void CalculateBounds()
      {
         Bounds = Geometry.GetBoundsFloat(Control);
      }
   }
}