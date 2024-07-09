using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class CountryLoading
   {
      private static readonly Regex CountryRegex = new (@"(?<tag>[A-Z]{3})\s*=\s*""(?<path>[^""]+)""", RegexOptions.Compiled);

      public static void LoadCountries(ModProject project, ref Log loadingLog, ref Log errorLog)
      {
         var sw = Stopwatch.StartNew();
         FilesHelper.GetFilesUniquelyAndCombineToOne(project.ModPath, project.VanillaPath, out var content, "common", "country_tags");

         Parsing.RemoveCommentFromMultilineString(ref content);
         var matches = CountryRegex.Matches(content);
         Dictionary<Tag, Country> countries = new(matches.Count);

         foreach (Match match in matches)
         {
            var tag = new Tag(match.Groups["tag"].Value);
            if (countries.ContainsKey(tag))
            {
               errorLog.Write($"Duplicate country tag: {tag}");
               continue;
            }
            countries.Add(tag, new (tag, match.Groups["path"].Value));
         }

         Globals.Countries = countries;

         sw.Stop();
         loadingLog.WriteTimeStamp("CountryLoading", sw.ElapsedMilliseconds);
         ParseCountryAttributes(project, ref loadingLog, ref errorLog);
      }

      public static void ParseCountryAttributes(ModProject project, ref Log loadingLog, ref Log errorLog)
      {
         var sw = Stopwatch.StartNew();
         foreach (var country in Globals.Countries.Values)
         {
            FilesHelper.GetFileUniquely(project.ModPath, project.VanillaPath, out var content, "common", country.FileName);
            Parsing.RemoveCommentFromMultilineString(ref content);
            var blocks = Parsing.GetNestedElementsIterative(0, ref content);

            AssignCountryAttributes(country, ref blocks, ref errorLog);
         }

         sw.Stop();
         loadingLog.WriteTimeStamp("CountryAttributes", sw.ElapsedMilliseconds);
      }

      public static void AssignCountryAttributes(Country country, ref List<IElement> blocks, ref Log errorLog)
      {
         foreach (var element in blocks)
         {
            if (element is not Block block)
            {
               AssignCountryContent(country, (Content)element, ref errorLog);
               continue;
            }

            switch (block.Name)
            {
               case "color":
                  if (block.Blocks.Count != 1)
                  {
                     errorLog.Write($"Invalid color block in {country.Tag} at [color]");
                     break;
                  }
                  country.Color = Parsing.ParseColor(((Content)block.Blocks[0]).Value);
                  break;
               case "revolutionary_colors":
                  if (block.Blocks.Count != 1)
                  {
                     errorLog.Write($"Invalid revolutionary_colors block in {country.Tag} at [revolutionary_colors]");
                     break;
                  }
                  country.RevolutionaryColor = Parsing.ParseColor(((Content)block.Blocks[0]).Value);
                  break;
               case "historical_idea_groups":
                  foreach (var idea in block.GetContentElements) 
                     country.HistoricalIdeas.AddRange(Parsing.GetStringList(idea.Value));
                  break;
               case "historical_units":
                  foreach (var unit in block.GetContentElements) 
                     country.HistoricalUnits.AddRange(Parsing.GetStringList(unit.Value));
                  break;
               case "monarch_names":
                  foreach (var name in block.GetContentElements)
                  {
                     Parsing.ParseMonarchNames(name.Value, out var monarchNames);
                     country.MonarchNames.AddRange(monarchNames);
                  }
                  break;
               case "ship_names":
                  foreach (var name in block.GetContentElements) 
                     country.ShipNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "fleet_names":
                  foreach (var name in block.GetContentElements) 
                     country.FleeTNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "army_names":
                  foreach (var name in block.GetContentElements) 
                     country.ArmyNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "leader_names":
                  foreach (var name in block.GetContentElements) 
                     country.LeaderNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               default:
                  errorLog.Write($"Unknown block in {country.Tag}: {block.Name}");
                  break;
            }
         }
      }

      private static void AssignCountryContent(Country country, Content element, ref Log errorLog)
      {
         var kvps = Parsing.GetKeyValueList(element.Value);
         if (element.Value.Contains("HistoricalScore"))
            country.HistoricalScore = 1;
         foreach (var kvp in kvps)
         {
            switch (kvp.Key)
            {
               case "historical_council":
                  country.HistoricalCouncil = kvp.Value;
                  break;
               case "historical_score":
                  country.HistoricalScore = int.Parse(kvp.Value);
                  break;
               case "graphical_culture":
                  country.Gfx = kvp.Value;
                  break;
               default:
                  errorLog.Write($"Unknown key in {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }
   }
}