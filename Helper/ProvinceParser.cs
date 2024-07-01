using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Editor.DataClasses;

namespace Editor.Helper;

public class ParsingProvince(List<HistoryEntry> entries, string remainder)
{
   public List<HistoryEntry> Entries = entries;
   public string Remainder = remainder;
   public List<KeyValuePair<string, string>> Attributes = [];
   public int Id { get; set; }
}

public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";
   private const string ATTRIBUTE_PATTERN = "(?<key>\\w+)\\s*=\\s*(?<value>\"[^\"]*\"|[\\w-]+)";

   private static readonly Regex DateRegex = new (DATE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex IdRegex = new (ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled);
   private static readonly Regex AttributeRegex = new (ATTRIBUTE_PATTERN, RegexOptions.Compiled);

   private static readonly HashSet<string> UniqueAttributeKeys = [
      "add_claim", "add_core", "add_local_autonomy", "add_nationalism", "base_manpower", "base_production", "base_tax", "capital", "center_of_trade", "controller", "culture", "discovered_by", "extra_cost", "fort_15th", "hre", "is_city", "native_ferocity", "native_hostileness", "native_size", "owner", "religion", "seat_in_parliament", "trade_goods", "tribal_owner", "unrest", "shipyard", "revolt_risk"
      ];

   public static void ParseAllProvinces(string modFolder, string vanillaFolder)
   {
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      
      var sw = Stopwatch.StartNew();

      Dictionary<int, ParsingProvince> entries = [];
      ParseProvinces(files, entries);
      
      sw.Stop();
      Debug.WriteLine($"Parsing provinces took {sw.ElapsedMilliseconds} ms for {entries.Count}");
      sw.Restart();

      foreach (var entry in entries.Values)
      {
         if (entry == null)
            continue;
         List<KeyValuePair<string, string>> attributes = [];
         GetProvinceAttributesKeyValuePairs(entry.Remainder, attributes);
         entry.Attributes = attributes;
      }

      sw.Stop();
      Debug.WriteLine($"Parsing attributes took {sw.ElapsedMilliseconds} ms for {entries.Count}");

      sw.Restart();
      AssignProvinceAttributes([.. entries.Values]);
      sw.Stop();
      Debug.WriteLine($"Assigning attributes took {sw.ElapsedMilliseconds} ms for {entries.Count}");

      Globals.ParsingProvinces = [.. entries.Values];
   }

   private static void ParseProvinces(List<string> files, Dictionary<int, ParsingProvince> entries)
   {
      Parallel.ForEach(files, file =>
      {
         var match = IdRegex.Match(file);
         if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
            throw new($"Could not parse province id from file name: {file}\nCould not match \'<number> <.*>\'");
         var historyEntries = ParesHistoryFromProvinceFile(file, out var remainder);
         entries.Add(id, new(historyEntries, remainder){Id = id});
      });
      Debug.WriteLine($"History files parsed");
   }

   private static void GetProvinceAttributesKeyValuePairs(string content, List<KeyValuePair<string, string>> attributes)
   {
      if (string.IsNullOrEmpty(content))
         return;
      StringBuilder sb = new(content);

      var matches = AttributeRegex.Matches(content);
      foreach (Match match in matches)
      {
         var key = match.Groups["key"].Value;
         if (!UniqueAttributeKeys.Contains(key))
            continue;
         var value = match.Groups["value"].Value;
         attributes.Add(new(key, value));
      }

      for (var i = matches.Count - 1; i >= 0; i--)
         sb.Remove(matches[i].Index, matches[i].Length);
   }

   private static List<HistoryEntry> ParesHistoryFromProvinceFile(string path, out string remainder)
   {
      IO.ReadAllInANSI(path, out var fileContent);

      var entries = new List<HistoryEntry>();
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

         var groups = Parsing.GetGroups(ref fileContent, match.Index, out var lastGroupEnding);
         var eol = Parsing.GetLineEndingAfterComment(lastGroupEnding, ref fileContent, out var comment);

         endOfLastMatch = lastGroupEnding + 1; // +1 to include the line ending

         var content = fileContent.Substring(match.Index, endOfLastMatch - match.Index);


         HistoryEntry entry = new(date, content, groups, comment)
         {
            Start = match.Index,
            End = Math.Max(eol, endOfLastMatch)
         };

         entries.Add(entry);
      }

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
               default:
                  Debug.WriteLine($"Unknown attribute {att.Key} for province id {prov.Id}");
                  break;
            }
         }
      } 
   }
}

public class AttributeParsingException(string message) : Exception(message);