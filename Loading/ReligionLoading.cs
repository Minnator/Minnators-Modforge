using System.Diagnostics;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class ReligionLoading
   {
      private static readonly HashSet<string> ForbiddenWords = ["flag_emblem_index_range", "religious_schools"];

      public static void Load(ModProject project)
      {
         var sw = Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely(project.ModPath, project.VanillaPath, "common",
            "religions");

         List<ReligiousGroup> religionGroups = [];

         foreach (var file in files)
         {
            var content = IO.ReadAllInUTF8(file);
            ParseReligionFile(content, ref religionGroups);
         }

         Globals.ReligionGroups = religionGroups;

         foreach (var group in religionGroups)
            foreach (var religion in group.Religions)
               if (!Globals.Religions.TryAdd(religion.Name, religion)) 
                  MessageBox.Show($"Religion {religion.Name} already exists in the dictionary");

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Religions", sw.ElapsedMilliseconds);
      }

      private static void ParseReligionFile(string file, ref List<ReligiousGroup> religionGroups)
      {
         var elements = Parsing.GetElements(0, file);

         foreach (var element in elements)
         {
            if (element is Block block)
            {
               ParseReligionGroup(block, ref religionGroups);
            }
         }
      }

      private static void ParseReligionGroup(Block block, ref List<ReligiousGroup> religionGroups)
      {
         List<Religion> religions = [];
         foreach (var element in block.GetBlockElements)
         {
            if (ForbiddenWords.Contains(element.Name.Trim()))
               continue;
            ParseReligion(element, ref religions);
         }
         religionGroups.Add(new (block.Name){Religions = religions});
      }

      private static void ParseReligion(Block religionBlock, ref List<Religion> religions)
      {
         var colorBlock = religionBlock.GetBlockWithName("color");
         Color color = Color.Empty;
         if (colorBlock != null) 
            color = Parsing.ParseColor(colorBlock.GetContent);
         religions.Add(new (religionBlock.Name) {Color = color});
      }
   }
}