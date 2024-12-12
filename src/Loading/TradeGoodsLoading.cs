using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   //=================================================================================================================
   // ALSO CONTAINS PRICE LOADING
   //=================================================================================================================

   
   public static class TradeGoodsLoading
   {

      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "tradegoods");

         foreach (var file in files)
         {
            ParseTradeGoodsFromFile(IO.ReadAllInUTF8(file));
         }
      }

      private static void ParseTradeGoodsFromFile(string content)
      {
         Parsing.RemoveCommentFromMultilineString(content, out var removed);
         var elements = Parsing.GetElements(0, removed);

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Cant parse Tradegood: Element is not a block: {((Content)element)}");
               continue;
            }
            
            var color = block.GetBlockWithName("color");
            if (color is null)
            {
               Globals.ErrorLog.Write($"Color is missing in Tradegood: {block.Name}");
               continue;
            }

            var tradeGood = new TradeGood(block.Name, Parsing.ParseColorPercentile(color.GetContent));
            if (!Globals.TradeGoods.TryAdd(tradeGood.Name, tradeGood))
               Globals.ErrorLog.Write($"TradeGood already exists: {tradeGood.Name}");
         }
      }

   }
}