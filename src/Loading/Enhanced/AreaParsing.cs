using Editor.ErrorHandling;

namespace Editor.Loading.Enhanced
{
   public static class AreaParsing
   {
      public static void Load()
      {
         var (blocks, _) = EnhancedParser.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly, out var po, "map", "area.txt");

         Globals.Areas = new (blocks.Count);
         foreach (var block in blocks)
         {
            var limit = 0;         
            var color = EnhancedParser.ParseBlock("color", block, po, ref limit, EnhancedParsing.GetColorFromBlock, Globals.ColorProvider.GetRandomColor);
            EnhancedParser.CheckLimit(block, limit, po);

            if (!Globals.Areas.TryAdd(block.Name, new (block.Name, color, ref po, EnhancedParsing.GetProvincesFromContent(block.ContentElements, po))))
               _ = new LoadingError(po, $"Area \"{block.Name}\" already exists!", block.StartLine, type: ErrorType.DuplicateElement);
         }
      }
   }
}