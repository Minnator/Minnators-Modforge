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

         Parsing.RemoveCommentFromMultilineString(ref content, out var removed);
         var matches = CountryRegex.Matches(removed);
         Dictionary<Tag, Country> countries = new(matches.Count);
         
         foreach (Match match in matches)
         {
            var tag = new Tag(match.Groups["tag"].Value);
            if (countries.ContainsKey(tag))
            {
               errorLog.Write($"Duplicate country tag: {tag}");
               continue;
            }
            countries.Add(tag, new(tag, match.Groups["path"].Value));
         }

         Globals.Countries = countries;

         ParseCountryAttributes(project, ref loadingLog, ref errorLog);
         sw.Stop();
         loadingLog.WriteTimeStamp("CountryLoading", sw.ElapsedMilliseconds);

         // Load country history
         sw.Restart();
         LoadCountryHistories(project, ref loadingLog, ref errorLog);
         sw.Stop();
         loadingLog.WriteTimeStamp("Loading CountryHistories", sw.ElapsedMilliseconds);
      }

      private static void LoadCountryHistories(ModProject project, ref Log loadingLog, ref Log errorLog)
      {
         var sw = Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely(project.ModPath, project.VanillaPath, "history", "countries");

         foreach (var file in files)
         {
            var content = IO.ReadAllInUTF8(file);
            var elements = Parsing.GetNestedElementsIterative(0, content);
            Tag tag = new(Path.GetFileName(file)[..3]);
            var country = Globals.Countries[tag];

            foreach (var element in elements)
            {
               if (element is Block block)
               {
                  ParseHistoryBlock(block, out var che, ref errorLog);
                  country.History.Add(che);      
               }
               else
               {
                  ParseCountryHistoryAttributes((Content)element, ref country, ref errorLog);
               }
            }
         }

         sw.Stop();
         loadingLog.WriteTimeStamp("CountryHistories", sw.ElapsedMilliseconds);
      }

      private static void ParseCountryHistoryAttributes(Content content, ref Country country, ref Log errorLog)
      {
         foreach (var kvp in Parsing.GetKeyValueList(content.Value))
         {
            var val = Parsing.RemoveCommentFromLine(kvp.Value);
            switch (kvp.Key)
            {
               case "government":
                  country.Government = val;
                  break;
               case "religion":
                  country.Religion = val;
                  break;
               case "technology_group":
                  country.TechnologyGroup = val;
                  break;
               case "national_focus":
                  country.NationalFocus = Parsing.ManaFromString(val);
                  break;
               case "add_historical_rival":
               case "historical_rival":
                  country.HistoricalRivals.Add(val);
                  break;
               case "add_historical_friend":
               case "historical_friend":
                  country.HistoricalFriends.Add(val);
                  break;
               case "set_estate_privilege":
                  country.EstatePrivileges.Add(val);
                  break;
               case "religious_school":
                  country.ReligiousSchool = val;
                  break;
               case "add_harmonized_religion":
                  country.HarmonizedReligions.Add(val);
                  break;
               case "secondary_religion":
                  country.SecondaryReligion = val;
                  break;
               case "unit_type":
                  country.UnitType = val;
                  break;
               case "capital":
                  if (int.TryParse(val, out var value))
                     country.Capital = value;
                  else
                     errorLog.Write($"Invalid capital in {country.Tag}: {val}"); 
                  break;
               case "add_government_reform":
                  country.GovernmentReforms.Add(val);
                  break;
               case "add_accepted_culture":
                  country.AcceptedCultures.Add(val);
                  break;
               case "government_rank":
                  if (int.TryParse(val, out var rank))
                     country.GovernmentRank = rank;
                  else
                     errorLog.Write($"Invalid government rank in {country.Tag}: {val}");
                  break;
               case "primary_culture":
                  country.PrimaryCulture = val;
                  break;
               case "fixed_capital":
                  if (int.TryParse(val, out var capProv))
                     country.FixedCapital = capProv;
                  else
                     errorLog.Write($"Invalid fixed capital in {country.Tag}: {val}");
                  break;
               case "mercantilism":
                  if (int.TryParse(val, out var mercantilism))
                     country.Mercantilism = mercantilism;
                  else
                     errorLog.Write($"Invalid mercantilism in {country.Tag}: {val}");
                  break;
               case "add_army_tradition":
                  if (int.TryParse(val, out var armyTradition))
                     country.ArmyTradition = armyTradition;
                  else
                     errorLog.Write($"Invalid army tradition in {country.Tag}: {val}");
                  break;
               case "add_army_professionalism":
                  if (float.TryParse(val, out var armyProfessionalism))
                     country.ArmyProfessionalism = armyProfessionalism;
                  else
                     errorLog.Write($"Invalid army professionalism in {country.Tag}: {val}");
                  break;
               case "add_prestige":
                  if (float.TryParse(val, out var prestige))
                     country.Prestige = prestige;
                  else
                     errorLog.Write($"Invalid prestige in {country.Tag}: {val}");
                  break;
               case "unlock_cult":
                  country.UnlockedCults.Add(val);
                  break;
               case "elector":
                  country.IsElector = Parsing.YesNo(val);
                  break;
               default:
                  errorLog.Write($"Unknown key in toppers {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }

      private static void ParseHistoryBlock(Block block, out CountryHistoryEntry che, ref Log errorLog)
      {
         che = null!;

         if (!DateTime.TryParse(block.Name, out var date))
         {
            switch (block.Name)
            {
               case "federation":
               case "change_estate_land_share":
                  //TODO
                  break;
               default:
                  errorLog.Write($"Invalid date in history block: {block.Name}");
                  break;
            }
            return;
         }

         che = new (date);
         AssignHistoryEntryAttributes(ref che, block.Blocks, ref errorLog);
         AssignHistoryEntryContent(ref che, block.GetContentElements, ref errorLog);
      }

      private static void AssignHistoryEntryAttributes(ref CountryHistoryEntry che, List<IElement> elements, ref Log errorLog)
      {
         if (elements.Count == 0)
            return;

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               continue;
            }

            switch (block.Name)
            {
               case "monarch":
               case "heir":
               case "queen":
                  Parsing.ParsePersonFromString(block.GetContentElements[0].Value, out var person, ref errorLog);
                  che.Persons.Add(person);
                  break;
               case "leader":
               case "change_estate_land_share":
               case "change_price":
               case "add_ruler_modifier":
                  //TODO
                  break;
               default:
                  errorLog.Write($"Unknown block in history entry: {block.Name}");
                  break;
            }
         }
      }

      private static void AssignHistoryEntryContent(ref CountryHistoryEntry che, List<Content> element, ref Log errorLog)
      {
         if (element.Count == 0)
            return;
         foreach (var content in element)
         {
            var kvp = Parsing.GetKeyValueList(content.Value);
            if (kvp.Count < 1)
            {
               errorLog.Write($"Invalid key value pair in history entry: {content.Value}");
               continue;
            }
            che.Effects.AddRange(kvp);
         }
      }

      #region CountryTags and non history data

      private static void ParseCountryAttributes(ModProject project, ref Log loadingLog, ref Log errorLog)
      {
         var sw = Stopwatch.StartNew();

         var log = errorLog;
         
         foreach (var country in Globals.Countries.Values)
         {
            FilesHelper.GetFileUniquely(project.ModPath, project.VanillaPath, out var content, "common", country.FileName);
            Parsing.RemoveCommentFromMultilineString(ref content, out var removed);
            var blocks = Parsing.GetNestedElementsIterative(0, ref removed);

            AssignCountryAttributes(country, ref blocks, log);
         }

         sw.Stop();
         loadingLog.WriteTimeStamp("CountryAttributes", sw.ElapsedMilliseconds);
      }

      private static void AssignCountryAttributes(Country country, ref List<IElement> blocks, Log errorLog)
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
         foreach (var kvp in Parsing.GetKeyValueList(element.Value))
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
               case "random_nation_chance":
                  if (int.TryParse(kvp.Value, out var value) && value == 0)
                     country.CanBeRandomNation = false;
                  break;
               case "preferred_religion":
                  country.PreferredReligion = kvp.Value;
                  break;
               case "colonial_parent":
                  country.ColonialParent = new (kvp.Value);
                  break;
               case "special_unit_culture":
                  country.SpecialUnitCulture = kvp.Value;
                  break;
               default:
                  errorLog.Write($"Unknown key in {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }

      #endregion

   }
}