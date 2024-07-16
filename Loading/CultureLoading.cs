using System.Diagnostics;
using System.Xml.Linq;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading;

public static class CultureLoading
{

   public static void LoadCultures(ModProject project)
   {
      var sw = Stopwatch.StartNew();
      FilesHelper.GetFilesUniquelyAndCombineToOne(project.ModPath, project.VanillaPath, out var culturesContent, "common", "cultures");

      var blocks = Parsing.GetElements(0, ref culturesContent);

      var (groups, cultures) = GetCultureGroups(ref blocks, project.ColorProvider);
      Globals.CultureGroups = groups;
      Globals.Cultures = cultures;
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing cultures", sw.ElapsedMilliseconds);
      sw.Restart();
      AnalyzeCultures([.. groups.Values]);
      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Analyzing cultures", sw.ElapsedMilliseconds);
   }

   private static (Dictionary<string, CultureGroup>, Dictionary<string, Culture>) GetCultureGroups(ref List<IElement> blocks, ColorProviderRgb colorProvider)
   {
      Dictionary<string, Culture> cultureDict = [];
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
                  CultureGroup = group.Name
               };
               SetCultureAttributes(ref culture, cult.GetBlockElements);
               SetCultureContent(ref culture, cult.GetContentElements);
               group.Cultures.Add(culture);
               lock (cultureDict)
               {
                  cultureDict.Add(culture.Name, culture);
               }
            }
         }
         SetCultureGroupAttributes(ref group, contents);
         lock (cultureGroupDict)
         {
            cultureGroupDict.Add(group.Name, group);
         }
      });

      return (cultureGroupDict, cultureDict);
   }

   private static void SetCultureContent(ref Culture culture, List<Content> cultGetContentElements)
   {
      if (cultGetContentElements.Count == 0)
         return;

      foreach (var content in cultGetContentElements)
      {
         var kvp = Parsing.GetKeyValueList(content.Value);

         foreach (var item in kvp)
         {
            if (item.Key.Equals("primary"))
               culture.Primaries.Add(item.Value);
         }
      }
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
         {
            if (!cultureNames.Add(culture.Name))
               Globals.ErrorLog.Write($"Duplicate culture name: {culture.Name}");

            HashSet<string> duplicateNames = [];
            List<string> nonUniqueNames = [];
            foreach (var male in culture.MaleNames)
            {
               if (!duplicateNames.Add(male))
                  nonUniqueNames.Add(male);
            }
            foreach (var female in culture.FemaleNames)
            {
               if (!duplicateNames.Add(female))
                  nonUniqueNames.Add(female);
            }
            foreach (var dynasty in culture.DynastyNames)
            {
               if (!duplicateNames.Add(dynasty))
                  nonUniqueNames.Add(dynasty);
            }
            if (nonUniqueNames.Count > 0)
               Globals.ErrorLog.Write($"Duplicate names in culture {culture.Name}: {DebugPrints.GetListAsString(nonUniqueNames)}");
         }
      }
   }
}