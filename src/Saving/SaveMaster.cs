using System.Collections;
using System.ComponentModel;
using System.Text;
using Editor.Forms.Feature.SavingClasses;
using Editor.Helper;
using Newtonsoft.Json;

namespace Editor.Saving
{
   public abstract class NewSaveable
   {
      protected ObjEditingStatus _editingStatus = ObjEditingStatus.Unchanged;
      [Browsable(false)]
      [JsonIgnore]
      public NewPathObj[] paths = [NewPathObj.Empty];

      [Browsable(false)]
      public virtual ObjEditingStatus EditingStatus
      {
         get => _editingStatus;
         set
         {
            if (_editingStatus == ObjEditingStatus.Immutable)
               return;
            if (Equals(value, _editingStatus))
               return;
            if (Equals(value, ObjEditingStatus.Modified))
               SaveMaster.AddToBeHandled(this);
            _editingStatus = value;
         }
      }

      public NewPathObj Path
      {
         get => paths[0];
         set => paths[0] = value;
      }

      public void SetPath(ref NewPathObj path)
      {
         paths[0] = path;
      }

      public virtual string GetHeader(NewPathObj path)
      {
         return string.Empty;
      }

      /// <summary>
      /// The folder where the object should be saved
      /// </summary>
      /// <returns></returns>
      public abstract string[][] GetDefaultFolderPath();

      /// <summary>
      /// ".txt"
      /// </summary>
      /// <returns></returns>
      public abstract string[] GetFileEnding();

      /// <summary>
      /// Does NOT contain the file ending
      /// </summary>
      /// <returns></returns>
      public abstract KeyValuePair<string, bool>[] GetFileName();

      public abstract string SavingComment(NewPathObj path);

      public abstract string GetSaveString(int tabs, NewPathObj path);

      public abstract string GetSavePromptString(NewPathObj path);
   }

   public class NewPathObj(string[] path, bool isModPathInit)
   {
      public string[] Path = path;
      public bool IsModPath = isModPathInit;
      public bool IsLocalisation => Path.Contains("localisation");

      public static readonly NewPathObj Empty = new([], false);

      public NewPathObj Copy(bool modPath)
      {
         return new(Path, modPath);
      }

      public static NewPathObj FromPath(string path, bool isModPath)
      {
         var internalPath = isModPath
            ? path.Remove(0, Globals.ModPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar)
            : path.Remove(0, Globals.VanillaPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar);
         return new(internalPath, isModPath);
      }

      public string ToPath()
      {
         return System.IO.Path.Combine(Globals.ModPath, System.IO.Path.Combine(Path));
      }

      public static bool operator ==(NewPathObj a, NewPathObj b)
      {
         return a.Path == b.Path && a.IsModPath == b.IsModPath;
      }

      public static bool operator !=(NewPathObj a, NewPathObj b)
      {
         return a.Path != b.Path || a.IsModPath != b.IsModPath;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(IsModPath.GetHashCode(), StructuralComparisons.StructuralEqualityComparer.GetHashCode(Path));
      }

      public override bool Equals(object? obj)
      {
         if (obj is not NewPathObj other)
            return false;
         return IsModPath == other.IsModPath && Path.SequenceEqual(other.Path);
      }

      public override string ToString()
      {
         return (IsModPath ? $"Mod: " : $"Vanilla: ") + System.IO.Path.Combine(Path);
      }

   }


   public static class SaveMaster
   {
      private static HashSet<NewPathObj> NeedsToBeHandled = [];
      private static HashSet<NewSaveable> NeedsToBeHandledReplace = [];
      private static Dictionary<NewPathObj, List<NewSaveable>> AllSaveableFiles = [];

      public static void SaveAllChanges()
      {
         if (NeedsToBeHandled.Contains(NewPathObj.Empty))
            AddNewSaveables();

         foreach (var path in NeedsToBeHandled)
         {
            if (!path.IsModPath) 
               SaveVanillaToMod(path);
            else
               SaveFile(path);
         }

         NeedsToBeHandled.Clear();
      }

