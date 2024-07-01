using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

   public static void ParseAllProvinces(string modFolder, string vanillaFolder)
   {
      var sw = Stopwatch.StartNew();
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      Dictionary<int, ParsingProvince> entries = [];
      ParseProvinces(files, entries);

      sw.Stop();
      Debug.WriteLine($"Parsing provinces took {sw.ElapsedMilliseconds} ms");

      var totalEntryCount = 0;
      foreach (var entry in entries) 
         totalEntryCount += entry.Value.Entries.Count;

      Debug.WriteLine($"Total entries: {totalEntryCount}");

      Debug.WriteLine(entries[1].Remainder);

   }

   private static void ParseProvinces(List<string> files, Dictionary<int, ParsingProvince> entries)
   {
      var idPattern = new Regex(ID_FROM_FILE_NAME_PATTERN);
      
      foreach (var file in files)
      {
         var match = idPattern.Match(file);
         if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
            throw new Exception($"Could not parse province id from file name: {file}\nCould not match \'<number> - <.*>\'");

         var historyEntries = ParesHistoryFile(file, out var remainder);

         entries.Add(id, new (historyEntries, remainder));

         // Percentage of completion
         if (entries.Count % 20 == 0)
         {
            Debug.WriteLine($"{entries.Count,4} / {files.Count,4} [{entries.Count * 100 / files.Count,3}%] files parsed");
         }
      }

   }

   private static List<HistoryEntry> ParesHistoryFile(string path, out string remainder)
   {
      if (!File.Exists(path))
         throw new Exception($"File does not exist: {path} or can not access");
      IO.ReadAllInANSI(path, out var fileContent );
      
      var entries = new List<HistoryEntry>();
      var regex = new Regex(DATE_PATTERN);
      var endOfLastMatch = 0;
      remainder = fileContent;

      while (true)
      {
         // Match and parse the date
         var match = regex.Match(fileContent, endOfLastMatch);
         if (!match.Success)
            break;
         if (!DateTime.TryParse(match.Value, out var date))
            throw new Exception($"Could not parse date: {match.Value} at position {match.Index} in file {path}");

         var groups = Parsing.GetGroups(ref fileContent, match.Index, out var last);

         endOfLastMatch = last + 1;
         var historyContent = fileContent.Substring(match.Index, endOfLastMatch - match.Index);
         var (content, comment) = Parsing.RemoveAndGetCommentFromString(historyContent);
         HistoryEntry entry = new(date, content, groups, comment)
         {
            Start = match.Index,
            End = endOfLastMatch
         };
         entries.Add(entry);
      }

      for (var i = entries.Count - 1; i >= 0; i--)
      {
         remainder = remainder.Remove(entries[i].Start, entries[i].End - entries[i].Start);
      }

      return entries;
   }

   private static List<HistoryEntry> ParseHistoryEntriesFromFile(string file)
   {
      if (!IO.ReadAllInANSI(file, out var fileContent))
         throw new Exception($"Could not read file: {file}");

      fileContent += "\n";

      var entries = new List<HistoryEntry>();
      var regex = new Regex(DATE_PATTERN);
      var eol = 0;

      while (true)
      {
         var match = regex.Match(fileContent, eol);
         if (!match.Success)
            break;

         // Try to parse the date
         if (!DateTime.TryParse(match.Value, out var date))
            throw new Exception($"Could not parse date: {match.Value} at position {match.Index} in file {file}");

         // Get the content of the entry by finding the closing bracket
         var (_, closingIndex) = Parsing.FindOpeningBracketAndClosingBracket(ref fileContent, match.Index);
         if (closingIndex == -1)
            throw new Exception($"Could not find closing bracket in file {file}");
         closingIndex++;
         eol = Parsing.GetLineEndingAfterComment(closingIndex, ref fileContent);

         // Get the content of the entry
         var historyContent = fileContent.Substring(match.Index, eol - match.Index);

         // Remove the comment from the content and add the entry to the list of entries
         var (content, comment) = Parsing.RemoveAndGetCommentFromString(historyContent);
         entries.Add(new HistoryEntry(date, content, comment));

      }

      return entries;
   }


}