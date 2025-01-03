using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading
{
   public static class ClimateLoading
   {
      public static void Load()
      {
         if (!FilesHelper.GetFilePathUniquely(out var path, "map", "climate.txt"))
         {
            _ = new ErrorObject(ErrorType.RequiredFileNotFound, "Can not locate climate.txt");
            return;
         }

         IO.ReadAllInANSI(path, out var rawContent);
         Parsing.RemoveCommentFromMultilineString(rawContent, out var content);
         var elements = Parsing.GetElements(0, content);

         var pathObject = PathObj.FromPath(path);

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               var kvp = Parsing.GetKeyValueList(((Content)element).Value);
               if (kvp.Count != 1)
               {
                  _ = new ErrorObject(ErrorType.TempParsingError, "Climate Loading Error");
                  return;
               }
               if (kvp[0].Key == "equator_y_on_province_image" && int.TryParse(kvp[0].Value.Trim(), out var equator))
                  Globals.EquatorY = equator;
               else
                  _ = new ErrorObject(ErrorType.TempParsingError, $"Forbidden Content or mis formed equator {kvp[0].Key} - {kvp[0].Key}");
            }
            else
            {
               var provinces = Parsing.GetProvincesFromString(block.GetContent);
               var climate = new Climate(block.Name.Trim(), Color.Empty, ref pathObject, provinces);
               if (climate.Name == "impassable")
                  Globals.Impassable = [..climate.SubCollection];
               else if (IsClimate(climate.Name))
                  Globals.Climates.Add(climate.Name, climate);
               else
                  Globals.Weather.Add(climate.Name, climate);
            }
         }

         foreach (var climate in Globals.Climates)
            SetVanillaColors(climate.Value);

         foreach (var weather in Globals.Weather)
            SetVanillaColors(weather.Value);
      }

      private static bool IsClimate(string climate)
      {
         return climate switch
         {
            "severe_monsoon" => false,
            "normal_monsoon" => false,
            "mild_monsoon" => false,
            "impassable" => false,
            "severe_winter" => false,
            "normal_winter" => false,
            "mild_winter" => false,
            "arctic" => true,
            "arid" => true,
            "tropical" => true,
            _ => true
         };
      }

      private static void SetVanillaColors(Climate climate)
      {
         climate.Color = climate.Name switch
         {
            "severe_monsoon" => Color.FromArgb(51, 45, 175),
            "normal_monsoon" => Color.FromArgb(84, 75, 186),
            "mild_monsoon" => Color.FromArgb(68, 60, 105),
            "impassable" => Color.DimGray,
            "severe_winter" => Color.FromArgb(197, 174, 163),
            "normal_winter" => Color.FromArgb(139, 123, 116),
            "mild_winter" => Color.FromArgb(99, 77, 73),
            "arctic" => Color.FromArgb(245, 218, 205),
            "arid" => Color.FromArgb(255, 246, 146),
            "tropical" => Color.FromArgb(110, 138, 72),
            _ => Color.FromArgb(92, 90, 64)
         };
      }
   }
}