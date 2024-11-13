using System.Diagnostics;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public static class ReligionLoading
   {
      private static readonly HashSet<string> ForbiddenWords = ["flag_emblem_index_range", "religious_schools"];

      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "religions");

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
         var color = Color.Empty;
         if (colorBlock == null || !Parsing.TryParseColor(colorBlock.GetContent, out color))
         {
            Globals.ErrorLog.Write($"Color not found for {religionBlock.Name}");
            return;
         }
         religions.Add(new (religionBlock.Name) {Color = color});
      }
   }
}