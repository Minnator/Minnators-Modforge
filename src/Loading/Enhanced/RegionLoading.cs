using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Region = Editor.DataClasses.Saveables.Region;

namespace Editor.Loading.Enhanced
{
   public static class RegionLoading
   {
      public static void Load()
      {
         var (blocks, _) = EnhancedParser.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly, out var po, "map", "region.txt");

         Globals.Regions = new(blocks.Count);
         foreach (var block in blocks)
         {
            var limit = 0;
            var areas = EnhancedParser.ParseBlock("areas", block, po, ref limit, EnhancedParsing.GetAreasFromBlock, () => []);
            var monsoons = EnhancedParser.ParseBlockMultiple("monsoon", block, po, ref limit, EnhancedParsing.GetMonsoonFromBlock);

            EnhancedParser.CheckLimit(block, limit, po);

            if (!Globals.Regions.TryAdd(block.Name, new Region(block.Name, Globals.ColorProvider.GetRandomColor(), ref po, areas) { Monsoon = monsoons }))
               _ = new LoadingError(po, $"Region \"{block.Name}\" already exists!", block.StartLine, type: ErrorType.DuplicateObjectDefinition);
         }
      }
   }
}