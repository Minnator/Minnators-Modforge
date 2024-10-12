using System.Diagnostics;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class TradeCompanyLoading
   {

      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         if(!FilesHelper.GetFilesUniquelyAndCombineToOne(out var rawContent, "common", "trade_companies"))
         {
            Globals.ErrorLog.Write("Error: No file for trade_companies was found");
            return;
         }

         Parsing.RemoveCommentFromMultilineString(rawContent, out var content);
         var elements = Parsing.GetElements(0, content);
         Dictionary<string, TradeCompany> tradeCompanies = [];

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Forbidden content in trade_companies: {((Content)element)}");
               continue;
            }

            var color = Color.Empty;
            List<Province> provinces = [];
            var genericName = string.Empty;
            var specificName = string.Empty;

            foreach (var subBlock in block.Blocks)
            {
               if (subBlock is not Block suBlock)
               {
                  Globals.ErrorLog.Write($"Forbidden content in trade_companies: {((Content)subBlock)}");
                  continue;
               }
               switch (suBlock.Name)
               {
                  case "color":
                     if (!Parsing.TryParseColor(suBlock.GetContent, out color))
                     {
                        Globals.ErrorLog.Write($"Color not found for {suBlock.Name}");
                        return;
                     }
                     break;
                  case "provinces":
                     provinces = Parsing.GetProvincesFromString(suBlock.GetContent);
                     break;
                  case "names":
                     if (suBlock.GetContent.Contains("Root_Culture"))
                     {
                        specificName = suBlock.GetContent;
                        break;
                     }
                     genericName = suBlock.GetContent;
                     break;
               }
            }

            if (!tradeCompanies.TryAdd(block.Name, new(block.Name, genericName, specificName, provinces, color)))
            {
               Globals.ErrorLog.Write($"Trade Company {block.Name} already exists in the dictionary");
            }
         }

         Globals.TradeCompanies = tradeCompanies;

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Trade Companies", sw.ElapsedMilliseconds);
      }

   }
}