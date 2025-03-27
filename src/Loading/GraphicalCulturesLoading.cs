using Editor.Helper;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class GraphicalCulturesLoading
   {

      public static void Load()
      {
         if (!PathManager.GetFilePathUniquely(out var path, "common", "graphicalculturetype.txt"))
         {
            Globals.ErrorLog.Write("Could not find graphicalculturetype.txt");
            return;
         }

         var lines = IO.ReadAllLinesInUTF8(path);

         foreach (var line in lines)
         {
            var clearLine = Parsing.RemoveCommentFromLine(line);
            if (clearLine == string.Empty)
               continue;

            clearLine = clearLine.Trim();
            Globals.GraphicalCultures.Add(clearLine);
         }
      }
      
   }
}