using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
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

      public static Color GetColorFromBlock(EnhancedBlock block, PathObj po)
      {
         if (!block.Name.Equals("color"))
         {
            _ = new LoadingError(po, $"Expected block name \"color\" but got \"{block.Name}\"!", block.StartLine, 0, ErrorType.IllegalBlockName);
            return Color.Empty;
         }

         var contentElements = block.ContentElements;
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
   }
}