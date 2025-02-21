using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Editor.Properties;

namespace Editor.DataClasses.Achievements
{
   public static class AchievementImageHelper
   {
      /// <summary>
      /// Replaces all black Pixels in the Bitmap with the given color.
      /// </summary>
      /// <param name="image"></param>
      /// <param name="color"></param>
      /// <returns></returns>
      public static Bitmap ColorMask(AchievementImage image, Color color)
      {
         if (!AchievementManager.GetImage(image, out var bmp))
            return Resources.AchievementExample;

         var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
         var bitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
         
         unsafe
         {
            var scan0 = (byte*)bitmapData.Scan0;

            for (var y = 0; y < bmp.Height; y++)
            {
               for (var x = 0; x < bmp.Width; x++)
               {
                  var index = y * bitmapData.Stride + x * 4;
                  if (*(int*)(scan0 + index) == -16777216) 
                     *(int*)(scan0 + index) = color.ToArgb();
               }
            }
         }

         bmp.UnlockBits(bitmapData);
         return bmp;
      }
   }
}