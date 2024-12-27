using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using Editor.Helper;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Editor.Forms.PopUps;
using Editor.Loading.TreeClasses;
using System.IO;

namespace Editor.Loading
{
   public class AreaFileListener(PathObj path) : PDXLanguageParserBaseListener
   {
      private readonly List<Area> _areas = [];
      private PathObj _path = path;

      public List<Area> GetAreas() => _areas;

      public override void ExitArea(PDXLanguageParser.AreaContext context)
      {
         // Extract provinces and color from the area context
         var (provinces, color) = TreeToObjHelper.GetProvincesFromContext(context.intList());

         // Create a new Area object and add it to the list
         _areas.Add(new Area(context.IDENTIFIER().GetText(), color, ref _path, provinces));
      }
   }


   public class LoadTesting
   {
      /*
       * General Idea
       * Load Master has multiple Loader Objects
       * LoaderSingle and LoaderMulti are variants of abstract Loader
       * One static Helper for Loading which can be used in the Visitor
       * Visitor is different for each file but uses common methods in the helper
       * 
      */ 

      /*
       * LOADER 
       * 
       * InternalPath : PathObj
       * SearchPattern: str
       * 
       * List<objects> Load()
       * AfterLoad(List<objects>)
       * 
      */

      public static void DEBUG()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "area.txt"))
         {
            Globals.ErrorLog.Write("Error: area.txt not found!");
            return;
         }
         IO.ReadAllInANSI(path, out var newContent);
         var pathObj = PathObj.FromPath(path, isModPath);

         var sw = Stopwatch.StartNew();
         var stream = CharStreams.fromString(newContent);
         ITokenSource lexer = new PDXLanguageTokens(stream);
         ITokenStream tokens = new CommonTokenStream(lexer);
         var parser = new PDXLanguageParser(tokens)
         {
            BuildParseTree = false
         };


         // Add the custom error listener
         parser.RemoveErrorListeners();
         parser.AddErrorListener(new MyErrorListener(ref pathObj));
         var listener = new AreaFileListener(pathObj);
         parser.AddParseListener(listener);
         parser.areaFile();
         //ParseTreeWalker.Default.Walk(listener, parser.areaFile());
         var areas = listener.GetAreas();
         sw.Stop();
         Debug.WriteLine($"{sw.ElapsedMilliseconds} : {areas.Count}");

         //new RoughEditorForm(areas, false).ShowDialog();
      }
   }
}