      public static void AddNewSaveables()
      {
         Dictionary<string[], NewPathObj> pathGrouping = [];
         foreach (var saveable in AllSaveableFiles[NewPathObj.Empty])
         {
            var filenamesKVP = saveable.GetFileName();
            var fileEndings = saveable.GetFileEnding();
            var defaultPaths = saveable.GetDefaultFolderPath();
            for (var i = 0; i < saveable.paths.Length; i++)
            {
               var path = saveable.paths[i];
               if (path != NewPathObj.Empty)
                  continue;

               GetNewFileAt(defaultPaths[i], filenamesKVP[i], fileEndings[i], out var defaultPath, out _, true);

               if (!pathGrouping.TryGetValue(defaultPath, out path))
               {
                  GetNewFileAt(defaultPaths[i], filenamesKVP[i], fileEndings[i], out var pathString, out var useGrouping, false, saveable.GetSavePromptString(path));
                  path = new(pathString, true);
                  if (useGrouping)
                     pathGrouping.Add(defaultPath, path);
               }


               if (!AllSaveableFiles.TryGetValue(path, out var otherSaveables))
               {
                  // Maybe call AddToDictionary instead
                  AllSaveableFiles.Add(path, [saveable]);
                  otherSaveables = AllSaveableFiles[path];
               }
               else
               {
                  path = otherSaveables[0].paths[i];
                  otherSaveables.Add(saveable);
               }
               saveable.paths[i] = path;
               NeedsToBeHandled.Add(path);
            }
         }
         NeedsToBeHandled.Remove(NewPathObj.Empty);
         AllSaveableFiles.Remove(NewPathObj.Empty);
      }

      public static void AddToDictionary(ref NewPathObj path, NewSaveable saveable)
      {
         if (!AllSaveableFiles.TryGetValue(path, out var saveables))
         {
            AllSaveableFiles.Add(path, []);


            //print all hashcodes for AllSaveableFiles
            foreach (var (key, value) in AllSaveableFiles)
            {
               System.Diagnostics.Debug.WriteLine($"{Path.Combine(key.Path)} : {key.GetHashCode()}");
            }

            System.Diagnostics.Debug.WriteLine($"{Path.Combine(path.Path)} : {path.GetHashCode()}");

            saveables = AllSaveableFiles[path];
         }
         saveables.Add(saveable);
      }

      public static void AddRangeToDictionary(NewPathObj path, IEnumerable<NewSaveable> newSaveables)
      {
         lock (AllSaveableFiles)
         {
            if (!AllSaveableFiles.TryGetValue(path, out var saveables))
            {
               AllSaveableFiles.Add(path, []);
               saveables = AllSaveableFiles[path];
            }
            saveables.AddRange(newSaveables);
         }
      }

      public static void AddToBeHandled(NewSaveable saveable)
      {
         var filenamesKVP = saveable.GetFileName();
         var fileEndings = saveable.GetFileEnding();
         var defaultPaths = saveable.GetDefaultFolderPath();
         for (var i = 0; i < saveable.paths.Length; i++)
         {
            var path = saveable.paths[i];
            if (path == NewPathObj.Empty)
            {
               if (!filenamesKVP[i].Value)
               {
                  AddToDictionary(ref path, saveable);
                  NeedsToBeHandled.Add(path);
                  continue;
               }

               if (!GetNewFileAt(defaultPaths[i], filenamesKVP[i], fileEndings[i], out var stringPath, out _))
                  throw new EvilActions($"Not able to make directory: {defaultPaths}!");
               
               path = new(stringPath, true);

               if (AllSaveableFiles.TryGetValue(path, out var saveables))
               {
                  path = saveables[0].paths[i];
               }
               else
               {
                  path.IsModPath = false;
                  if (!AllSaveableFiles.TryGetValue(path, out saveables))
                  {
                     AddToDictionary(ref path, saveable);
                  }
                  else
                     path = saveables[0].paths[i];
               }

               saveable.paths[i] = path;
            }
            NeedsToBeHandled.Add(path);
         }
      }

