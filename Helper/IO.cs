using System.Text;
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


   public static void OpenFolderDialog(string startPath, string filterText, out string folder)
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

   public static void OpenFileSelection(string startFolder, string filterText, out string file)
   {
      file = string.Empty;
      if (!Path.Exists(startFolder))
         return;

      using var dialog = new OpenFileDialog();
      dialog.InitialDirectory = startFolder;
      dialog.CheckFileExists = true;
      dialog.CheckPathExists = true;
      dialog.FileName = filterText;

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
      if (string.IsNullOrEmpty(path))
         return false;
      try
      {
         var directoryPath = Path.GetDirectoryName(path);
         if (string.IsNullOrEmpty(directoryPath))
            return false;
         if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath!);
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

   public static string GetDefaultPathForFolder(params string[] innerPath)
   {
      var path = Path.Combine(Globals.ModPath, Path.Combine(innerPath));
      // retrieve the last folder in the path
      var lastFolder = innerPath[^1];
      if (string.IsNullOrWhiteSpace(lastFolder))
      {
         MessageBox.Show("Failed to create default path!", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
         return string.Empty;
      }
      return Path.Combine(path, $"Modforge_Default_{lastFolder}.txt");
   }
}