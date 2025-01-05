using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Saving;

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
     { "add_base_manpower",                  (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseManpower))!) },
     { "add_base_production",                (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseProduction))!) },
     { "add_base_tax",                       (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseTax))!) },
     { "add_claim",                          (Add<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.Claims))!) },
     { "add_core",                           (Add<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.Cores))!) },
     { "add_local_autonomy",                 (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.LocalAutonomy))!) },
     { "add_nationalism",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.Nationalism))!) },
     { "add_permanent_claim",                (Add<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.PermanentClaims))!) },
     { "add_permanent_province_modifier",    (Add<ApplicableModifier>,     Province.Empty.GetPropertyInfo(nameof(Province.PermanentProvinceModifiers))!) },
     { "add_province_modifier",              (Add<ApplicableModifier>,     Province.Empty.GetPropertyInfo(nameof(Province.ProvinceModifiers))!) },
     { "add_province_triggered_modifier",    (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ProvinceTriggeredModifiers))!) },
     { "add_to_trade_company",               (Set<TradeCompany>,           Province.Empty.GetPropertyInfo(nameof(Province.TradeCompany))!) },
     { "add_trade_company_investment",       (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.TradeCompanyInvestments))!) },
     { "base_manpower",                      (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseManpower))!) },
     { "base_production",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseProduction))!) },
     { "base_tax",                           (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.BaseTax))!) },
     { "capital",                            (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.Capital))!) },
     { "center_of_trade",                    (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.CenterOfTrade))!) },
     { "citysize",                           (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.CitySize))!) },
     { "controller",                         (Set<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.Controller))!) },
     { "culture",                            (Set<Culture>,                Province.Empty.GetPropertyInfo(nameof(Province.Culture))!) },
     { "devastation",                        (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.Devastation))!) },
     { "discovered_by",                      (Add<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.DiscoveredBy))!) },
     { "extra_cost",                         (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.ExtraCost))!) },
     { "hre",                                (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.IsHre))!) },
     { "is_city",                            (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.IsCity))!) },
     { "native_ferocity",                    (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.NativeFerocity))!) },
     { "native_hostileness",                 (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.NativeHostileness))!) },
     { "native_size",                        (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.NativeSize))!) },
     { "owner",                              (Set<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.Owner))!) },
     { "prosperity",                         (Set<float>,                  Province.Empty.GetPropertyInfo(nameof(Province.Prosperity))!) },
     { "reformation_center",                 (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.ReformationCenter))!) },
     { "religion",                           (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.Religion))!) },
     { "remove_claim",                       (Remove<Tag>,                 Province.Empty.GetPropertyInfo(nameof(Province.Claims))!) },
     { "remove_core",                        (Remove<Tag>,                 Province.Empty.GetPropertyInfo(nameof(Province.Cores))!) },
     { "remove_discovered_by",               (Remove<string>,              Province.Empty.GetPropertyInfo(nameof(Province.DiscoveredBy))!) },
     { "remove_permanent_claim",             (Remove<Tag>,                 Province.Empty.GetPropertyInfo(nameof(Province.PermanentClaims))!) },
     { "remove_permanent_province_modifier", (Remove<ApplicableModifier>,  Province.Empty.GetPropertyInfo(nameof(Province.PermanentProvinceModifiers))!) },
     { "remove_province_modifier",           (Remove<ApplicableModifier>,  Province.Empty.GetPropertyInfo(nameof(Province.ProvinceModifiers))!) },
     { "remove_province_triggered_modifier", (Remove<string>,              Province.Empty.GetPropertyInfo(nameof(Province.ProvinceTriggeredModifiers))!) },
     { "revolt",                             (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.HasRevolt))!) },
     { "revolt_risk",                        (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.RevoltRisk))!) },
     { "seat_in_parliament",                 (Set<bool>,                   Province.Empty.GetPropertyInfo(nameof(Province.IsSeatInParliament))!) },
     { "trade_goods",                        (Set<string>,                 Province.Empty.GetPropertyInfo(nameof(Province.TradeGood))!) },
     { "tribal_owner",                       (Set<Tag>,                    Province.Empty.GetPropertyInfo(nameof(Province.TribalOwner))!) },
     { "unrest",                             (Set<int>,                    Province.Empty.GetPropertyInfo(nameof(Province.RevoltRisk))!) }
 };


   private static void Add<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is ICollection<T>, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type ICollection<{typeof(T)}>");
      if (!Converter.Convert<T>(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum)))
         return;
      ((ICollection<T>)propertyInfo.GetValue(saveable))!.Add(obj);
   }

   private static void Remove<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is ICollection<T>, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type ICollection<{typeof(T)}>");
      if (!Converter.Convert<T>(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum)))
         return;
      ((ICollection<T>)propertyInfo.GetValue(saveable))!.Remove(obj);
   }

   private static void Set<T>(Saveable saveable, PropertyInfo propertyInfo, string value, PathObj po, int lineNum)
   {
      Debug.Assert(propertyInfo.GetValue(saveable) is T, $"{propertyInfo.Name} ({propertyInfo.PropertyType}) is not type {typeof(T)}");
      Converter.Convert(value, out T obj).Then((obj) => obj.ConvertToLoadingError(po, $"Could not convert value \"{value}\" to {typeof(T)}", lineNum));
      
      propertyInfo.SetValue(saveable, obj);
   }


   public static void ParseAllUniqueProvinces()
   {
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "history", "provinces");
      var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };
      //Parallel.ForEach(files, po, ProcessProvinceFile);
      foreach (var file in files)
         ProcessProvinceFile(file);
   }

   private static void ProcessProvinceFile(string path)
   {
      var match = IdRegex.Match(Path.GetFileName(path));
      if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
      {
         Globals.ErrorLog.Write($"Could not parse province id from file name: {path}\nCould not match \'<number> <.*>\'");
         return;
      }

      if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
      {
         Globals.ErrorLog.Write($"Could not find province with id {id}");
         return;
      } 
      var pathObj = PathObj.FromPath(path);
      province.SetPath(ref pathObj);
      
      IO.ReadAllInANSI(path, out var rawContent);
      Parsing.RemoveCommentFromMultilineString(rawContent, out var fileContent);

      List<IElement> blocks = [];
      
      try
      {
         blocks = Parsing.GetElements(0, ref fileContent);
      }
      catch (ParsingException e)
      {
         Globals.ErrorLog.Write($"Error parsing province file: {path} || {e.Message}");
         return;
      }
      foreach (var block in blocks)
         if (block is Content content)
            foreach (KeyValuePair<string, string> listWithoutQuote in Parsing.GetKeyValueListWithoutQuotes(content.Value))
               Eu4ProvAttributeRouting(province, listWithoutQuote.Key, listWithoutQuote.Value, pathObj, -1);
         else
            // Parse the block, aka the history entries and some special cases
            ParseProvinceBlockBlock(ref province, (Block)block);

      SaveMaster.AddToDictionary(ref pathObj, province);
   }

   private static void Eu4ProvAttributeRouting(Province province, string attribute, string value, PathObj po, int lineNum)
   {
      if (string.IsNullOrEmpty(attribute) || string.IsNullOrEmpty(value))
         _ = new LoadingError(po, "Either key or value for an attribute setter is null or empty!");
      else
      {
         if (!ProvinceActionsAndProperties.TryGetValue(attribute.ToLower(), out var tuple))
            return;
         var (action, propertyInfo) = tuple;
         action(province, propertyInfo, value, po, lineNum);
      }
   }

   private static void ParseProvinceBlockBlock(ref Province province, Block block)
   {
      if (!Parsing.TryParseDate(block.Name, out var date))
      {
         // Take any scope with its effects save them as a complex effect and thus put it back into the province on saving
         if (ScopeParser.IsAnyScope(block.Name) || EffectParser.IsScriptedEffect(block.Name))
         {
            //province.Effects.Add(block);
            EffectFactory.CreateComplexEffect(block.Name, block.GetContent, EffectValueType.Complex);
            return;
         }

         switch (block.Name.ToLower())
         {
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
               */
               return;
            default:
               Globals.ErrorLog.Write($"Could not parse date: {block.Name}");
               return;
         }
      }

      var che = new ProvinceHistoryEntry(date);

      foreach (var element in block.Blocks)
      {
         if (element is Content content)
         {
            AddEffectsToHistory(ref che, content);
         }
         else if (element is Block { HasOnlyContent: true } subBlock)
         {
            var ce = EffectFactory.CreateComplexEffect(subBlock.Name, subBlock.GetContent, EffectValueType.Complex);
            if (subBlock.Blocks.Count == 0)
               AddEffectsToComplexEffect(ref ce, string.Empty);
            else
               AddEffectsToComplexEffect(ref ce, subBlock.GetContentElements[0].Value);

            che.Effects.Add(ce);
         }
      }

      province.History.Add(che);
   }

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

}