      private static void SaveFile(NewPathObj path)
      {
         var sb = new StringBuilder();
         List<NewSaveable> unchanged = [];
         List<NewSaveable> changed = [];

         if (!path.IsModPath)
            throw new EvilActions($"The file {path} is a vanilla file and cannot be overwritten!");

         foreach (var item in AllSaveableFiles[path])
         {
            if (item.EditingStatus == ObjEditingStatus.Modified)
            {
               item.EditingStatus = ObjEditingStatus.Unchanged;
               changed.Add(item);
            }
            else if (item.EditingStatus != ObjEditingStatus.Deleted)
               unchanged.Add(item);
            // TODO final delete
         }

         if (!string.IsNullOrWhiteSpace(changed[0].GetHeader(path)))
            sb.AppendLine(changed[0].GetHeader(path));

         // Save the unchanged first
         AddListToStringBuilder(ref sb, unchanged, path);
         sb.AppendLine($"### Modified {DateTime.Now} ###");
         AddListToStringBuilder(ref sb, changed, path);

         if (path.IsLocalisation)
            IO.WriteLocalisationFile(path.ToPath(), sb.ToString(), false);
         else
            IO.WriteToFile(path.ToPath(), sb.ToString(), false);
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<NewSaveable> items, NewPathObj path)
      {
         foreach (var item in items)
         {
            var saveComment = item.SavingComment(path);
            if (!string.IsNullOrWhiteSpace(saveComment))
               sb.AppendLine($"# {saveComment}");
            sb.AppendLine(item.GetSaveString(0, path));
         }
      }

      // Only for not replace
      private static void SaveVanillaToMod(NewPathObj path)
      {
         var savables = AllSaveableFiles[path];
         var modPath = path.Copy(true);
         // if the modPath already exisist we did something wrong because this is for no Replace!
         if (AllSaveableFiles.TryGetValue(modPath, out _))
            throw new EvilActions($"The file {modPath} is already saved in AllSavableFiles, but should not be present!");
         // swap the references to the mod version
         AllSaveableFiles.Remove(path);
         path.IsModPath = true;
         if(savables[0].paths[0] != path)
            throw new EvilActions($"Path was not updated!");
         AllSaveableFiles.Add(path, savables);
         SaveFile(path);
      }

      private static bool GetNewFileAt(string[] folderPath, KeyValuePair<string, bool> filename, string ending, out string[] path, out bool usedGrouping, bool overrideValue = false, string savePromptString = ": )")
      {
         path = folderPath;
         usedGrouping = false;
         // Ask the user where to create the file if it can be decided.
         if (Globals.Settings.Saving.AlwaysAskBeforeCreatingFiles)
         {
            // The path is forced the user can NOT decide the filename or where to save it
            if (filename.Value || overrideValue)
               goto DefaultPath;

            var form = new GetSavingFileForm(Path.Combine(path), $"Please enter your input for: {savePromptString}", ending);
            if (form.ShowDialog() == DialogResult.OK)
            {
               path = form.NewPath.Split(Path.DirectorySeparatorChar);
               usedGrouping = form.UseGrouping;
               return true;
            }
            MessageBox.Show($"Error selecting file for {filename} in {folderPath[^1]}\nUsing default.", "Error retrieving path", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }

         DefaultPath:
         try
         {
            Directory.CreateDirectory(Path.Combine(path));
         }
         catch (Exception)
         {
            MessageBox.Show($"Error creating default file for {filename} in {folderPath[^1]}", "Error creating path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            path = [];
            return false;
         }

         path = [.. path.Concat([filename.Key + ending])];
         if (!VerifyFilename(filename + ending))
         {
            MessageBox.Show($"Error creating default file for {filename} in {folderPath[^1]}", "Error creating path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            path = [];
            return false;
         }
         return true;
      }

      private static bool VerifyFilename(string fileName)
      {
         return !string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
      }

   }
}