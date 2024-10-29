using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.Gaming.Preview.GamesEnumeration;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static partial class AdjacenciesLoading
   {
      private static readonly Regex AdjacencyRegex = CompileAdjacencyRegex();

      [GeneratedRegex(@"^(\d+);(\d+);([^;]+);(\d+);(-?\d+);(-?\d+);(-?\d+);(-?\d+);(.*)$", RegexOptions.Compiled)]
      private static partial Regex CompileAdjacencyRegex();

      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "adjacencies.csv"))
         {
            Globals.ErrorLog.Write("Could not find adjacencies.csv");
            return;
         }

         var lines = Parsing.RemoveAllEmptyLines(IO.ReadAllLinesInUTF8(path).ToList());
         lines.RemoveAt(0); // Remove header row
         lines.RemoveAt(lines.Count - 1); // Remove last empty line

         foreach (var line in lines)
         {
            if (!ParseStrait(line, out var strait))
               continue;
            Globals.Straits.Add(strait);
         }

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Adjacencies", sw.ElapsedMilliseconds);
      }

      private static bool ParseStrait(string straitString, out Strait strait)
      {
         strait = Strait.Empty;

         var match = AdjacencyRegex.Match(straitString);
         if (!match.Success)
         {
            Globals.ErrorLog.Write($"Invalid strait string: {straitString}");
            return false;
         }

         // Parse the straitType to an enum
         if (!Enum.TryParse(match.Groups[3].Value, true, out StraitType straitType))
         {
            Globals.ErrorLog.Write($"Invalid strait type: {match.Groups[3].Value}");
            return false;
         }

         if (!Globals.ProvinceIdToProvince.TryGetValue(int.Parse(match.Groups[1].Value), out var from)
             || !Globals.ProvinceIdToProvince.TryGetValue(int.Parse(match.Groups[2].Value), out var to)
             || !Globals.ProvinceIdToProvince.TryGetValue(int.Parse(match.Groups[4].Value), out var through))
         {
            Globals.ErrorLog.Write($"Invalid province id in strait: {straitString}");
            return false;
         
         }

         var start = new Point(int.Parse(match.Groups[5].Value), Globals.MapHeight - int.Parse(match.Groups[6].Value));
         var end = new Point(int.Parse(match.Groups[7].Value), Globals.MapHeight - int.Parse(match.Groups[8].Value));

         strait = new(from, to, through, straitType, start, end, match.Groups[9].Value);

         return true;
      }

   }
}