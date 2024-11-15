using System.Text;

namespace Editor.DataClasses.GameDataClasses;

public class TradeGood(string name, Color color)
{
   public string Name { get; } = name;
   public float Price { get; set; } = 0;
   public float BasePrice { get; set; } = 0;
   public Color Color { get; set; } = color;
   public static TradeGood Empty { get; } = new ("", Color.Empty);


   public override bool Equals(object? obj)
   {
      if (obj is TradeGood other)
         return Name == other.Name;
      return false;
   }

   public override int GetHashCode()
   {
      return Name.GetHashCode();
   }

   public override string ToString()
   {
      return $"{Name} -> {Price} - {Color}";
   }
}

public static class TradeGoodHelper
{
   public static bool IsTradeGood(string str)
   {
      return Globals.TradeGoods.ContainsKey(str);
   }

   public static TradeGood StringToTradeGood(string str)
   {
      if (Globals.TradeGoods.TryGetValue(str, out var tradeGood))
         return tradeGood;
      if (str == string.Empty)
         return TradeGood.Empty;
      Globals.ErrorLog.Write($"Can not find tradegood {str}");
      return TradeGood.Empty;
   }
   public static void DumpTradeGoods(string folderPath)
   {
      var sb = new StringBuilder();
      foreach (var tradeGood in Globals.TradeGoods.Values)
      {
         sb.AppendLine(tradeGood.ToString());
      }

      File.WriteAllText(Path.Combine(folderPath, "tradegoods_dump.txt"), sb.ToString());
   }
}
