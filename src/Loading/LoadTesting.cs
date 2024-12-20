using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Saving;
using Editor.Helper;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;
using Editor.Forms.PopUps;
using Editor.Loading.TreeClasses;

namespace Editor.Loading
{
   public class DummyLoader(PathObj path)
   {
      private PathObj _path = path;

      public List<Area> VisitAreaFile(PDXLanguageParser.AreaFileContext context)
      {
         List<Area> areas = [];
         foreach (var areaContext in context.area())
         {
            Area area = new(areaContext.IDENTIFIER().GetText(), TreeToObjHelper.GetColorFromContext(areaContext.color()));
            area.AddRange(TreeToObjHelper.GetProvincesFromContext(areaContext.intList()));
            area.SetPath(ref _path);
            areas.Add(area);
         }
         return areas;
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

         var stream = CharStreams.fromString(newContent);
         ITokenSource lexer = new PDXLanguageTokens(stream);
         ITokenStream tokens = new CommonTokenStream(lexer);
         var parser = new PDXLanguageParser(tokens);

         parser.RemoveErrorListeners();

         // Add the custom error listener
         parser.AddErrorListener(new MyErrorListener(ref pathObj));


         var loader = new DummyLoader(pathObj);

         for (int i = 0; i < 10; i++)
         {
            var sw = Stopwatch.StartNew();
            IParseTree tree = parser.areaFile();
            sw.Stop();
            var areas = loader.VisitAreaFile((PDXLanguageParser.AreaFileContext)tree);
            Debug.WriteLine(sw.ElapsedMilliseconds);
         }

         //new RoughEditorForm(areas, false).ShowDialog();
      }
   }
}