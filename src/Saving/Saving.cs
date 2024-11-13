#nullable enable
using Editor.Helper;

namespace Editor.Saving;

public enum SaveFeedback
{
   Ok,
   Error,
   InvalidPath,
   NonExistentPath,
   CanNotAccess
}

internal static class Saving
{
   public static SaveFeedback TrySaveRawFile(string path, string data, bool append = false, bool inform = true)
   {
      if (!File.Exists(path))
      {
         var result = TryCreateFile(path);
         if (result != SaveFeedback.Ok)
            return result;
      }

      if (!IO.WriteAllInANSI(path, data, append))
         return Inform(SaveFeedback.CanNotAccess, path, inform);

      return SaveFeedback.Ok;
   }

   public static SaveFeedback TrySaveBytes(string path, byte[] data, bool inform = true)
   {
      if (!File.Exists(path))
      {
         var result = TryCreateFile(path);
         if (result != SaveFeedback.Ok)
            return result;
      }

      try
      {
         File.WriteAllBytes(path, data);
      }
      catch (IOException e)
      {
         if (inform)
            MessageBox.Show(e.Message, "Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         return SaveFeedback.Error;
      }

      return SaveFeedback.Ok;
   }
   
   /// <summary>
   /// FileExtension should be with a dot
   /// </summary>
   /// <param name="path"></param>
   /// <returns></returns>
   private static SaveFeedback TryCreateFile(string? path)
   {
      if (!Path.IsPathRooted(path))
         return SaveFeedback.InvalidPath;
      var result = TryCreatePath(Path.GetDirectoryName(path));
      if (result != SaveFeedback.Ok)
         return result;

      using (File.Create(Path.Combine(path))) { }

      return SaveFeedback.Ok;
   }

   private static SaveFeedback TryCreatePath(string? path)
   {
      if (path == null || !Path.IsPathRooted(path))
         return SaveFeedback.InvalidPath;

      var directories = path.Split(Path.DirectorySeparatorChar);

      for (var i = 0; i < directories.Length; i++)
      {
         var result = new string[i + 1];
         Array.Copy(directories, result, i + 1);
         var directory = string.Join(Path.DirectorySeparatorChar.ToString(), result);
         if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
      }
      return SaveFeedback.Ok;
   }

   private static SaveFeedback Inform(SaveFeedback feedback, string path, bool inform = true)
   {
      if (feedback == SaveFeedback.Ok || !inform)
         return feedback;
      string message = feedback switch
      {
         SaveFeedback.Error => $"An error occurred while saving the file: \"{path}\"",
         SaveFeedback.InvalidPath => $"The path: \"{path}\" is invalid",
         SaveFeedback.NonExistentPath => $"The path: \"{path}\" does not exist and could not be created.",
         SaveFeedback.CanNotAccess => $"The path: \"{path}\" could not be accessed. It may be used by another process!",
         _ => throw new ArgumentOutOfRangeException(nameof(feedback), feedback, null)
      };
      MessageBox.Show(message, "Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      return feedback;
   }
}