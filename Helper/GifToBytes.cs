using System.Diagnostics;
using System.Drawing.Imaging;
using System.Text;
using Editor.Resources;

namespace Editor.Helper
{
   public static class GifToBytes
   {
      public static bool GetBytesAsString(byte[] bytes, out string str)
      {
         str = null;
         try
         {
            str = Encoding.UTF8.GetString(bytes);
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool GetImageFromBytes(byte[] bytes, out Image image)
      {
         image = null;
         try
         {
            using var ms = new MemoryStream(bytes);
            image = Image.FromStream(ms);
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool ConvertGifToBytes(string path, out byte[] bytes)
      {
         bytes = null;
         try
         {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var image = Image.FromStream(fs);
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Gif);
            bytes = ms.ToArray();

            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool ConvertBytesToGif(byte[] bytes, string path)
      {
         try
         {
            using var ms = new MemoryStream(bytes);
            var image = Image.FromStream(ms);
            image.Save(path, ImageFormat.Gif);
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool ConvertBytesToHex (byte[] bytes, out string hex)
      {
         hex = null;
         try
         {
            hex = BitConverter.ToString(bytes).Replace("-", "");
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool ConvertFormattedHexToBytes(string hex, out byte[] bytes)
      {
         bytes = null;

         try
         {
            // Remove any whitespace (spaces, newlines, etc.)
            var cleanedHexString = hex.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");

            // Ensure that the cleaned string has an even length
            if (cleanedHexString.Length % 2 != 0)
            {
               return false; // Invalid format
            }

            int byteCount = cleanedHexString.Length / 2;
            bytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
               // Take each pair of characters and convert to a byte
               string byteValue = cleanedHexString.Substring(i * 2, 2);
               bytes[i] = Convert.ToByte(byteValue, 16);
            }

            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool ConvertHexToBytes(string hex, out byte[] bytes)
      {
         bytes = null;
         try
         {
            bytes = Enumerable.Range(0, hex.Length)
               .Where(x => x % 2 == 0)
               .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
               .ToArray();
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

      public static bool GetFormattedBytesAsHex(string name, byte[] bytes, out string formattedHex)
      {
         formattedHex = null;
         try
         {
            var sb = new StringBuilder();
            sb.AppendLine($"public static string {name} = @\"");
            sb.Append("\t");

            for (var i = 0; i < bytes.Length; i++)
            {
               // Append the byte as a hex string
               sb.Append($"{bytes[i]:X2}");

               // Add a space after every 2 bytes (4 characters)
               if ((i + 1) % 2 == 0 && i < bytes.Length - 1)
               {
                  sb.Append(' ');
               }

               // Add a new line after every 8 bytes (16 characters)
               if ((i + 1) % 48 == 0 && i < bytes.Length - 1)
               {
                  sb.AppendLine();
                  sb.Append("\t");
               }
            }

            sb.AppendLine("\n\";");
            formattedHex = sb.ToString();
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }


      public static bool GetFormattedByteArray(string name, byte[] bytes, out string array)
      {
         array = null;
         try
         {
            var sb = new StringBuilder();
            sb.AppendLine($"public static byte[] {name} = new byte[] {{");
            for (var i = 0; i < bytes.Length; i++)
            {
               sb.Append($"{bytes[i],3}");
               if (i < bytes.Length - 1)
               {
                  sb.Append(',');
               }

               if ((i + 1) % 25 == 0)
               {
                  sb.AppendLine("\t");
               }
            }

            sb.AppendLine("};");
            array = sb.ToString();
            return true;
         }
         catch (Exception e)
         {
            Debug.WriteLine(e);
            return false;
         }
      }

   }
}