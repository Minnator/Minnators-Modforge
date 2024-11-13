using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public static class CountryLoading
   {
      private static readonly Regex CountryRegex = new(@"(?<tag>[A-Z0-9]{3})\s*=\s*""(?<path>[^""]+)""", RegexOptions.Compiled);

      public static void LoadCountries()
      {
         var sw = Stopwatch.StartNew();
         // Loads the country_tags file
         FilesHelper.GetFilesUniquelyAndCombineToOne(out var content, "common", "country_tags");

         Parsing.RemoveCommentFromMultilineString(ref content, out var removed);
         var matches = CountryRegex.Matches(removed);
         Dictionary<Tag, Country> countries = new(matches.Count);

         foreach (Match match in matches)
         {
            var tag = new Tag(match.Groups["tag"].Value);
            if (countries.ContainsKey(tag))
            {
               Globals.ErrorLog.Write($"Duplicate country tag: {tag}");
               continue;
            }

            Country country = new(tag, Color.Empty, match.Groups["path"].Value);
            country.SetBounds();
            countries.Add(tag, country);
         }

         Globals.Countries = countries;

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Parsing Country Tags", sw.ElapsedMilliseconds);
         ParseCountryAttributes();

         // CreateProvinceGroups country history
         sw.Restart();
         LoadCountryHistories();
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Loading CountryHistories", sw.ElapsedMilliseconds);
      }

      public static void LoadProvincesToCountries()
      {
         var sw = Stopwatch.StartNew();
         AssignProvinces();
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Country province init", sw.ElapsedMilliseconds);
      }

      private static void LoadCountryHistories()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "history", "countries");

         Parallel.ForEach(files, new () { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },file =>
         {
            if (!IO.ReadAllInANSI(file, out var content))
               return;
            Parsing.RemoveCommentFromMultilineString(content, out var removed);
            var elements = Parsing.GetElements(0, removed);
            
            foreach (var element in elements)
            {
               Tag tag = new(Path.GetFileName(file)[..3]);
               if (Globals.Countries.TryGetValue(tag, out var country))
                  AnalyzeCountryStuff(element, country);
               else
                  Globals.ErrorLog.Write($"Found country file with no no tag reference in 'country_tag' folder: {tag}");
            }
         });
      }

      private static void AnalyzeCountryStuff(IElement element, Country country)
      {
         if (element is Block block)
         {
            switch (block.Name)
            {
               case "add_country_modifier":
                  if (ModifierParser.ParseApplicableModifier(block.GetContent, out var mod)) 
                     country.Modifiers.Add(mod);
                  else
                     Globals.ErrorLog.Write($"Invalid modifier in {country.Tag}: {block.GetContent}");
                  break;
               case "add_ruler_modifier":
                  if (ModifierParser.ParseRulerModifier(block.GetContent, out var rulerMod))
                     country.RulerModifiers.Add(rulerMod);
                  else
                     Globals.ErrorLog.Write($"Invalid ruler modifier in {country.Tag}: {block.GetContent}");
                  break;
               case "add_opinion":
                  if (EffectParser.ParseOpinionEffects("add_opinion", block.GetContent, out var effect))
                     country.InitialEffects.Add(effect);
                  else
                     Globals.ErrorLog.Write($"Invalid opinion effect in {country.Tag}: {block.GetContent}");
                  break;
               case "add_estate_loyalty":
                  if (EffectParser.ParseAddEstateLoyaltyEffect(block.GetContent, out var loyaltyEffect))
                     country.InitialEffects.Add(loyaltyEffect);
                  else
                     Globals.ErrorLog.Write($"Invalid estate loyalty effect in {country.Tag}: {block.GetContent}");
                  break;
               default:
                  ParseHistoryBlock(block, out var che);
                  country.History.Add(che);
                  break;
            }

         }
         else
         {
            ParseCountryHistoryAttributes((Content)element, ref country);
         }
      }

      private static void ParseCountryHistoryAttributes(Content content, ref Country country)
      {
         foreach (var kvp in Parsing.GetKeyValueList(content.Value))
         {
            var val = Parsing.RemoveCommentFromLine(kvp.Value);

            if (EffectParser.ParseSimpleEffect(kvp.Key, val, out var effect))
            {
               country.InitialEffects.Add(effect);
               continue;
            }
            
            switch (kvp.Key)
            {
               case "government":
                  country.Government = val;
                  break;
               case "religion":
                  country.Religion = val;
                  break;
               case "technology_group":
                  country.TechnologyGroup = new(val);
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
                     country.Capital = Globals.ProvinceIdToProvince[value];
                  else
                     Globals.ErrorLog.Write($"Invalid capital in {country.Tag}: {val}");
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
                     Globals.ErrorLog.Write($"Invalid government rank in {country.Tag}: {val}");
                  break;
               case "primary_culture":
                  country.PrimaryCulture = val;
                  break;
               case "fixed_capital":
                  if (int.TryParse(val, out var capProv))
                     country.FixedCapital = capProv;
                  else
                     Globals.ErrorLog.Write($"Invalid fixed capital in {country.Tag}: {val}");
                  break;
               case "mercantilism":
                  if (float.TryParse(val, out var mercantilism))
                     country.Mercantilism = mercantilism;
                  else
                     Globals.ErrorLog.Write($"Invalid mercantilism in {country.Tag}: {val}");
                  break;
               case "add_army_tradition":
                  if (int.TryParse(val, out var armyTradition))
                     country.ArmyTradition = armyTradition;
                  else
                     Globals.ErrorLog.Write($"Invalid army tradition in {country.Tag}: {val}");
                  break;
               case "add_army_professionalism":
                  if (float.TryParse(val, out var armyProfessionalism))
                     country.ArmyProfessionalism = armyProfessionalism;
                  else
                     Globals.ErrorLog.Write($"Invalid army professionalism in {country.Tag}: {val}");
                  break;
               case "add_prestige":
                  if (float.TryParse(val, out var prestige))
                     country.Prestige = prestige;
                  else
                     Globals.ErrorLog.Write($"Invalid prestige in {country.Tag}: {val}");
                  break;
               case "unlock_cult":
                  country.UnlockedCults.Add(val);
                  break;
               case "elector":
                  country.IsElector = Parsing.YesNo(val);
                  break;
               case "add_truce_with":
                  //TODO
                  break;
               case "add_piety":
                  //TODO
                  break;

               default:
                  Globals.ErrorLog.Write($"Unknown key in toppers {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }

      private static void ParseHistoryBlock(Block block, out CountryHistoryEntry che)
      {
         che = null!;

         if (!Parsing.TryParseDate(block.Name, out var date))
         {
            if (!Parsing.ParseDynamicContent(block, out _))
               return;
         }

         che = new(date);
         AssignHistoryEntryAttributes(ref che, block.Blocks);
         AssignHistoryEntryContent(ref che, block.GetContentElements);
      }

      private static void AssignHistoryEntryAttributes(ref CountryHistoryEntry che, List<IElement> elements)
      {
         if (elements.Count == 0)
            return;

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               continue;
            }

            if (Globals.ScriptedEffectNames.Contains(block.Name))
            {
               var effect = EffectFactory.CreateScriptedEffect(block.Name, block.GetContent, EffectValueType.Complex);
               che.Effects.Add(effect);
               continue;
            }

            if (int.TryParse(block.Name, out var provId) && Globals.ProvinceIdToProvince.ContainsKey(provId))
            {
               // TODO add scoped effects when implemented
               continue;
            }

            switch (block.Name)
            {
               case "monarch":
               case "heir":
               case "queen":
               case "monarch_consort":
                  Parsing.ParsePersonFromString(block.GetContentElements[0].Value, out var person);
                  che.Persons.Add(person);
                  break;
               case "leader":
                  Parsing.ParseLeaderFromString(block.GetContentElements[0].Value, out var leader);
                  che.Leaders.Add(leader);
                  break;
               case "federation":
                  var effect = EffectFactory.CreateComplexEffect(block.Name, block.GetContent, EffectValueType.Complex);
                  break;
               default:
                  if (Parsing.ParseDynamicContent(block, out _))
                     break;
                  Globals.ErrorLog.Write($"Unknown block in history entry: {block.Name}");
                  break;
            }
         }
      }

      private static void AssignHistoryEntryContent(ref CountryHistoryEntry che, List<Content> element)
      {
         if (element.Count == 0)
            return;
         foreach (var content in element)
         {
            var kvps= Parsing.GetKeyValueList(content.Value);
            if (kvps.Count < 1)
            {
               Globals.ErrorLog.Write($"Invalid key value pair in history entry effects: {content.Value}");
               continue;
            }

            foreach (var kvp in kvps)
            {
               if (EffectParser.IsAnyEffectCountryHistory(kvp.Key))
               {
                  var eff = EffectFactory.CreateSimpleEffect(kvp.Key, kvp.Value, EffectValueType.String, Scope.Country);
                  che.Effects.Add(eff);
               }
               else
               {
                  Globals.ErrorLog.Write($"Unknown key in history entry effects: {kvp.Key} in {content}");
               }
            }
         }
      }

      #region CountryTags and non history data

      private static void ParseCountryAttributes()
      {
         var sw = Stopwatch.StartNew();

         Parallel.ForEach(Globals.Countries.Values, new () { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 }, country =>
         {
            FilesHelper.GetFileUniquely(out var content, "common", country.FileName);
            Parsing.RemoveCommentFromMultilineString(ref content, out var removed);
            var blocks = Parsing.GetElements(0, ref removed);

            AssignCountryAttributes(country, ref blocks);
         });

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("CountryAttributes", sw.ElapsedMilliseconds);
      }

      private static void AssignCountryAttributes(Country country, ref List<IElement> blocks)
      {
         foreach (var element in blocks)
         {
            if (element is not Block block)
            {
               AssignCountryContent(country, (Content)element);
               continue;
            }

            switch (block.Name)
            {
               case "color":
                  if (block.Blocks.Count != 1)
                  {
                     Globals.ErrorLog.Write($"Invalid color block in {country.Tag} at [color]");
                     break;
                  }

                  if (!Parsing.TryParseColor(((Content)block.Blocks[0]).Value, out var countryColor))
                  {
                     Globals.ErrorLog.Write($"Invalid color in {country.Tag}: {((Content)block.Blocks[0]).Value}");
                     break;
                  }
                  country.Color = countryColor;
                  break;
               case "revolutionary_colors":
                  if (block.Blocks.Count != 1)
                  {
                     Globals.ErrorLog.Write($"Invalid revolutionary_colors block in {country.Tag} at [revolutionary_colors]");
                     break;
                  }
                  if (!Parsing.TryParseColor(((Content)block.Blocks[0]).Value, out var revolutionaryColor))
                  {
                     Globals.ErrorLog.Write($"Invalid revolutionary color in {country.Tag}: {((Content)block.Blocks[0]).Value}");
                     break;
                  }
                  country.RevolutionaryColor = revolutionaryColor;
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
                     country.FleetNames.AddRange(Parsing.GetStringList(name.Value));
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
                  Globals.ErrorLog.Write($"Unknown block in {country.Tag}: {block.Name}");
                  break;
            }
         }
      }

      private static void AssignCountryContent(Country country, Content element)
      {
         foreach (var kvp in Parsing.GetKeyValueList(element.Value))
         {
            if (Globals.CountryAttributes.Contains(kvp.Key))
            {
               country.CustomAttributes.Add(kvp.Key);
               continue;
            }

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
                  country.ColonialParent = new(kvp.Value);
                  break;
               case "special_unit_culture":
                  country.SpecialUnitCulture = kvp.Value;
                  break;
               default:
                  Globals.ErrorLog.Write($"Unknown key in {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }

      #endregion
      public static void AssignProvinces()
      {
         foreach (var province in Globals.Provinces)
         {
            if (province.Owner == Tag.Empty)
               continue;

            if (!Globals.Countries.TryGetValue(province.Owner, out var country))
            {
               Globals.ErrorLog.Write($"Province {province.Id} has unknown country owner: {province.Owner}");
               continue;
            }

            country.Add(province);
         }
      }
   }

}