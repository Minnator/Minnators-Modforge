using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;
using Effect = Editor.Loading.Enhanced.PCFL.Implementation.Effect;

namespace Editor.Parser;


public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";
   private const string ATTRIBUTE_PATTERN = "(?<key>\\w+)\\s*=\\s*(?<value>\"[^\"]*\"|[\\w-]+)";

   private const string MULTILINE_ATTRIBUTE_PATTERN =
      "(?<name>[A-Za-z_.0-9]+)\\s*=\\s*\\{\\s*(?<pairs>(?:\\s*[A-Za-z_.0-9]+\\s*=\\s*[^}\\s]+(?:\\s*\\n?)*)*)\\s*\\}\\s*(?<comment>#.*)?";

   private static readonly Regex DateRegex = new(DATE_PATTERN, RegexOptions.Compiled);
   internal static readonly Regex IdRegex = new(ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled);
   private static readonly Regex AttributeRegex = new(ATTRIBUTE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex MultilineAttributeRegex = new(MULTILINE_ATTRIBUTE_PATTERN, RegexOptions.Compiled);


   private static readonly Dictionary<string, (Action<Saveable, PropertyInfo, string, PathObj, int>, PropertyInfo)>
 ProvinceActionsAndProperties = new()
 {
     { "add_base_manpower",                  (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseManpower))!) },
     { "add_base_production",                (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseProduction))!) },
     { "add_base_tax",                       (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseTax))!) },
     { "add_claim",                          (Add<Country>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioClaims))!) },
     { "add_core",                           (Add<Country>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCores))!) },
     { "add_local_autonomy",                 (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioLocalAutonomy))!) },
     { "add_nationalism",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioNationalism))!) },
     { "add_permanent_claim",                (Add<Country>,                    Province.Empty.GetPropertyInfo(nameof(Province.PermaClaims))!) },
     { "add_permanent_province_modifier",    (Add<ApplicableModifier>,     Province.Empty.GetPropertyInfo(nameof(Province.PermanentProvinceModifiers))!) },
     { "add_province_modifier",              (Add<ApplicableModifier>,     Province.Empty.GetPropertyInfo(nameof(Province.ProvinceModifiers))!) },
     { "add_province_triggered_modifier",    (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ProvinceTriggeredModifiers))!) },
     { "add_to_trade_company",               (Set<TradeCompany>,           Province.Empty.GetPropertyInfo(nameof(Province.TradeCompany))!) },
     { "add_trade_company_investment",       (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioTradeCompanyInvestments))!) },
     { "base_manpower",                      (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseManpower))!) },
     { "base_production",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseProduction))!) },
     { "base_tax",                           (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioBaseTax))!) },
     { "capital",                            (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCapital))!) },
     { "center_of_trade",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCenterOfTrade))!) },
     { "citysize",                           (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCitySize))!) },
     { "controller",                         (Set<Country>,                Province.Empty.GetPropertyInfo(nameof(Province.ScenarioController))!) },
     { "culture",                            (Set<Culture>,                Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCulture))!) },
     { "devastation",                        (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioDevastation))!) },
     { "discovered_by",                      (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioDiscoveredBy))!) },
     { "extra_cost",                         (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioExtraCost))!) },
     { "hre",                                (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.ScenarioIsHre))!) },
     { "is_city",                            (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.ScenarioIsCity))!) },
     { "native_ferocity",                    (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioNativeFerocity))!) },
     { "native_hostileness",                 (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioNativeHostileness))!) },
     { "native_size",                        (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioNativeSize))!) },
     { "owner",                              (Set<Country>,                Province.Empty.GetPropertyInfo(nameof(Province.ScenarioOwner))!) },
     { "prosperity",                         (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioProsperity))!) },
     { "reformation_center",                 (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioReformationCenter))!) },
     { "religion",                           (Set<Religion>,               Province.Empty.GetPropertyInfo(nameof(Province.ScenarioReligion))!) },
     { "remove_claim",                       (Remove<Country>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioClaims))!) },
     { "remove_core",                        (Remove<Country>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioCores))!) },
     { "remove_discovered_by",               (Remove<string>,              Province.Empty.GetPropertyInfo(nameof(Province.ScenarioDiscoveredBy))!) },
     { "remove_permanent_claim",             (Remove<Country>,                 Province.Empty.GetPropertyInfo(nameof(Province.ScenarioPermanentClaims))!) },
     { "remove_permanent_province_modifier", (Remove<ApplicableModifier>,  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioPermanentProvinceModifiers))!) },
     { "remove_province_modifier",           (Remove<ApplicableModifier>,  Province.Empty.GetPropertyInfo(nameof(Province.ScenarioProvinceModifiers))!) },
     { "remove_province_triggered_modifier", (Remove<string>,              Province.Empty.GetPropertyInfo(nameof(Province.ScenarioProvinceTriggeredModifiers))!) },
     { "revolt",                             (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.ScenarioHasRevolt))!) },
     { "revolt_risk",                        (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioRevoltRisk))!) },
     { "seat_in_parliament",                 (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.ScenarioIsSeatInParliament))!) },
     { "trade_goods",                        (Set<TradeGood>,              Province.Empty.GetPropertyInfo(nameof(Province.ScenarioTradeGood))!) },
     { "tribal_owner",                       (Set<Country>,                Province.Empty.GetPropertyInfo(nameof(Province.ScenarioTribalOwner))!) },
     { "unrest",                             (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ScenarioRevoltRisk))!) }
 };


   private static void Add<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is ICollection<T>, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type ICollection<{typeof(T)}>");
      if (!Converter.Convert<T>(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum)))
         return;

      var current = new List<T>((ICollection<T>)propertyInfo.GetValue(saveable)!) { obj };
      propertyInfo.SetValue(saveable, current);
   }

   private static void Remove<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is ICollection<T>, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type ICollection<{typeof(T)}>");
      if (!Converter.Convert<T>(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum)))
         return;
      ((ICollection<T>)propertyInfo.GetValue(saveable)!).Remove(obj);
   }

   private static void Set<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is T, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type {typeof(T)}");
      Converter.Convert(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum));
      
      propertyInfo.SetValue(saveable, obj);
   }


   public static void ParseAllUniqueProvinces()
   {
      var files = PathManager.GetAllFilesInFolder("*.txt", "history", "provinces");
      var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };
      Parallel.ForEach(files, po, ProcessProvinceFile);
      //foreach (var file in files)
      //   ProcessProvinceFile(file);
   }

   private static void ProcessProvinceFile(string path)
   {

      List<IEnhancedElement> blocks = [];
      var po = PathObj.FromPath(path);
      blocks = po.LoadBaseOrder();

      var match = IdRegex.Match(Path.GetFileName(path));
      if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
      {
         _ = new LoadingError(po, $"Could not parse id from file name {Path.GetFileName(path)}");
         return;
      }
      if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
      {
         _ = new LoadingError(po, $"Could not find province with id {id}");
         return;
      } 
      province.SetPath(ref po);

      foreach (var block in blocks)
         if (block is EnhancedContent content)
            foreach (var line in content.GetLineKvpEnumerator(po, false))
               Eu4ProvAttributeRouting(province, line.Key, line.Value, po, line.Line);
         else
            // Parse the block, aka the history entries and some special cases
            ParseProvinceBlockBlock(ref province, (EnhancedBlock)block, po);

      SaveMaster.AddToDictionary(ref po, province);
      province.History.Sort();
   }

   private static void Eu4ProvAttributeRouting(Province province, string attribute, string value, PathObj po, int lineNum)
   {
      if (string.IsNullOrEmpty(attribute) || string.IsNullOrEmpty(value))
         _ = new LoadingError(po, "Either key or value for an attribute setter is null or empty!", line:lineNum);
      else
      {
         if (!ProvinceActionsAndProperties.TryGetValue(attribute.ToLower(), out var tuple))
         {
            //TODO rethink this usage of effects maybe?
            if (Scopes.ScriptedEffects.TryGetValue(attribute.ToLower(), out var scriptedEffect))
            {
               var effect = new Effect(province, ITarget.Empty, []);
               scriptedEffect.Invoke(null, new(attribute, value, lineNum), new(new SimpleFileScopeSwitch(Scopes.Province, province), effect), po);
               effect.Activate();
               return;
            }

            var building = Globals.Buildings.Find(x => x.Name.Equals(attribute.ToLower()));
            if (building != null)
            {
               province.ScenarioBuildings = [..province.ScenarioBuildings, building];
            }
            else
               _ = new LoadingError(po, $"Could not find attribute {attribute} in province attribute list", line: lineNum);
            return;
         }
         var (action, propertyInfo) = tuple;
         action(province, propertyInfo, value, po, lineNum);
      }
   }

   private static void ParseProvinceBlockBlock(ref Province province, EnhancedBlock block, PathObj po)
   {
      if (!Parsing.TryParseDate(block.Name, out var date))
      {
         // Take any scope with its effects save them as a complex effect and thus put it back into the province on saving
         if (ScopeParser.IsAnyScope(block.Name))
         {
            //province.Effects.Add(block);
            // TODO
            //EffectFactory.CreateComplexEffect(block.Name, block.GetContent, EffectValueType.Complex);
            return;
         }

         switch (block.Name.ToLower())
         {
            /*
            case "latent_trade_goods":
               {
                  if (Parsing.IsValidTradeGood(block.GetContent))
                     province.LatentTradeGood = TradeGoodHelper.StringToTradeGood(block.GetContent).Name;
                  return;
               }
            case "add_permanent_province_modifier":
               if (ModifierParser.ParseApplicableModifier(block.GetContent, out var mod))
                  province.PermanentProvinceModifiers.Add(mod);
               return;
            case "add_province_modifier":
               if (ModifierParser.ParseApplicableModifier(block.GetContent, out var mod2))
                  province.ProvinceModifiers.Add(mod2);
               return;
            case "add_trade_modifier":
               if (ModifierParser.ParseTradeModifier(block.GetContent, out var tradeMod))
                  province.TradeModifiers.Add(tradeMod);
               return;
            case "spawn_rebels":
               /*
               province.Effects.Add(block);
               if (EffectParser.ParseSpawnRebels(block.GetContent, out var rebelsEffect))
                  province.Effects.Add(rebelsEffect);
               
               return;
            */
            default:
               Globals.ErrorLog.Write($"Could not parse date: {block.Name}");
               return;
         }
      }

      var historyEntry = new ProvinceHistoryEntry(date, HistoryEntryManager.PHEIndex++);

      var effect = Effect.ConstructEffect(block.GetElements(), po, Scopes.Province, province);

      historyEntry.Effects.AddRange(effect.tokens);

      province.History.Add(historyEntry);
   }

   /*
   private static void AddEffectsToComplexEffect(ref ComplexEffect ce, string content)
   {
      var attributes = Parsing.GetKeyValueList(content);
      foreach (var element in attributes)
      {
         var type = EffectValueType.String;
         if (int.TryParse(element.Value, out _))
            type = EffectValueType.Int;
         else if (float.TryParse(element.Value, out _))
            type = EffectValueType.Float;
         ce.Effects.Add(EffectFactory.CreateSimpleEffect(element.Key, element.Value, type, Scope.Country));//TODO check if it is country scope
      }
   }

   private static void AddEffectsToHistory(ref ProvinceHistoryEntry che, Content content)
   {
      var attributes = Parsing.GetKeyValueList(content.Value);
      foreach (var element in attributes)
      {
         var type = EffectValueType.String;
         if (int.TryParse(element.Value, out _))
            type = EffectValueType.Int;
         else if (float.TryParse(element.Value, out _))
            type = EffectValueType.Float;
         che.Effects.Add(EffectFactory.CreateSimpleEffect(element.Key, element.Value, type, Scope.Country));//TODO check if it is country scope
      }
   }
   */
}