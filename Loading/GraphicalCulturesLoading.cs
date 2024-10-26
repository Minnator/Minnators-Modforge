using System.Diagnostics;
using Editor.Helper;

namespace Editor.Loading
{
   public static class GraphicalCulturesLoading
   {

      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         if (!FilesHelper.GetFilePathUniquely(out var path, "common", "graphicalculturetype.txt"))
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

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("GraphicalCultures", sw.ElapsedMilliseconds);
      }
      
   }
}