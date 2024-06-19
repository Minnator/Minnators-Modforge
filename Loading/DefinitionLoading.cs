using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Editor.DataClasses;

namespace Editor;

public static class DefinitionLoading
{

   public static Province[] LoadDefinition(List<string> lines, ref Log loadingLog)
   {
      var sw = new Stopwatch();
      sw.Start();
      var provinces = new Dictionary<int, Province>(lines.Count);
      var colorToId = new Dictionary<Color, int>(lines.Count);
      
      var regex = new Regex(@"\s*(?:(\d+);(\d+);(\d+);(\d+);(.*);).*");
      
      foreach (var line in lines)
      {
         var match = regex.Match(line);

         if (!match.Success)
            continue;

         if (!int.TryParse(match.Groups[1].Value, out var id) ||
             !int.TryParse(match.Groups[2].Value, out var r) ||
             !int.TryParse(match.Groups[3].Value, out var g) ||
             !int.TryParse(match.Groups[4].Value, out var b))
         {
            MessageBox.Show($"Invalid values in the definition line: {line}","Corrupted definitions.csv");
            throw new Exception("Corrupted definitions.csv");
         }

         var color = Color.FromArgb(r, g, b);

         provinces[id] = new Province
         {
            Id = id,
            Color = color
         };

         //There are some duplicate colors in the definition file in vanilla, so we link to the first found
         if (!colorToId.TryGetValue(color, out var existingId)) 
            colorToId.Add(color, id);
      }

      sw.Stop();
      loadingLog.WriteTimeStamp("DefinitionLoading", sw.ElapsedMilliseconds);

      return [.. provinces.Values];
   }

   public static void PrintLoadedDefinitions (Province[] provinces)
   {
      var sb = new StringBuilder();
      sb.AppendLine($"Loaded [{provinces.Length}] provinces:");
      foreach (var province in provinces)
      {
         sb.AppendLine($"Id: {province.Id,4}, Color: {province.Color}");
      }
      File.WriteAllText(@"C:\Users\david\Downloads\definition.txt", sb.ToString());
   }

}
