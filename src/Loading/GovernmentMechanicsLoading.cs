using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   [Loading]
   public static class GovernmentMechanicsLoading
   {
      public static void Load()
      {
         var sw = System.Diagnostics.Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt","common", "government_mechanics");

         foreach (var file in files)
         {
            LoadFile(file);
         }
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Government Mechanics", sw.ElapsedMilliseconds);
      }

      private static void LoadFile(string file)
      {
         var rawContent = IO.ReadAllInUTF8(file);
         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var fileContent);

         var elements = Parsing.GetElements(0, fileContent);
         if (elements.Count < 1 || (elements.Count == 1 && !elements[0].IsBlock))
            return;
         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Invalid element in {file}: {element}");
               continue;
            }

            var subBlocks = block.Blocks;
            foreach (var subElement in subBlocks)
            {
               if (subElement is not Block subBlock)
                  continue;
               if (!subBlock.Name.Trim().Equals("powers"))
                  continue;

               var subSubBlocks = subBlock.Blocks;
               foreach (var subSubElement in subSubBlocks)
               {
                  if (subSubElement is not Block subSubBlock)
                     continue;

                  ModifierParser.GovernmentMechanics.Add($"monthly_{subSubBlock.Name.Trim()}");
                  ModifierParser.GovernmentMechanics.Add($"{subSubBlock.Name.Trim()}_gain_modifier");
               }
            }
         }
      }

   }
}