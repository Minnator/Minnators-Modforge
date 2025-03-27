﻿using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;


public static class ContinentLoading
{
   private static readonly string Pattern = @"(?<name>[A-Za-z_]*)\s*=\s*{(?<ids>(?:\s*[0-9]+\s*)*)}";

   public static void Load()
   {
      if (!PathManager.GetModOrVanillaPath(out var path, out var isModPath, "map", "continent.txt"))
      {
         Globals.ErrorLog.Write("Error: continent.txt not found!");
         return;
      }
      var newContent = IO.ReadAllLinesInUTF8(path);

      var pathObj = PathObj.FromPath(path, isModPath);
      var continentDictionary = new Dictionary<string, Continent>();
      var sb = new StringBuilder();

      foreach (var line in newContent)
         sb.Append(Parsing.RemoveCommentFromLine(line));

      var matches = Regex.Matches(sb.ToString(), Pattern, RegexOptions.Multiline);
      foreach (Match match in matches)
      {
         var name = match.Groups["name"].Value;
         var provinces = Parsing.GetProvincesFromString(match.ToString());
         Continent continent = new(name, Globals.ColorProvider.GetRandomColor(), ref pathObj, provinces);
         continentDictionary.Add(name, continent);
      }

      SaveMaster.AddRangeToDictionary(pathObj, continentDictionary.Values);
      Globals.Continents = continentDictionary;
   }
}