using System.Diagnostics;
using System.Xml.Linq;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Parser;
using Editor.Saving;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;

public static class CultureLoading
{
   public static void Load()
   {
      PathManager.GetFilesUniquelyAndCombineToOne(out var culturesContent, "common", "cultures");

      var (modFiles, vanillaFiles) = PathManager.GetFilesFromModAndVanillaUniquelySeparated("*.txt", "common", "cultures");

      // we load the vanilla files first to override if necessary with mod data
      Globals.CultureGroups = [];
      var groupsDict = new Dictionary<string, CultureGroup>();

      foreach (var file in vanillaFiles)
      {
         var po = PathObj.FromPath(file, false);
         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

         var cultureGroups = GetCultureGroups(blocks, Globals.ColorProvider);
         foreach (var group in cultureGroups)
            groupsDict.TryAdd(group.Key, group.Value);
      }

      foreach (var file in modFiles)
      {
         var po = PathObj.FromPath(file);
         var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

         var cultureGroups = GetCultureGroups(blocks, Globals.ColorProvider);
         foreach (var group in cultureGroups)
         {
            if (!groupsDict.TryAdd(group.Key, group.Value))
            {
               var existing = groupsDict[group.Key];
               // if the group already exists, we merge the cultures but on the obj of the mod
               group.Value.Cultures.AddRange(existing.Cultures);
               groupsDict[group.Key] = group.Value;
            }
         }
      }

      Globals.CultureGroups = groupsDict;
   }

   private static Dictionary<string, CultureGroup> GetCultureGroups(List<EnhancedBlock> blocks, ColorProviderRgb colorProvider)
   {
      Dictionary<string, CultureGroup> cultureGroupDict = [];


      foreach (var block in blocks)
      {
         var group = new CultureGroup(block.Name)
         {
            Color = colorProvider.GetRandomColor()
         };
         var contents = block.ContentElements;
         var cultures = block.SubBlocks;

         foreach (var cult in cultures)
         {

            if (cult.Name.Equals("country") || cult.Name.Equals("province"))
               continue;
            if (cult.Name.Equals("male_names") || cult.Name.Equals("female_names") || cult.Name.Equals("dynasty_names"))
               SetCultureGroupNames(ref group, cult);
            else
            {
               Culture culture = new(cult.Name)
               {
                  Color = colorProvider.GetRandomColor(),
                  CultureGroup = group
               };
               SetCultureAttributes(ref culture, cult.SubBlocks);
               SetCultureContent(ref culture, cult.ContentElements);
               group.Cultures.Add(culture);
               lock (Globals.Cultures)
               {
                  if (!Globals.Cultures.TryAdd(culture.Name, culture))
                  {
                     var existingCulture = Globals.Cultures[culture.Name];
                     Globals.Cultures[culture.Name] = culture;
                  }
               }
            }
         }

         SetCultureGroupAttributes(ref group, contents);
         lock (cultureGroupDict)
         {
            if (!cultureGroupDict.TryAdd(group.Name, group))
            {
               cultureGroupDict[group.Name].Cultures.AddRange(group.Cultures);
            }
         }
      }

      return cultureGroupDict;
   }

   private static void SetCultureContent(ref Culture culture, List<EnhancedContent> cultGetContentElements)
   {
      if (cultGetContentElements.Count == 0)
         return;

      foreach (var content in cultGetContentElements)
         foreach (var item in Parsing.GetKeyValueList(content.Value))
            if (item.Key.Equals("primary"))
               culture.Primaries.Add(item.Value);
   }

   private static void SetCultureGroupNames(ref CultureGroup group, EnhancedBlock block)
   {
      if (block.SubBlocks.Count == 0)
         return;
      var content = block.ContentElements[0];
      switch (block.Name)
      {
         case "male_names":
            group.MaleNames = [.. EnhancedParsing.GetStringListFromContent(content, PathObj.Empty)];
            break;
         case "female_names":
            group.FemaleNames = [.. EnhancedParsing.GetStringListFromContent(content, PathObj.Empty)];
            break;
         case "dynasty_names":
            group.DynastyNames = [.. EnhancedParsing.GetStringListFromContent(content, PathObj.Empty)];
            break;
      }
   }

   private static void SetCultureGroupAttributes(ref CultureGroup group, List<EnhancedContent> contents)
   {
      foreach (var content in contents)
      {
         Parsing.RemoveCommentFromMultilineString(content.Value, out var removed);
         var kvps = Parsing.GetKeyValueList(removed);

         foreach (var kvp in kvps)
         {
            switch (kvp.Key)
            {
               case "graphical_culture":
                  group.Gfx = kvp.Value;
                  break;
               case "second_graphical_culture":
                  group.SecondGfx = kvp.Value;
                  break;
               case "country":
                  group.CountryModifiers = Parsing.GetKeyValueList(kvp.Value);
                  break;
               case "province":
                  group.ProvinceModifiers = Parsing.GetKeyValueList(kvp.Value);
                  break;
               default:
                  Globals.ErrorLog.Write($"Unknown Group in a culture group file:{kvp.Key}");
                  break;
            }
         }
      }
   }

   private static void SetCultureAttributes(ref Culture culture, List<EnhancedBlock> blocks)
   {
      foreach (var block in blocks)
      {
         if (block.SubBlocks.Count == 0)
            continue;
         var content = block.ContentElements[0].Value;
         Parsing.RemoveCommentFromMultilineString(ref content, out var removed);
         switch (block.Name.ToLower())
         {
            case "male_names":
               culture.MaleNames = [.. Parsing.GetStringList(removed)];
               break;
            case "female_names":
               culture.FemaleNames = [.. Parsing.GetStringList(removed)];
               break;
            case "dynasty_names":
               culture.DynastyNames = [.. Parsing.GetStringList(removed)];
               break;
            case "country":
               culture.CountryModifiers = Parsing.GetKeyValueList(removed);
               break;
            case "province":
               culture.ProvinceModifiers = Parsing.GetKeyValueList(removed);
               break;
            default:
               Globals.ErrorLog.Write($"Unknown Group in a cultures file:{block.Name}");
               break;
         }
      }
   }

   private static void AnalyzeCultures(List<CultureGroup> groups)
   {
      HashSet<string> cultureNames = [];
      HashSet<string> cultureGroupNames = [];

      foreach (var group in groups)
      {
         if (!cultureGroupNames.Add(group.Name))
            Globals.ErrorLog.Write($"Duplicate culture group name: {group.Name}");

         foreach (var culture in group.Cultures)
            if (!cultureNames.Add(culture.Name))
               Globals.ErrorLog.Write($"Duplicate culture name: {culture.Name}");
      }
   }
}