using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Loading;

[Loading]
public static class DefinitionLoading
{

   public static Province[] LoadDefinition(List<string> lines)
   {
      var sw = Stopwatch.StartNew();
      var provinces = new Dictionary<int, Province>(lines.Count);
      var colorToId = new Dictionary<Color, int>(lines.Count);
      var regex = new Regex(@"\s*(?:(\d+);(\d+);(\d+);(\d+);(.*);).*");

      try
      {
         foreach (var line in lines)
         {
            var match = regex.Match(line);

            if (!match.Success)
               continue;

            if (!int.TryParse(match.Groups[1].Value, out var id) ||
                !int.TryParse(match.Groups[2].Value, out var r) ||
                !int.TryParse(match.Groups[3].Value, out var g) ||
                !int.TryParse(match.Groups[4].Value, out var b))
               throw new FormatException($"Invalid values in the definition line: {line}");

            var color = Color.FromArgb(r, g, b);

            if (!provinces.ContainsKey(id))
            {
               var province = new Province(id, color);
               provinces.Add(id, province);
            }

            // Link to the first found color if there are duplicates
            colorToId.TryAdd(color, id);
            //else
            //   Debug.WriteLine($"Duplicate color found in definition file: {color}");
         }
      }
      catch (Exception ex)
      {
         MessageBox.Show($"Error while loading definitions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         throw;
      }

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("DefinitionLoading", sw.ElapsedMilliseconds);

      return [.. provinces.Values];
   }

}
