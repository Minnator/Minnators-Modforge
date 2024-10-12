namespace Editor.DataClasses.GameDataClasses
{
   public class TradeNode(string name, int location)
   {
      public string Name { get; set; } = name;
      public int Location { get; set; } = location;
      public Color Color { get; set; } = Color.Empty;
      public bool IsInland { get; set; } = false;
      public HashSet<Province> Members { get; set; } = [];
      public List<string> Incoming { get; set; } = [];
      public List<string> Outgoing { get; set; } = [];
      public static TradeNode Empty => new ("", 0);

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
}