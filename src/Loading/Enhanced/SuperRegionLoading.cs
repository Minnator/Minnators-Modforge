namespace Editor.Loading.Enhanced
{
   public class SuperRegionLoading
   {
      public static void Load()
      {
         //TODO restrict charter
         var (blocks, _) = EnhancedParser.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly, out var po, "map", "superregion.txt");
      }
   }
}