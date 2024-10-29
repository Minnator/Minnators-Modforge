using System.Diagnostics;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class GovernmentLoading
   {

      public static void Load()
      {
         var sw = Stopwatch.StartNew();

         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "governments");

         List<Government> governments = [];
         foreach (var file in files)
         {
            LoadGovernmentsFiles(file, governments);
         }

         foreach (var government in governments) 
            Globals.GovernmentTypes.Add(government.Name, government);

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Governments", sw.ElapsedMilliseconds);
      }

      private static void LoadGovernmentsFiles(string filePath, List<Government> governments)
      {
         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(filePath), out var content);
         if (string.IsNullOrEmpty(content))
            return;

         var elements = Parsing.GetElements(0, content);
         
         foreach (var element in elements)
         {
            if (element is not Block govBlock)
            {
               Globals.ErrorLog.Write($"Error in {filePath} : Expected block, got {element}");
               continue;
            }

            var government = new Government(govBlock.Name);
            ParseGovernmentContent(govBlock.GetContent, ref government);

            foreach (var govSubBLock in govBlock.GetBlockElements)
            {
               switch (govSubBLock.Name)
               {
                  case "exclusive_reforms":
                     var exList = Parsing.GetStringList(govSubBLock.GetContent);
                     HashSet<GovernmentReform> exclusiveReforms = [];
                     foreach (var exclusiveReform in exList)
                     {
                        if (!Globals.GovernmentReforms.TryGetValue(exclusiveReform, out var exReform))
                        {
                           Globals.ErrorLog.Write($"Error in {filePath} : Exclusive government reform {exclusiveReform} not found.");
                           continue;
                        }
                        exclusiveReforms.Add(exReform);
                     }
                     government.ExclusiveReforms.Add(exclusiveReforms);
                     break;
                  case "legacy_government":
                     var stringList = Parsing.GetStringList(govSubBLock.GetContent);
                     foreach (var legacyGovernment in stringList)
                     {
                        if (!Globals.GovernmentReforms.TryGetValue(legacyGovernment, out var legacyReform))
                        {
                           Globals.ErrorLog.Write($"Error in {filePath} : Legacy government reform {legacyGovernment} not found.");
                           continue;
                        }
                        government.LegacyReforms.Add(legacyReform);
                     }
                     break;
                  case "reform_levels":
                     foreach (var block in govSubBLock.GetBlockElements)
                     {
                        var reformLevel = new ReformLevel(block.Name);
                        var reformList = Parsing.GetStringList(block.GetContent);
                        foreach (var item in reformList)
                        {
                           if (!Globals.GovernmentReforms.TryGetValue(item, out var reform))
                           {
                              Globals.ErrorLog.Write($"Error in {filePath} : Reform {item} not found.");
                              continue;
                           }
                           reformLevel.Reforms.Add(reform);
                        }
                        government.ReformLevels.Add(reformLevel);
                     }
                     break;
               }
            }

            foreach (var govProperty in govBlock.GetContentElements)
            {
               ParseGovernmentContent(govProperty.Value, ref government);
            }
         }

      }

      private static void ParseGovernmentContent(string content, ref Government government)
      {

      }
   }
}