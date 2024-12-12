using Editor.Helper;

namespace Editor.Loading
{
   
   public static class RiverLoading
   {
      public static void Load()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "rivers.bmp"))
         {
            Globals.ErrorLog.Write("Could not find rivers.bmp");
            return;
         }

         var (riverSizes, palette) = BmpLoading.LoadIndexedBitMap(path);

         foreach (var river in riverSizes)
         {
            var color = palette[river.Key].ToArgb();
            Globals.Rivers.Add(color, river.Value.ToArray());
         }
      }
   }
}