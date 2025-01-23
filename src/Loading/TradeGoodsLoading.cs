using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Parser;
using Editor.Saving;
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
            EnhancedTradeGoodParse(file);

         files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "prices");

         foreach (var file in files)
            EnhancedPriceParse(file);
      }

      private static void EnhancedPriceParse(string fileName)
      {
         var po = PathObj.FromPath(fileName);
         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

         foreach (var block in blocks)
         {
            var price = EnhancedParsing.GetPriceFromBlock(block, po);
            price.SetPath(ref po);
         }
      }

      private static void EnhancedTradeGoodParse(string filePath)
      {
         var po = PathObj.FromPath(filePath);
         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

         foreach (var block in blocks)
         {
            var tg = EnhancedParsing.GetTradeGoodFromBlock(block, po);
            tg.SetPath(ref po);
            if (!Globals.TradeGoods.TryAdd(tg.Name, tg)) 
               _ = new LoadingError(po, $"TradeGood \"{tg.Name}\" already exists!", block.StartLine, type: ErrorType.DuplicateObjectDefinition);
         }
      }
   }
}