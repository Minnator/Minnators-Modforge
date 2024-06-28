﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Editor.DataClasses;

namespace Editor.Helper;

public static class ProvinceParser
{
   private const string ID_FROM_FILE_NAME_PATTERN = @"(\d+)\s*-?";
   private const string DATE_PATTERN = @"\d{1,4}\.\d{1,2}\.\d{1,2}";

   public static void ParseAllProvinces(string modFolder, string vanillaFolder)
   {
      var sw = Stopwatch.StartNew();
      var files = FilesHelper.GetFilesFromModAndVanillaUniquely(modFolder, vanillaFolder, "history", "provinces");
      Dictionary<int, List<HistoryEntry>> entries = [];
      ParseIdsFromFileNames(files, entries);

      sw.Stop();
      Debug.WriteLine($"Parsing provinces took {sw.ElapsedMilliseconds} ms");

      var totalEntryCount = 0;
      foreach (var entry in entries) 
         totalEntryCount += entry.Value.Count;

      Debug.WriteLine($"Total entries: {totalEntryCount}");

   }

   private static void ParseIdsFromFileNames(List<string> files, Dictionary<int, List<HistoryEntry>> entries)
   {
      var idPattern = new Regex(ID_FROM_FILE_NAME_PATTERN);
      
      foreach (var file in files)
      {
         var match = idPattern.Match(file);
         if (!match.Success || !int.TryParse(match.Groups[1].Value, out var id))
            throw new Exception($"Could not parse province id from file name: {file}\nCould not match \'<number> - <.*>\'");

         entries.Add(id, ParseHistoryEntriesFromFile(file));

         // Percentage of completion
         if (entries.Count % 20 == 0)
         {
            Debug.WriteLine($"{entries.Count,4} / {files.Count,4} [{entries.Count * 100 / files.Count,3}%] files parsed");
         }
      }

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