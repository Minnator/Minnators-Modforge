using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static partial class EnhancedParsing
   {
      #region Regexes

      private static readonly Regex IntListRegex = IntListRegexGenerate();
      [GeneratedRegex(@"(\d+)")]
      private static partial Regex IntListRegexGenerate();

      #endregion

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


      public static List<string> GetStringListFromContent(EnhancedContent content, PathObj po)
      {
         var strings = new List<string>();

         foreach (var (line, num) in content.GetLineEnumerator(po))
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


      public static List<Province> GetProvincesFromString(EnhancedContent content, PathObj po)
      {
         var provinces = new List<Province>();

         foreach (var (line, num) in content.GetLineEnumerator(po))
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

         return Color.FromArgb(ints[0], ints[1], ints[2]);
      }

      public static List<Province> GetProvincesFromContent(List<EnhancedContent> contents, PathObj po)
      {
         List<Province> provinces = [];
         foreach (var contE in contents)
            provinces.AddRange(GetProvincesFromString(contE, po));
         return provinces;
      }
   }
}