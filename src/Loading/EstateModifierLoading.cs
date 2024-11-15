using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   [Loading]
   public static class LoadEstateModifiers
   {
      public static void Load()
      {
         var sw = System.Diagnostics.Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt","common", "estates_preload");

         foreach (var file in files)
         {
            LoadFile(file);
         }
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Estate Modifiers", sw.ElapsedMilliseconds);
      }

      private static void LoadFile(string file)
      {
         var rawContent = IO.ReadAllInUTF8(file);
         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var fileContent);

         var elements = Parsing.GetElements(0, fileContent);
         if (elements.Count < 1)
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
               if (!subBlock.Name.Equals("modifier_definition"))
                  continue;

               var content = subBlock.GetContent;
               var kvps = Parsing.GetKeyValueList(content);
               foreach (var kvp in kvps)
               {
                  if (!kvp.Key.Trim().Equals("key"))
                     continue;
                  ModifierParser.EstateModifiers.Add(kvp.Value.Trim());
               }

            }

         }
      }
   }
}