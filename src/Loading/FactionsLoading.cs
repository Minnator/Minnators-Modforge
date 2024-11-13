using Editor.DataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public static class FactionsLoading
   {
      public static void Load()
      {
         var sw = System.Diagnostics.Stopwatch.StartNew();
         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt","common", "factions");
 
         foreach (var file in files)
         {
            LoadFile(file);
         }
         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Factions", sw.ElapsedMilliseconds);
      }

      private static void LoadFile(string file)
      {
         var rawContent = IO.ReadAllInUTF8(file);
         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var fileContent);

         var elements = Parsing.GetElements(0, fileContent);
         if (elements.Count < 1 || elements is [{ IsBlock: false }])
            return;
         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Invalid element in {file}: {element}");
               continue;
            }

            try
            {

               var modName = $"{block.Name}_influence";
               if (ModifierParser.FactionModifiers.Contains(modName))
                  continue;
               ModifierParser.FactionModifiers.Add(modName);

            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               throw;
            }
         }
      }
   }
}