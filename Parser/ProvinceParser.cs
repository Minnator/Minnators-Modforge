using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Parser;

public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";
   private const string ATTRIBUTE_PATTERN = "(?<key>\\w+)\\s*=\\s*(?<value>\"[^\"]*\"|[\\w-]+)";

   private const string MULTILINE_ATTRIBUTE_PATTERN =
      "(?<name>[A-Za-z_.0-9]+)\\s*=\\s*\\{\\s*(?<pairs>(?:\\s*[A-Za-z_.0-9]+\\s*=\\s*[^}\\s]+(?:\\s*\\n?)*)*)\\s*\\}\\s*(?<comment>#.*)?";

   private static readonly Regex DateRegex = new(DATE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex IdRegex = new(ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled);
   private static readonly Regex AttributeRegex = new(ATTRIBUTE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex MultilineAttributeRegex = new(MULTILINE_ATTRIBUTE_PATTERN, RegexOptions.Compiled);


   public static void ParseAllUniqueProvinces()
   {
      var sw = Stopwatch.StartNew();
      // Get all unique province files from mod and vanilla
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(Globals.MapWindow.Project.ModPath, Globals.MapWindow.Project.VanillaPath, "history", "provinces");
      // Get All nested Blocks and Attributes from the files
      var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };
      //Parallel.ForEach(files, po, ProcessProvinceFile);

      foreach (var file in files)
      {
         ProcessProvinceFile(file);
      }

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing provinces", sw.ElapsedMilliseconds);
      DebugPrints.PrintAllProvinceHistories();
   }

   private static void ProcessProvinceFile(string path)
   {
      // Retrieve the ID from the file name
      var match = IdRegex.Match(Path.GetFileName(path));
      if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
      {
         Globals.ErrorLog.Write($"Could not parse province id from file name: {path}\nCould not match \'<number> <.*>\'");
         return;
      }

      if (!Globals.Provinces.TryGetValue(id, out var province))
      {
         Globals.ErrorLog.Write($"Could not find province with id {id}");
         return;
      } 
      IO.ReadAllInANSI(path, out var rawContent);
      Parsing.RemoveCommentFromMultilineString(rawContent, out var fileContent);
      var blocks = Parsing.GetElements(0, ref fileContent);

      foreach (var block in blocks)
      {
         // Check if the block is a Content or a Block
         if (block is Content content)
         {
            // Parse the content, aka the attributes
            foreach (var att in Parsing.GetKeyValueList(content.Value))
            {
               province.SetAttribute(att.Key, att.Value);
            }

         }
         else
         {
            // Parse the block, aka the history entries and some special cases
            ParseProvinceBlockBlock(ref province, (Block)block);
         }
      }

      // Copy the initial attributes to the ProvinceData to be able to revert to the initial state
      province.InitializeInitial();
   }

   private static void ParseProvinceBlockBlock(ref Province province, Block block)
   {
      if (!DateTime.TryParse(block.Name, out var date))
      {
         // Take any scope with its effects save them as a complex effect and thus put it back into the province on saving
         if (ScopeParser.IsAnyScope(block.Name))
         {
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
               if (ModifierParser.ParseProvinceModifier(block.GetContent, out var mod))
                  province.PermanentProvinceModifiers.Add(mod);
               return;
            case "add_trade_modifier":
               if (ModifierParser.ParseTradeModifier(block.GetContent, out var tradeMod))
                  province.TradeModifiers.Add(tradeMod);
               return;
            case "spawn_rebels":
               if (EffectParser.ParseSpawnRebels(block.GetContent, out var rebelsEffect))
                  province.Effects.Add(rebelsEffect);
               return;
            default:
               Globals.ErrorLog.Write($"Could not parse date: {block.Name}");
               return;
         }
      }

      var che = new HistoryEntry(date);

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

   private static void AddEffectsToHistory(ref HistoryEntry che, Content content)
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

public class AttributeParsingException(string message) : Exception(message);