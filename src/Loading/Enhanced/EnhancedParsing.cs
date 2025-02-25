using System.Globalization;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Parser;
using Editor.Saving;
using Region = Editor.DataClasses.Saveables.Region;

namespace Editor.Loading.Enhanced
{
   public static partial class EnhancedParsing
   {
      #region Regexes

      private static readonly Regex IntListRegex = IntListRegexGenerate();
      [GeneratedRegex(@"(\d+)")]
      private static partial Regex IntListRegexGenerate();

      #endregion

      public static Price GetPriceFromBlock(EnhancedBlock block, PathObj po)
      {
         var price = new Price(-1);

         if (block.ContentElements.Count != 1)
         {
            _ = new LoadingError(po, $"Expected 1 sub block but got {block.SubBlocks.Count}!", block.StartLine, 0, ErrorType.UnexpectedBlockElement);
            return price;
         }

         foreach (var kvp in block.ContentElements[0].GetLineKvpEnumerator(po))
         {
            if (!kvp.Key.Equals("base_price"))
            {
               _ = new LoadingError(po, $"Unknown attribute: {kvp.Key}!", block.StartLine, 0, ErrorType.UnexpectedAttribute);
               continue;
            }

            if (!float.TryParse(kvp.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
            {
               _ = new LoadingError(po, $"Expected a float but got {kvp.Value}!", block.StartLine, 0, ErrorType.UnexpectedDataType);
               continue;
            }
            price.Value = num;
            break;
         }

         var tg = Globals.TradeGoods.Values.Where(x => x.Name.Equals(block.Name)).ToArray();
         if (tg.Length != 1)
         {
            _ = new LoadingError(po, $"TradeGood \"{block.Name}\" not found!", block.StartLine, 0, ErrorType.UnresolveableTradeGoodReference);
            return price;
         }

         price.TradeGood = tg[0];
         return price;
      }

      public static TradeGood GetTradeGoodFromBlock(EnhancedBlock block, PathObj po)
      {
         var tg = new TradeGood(block.Name);
         foreach (var sBlock in block.SubBlocks)
         {
            switch (sBlock.Name)
            {
               case "color":
                  tg.Color = GetColorFromPercentage(sBlock, po);
                  break;
               case "modifier":
                  tg.Modifier = GetKeyValuePairsFromContent(sBlock.ContentElements, po);
                  break;
               case "province":
                  tg.ProvinceModifier = GetKeyValuePairsFromContent(sBlock.ContentElements, po);
                  break;
               case "chance":
                  tg.Chance = sBlock;
                  break;
               case "trigger":
                  tg.Trigger = sBlock;
                  break;
            }
         }

         foreach (var sElement in block.ContentElements)
         {
            foreach (var kvp in sElement.GetLineKvpEnumerator(po))
            {
               switch (kvp.Key)
               {
                  case "is_latent":
                     tg.IsLatent = IsTrue(kvp.Value, po);
                     break;
                  case "is_valuable":
                     tg.IsValuable = IsTrue(kvp.Value, po);
                     break;
                  case "rnw_latent_chance":
                     if (int.TryParse(kvp.Value, out var num))
                        tg.RNWLatentChance = num;
                     else
                        _ = new LoadingError(po, $"Expected an integer but got {kvp.Value}!", sElement.StartLine, 0, ErrorType.UnexpectedDataType);
                     break;
                  default:
                     _ = new LoadingError(po, $"Unknown attribute: {kvp.Key}!", sElement.StartLine, 0, ErrorType.UnexpectedAttribute);
                     break;
               }
            }
         }

         return tg;
      }

      public static bool IsTrue(string str, PathObj po)
      {
         if (str.Equals("yes", StringComparison.OrdinalIgnoreCase))
            return true;
         if (str.Equals("no", StringComparison.OrdinalIgnoreCase))
            return false;

         _ = new LoadingError(po, $"Expected yes or no but got {str}!", 0, 0, ErrorType.UnexpectedDataType);
         return false;
      }

      public static Monsoon? GetMonsoonFromBlock(EnhancedBlock block, PathObj po)
      {
         var contentElements = block.GetContentElements(true, po);
         if (contentElements.Count != 1)
         {
            _ = new LoadingError(po, $"Expected 1 content element but got {contentElements.Count}!",
               block.StartLine, 0, ErrorType.UnexpectedContentElement);
            return null;
         }

         var lines = GetStringListFromContent(contentElements[0], po);
         if (lines.Count != 2)
         {
            _ = new LoadingError(po, $"Expected 2 dates but got {lines.Count}!", block.StartLine, 0, ErrorType.UnexpectedContentElement);
            return null;
         }

         if (!Date.TryParse(lines[0], out var start).Then((error) => error.ConvertToLoadingError(po, string.Empty, contentElements[0].StartLine)))
            return null;

         if (!Date.TryParse(lines[1], out var end).Then((error) => error.ConvertToLoadingError(po, string.Empty, contentElements[0].StartLine + 1)))
            return null;
         
         return new (start, end);
      }

      public static List<KeyValuePair<string, string>> GetKeyValuePairsFromContent(List<EnhancedContent> content, PathObj po)
      {
         var pairs = new List<KeyValuePair<string, string>>();

         foreach (var cont in content)
         {
            foreach (var (line, num) in cont.GetLineEnumerator())
            {
               var lineElements = line.Split('=');

               if (lineElements.Length != 2)
               {
                  _ = new LoadingError(po, $"Expected 2 elements but got {lineElements.Length}!", num, 0, ErrorType.UnexpectedContentElement);
                  continue;
               }

               pairs.Add(new(lineElements[0].Trim(), lineElements[1].Trim().TrimQuotes()));
            }
         }

         return pairs;
      }

      public static List<string> GetStringListFromContent(EnhancedContent content, PathObj po)
      {
         var strings = new List<string>();

         foreach (var (line, num) in content.GetLineEnumerator())
         {
            var lineElements = line.Split(' ');

            foreach (var element in lineElements)
            {
               strings.Add(element);
            }
         }
         return strings;
      }

      public static List<Area> GetAreasFromBlock(EnhancedBlock block, PathObj po)
      {
         List<Area> areas = [];
         List<string> areaStrings = [];
         foreach (var content in block.GetContentElements(false, po)) 
            areaStrings.AddRange(GetStringListFromContent(content, po));

         foreach (var areaString in areaStrings)
         {
            if (Globals.Areas.TryGetValue(areaString, out var area))
               areas.Add(area);
            else
               _ = new LoadingError(po, $"Area \"{areaString}\" not found!", block.StartLine, 0, ErrorType.UnresolveableAreaReference);
         }

         return areas;
      }

      public static bool GetRegionFromString(string str, PathObj po, int startLine, out Region region)
      {

         if (Globals.Regions.TryGetValue(str, out region!))
            return true;
         _ = new LoadingError(po, $"Region \"{str}\" not found!", startLine, 0, ErrorType.UnresolveableRegionReference);
         return false;
      }

      public static HashSet<Province> GetProvincesFromString(EnhancedContent content, PathObj po)
      {
         var provinces = new HashSet<Province>();

         foreach (var (line, num) in content.GetLineEnumerator())
         {
            var lineElements = line.Split(' ', '\n');

            foreach (var element in lineElements)
            {
               if (!GetIntFromString(element, num, po, out var result))
                  continue;

               if (Globals.ProvinceIdToProvince.TryGetValue(result, out var province))
                  provinces.Add(province);
               else
                  _ = new LoadingError(po, $"Province id: \"{element}\" can not be resolved!", num, 0, ErrorType.UnresolveableProvinceReference);
            }
         }
         return provinces;
      }

      /// <summary>
      ///  Returns the attributes in the order of attributeName if there are any. If an attribute is not found it will be null.
      /// </summary>
      /// <param name="content"></param>
      /// <param name="po"></param>
      /// <param name="attributeName"></param>
      /// <returns></returns>
      public static string?[] GetAttributesFromContent(EnhancedContent content, PathObj po, params string[] attributeName)
      {
         var attributes = new string[attributeName.Length];

         foreach (var line in content.GetLineKvpEnumerator(po))
         {
            // find the index of the attribute in attributeName and set the value in attributes
            for (var i = 0; i < attributeName.Length; i++)
            {
               if (!line.Key.Equals(attributeName[i]))
                  continue;

               attributes[i] = line.Value;
               break;
            }
         }
         
         return attributes;
      }

      public static string?[] GetAttributesFromContentElements(List<EnhancedContent> contents, PathObj po,
         params string[] attributeName)
      {
         var attributes = new string?[attributeName.Length];

         foreach (var content in contents)
         {
            var tempAttributes = GetAttributesFromContent(content, po, attributeName);

            for (var i = 0; i < attributeName.Length; i++)
            {
               if (tempAttributes[i] is null)
                  continue;

               if (!string.IsNullOrEmpty(attributes[i]))
               {
                  _ = new LoadingError(po, $"Attribute \"{attributeName[i]}\" was defined multiple times in file: {po.GetPath()}!",
                     content.StartLine, 0, ErrorType.DuplicateAttributeDefinition);
                  continue;
               }

               attributes[i] = tempAttributes[i];
            }
         }

         return attributes;
      }

      public static bool GetIntFromString(string str, int lineNum, PathObj po, out int result)
      {
         if (!int.TryParse(str, out result))
         {
            _ = new LoadingError(po, $"{str} could not be parsed to an Integer!", lineNum, 0, ErrorType.UnexpectedDataType);
            return false;
         }
         return true;
      }

      public static List<int> GetListIntFromString(string line, int lineNum, PathObj po)
      {
         var lineElements = line.Split(' ');
         List <int> results = [];

         foreach (var element in lineElements)
         {
            if (GetIntFromString(element, lineNum, po, out var result))
               results.Add(result);   
         }
         return results;
      }

      public static List<float> GetListFloatFromString(string line, int lineNum, PathObj po)
      {
         var lineElements = line.Split(' ');
         List <float> results = [];

         foreach (var element in lineElements)
         {
            if (!float.TryParse(element, CultureInfo.InvariantCulture, out var result))
            {
               _ = new LoadingError(po, $"{element} could not be parsed to a Float!", lineNum, 0, ErrorType.UnexpectedDataType);
               continue;
            }
            results.Add(result);   
         }
         return results;
      }

      /// <summary>
      /// Only put in a block with the name "color"
      /// </summary>
      /// <param name="block"></param>
      /// <param name="po"></param>
      /// <returns></returns>
      public static Color GetColorFromBlock(EnhancedBlock block, PathObj po)
      {
         var contentElements = block.GetContentElements(true, po);
         if (contentElements.Count != 1)
         {
            _ = new LoadingError(po, $"Expected 1 content element but got {contentElements.Count}!",
               block.StartLine, 0, ErrorType.UnexpectedContentElement);
            return Color.Empty;
         }

         var ints = GetListIntFromString(contentElements[0].Value, block.StartLine, po);
         if (ints.Count != 3)
         {
            _ = new LoadingError(po,
               $"3 integers are required to form a color but only {ints.Count} were found!",
               block.StartLine, 0, ErrorType.UnexpectedDataType);
            return Color.Empty;
         }

         var r = ints[0];
         var g = ints[1];
         var b = ints[2];

         if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
         {
            _ = new LoadingError(po, $"Color values must be between 0 and 255 but got {r}, {g}, {b}!",
               block.StartLine, 0, ErrorType.UnexpectedDataType);
            return Color.Empty;
         }

         return Color.FromArgb(ints[0], ints[1], ints[2]);
      }

      public static Color GetColorFromPercentage(EnhancedBlock block, PathObj po)
      {
         var contentElements = block.ContentElements;
         if (contentElements.Count != 1)
         {
            _ = new LoadingError(po, $"Expected 1 content element but got {contentElements.Count}!",
                                 block.StartLine, 0, ErrorType.UnexpectedContentElement);
            return Color.Empty;
         }

         var floats = GetListFloatFromString(contentElements[0].Value, block.StartLine, po);
         if (floats.Count != 3)
         {
            _ = new LoadingError(po,
                                 $"3 floats are required to form a color but only {floats.Count} were found!",
                                 block.StartLine, 0, ErrorType.UnexpectedDataType);
            return Color.Empty;
         }

         var r = (int)(floats[0] * 255);
         var g = (int)(floats[1] * 255);
         var b = (int)(floats[2] * 255);

         if (floats[0] < 0 || floats[0] > 1 || floats[1] < 0 || floats[1] > 1 || floats[2] < 0 || floats[2] > 1)
         {
            _ = new LoadingError(po, $"Color values must be between 0 and 1 but got {floats[0]}, {floats[1]}, {floats[2]}!",
                                 block.StartLine, 0, ErrorType.UnexpectedDataType);
            return Color.Empty;
         }

         return Color.FromArgb(r, g, b);
      }

      public static HashSet<Province> GetProvincesFromContent(List<EnhancedContent> contents, PathObj po)
      {
         HashSet<Province> provinces = [];
         foreach (var contE in contents)
            provinces.UnionWith(GetProvincesFromString(contE, po));
         return provinces;
      }


   }
}