﻿using System.Text;
// ReSharper disable InconsistentNaming

namespace Editor.Helper;

internal static class IO
{
   private static readonly Encoding encoding;
   // Get the ANSI encoding
   static IO()
   {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      encoding = Encoding.GetEncoding("windows-1250");
   }


   public static void OpenFileDialog(string startPath, string filterText, out string folder)
   {
      folder = string.Empty;
      if (!Path.Exists(startPath))
         return;

      using var dialog = new OpenFileDialog();
      dialog.InitialDirectory = startPath;
      dialog.CheckFileExists = false;
      dialog.CheckPathExists = true;
      dialog.FileName = filterText;

      if (dialog.ShowDialog() == DialogResult.OK)
         folder = Path.GetDirectoryName(dialog.FileName) ?? Environment.SpecialFolder.MyDocuments.ToString();
   }


   /// <summary>
   /// Reads the entire content of a file in ANSI encoding if the file exists
   /// </summary>
   /// <param name="path"></param>
   /// <param name="data"></param>
   /// <returns></returns>
   public static bool ReadAllInANSI(string path, out string data)
   {
      data = string.Empty;
      if (!File.Exists(path))
         return false;
      try
      {
         using StreamReader reader = new (path, encoding);
         data = reader.ReadToEnd();
         return true;
      }
      catch (IOException)
      {
         return false;
      }
   }

   public static bool ReadAllLinesANSI(string path, out string[] data)
   {
      data = [];
      if (!File.Exists(path))
         return false;
      try
      {
         data = File.ReadAllLines(path, encoding);
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

   public static bool WriteToFile(string path, string data, bool append)
   {
      try
      {
         if (append)
            File.AppendAllText(path, data);
         else
            File.WriteAllText(path, data);
         return true;
      }
      catch (IOException)
      {
         return false;
      }
   }
}