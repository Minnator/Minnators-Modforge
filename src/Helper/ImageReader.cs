using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DirectXTexNet;
using Image = DirectXTexNet.Image;

namespace Editor.Helper
{
   public partial class ImageReader
   {
      /// <summary>
      /// Loads a DDS image from the specified file path and returns it as a Bitmap
      /// </summary>
      /// <param name="filePath"></param>
      /// <returns></returns>
      /// <exception cref="InvalidOperationException"></exception>
      public static Bitmap ReadDDSImage(string filePath)
      {
         var image = TexHelper.Instance.LoadFromDDSFile(filePath, DDS_FLAGS.NONE);

         if (image == null)
            throw new InvalidOperationException("Failed to load the DDS image.");

         return ConvertScratchImageToBitmap(image);
      }

      /// <summary>
      /// Loads a TGA image from the specified file path and returns it as a Bitmap
      /// </summary>
      /// <param name="filePath"></param>
      /// <returns></returns>
      /// <exception cref="InvalidOperationException"></exception>
      public static Bitmap ReadTGAImage(string filePath)
      {
         if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file does not exist.", filePath);
         var image = TexHelper.Instance.LoadFromTGAFile(filePath);

         var sw = Stopwatch.StartNew();
         image = image.Convert(DXGI_FORMAT.B8G8R8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0.5f);
         sw.Stop();
         Debug.WriteLine($"Convert time: {sw.ElapsedMilliseconds}ms");

         if (image == null)
            throw new InvalidOperationException("Failed to load the TGA image.");

         return ConvertScratchImageToBitmapBGR(image);
      }

      /// <summary>
      /// Converts a ScratchImage to a Bitmap ONLY WITH 32BPP ARGB PIXEL FORMAT
      /// </summary>
      /// <param name="scratchImage"></param>
      /// <returns></returns>
      private static Bitmap ConvertScratchImageToBitmap(ScratchImage scratchImage)
      {
         var metadata = scratchImage.GetMetadata();

         // Create a new Bitmap with the dimensions of the DDS image
         var bitmap = new Bitmap(metadata.Width, metadata.Height, PixelFormat.Format32bppArgb);

         var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
         var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

         var sourcePtr = scratchImage.GetImage(0).Pixels;
         var byteCount = metadata.Width * metadata.Height * 4;

         CopyMemory(bmpData.Scan0, sourcePtr, byteCount);

         bitmap.UnlockBits(bmpData);
         return bitmap;
      }

      private static Bitmap ConvertScratchImageToBitmapBGR(ScratchImage scratchImage)
      {
         var sw = Stopwatch.StartNew();
         var metadata = scratchImage.GetMetadata();
         var bitmap = new Bitmap(metadata.Width, metadata.Height, PixelFormat.Format32bppArgb);
         var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
         var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

         var sourcePtr = scratchImage.GetImage(0).Pixels;
         try
         {
            unsafe
            {
               // Get the pointer to the destination bitmap data
               var destPtr = (byte*)bmpData.Scan0;
               var srcPtr = (byte*)sourcePtr.ToPointer();

               // Copy and swap channels directly in memory
               for (var i = 0; i < metadata.Width * metadata.Height; i++)
               {
                  destPtr[0] = srcPtr[0]; // R (from source) to B (in dest)
                  destPtr[1] = srcPtr[1]; // B (from source) to R (in dest)
                  destPtr[2] = srcPtr[2]; // G stays the same
                  destPtr[3] = srcPtr[3]; // A channel

                  srcPtr += 4;
                  destPtr += 4;
               }
            }

         }
         catch (AccessViolationException)
         {
            return bitmap;
         }

         bitmap.UnlockBits(bmpData);
         sw.Stop();
         Debug.WriteLine($"Convert to bmp time: {sw.ElapsedMilliseconds}ms");

         return bitmap;
      }



      [LibraryImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
      private static partial void CopyMemory(IntPtr dest, IntPtr src, int count);
   }
}