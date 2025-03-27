using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;


public static class CultureLoading
{
   public static void Load()
   {
      PathManager.GetFilesUniquelyAndCombineToOne(out var culturesContent, "common", "cultures");

      Parsing.RemoveCommentFromMultilineString(ref culturesContent, out var commentLessContent);

      var blocks = Parsing.GetElements(0, ref commentLessContent);

      var groups = GetCultureGroups(ref blocks, Globals.ColorProvider);
      Globals.CultureGroups = groups;
   }

   private static Dictionary<string, CultureGroup> GetCultureGroups(ref List<IElement> blocks, ColorProviderRgb colorProvider)
   {
      Dictionary<string, CultureGroup> cultureGroupDict = [];
      
      Parallel.ForEach(blocks, element =>
      {
         if (element is not Block block)
            return;
         var group = new CultureGroup(block.Name)
         {
            Color = colorProvider.GetRandomColor()
         };
         var contents = block.GetContentElements;
         var cultures = block.GetBlockElements;

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
               SetCultureAttributes(ref culture, cult.GetBlockElements);
               SetCultureContent(ref culture, cult.GetContentElements);
               group.Cultures.Add(culture);
               lock (Globals.Cultures)
               {
                  if (!Globals.Cultures.TryAdd(culture.Name, culture))
                     Globals.Cultures[culture.Name] = culture;
               }
            }
         }
         SetCultureGroupAttributes(ref group, contents);
         lock (cultureGroupDict)
         {
            if (!cultureGroupDict.TryAdd(group.Name, group))
            {
               Globals.ErrorLog.Write($"Duplicate culture group name: {group.Name}");
               cultureGroupDict[group.Name] = group;
            }
         }
      });
      
      return cultureGroupDict;
   }

   private static void SetCultureContent(ref Culture culture, List<Content> cultGetContentElements)
   {
      if (cultGetContentElements.Count == 0)
         return;

      foreach (var content in cultGetContentElements)
         foreach (var item in Parsing.GetKeyValueList(content.Value))
            if (item.Key.Equals("primary"))
               culture.Primaries.Add(item.Value);
   }

   private static void SetCultureGroupNames(ref CultureGroup group, Block block)
   {
      if (block.Blocks.Count == 0)
         return;
      var content = ((Content)block.Blocks[0]).Value;
      switch (block.Name)
      {
         case "male_names":
            group.MaleNames = [.. Parsing.GetStringList(content)];
            break;
         case "female_names":
            group.FemaleNames = [.. Parsing.GetStringList(content)];
            break;
         case "dynasty_names":
            group.DynastyNames = [.. Parsing.GetStringList(content)];
            break;
      }
   }

   private static void SetCultureGroupAttributes(ref CultureGroup group, List<Content> contents)
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

   private static void SetCultureAttributes(ref Culture culture, List<Block> blocks)
   {
      foreach (var block in blocks)
      {
         if (block.Blocks.Count == 0)
            continue;
         var content = ((Content)block.Blocks[0]).Value;
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