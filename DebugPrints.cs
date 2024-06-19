using System.IO;
using System.Text;

namespace Editor;

public static class DebugPrints
{



   public static void PrintOptimizedProvinces()
   {
      var sb = new StringBuilder();
      sb.AppendLine("ID   .. Pixels .. PixelPrt .. Borders .. BorderPtr");
      foreach (var province in Data.Provinces)
      {
         sb.AppendLine($"ID: {province.Id,4} .. {province.PixelCnt,6} .. {province.PixelPtr,8} .. {province.BorderCnt,6} .. {province.BorderPtr,8}");
      }
      File.WriteAllText(@"C:\Users\david\Downloads\provincesDEBUG.txt", sb.ToString());
   }
}