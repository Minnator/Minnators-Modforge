using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;

namespace Editor.Loading.Enhanced
{
   public static class AreaParsing
   {
      public static void Load()
      {
         var (elements, po)= EnhancedParser.GetElements("map", "area.txt");

         var blocks = elements.Where(e => e.IsBlock).Select(e => e as EnhancedBlock).ToList();
         if (blocks.Count != elements.Count)
         {
            _ = new LoadingError(po, "Detected content in a file where only blocks are allowed!", type:ErrorType.UnexpectedContentElement, level:LogType.Critical);
            return;
         }
         
         foreach (var block in blocks)
         {
            var contentElements = block!.ContentElements;
            var subBlocks = block.SubBlocks;

            if (subBlocks.Count > 1)
            {
               // TODO should be still able to parse, just ignore blocks which are not needed
               _ = new LoadingError(po, $"Unexpected block element in block \"{block.Name}\"! Expected 1 but got {subBlocks.Count}", block.StartLine, type:ErrorType.UnexpectedBlockElement);
               continue;
            }

            var color = Color.Empty;
            if (subBlocks.Count == 1) 
               color = EnhancedParsing.GetColorFromBlock(subBlocks[0], po);

            List<Province> provinces = [];
            foreach (var contE in contentElements) 
               provinces.AddRange(EnhancedParsing.GetProvincesFromString(contE, po));

            if (color == Color.Empty)
               color = Globals.ColorProvider.GetRandomColor();

            var area = new Area(block.Name, color, ref po, provinces);
            Globals.Areas.Add(block.Name, area);
         }
      }
   }
}