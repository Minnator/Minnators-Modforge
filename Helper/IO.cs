using System.IO;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Editor.Helper;

internal static class IO
{
   private static Encoding encoding;

   static IO()
   {
      encoding = Encoding.GetEncoding("Windows-1252");
   }

   /// <summary>
   /// Reads the entire content of a file in ANSI encoding if the file exists
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   public static bool ReadAllInANSI(string path, out string data)
   {
      data = string.Empty;
      if (!File.Exists(path))
         return false;
      try
      {
         data = File.ReadAllText(path, encoding);
         return true;
      }
      catch (IOException)
      {
         return false;
      }
   }

   public static string ReadAllInUTF8(string path)
   {
      if (!File.Exists(path))
         return string.Empty;
      try
      {
         return File.ReadAllText(path, Encoding.UTF8);
      }
      catch (IOException)
      {
         return string.Empty;
      }
   }

   public static string[] ReadAllLinesInUTF8(string path)
   {
      if (!File.Exists(path))
         return [];
      try
      {
         return File.ReadAllLines(path, Encoding.UTF8);
      }
      catch (IOException)
      {
         return [];
      }
   }

   public static bool WriteAllInANSI(string path, string data, bool append)
   {
      if (!File.Exists(path))
         return false;
      try
      {
         if (append)
            File.AppendAllText(path, data, encoding);
         else
            File.WriteAllText(path, data, encoding);
         return true;
      }
      catch (IOException)
      {
         return false;
      }
   }
}