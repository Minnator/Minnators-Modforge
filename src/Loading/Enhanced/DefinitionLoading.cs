using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Saving;

namespace Editor.Loading;


public static partial class DefinitionLoading
{
   private static readonly Regex Regex = DefinitionRegex();

   private static bool GetProvince(ref Match match, int lineNum, PathObj po)
   {
      var color = Color.FromArgb(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value),
         int.Parse(match.Groups[4].Value));

      var id = int.Parse(match.Groups[1].Value);

      var argb = color.ToArgb();

      if (Globals.ColorToProvId.ContainsKey(argb))
      {
         _ = new LoadingError(po, "Duplicate province color for id: " + id, lineNum, type: ErrorType.ProvinceDefinitionError);
         return false;
      }

      Province province = new(id, color, ObjEditingStatus.Unchanged);

      if (!Globals.Provinces.Add(province))
      {
         _ = new LoadingError(po, "Duplicate province id: " + id, lineNum, type: ErrorType.ProvinceDefinitionError);
         return false;
      }

      Globals.ProvinceIdToProvince.Add(id, province);
      Globals.ColorToProvId.Add(argb, province);
      return true;
   }

   public static void LoadDefinition()
   {
      EnhancedParser.GetModOrVanillaPath(out var po, "map", "definition.csv");
      var lines = IO.ReadAllLinesInUTF8(po);
      Globals.Provinces = new(lines.Length);

      if (lines.Length > 0)
      {
         var headerLine = lines[0].Trim();
         var match = Regex.Match(headerLine);
         if (match.Success)
            GetProvince(ref match, 0, po);
         else if (headerLine.Count(c => c == ';') != 5)
            _ = new LoadingError(po, "Invalid header line in definition.csv", 0, type: ErrorType.SyntaxError);
      }

      for (var i = 1; i < lines.Length; i++)
      {
         var match = Regex.Match(lines[i].Trim());
         if (!match.Success)
         {
            _ = new LoadingError(po, $"Invalid line: {lines[i]} in definition.csv", i, type: ErrorType.SyntaxError);
            continue;
         }
         GetProvince(ref match, i, po);
      }
   }

   [GeneratedRegex(@"^(\d+);(\d+);(\d+);(\d+);[^;]*;[^;]*")]
   private static partial Regex DefinitionRegex();
}
