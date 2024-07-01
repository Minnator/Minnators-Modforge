using System;
using Editor.Helper;

namespace Editor.DataClasses;

public enum TradeGood
{
   Unknown = 0,
   Iron,
   Cloth,
   Fish,
   Fur,
   Grain,
   Livestock,
   Naval_Supplies,
   Salt,
   Wine,
   Wool,
   Copper,
   Ivory,
   Slaves,
   Chinaware,
   Spices,
   Tea,
   Cocoa,
   Coffee,
   Cotton,
   Sugar,
   Tobacco,
   Dyes,
   Silk,
   Tropical_Wood,
   Incense,
   Glass,
   Paper,
   Gems,
   Coal,
   Cloves,
   Gold
}

public static class TradeGoodHelper
{
   public static TradeGood FromString (string str)
   {
      if (Enum.TryParse<TradeGood>(str, true, out var value))
         return value;
      throw new AttributeParsingException( $"TradeGood {str} not found");
   }
}
