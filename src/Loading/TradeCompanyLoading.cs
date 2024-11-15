using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   [Loading]
   public static class TradeCompanyLoading
   {

      public static void Load()
      {
         var sw = Stopwatch.StartNew();

         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "trade_companies");


         Dictionary<string, TradeCompany> tradeCompanies = [];

         foreach (var file in files)
         {
            Dictionary<string, TradeCompany> tradeCompaniesInternal = [];
            var pathObj = NewPathObj.FromPath(file);
            ParseTradeCompabies(IO.ReadAllInUTF8(file), ref pathObj, ref tradeCompaniesInternal);

            SaveMaster.AddRangeToDictionary(pathObj, tradeCompaniesInternal.Values);
            foreach (var tcp in tradeCompaniesInternal)
               if (!tradeCompanies.TryAdd(tcp.Key, tcp.Value))
                  Globals.ErrorLog.Write($"Trade Company {tcp.Key} already exists in the dictionary");
         }

         Globals.TradeCompanies = tradeCompanies;

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Trade Companies", sw.ElapsedMilliseconds);
      }

      private static void ParseTradeCompabies(string rawContent, ref NewPathObj pathObj, ref Dictionary<string, TradeCompany> tradeCompanies)
      {

         Parsing.RemoveCommentFromMultilineString(rawContent, out var content);
         var elements = Parsing.GetElements(0, content);

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

            TradeCompany tradeCompany = new(block.Name, genericName, specificName, provinces, color);
            tradeCompany.SetPath(ref pathObj);
            if (!tradeCompanies.TryAdd(block.Name, tradeCompany)) 
               Globals.ErrorLog.Write($"Trade Company {block.Name} already exists in the dictionary");
         }

      }
   }
}