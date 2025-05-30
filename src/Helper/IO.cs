﻿using System.Drawing.Imaging;
using System.Text;
using Editor.ErrorHandling;
using Editor.Saving;

// ReSharper disable InconsistentNaming

namespace Editor.Helper;

internal static class IO
{
   private static readonly Encoding encoding;
   private static readonly UTF8Encoding bomUtf8Encoding = null!;
   // Get the ANSI encoding
   static IO()
   {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      encoding = Encoding.GetEncoding("windows-1250");
      bomUtf8Encoding = new (true);
   }


   public static void OpenFolderDialog(string startPath, string filterText, out string folder)
   {
      folder = string.Empty;
      CreateDirectoryIfRequired(startPath);

      using var dialog = new OpenFileDialog();
      dialog.InitialDirectory = startPath;
      dialog.CheckFileExists = false;
      dialog.CheckPathExists = true;
      dialog.FileName = filterText;

      if (dialog.ShowDialog() == DialogResult.OK)
         folder = Path.GetDirectoryName(dialog.FileName) ?? Environment.SpecialFolder.MyDocuments.ToString();
   }

   public static void OpenFileSelection(string startFolder, string filterText, out string file)
   {
      file = string.Empty;
      CreateDirectoryIfRequired(startFolder);

      using var dialog = new OpenFileDialog();
      dialog.InitialDirectory = startFolder;
      dialog.CheckFileExists = true;
      dialog.CheckPathExists = true;
      dialog.Filter = filterText;

      if (dialog.ShowDialog() == DialogResult.OK)
         file = dialog.FileName;
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
         _ = new ErrorObject(ErrorType.ApplicationCouldNotOpenFile, $"Could not open or read file {path}");
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
         _ = new ErrorObject(ErrorType.ApplicationCouldNotOpenFile, $"Could not open or read file {path}");
         return false;
      }
   }

   public static bool ReadAllInANSI(PathObj po, out string data)
   {
      return ReadAllInANSI(po.GetPath(), out data);
   }

   public static bool ReadAllLinesANSI(PathObj po, out string[] data)
   {
      return ReadAllLinesANSI(po.GetPath(), out data);
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
         _ = new ErrorObject(ErrorType.ApplicationCouldNotOpenFile, $"Could not open or read file {path}");
         return string.Empty;
      }
   }

   public static string[] ReadAllLinesInUTF8(PathObj po)
   {
      return ReadAllLinesInUTF8(po.GetPath());
   }

   public static string ReadAllInUTF8(PathObj po)
   {
      return ReadAllInUTF8(po.GetPath());
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
         _ = new ErrorObject(ErrorType.ApplicationCouldNotOpenFile, $"Could not open or read file {path}");
         return [];
      }
   }

   public static bool WriteAllInANSI(string path, string data, bool append)
   {
      try
      {
         CreateDirectoryIfRequired(path);
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

   public static bool CreateDirectoryIfRequired(string filePath)
   {
      if (!File.Exists(filePath))
      {
         var directoryPath = Path.GetDirectoryName(filePath);
         if (string.IsNullOrEmpty(directoryPath))
            return false;
         if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath!);
      }
      if (IsProbablyDirectory(filePath) && !Directory.Exists(filePath)) 
         Directory.CreateDirectory(filePath);
      return true;
   }

   private static bool IsProbablyDirectory(string path)
   {
      return path.EndsWith(Path.DirectorySeparatorChar) ||
             path.EndsWith(Path.AltDirectorySeparatorChar) ||
             string.IsNullOrEmpty(Path.GetExtension(path));
   }

   public static bool WriteToFile(string path, string data, bool append)
   {
      if (string.IsNullOrEmpty(path))
         return false;
      try
      {
         CreateDirectoryIfRequired(path);
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

   public static bool WriteToModforgeData(string data, string internalPath, bool append)
   {
      return WriteToFile(Path.Combine(Globals.AppDataPath, internalPath), data, append);
   }

   public static bool WriteLocalisationFile(string path, string content, bool append)
   {
      CreateDirectoryIfRequired(path);
      var streamWriter = new StreamWriter(path, false, bomUtf8Encoding);
      streamWriter.WriteLine($"l_{Globals.Settings.Misc.Language.ToString().ToLower()}:");
      streamWriter.WriteLine(content);
      streamWriter.Close();
      return true;
   }
   
   public static bool Exists(string path) => !string.IsNullOrEmpty(path) && File.Exists(path);

   public static void SaveBmpToModforgeData(this Bitmap bmp, string internalPath)
   {
      bmp.Save(Path.Combine(Globals.AppDataPath, internalPath), ImageFormat.Png);
   }

   public static void SaveImage(string path, Bitmap bmp)
   {
      CreateDirectoryIfRequired(path);
      bmp.Save(path, ImageFormat.Png);
   }
}