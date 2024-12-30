using Editor.ErrorHandling;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Loading.Enhanced
{
   public class SuperRegionLoading
   {
      public static void Load()
      {
         //TODO restrict charter
         var (blocks, _) = EnhancedParser.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly, out var po, "map", "superregion.txt");
         
         Globals.SuperRegions = new(blocks.Count);
         foreach (var block in blocks)
         {

            bool restrictCharter = false;

            HashSet<Region> regions = [];

            var contents = block.GetContentElements(true, po);
            foreach (var content in contents)
            {
               foreach (var (str, lineNum) in content.GetStringListEnumerator())
               {
                  if (!restrictCharter && str.Equals("restrict_charter"))
                  {
                     restrictCharter = true;
                     continue;
                  }
                  
                  if (EnhancedParsing.GetRegionFromString(str, po, lineNum, out var region))
                     if (!regions.Add(region))
                        _ = new LoadingError(po, $"SuperRegion \"{block.Name}\"contains Region \"{str}\" multiple times!", lineNum, 0, ErrorType.UnresolveableRegionReference, LogType.Warning);

               }
            }
            if (!Globals.SuperRegions.TryAdd(block.Name, new(block.Name, Globals.ColorProvider.GetRandomColor(), ref po, regions) { RestrictCharter = restrictCharter }))
            {
               _ = new LoadingError(po, $"SuperRegion \"{block.Name}\" was defined multiple times!", block.StartLine, 0, ErrorType.DuplicateObjectDefinition, LogType.Warning);
            };

         }


      }
   }
}