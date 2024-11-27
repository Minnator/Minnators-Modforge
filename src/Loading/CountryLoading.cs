using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static partial class CountryLoading
   {
      private static readonly Regex CountryRegex = CountryTagRegex();

      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "country_tags");


         foreach (var file in files)
         {
            if (!IO.ReadAllInANSI(file, out var content))
               continue;
            Parsing.RemoveCommentFromMultilineString(content, out var removed);
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

               var pathObj = PathObj.FromPath(file);
               var fileNameObj = new CountryFilePath(match.Groups["path"].Value, ref pathObj);
               Country country = new(tag, fileNameObj, Color.Empty, ObjEditingStatus.Unchanged);
               fileNameObj.SetCountry(country);
               country.SetBounds();
               countries.Add(tag, country);
            }

            Globals.Countries = countries;

            ParseCountryAttributes();

            LoadCountryHistories();
         }

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

         Parallel.ForEach(files, new (),file =>
         {
            if (!IO.ReadAllInANSI(file, out var content))
               return;
            
            Parsing.RemoveCommentFromMultilineString(content, out var removed);
            var newPathObj = PathObj.FromPath(file);

            Tag tag = new(Path.GetFileName(file)[..3]);
            if (Globals.Countries.TryGetValue(tag, out var country))
               SaveMaster.AddToDictionary(ref newPathObj, new HistoryCountry(country, ref newPathObj));
            else
            {
               Globals.ErrorLog.Write($"Found country file with no no tag reference in 'country_tag' folder: {tag}");
               return;
            }

            foreach (var element in Parsing.GetElements(0, removed)) 
               ParseCountryStuff(element, ref file, country);
         });
      }

      private static void ParseCountryStuff(IElement element, ref string file, Country country)
      {
         if (element is Block block)
         {
            if (EffectParser.IsAnyEffectCountryHistory(block.Name))
            {
               EffectParser.ParseEffect(block.Name, block.GetContent, out var eff);
               country.HistoryCountry.InitialEffects.Add(eff);
               return;
            }

            switch (block.Name)
            {
               case "add_country_modifier":
                  if (ModifierParser.ParseApplicableModifier(block.GetContent, out var mod)) 
                     country.HistoryCountry.Modifiers.Add(mod);
                  else
                     Globals.ErrorLog.Write($"Invalid modifier in {country.Tag}: {block.GetContent}");
                  break;
               case "add_ruler_modifier":
                  if (ModifierParser.ParseRulerModifier(block.GetContent, out var rulerMod))
                     country.HistoryCountry.RulerModifiers.Add(rulerMod);
                  else
                     Globals.ErrorLog.Write($"Invalid ruler modifier in {country.Tag}: {block.GetContent}");
                  break;
               case "add_opinion":
                  if (EffectParser.ParseOpinionEffects("add_opinion", block.GetContent, out var effect))
                     country.HistoryCountry.InitialEffects.Add(effect);
                  else
                     Globals.ErrorLog.Write($"Invalid opinion effect in {country.Tag}: {block.GetContent}");
                  break;
               case "add_estate_loyalty":
                  if (EffectParser.ParseAddEstateLoyaltyEffect(block.GetContent, out var loyaltyEffect))
                     country.HistoryCountry.InitialEffects.Add(loyaltyEffect);
                  else
                     Globals.ErrorLog.Write($"Invalid estate loyalty effect in {country.Tag}: {block.GetContent}");
                  break;
               default:
                  ParseHistoryBlock(block, ref file, out var che);
                  country.HistoryCountry.History.Add(che);
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

            switch (kvp.Key)
            {
               case "government":
                  country.HistoryCountry.Government = val;
                  break;
               case "religion":
                  country.HistoryCountry.Religion = val;
                  break;
               case "technology_group":
                  country.HistoryCountry.TechnologyGroup = new(val);
                  break;
               case "national_focus":
                  country.HistoryCountry.NationalFocus = Parsing.ManaFromString(val);
                  break;
               case "add_historical_rival":
               case "historical_rival":
                  country.HistoryCountry.HistoricalRivals.Add(val);
                  break;
               case "add_historical_friend":
               case "historical_friend":
                  country.HistoryCountry.HistoricalFriends.Add(val);
                  break;
               case "set_estate_privilege":
                  country.HistoryCountry.EstatePrivileges.Add(val);
                  break;
               case "religious_school":
                  country.HistoryCountry.ReligiousSchool = val;
                  break;
               case "add_harmonized_religion":
                  country.HistoryCountry.HarmonizedReligions.Add(val);
                  break;
               case "secondary_religion":
                  country.HistoryCountry.SecondaryReligion = val;
                  break;
               case "unit_type":
                  country.HistoryCountry.UnitType = val;
                  break;
               case "capital":
                  if (int.TryParse(val, out var value))
                     country.HistoryCountry.Capital = Globals.ProvinceIdToProvince[value];
                  else
                     Globals.ErrorLog.Write($"Invalid capital in {country.Tag}: {val}");
                  break;
               case "add_government_reform":
                  country.HistoryCountry.GovernmentReforms.Add(val);
                  break;
               case "add_accepted_culture":
                  country.HistoryCountry.AcceptedCultures.Add(val);
                  break;
               case "government_rank":
                  if (int.TryParse(val, out var rank))
                     country.HistoryCountry.GovernmentRank = rank;
                  else
                     Globals.ErrorLog.Write($"Invalid government rank in {country.Tag}: {val}");
                  break;
               case "primary_culture":
                  country.HistoryCountry.PrimaryCulture = val;
                  break;
               case "fixed_capital":
                  if (int.TryParse(val, out var capProv))
                     country.HistoryCountry.FixedCapital = capProv;
                  else
                     Globals.ErrorLog.Write($"Invalid fixed capital in {country.Tag}: {val}");
                  break;
               case "elector":
                  country.HistoryCountry.IsElector = Parsing.YesNo(val);
                  break;
               case "mercantilism":
                  if (int.TryParse(val, out var merc))
                     country.HistoryCountry.Mercantilism = merc;
                  else
                     Globals.ErrorLog.Write($"Invalid mercantilism in {country.Tag}: {val}");
                  break;

               default:

                  if (EffectParser.ParseSimpleEffect(kvp.Key, val, out var effect))
                  {
                     country.HistoryCountry.InitialEffects.Add(effect);
                     continue;
                  }

                  Globals.ErrorLog.Write($"Unknown key in toppers {country.Tag}: {kvp.Key}");
                  break;
            }
         }
      }

      private static void ParseHistoryBlock(Block block, ref string file, out CountryHistoryEntry che)
      {
         che = null!;

         if (!Parsing.TryParseDate(block.Name, out var date))
         {
            if (!Parsing.ParseDynamicContent(block, out _))
               return;
         }


         che = new(date);
         //TempHistoryEntries(ref che, block.Blocks);
         //return;
         //TODO: properly implement this after pdx language parser is written
         AssignHistoryEntryAttributes(ref che, ref file, block.Blocks);
         AssignHistoryEntryContent(ref che, ref file, block.GetContentElements);
      }

      private static void TempHistoryEntries(ref CountryHistoryEntry entry, List<IElement> elements)
      {
         var sb = new StringBuilder();
         foreach (var element in elements)
         {
            if (element is Content c)
            {
               sb.AppendLine(c.Value);
               continue;
            }
            sb.AppendLine(((Block)element).GetAllContent);
         }
         entry.Content = sb.ToString();
      }

      private static void AssignHistoryEntryAttributes(ref CountryHistoryEntry che, ref string file, List<IElement> elements)
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
                  che.Effects.Add(effect);
                  break;
               case "if":
                  che.IsDummy = true;
                  che.Content = block.GetAllContent;
                  break;
               case "change_price":
                  if (EffectParser.ParseChangePriceEffect(block.GetContent, out var priceEffect))
                     che.Effects.Add(priceEffect);
                  else
                     Globals.ErrorLog.Write($"Invalid change_price effect in {che}: {block.GetContent}");
                  break;
               default:

                  Globals.ErrorLog.Write($"Unknown block in history entry: {block.Name} ({file})");
                  break;
            }
         }
      }

      private static void AssignHistoryEntryContent(ref CountryHistoryEntry che, ref string file, List<Content> element)
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
                  Globals.ErrorLog.Write($"Unknown key in history entry effects: {kvp.Key} in {content} ({file})");
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
            FilesHelper.GetFilePathUniquely(out var path, "common", Path.Combine(country.CountryFilePath.FilePathArr));
            if (!IO.ReadAllInANSI(path, out var content))
               return;

            path = path.Replace('/', '\\');

            var newPathObj = PathObj.FromPath(path);
            SaveMaster.AddToDictionary(ref newPathObj, new CommonCountry(country, ref newPathObj));

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
                  country.CommonCountry.RevolutionaryColor = revolutionaryColor;
                  break;
               case "historical_idea_groups":
                  foreach (var idea in block.GetContentElements)
                     country.CommonCountry.HistoricIdeas.AddRange(Parsing.GetStringList(idea.Value));
                  break;
               case "historical_units":
                  foreach (var unit in block.GetContentElements)
                     country.CommonCountry.HistoricUnits.AddRange(Parsing.GetStringList(unit.Value));
                  break;
               case "monarch_names":
                  foreach (var name in block.GetContentElements)
                  {
                     Parsing.ParseMonarchNames(name.Value, out var monarchNames);
                     country.CommonCountry.MonarchNames.AddRange(monarchNames);
                  }
                  break;
               case "ship_names":
                  foreach (var name in block.GetContentElements)
                     country.CommonCountry.ShipNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "fleet_names":
                  foreach (var name in block.GetContentElements)
                     country.CommonCountry.FleetNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "army_names":
                  foreach (var name in block.GetContentElements)
                     country.CommonCountry.ArmyNames.AddRange(Parsing.GetStringList(name.Value));
                  break;
               case "leader_names":
                  foreach (var name in block.GetContentElements)
                     country.CommonCountry.LeaderNames.AddRange(Parsing.GetStringList(name.Value));
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
               country.CommonCountry.CustomAttributes.Add(kvp.Key);
               continue;
            }

            switch (kvp.Key)
            {
               case "historical_council":
                  country.CommonCountry.HistoricalCouncil = kvp.Value;
                  break;
               case "historical_score":
                  country.CommonCountry.HistoricalScore = int.Parse(kvp.Value);
                  break;
               case "graphical_culture":
                  country.CommonCountry.GraphicalCulture = kvp.Value;
                  break;
               case "random_nation_chance":
                  if (int.TryParse(kvp.Value, out var value) && value == 0)
                     country.CommonCountry.RandomNationChance = value;
                  break;
               case "preferred_religion":
                  country.CommonCountry.PreferredReligion = kvp.Value;
                  break;
               case "colonial_parent":
                  country.CommonCountry.ColonialParent = new(kvp.Value);
                  break;
               case "special_unit_culture":
                  country.CommonCountry.SpecialUnitCulture = kvp.Value;
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

      [GeneratedRegex(@"(?<tag>[A-Z0-9]{3})\s*=\s*""(?<path>[^""]+)""", RegexOptions.Compiled)]
      private static partial Regex CountryTagRegex();
   }

}