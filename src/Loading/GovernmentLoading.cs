using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class GovernmentLoading
   {
      public static string PreDharmaMapping = string.Empty;
      public static void Load()
      {
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "governments");

         List<Government> governments = [];
         foreach (var file in files)
         {
            LoadGovernmentsFiles(file, governments);
         }

         foreach (var government in governments) 
            Globals.GovernmentTypes.Add(government.Name, government);
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

            if (govBlock.Name.Equals("pre_dharma_mapping"))
            {
               PreDharmaMapping = govBlock.GetContent;
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
                     foreach (var outerBlock in govSubBLock.GetBlockElements)
                     {
                        var block = outerBlock.GetBlockWithName("reforms");
                        if (block == null) 
                        {
                           Globals.ErrorLog.Write($"Error in {filePath} : Expected reforms block, got {outerBlock.Name}");
                           continue;
                        }

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
                  case "color":
                     if (!Parsing.TryParseColor(govSubBLock.GetContent, out government.Color))
                        Globals.ErrorLog.Write($"Error in {filePath} : Failed to parse color {govSubBLock.GetContent}");
                     break;
                  default:
                     Globals.ErrorLog.Write($"Error in {filePath} : Unknown block {govSubBLock.Name}");
                     break;
               }
            }

            foreach (var govProperty in govBlock.GetContentElements)
            {
               ParseGovernmentContent(govProperty.Value, ref government);
            }

            governments.Add(government);
         }

      }

      private static void ParseGovernmentContent(string content, ref Government government)
      {
         var kvps = Parsing.GetKeyValueList(content);
         foreach (var kvp in kvps)
         {
            switch (kvp.Key)
            {
               case "basic_reform":
                  government.BasicReform = Globals.GovernmentReforms[kvp.Value];
                  break;
               default:
                  Globals.ErrorLog.Write($"Error in government {government.Name} : Unknown property {kvp.Key}");
                  break;
            }
         }
      }
   }
}