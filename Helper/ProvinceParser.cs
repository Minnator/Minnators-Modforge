using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses;

namespace Editor.Helper;

public class ParsingProvince(List<HistoryEntryOld> entries, string remainder)
{
   public List<HistoryEntryOld> Entries = entries;
   public string Remainder = remainder;
   public List<KeyValuePair<string, string>> Attributes = [];
   public List<MultilineAttribute> MultilineAttributes = [];
   public int Id { get; set; }
}

public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";
   private const string ATTRIBUTE_PATTERN = "(?<key>\\w+)\\s*=\\s*(?<value>\"[^\"]*\"|[\\w-]+)";

   private const string MULTILINE_ATTRIBUTE_PATTERN =
      "(?<name>[A-Za-z_.0-9]+)\\s*=\\s*\\{\\s*(?<pairs>(?:\\s*[A-Za-z_.0-9]+\\s*=\\s*[^}\\s]+(?:\\s*\\n?)*)*)\\s*\\}\\s*(?<comment>#.*)?";

   private static readonly Regex DateRegex = new (DATE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex IdRegex = new (ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled);
   private static readonly Regex AttributeRegex = new (ATTRIBUTE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex MultilineAttributeRegex = new (MULTILINE_ATTRIBUTE_PATTERN, RegexOptions.Compiled);



   public static void ParseAllUniqueProvinces(string modFolder, string vanillaFolder)
   {
      var sw = Stopwatch.StartNew();
      // Get all unique province files from mod and vanilla
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      // Get All nested Blocks and Attributes from the files
      foreach (var file in files)
      {
         ProcessProvinceFile(file);
      }
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing provinces", sw.ElapsedMilliseconds);
   }

   private static void ProcessProvinceFile(string path)
   {
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

      var fileContent = IO.ReadAllInUTF8(path);
      var blocks = Parsing.GetNestedElementsIterative(0, ref fileContent);

      foreach (var block in blocks)
      {
         if (block is Content content)
         {
            ParseProvinceContentBlock(ref province, content);
         }
         else
         {
            ParseProvinceBlockBlock(ref province, (Block)block);
         }
      }

   }

   private static void ParseProvinceContentBlock(ref Province province, Content content)
   {
      var attributes = Parsing.GetKeyValueList(content.Value);
      AssignAttributesToProvince(attributes, ref province);
   }

   private static void ParseProvinceBlockBlock(ref Province province, Block block)
   {
      if (!DateTime.TryParse(block.Name, out var date))
      {
         switch (block.Name.ToLower())
         {
            case "latent_trade_goods":
               var ltg = Parsing.GetLatentTradeGood(block.GetContentElements[0]);
               province.LatentTradeGood = ltg;
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
         else if (element is Block subBlock && subBlock.HasOnlyContent)
         {
            var ce = EffectFactory.CreateComplexEffect(subBlock.Name, EffectValueType.Complex);
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
         ce.Effects.Add(EffectFactory.CreateSimpleEffect(element.Key, element.Value, type));
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
         che.Effects.Add(EffectFactory.CreateSimpleEffect(element.Key, element.Value, type));
      }
   }

   private static void AssignAttributesToProvince(List<KeyValuePair<string, string>> attributes, ref Province prov)
   {
      foreach (var att in attributes)
      {
         prov.SetAttribute(att.Key, att.Value);
      }
   }







   // OLD AND OBSOLETE

   public static void ParseAllProvinces(string modFolder, string vanillaFolder, ref Log loadingLog)
   {
      var sw = Stopwatch.StartNew();

      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      ParseProvinces(files, out var entries);

      sw.Stop();
      loadingLog.WriteTimeStamp("Parsing provinces", sw.ElapsedMilliseconds);
      sw.Restart();

      foreach (var entry in entries.Values)
      {
         GetMultilineAttributes(entry.Remainder, out var multilineAttributes);
         entry.MultilineAttributes = multilineAttributes;
         GetProvinceSpecificAttributes(ref entry.Remainder, out var attributes);
         entry.Attributes = attributes;
      }

      AssignProvinceAttributes([.. entries.Values]);
      sw.Stop();
      loadingLog.WriteTimeStamp("Assigning province attributes", sw.ElapsedMilliseconds);
      Debug.WriteLine($"Parsing and Init of provinces took {sw.ElapsedMilliseconds} ms for {entries.Count}");
   }

   private static void GetMultilineAttributes(string entryRemainder, out List<MultilineAttribute> attrs)
   {
      attrs = [];
      var matches = MultilineAttributeRegex.Matches(entryRemainder);
      if (matches.Count == 0) // Add a check for matches to prevent the overhead of entering the loop as it is only needed in very few cases
         return;
      foreach (Match match in matches)
      {
         GetAttributes(match.Groups["pairs"].Value, out var values);
         attrs.Add(new(match.Groups["name"].Value, values));
      }
   }

   private static void ParseProvinces(List<string> files, out Dictionary<int, ParsingProvince> entries)
   {
      entries = [];
      var provinces = entries;

      Parallel.ForEach(files, file =>
      {
         var match = IdRegex.Match(Path.GetFileName(file));
         if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
            throw new($"Could not parse province id from file name: {file}\nCould not match \'<number> <.*>\'");
         var historyEntries = ParesHistoryFromProvinceFile(file, out var remainder);
         ParsingProvince province = new(historyEntries, remainder) { Id = id };
         lock (provinces)
         {
            if (!provinces.TryAdd(id, province))
                Debug.WriteLine($"Could not add province {id} to the dictionary");
         }
      });
      Debug.WriteLine($"History files parsed");
   }

   private static void GetAttributes(string content, out List<KeyValuePair<string, string>> attributes)
   {
      attributes = [];
      if (string.IsNullOrEmpty(content))
         return;
      StringBuilder sb = new(content);

      var lines = content.Split('\n');

      for (var index = lines.Length - 1; index >= 0; index--)
      {
         var lineContent = Parsing.RemoveAndGetCommentFromString(lines[index]);
         var match = AttributeRegex.Match(lineContent);

         if (!match.Success)
            continue;
         attributes.Add(new(match.Groups["key"].Value, match.Groups["value"].Value));

         sb.Remove(match.Index, match.Length);
      }
   }
   private static void GetProvinceSpecificAttributes(ref string content, out List<KeyValuePair<string, string>> attributes)
   {
      attributes = [];
      if (string.IsNullOrEmpty(content))
         return;

      var lines = content.Split('\n');

      foreach (var line in lines)
      {
         var match = AttributeRegex.Match(Parsing.RemoveAndGetCommentFromString(line));

         if (!match.Success)
            continue;
         var key = match.Groups["key"].Value;
         if (!Globals.UniqueAttributeKeys.Contains(key))
            continue;
         var value = match.Groups["value"].Value;
         attributes.Add(new(key, value));
      }
   }


   private static List<HistoryEntryOld> ParesHistoryFromProvinceFile(string path, out string remainder)
   {
      IO.ReadAllInANSI(path, out var fileContent);

      var entries = new List<HistoryEntryOld>();
      var endOfLastMatch = 0;
      remainder = fileContent;

      while (true)
      {
         // Match and parse the date
         var match = DateRegex.Match(fileContent, endOfLastMatch);
         if (!match.Success)
            break;
         if (!DateTime.TryParse(match.Value, out var date))
            throw new ($"Could not parse date: {match.Value} at position {match.Index} in file {path}");

         // Get child groups and the line ending after the last group
         var groups = Parsing.GetGroups(ref fileContent, match.Index, out var lastGroupEnding);
         var eol = Parsing.GetLineEndingAfterComment(lastGroupEnding, ref fileContent, out var comment);

         endOfLastMatch = lastGroupEnding + 1; // +1 to include the line ending

         var content = fileContent.Substring(match.Index, endOfLastMatch - match.Index);

         // Create the history entryOld and add it to the list
         HistoryEntryOld entryOld = new(date, content, groups, comment)
         {
            Start = match.Index,
            End = Math.Max(eol, endOfLastMatch)
         };

         entries.Add(entryOld);
      }

      // Remove all entries from the remainder to prevent future misinterpretation in the MultilineAttribute parsing
      StringBuilder remainderBuilder = new(remainder);
      for (var i = entries.Count - 1; i >= 0; i--) 
         remainderBuilder.Remove(entries[i].Start, entries[i].End - entries[i].Start);
      remainder = remainderBuilder.ToString();

      return entries;
   }

   public static void AssignProvinceAttributes(List<ParsingProvince> parsingProvinces)
   {
      foreach (var parsProv in parsingProvinces)
      {
         var prov = Globals.Provinces[parsProv.Id];
         foreach (var att in parsProv.Attributes)
         {
            switch (att.Key)
            {
               case "add_claim":
                  prov.Claims.Add(Tag.FromString(att.Value));
                  break;
               case "add_core":
                  prov.Cores.Add(Tag.FromString(att.Value));
                  break;
               case "base_manpower":
                  if (int.TryParse(att.Value, out var manpower))
                     prov.BaseManpower = manpower;
                  else
                     throw new AttributeParsingException ($"Could not parse base_manpower: {att.Value} for province id {prov.Id}");
                  break;
               case "base_production":
                  if (int.TryParse(att.Value, out var production))
                     prov.BaseProduction = production;
                  else
                     throw new AttributeParsingException ($"Could not parse base_production: {att.Value} for province id {prov.Id}");
                  break;
               case "base_tax":
                  if (int.TryParse(att.Value, out var tax))
                     prov.BaseTax = tax;
                  else
                     throw new AttributeParsingException ($"Could not parse base_tax: {att.Value} for province id {prov.Id}");
                  break;
               case "capital":
                  prov.Capital = att.Value;
                  break;
               case "center_of_trade":
                  if (int.TryParse(att.Value, out var cot))
                     prov.CenterOfTrade = cot;
                  else
                     throw new AttributeParsingException ($"Could not parse center_of_trade: {att.Value} for province id {prov.Id}");
                  break;
               case "controller":
                  prov.Controller = Tag.FromString(att.Value);
                  break;
               case "culture":
                  prov.Culture = att.Value;
                  break;
               case "discovered_by":
                  prov.DiscoveredBy.Add(att.Value);
                  break;
               case "extra_cost":
                  if (int.TryParse(att.Value, out var cost))
                     prov.ExtraCost = cost;
                  else
                     throw new AttributeParsingException ($"Could not parse extra_cost: {att.Value} for province id {prov.Id}");
                  break;
               case "fort_15th":
                  prov.HasFort15Th = Parsing.YesNo(att.Value);
                  break;
               case "hre":
                  prov.IsHre = Parsing.YesNo(att.Value);
                  break;
               case "is_city":
                  prov.IsCity = Parsing.YesNo(att.Value);
                  break;
               case "native_ferocity":
                  if (int.TryParse(att.Value, out var ferocity))
                     prov.NativeFerocity = ferocity;
                  else
                     throw new AttributeParsingException ($"Could not parse native_ferocity: {att.Value} for province id {prov.Id}");
                  break;
               case "native_hostileness":
                  if (int.TryParse(att.Value, out var hostileness))
                     prov.NativeHostileness = hostileness;
                  else
                     throw new AttributeParsingException ($"Could not parse native_hostileness: {att.Value} for province id {prov.Id}");
                  break;
               case "native_size":
                  if (int.TryParse(att.Value, out var size))
                     prov.NativeSize = size;
                  else
                     throw new AttributeParsingException ($"Could not parse native_size: {att.Value} for province id {prov.Id}");
                  break;
               case "owner":
                  prov.Owner = Tag.FromString(att.Value);
                  break;
               case "religion":
                  prov.Religion = att.Value;
                  break;
               case "seat_in_parliament":
                  prov.IsSeatInParliament = Parsing.YesNo(att.Value);
                  break;
               case "trade_goods":
                  prov.TradeGood = TradeGoodHelper.FromString(att.Value);
                  break;
               case "tribal_owner":
                  prov.TribalOwner = Tag.FromString(att.Value);
                  break;
               case "unrest":
                  if (int.TryParse(att.Value, out var unrest))
                     prov.RevoltRisk = unrest;
                  else
                     throw new AttributeParsingException ($"Could not parse unrest: {att.Value} for province id {prov.Id}");
                  break;
               case "shipyard":
                  // TODO parse shipyard
                  break;
               case "revolt_risk":
                  if (int.TryParse(att.Value, out var risk))
                     prov.RevoltRisk = risk;
                  else
                     throw new AttributeParsingException ($"Could not parse revolt_risk: {att.Value} for province id {prov.Id}");
                  break;
               case "add_local_autonomy":
                  if (int.TryParse(att.Value, out var autonomy))
                     prov.LocalAutonomy = autonomy;
                  else
                     throw new AttributeParsingException ($"Could not parse add_local_autonomy: {att.Value} for province id {prov.Id}");
                  break;
               case "add_nationalism":
                  if (int.TryParse(att.Value, out var nationalism))
                     prov.Nationalism = nationalism;
                  else
                     throw new AttributeParsingException ($"Could not parse add_nationalism: {att.Value} for province id {prov.Id}");
                  break;
               default:
                  Debug.WriteLine($"Unknown attribute {att.Key} for province id {prov.Id}");
                  break;
            }
         }
      } 
   }
}

public class AttributeParsingException(string message) : Exception(message);