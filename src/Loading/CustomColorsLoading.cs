using System.Diagnostics;
using Editor.Helper;
using Editor.Parser;

namespace Editor.Loading
{
   public class CustomColorsLoading
   {
      
      public static void Load()
      {
         var sw = Stopwatch.StartNew();

         if (!FilesHelper.GetFilePathUniquely(out var path, "common", "custom_country_colors", "00_custom_country_colors.txt"))
         {
            Globals.ErrorLog.Write("Could not find custom_colors.txt");
            goto finishLoad;
         }

         var content = IO.ReadAllInUTF8(path);
         Parsing.RemoveCommentFromMultilineString(ref content, out content);
         var elements = Parsing.GetElements(0, content);
         
         foreach (var element in elements)
         {
            if (element is Block blk)
               ParseColorBlock(blk, Globals.RevolutionaryColors, Globals.CustomCountryColors);
         }

         finishLoad:
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Custom Colors", sw.ElapsedMilliseconds);
      }

      private static void ParseColorBlock(Block blk, Dictionary<int, Color> revColors, HashSet<Color> customColors)
      {
         switch (blk.Name)
         {
            case "flag_color":
               if (Parsing.TryParseColor(blk.GetContent, out var revColor))
                  revColors.Add(revColors.Count, revColor);
               else
                  Globals.ErrorLog.Write($"Could not parse revolutionary color: {blk.GetContent}");
               break;
            case "color":
               if (Parsing.TryParseColor(blk.GetContent, out var color))
                  customColors.Add(color);
               else
                  Globals.ErrorLog.Write($"Could not parse custom color: {blk.GetContent}");
               break;
         }
      }

   }
}