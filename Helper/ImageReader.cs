﻿using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DirectXTexNet;

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
         var image = TexHelper.Instance.LoadFromTGAFile(filePath);

         if (image == null)
            throw new InvalidOperationException("Failed to load the TGA image.");

         return ConvertScratchImageToBitmap(image);
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


      [LibraryImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
      private static partial void CopyMemory(IntPtr dest, IntPtr src, int count);
   }
}