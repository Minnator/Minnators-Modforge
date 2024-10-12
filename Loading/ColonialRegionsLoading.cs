using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class ColonialRegionsLoading
   {
      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         if (!FilesHelper.GetFilesUniquelyAndCombineToOne(out var rawContent, "common", "colonial_regions"))
         {
            Globals.ErrorLog.Write("Error: No files for colonial_regions found!");
            return;
         }

         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var content);
         var colonialRegionBlocks = Parsing.GetElements(0, content);

         foreach (var regionElement in colonialRegionBlocks)
         {
            if (regionElement is not Block regionBlock)
            {
               Globals.ErrorLog.Write($"Error: Illegal Content found in {((Content)regionElement).Value}");
               continue;
            }

            var region = new ColonialRegion(regionBlock.Name);

            foreach (var block in regionBlock.Blocks)
            {
               if (block is not Block b)
               {
                  AssignAttributes(region, (Content)block);         
                  continue;
               }

               switch (b.Name)
               {
                  case "color":
                     if (!Parsing.TryParseColor(b.GetContent, out var color))
                     {
                        Globals.ErrorLog.Write($"Error: Illegal Color found in {b.Name}");
                        continue;
                     }
                     region.Color = color;
                     break;
                  case "cultures":
                     Parsing.GetChancesListFromKeyValuePairs(Parsing.GetKeyValueList(b.GetContent), out var cultureChances);
                     region.Cultures = cultureChances;
                     break;
                  case "religions":
                     Parsing.GetChancesListFromKeyValuePairs(Parsing.GetKeyValueList(b.GetContent), out var religionChances);
                     region.Religions = religionChances;
                     break;
                  case "trade_goods":
                     Parsing.GetChancesListFromKeyValuePairs(Parsing.GetKeyValueList(b.GetContent), out var tradeGoodChances);
                     region.TradeGoods = tradeGoodChances;
                     break;
                  case "provinces":
                     region.Provinces = [..Parsing.GetProvincesFromString(b.GetContent)];
                     break;
                  case "names":
                     Parsing.ParseTriggeredName(b, out var tn);
                     region.Names.Add(tn);
                     break;
               }
            }
            if (!Globals.ColonialRegions.TryAdd(region.Name, region))
               Globals.ErrorLog.Write($"Error: Colonial Region {region.Name} already exists!");
         }

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Colonial Regions", sw.ElapsedMilliseconds);
      }

      private static void AssignAttributes(ColonialRegion cr, Content content)
      {
         Parsing.GetSimpleKvpArray(content.Value, out var kvps);
         for (var i = 0; i < kvps.Length; i += 2)
         {
            switch (kvps[i])
            {
               case "tax_income":
                  if (!int.TryParse(kvps[i + 1], out var taxIncome))
                  {
                     Globals.ErrorLog.Write($"Error: Illegal Tax Income found in {content.Value}");
                     continue;
                  }
                  cr.TaxIncome = taxIncome;
                  break;
               case "native_size":
                  if (!int.TryParse(kvps[i + 1], out var nativeSize))
                  {
                     Globals.ErrorLog.Write($"Error: Illegal Native Size found in {content.Value}");
                     continue;
                  }
                  cr.NativeSize = nativeSize;
                  break;
               case "native_ferocity":
                  if (!int.TryParse(kvps[i + 1], out var nativeFerocity))
                  {
                     Globals.ErrorLog.Write($"Error: Illegal Native Ferocity found in {content.Value}");
                     continue;
                  }

                  cr.NativeFerocity = nativeFerocity;
                  break;
               case "native_hostileness":
                  if (!int.TryParse(kvps[i + 1], out var nativeHostileness))
                  {
                     Globals.ErrorLog.Write($"Error: Illegal Native Hostileness found in {content.Value}");
                     continue;
                  }
                  cr.NativeHostileness = nativeHostileness;
                  break;
            }
         }
      }
   }
}