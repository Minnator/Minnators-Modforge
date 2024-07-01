using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor.DataClasses;
using Group = Editor.DataClasses.Group;

namespace Editor.Helper;

public struct ParsingProvince(List<HistoryEntry> entries, string remainder)
{
   public List<HistoryEntry> Entries = entries;
   public string Remainder = remainder;
}

public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";

   private static readonly Regex DateRegex = new (DATE_PATTERN, RegexOptions.Compiled);
   private static readonly Regex IdRegex = new (ID_FROM_FILE_NAME_PATTERN, RegexOptions.Compiled);

   public static void ParseAllProvinces(string modFolder, string vanillaFolder)
   {
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      
      var sw = Stopwatch.StartNew();

      Dictionary<int, ParsingProvince> entries = [];
      ParseProvinces(files, entries);
      
      sw.Stop();
      Debug.WriteLine($"Parsing provinces took {sw.ElapsedMilliseconds} ms for {entries.Count}");

   }

   private static void ParseProvinces(List<string> files, Dictionary<int, ParsingProvince> entries)
   {
      Parallel.ForEach(files, file =>
      {
         var match = IdRegex.Match(file);
         if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
            throw new($"Could not parse province id from file name: {file}\nCould not match \'<number> - <.*>\'");

         var historyEntries = ParesHistoryFile(file, out var remainder);

         entries.Add(id, new(historyEntries, remainder));
      });
      Debug.WriteLine($"History files parsed");
   }

   private static List<HistoryEntry> ParesHistoryFile(string path, out string remainder)
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

         endOfLastMatch = lastGroupEnding + 1;

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

}