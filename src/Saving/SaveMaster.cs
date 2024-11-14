using System.Collections;
using System.ComponentModel;
using System.Text;
using Editor.DataClasses.Misc;
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
      public NewPathObj Path = NewPathObj.Empty;

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

      public void SetPath(ref NewPathObj path)
      {
         Path = path;
      }

      public virtual string GetHeader()
      {
         return string.Empty;
      }

      public abstract SaveableType WhatAmI();

      /// <summary>
      /// The folder where the object should be saved
      /// </summary>
      /// <returns></returns>
      public abstract string[] GetDefaultFolderPath();

      /// <summary>
      /// Formatted like: ".txt"
      /// </summary>
      /// <returns></returns>
      public abstract string GetFileEnding();

      /// <summary>
      /// Returns the default file name for a saveable, if it was not saved in vanilla and was not overwritten by the user unless it is forced
      /// </summary>
      /// <returns>KeyValuePair where the key is the default file name and the bool, which indicates if it is forced aka not overwritteable by the user</returns>
      public abstract KeyValuePair<string, bool> GetFileName();

      public abstract string SavingComment();

      public abstract string GetSaveString(int tabs);

      public abstract string GetSavePromptString();
   }

   public class NewPathObj(string[] path, bool isModPathInit)
   {
      public string[] Path = path;
      public bool IsModPath = isModPathInit;
      public bool IsLocalisation => Path.Contains("localisation");

      public static readonly NewPathObj Empty = new([], false);

      public NewPathObj Copy(bool modPath, bool addReplacePath = false)
      {
         if (!addReplacePath)
            return new(Path, modPath);
         var pathParts = new string[Path.Length + 1];
         Array.Copy(Path, pathParts, Path.Length - 1);
         pathParts[^2] = "replace";
         pathParts[^1] = Path[^1];
         return new(pathParts, modPath);
      }

      public static NewPathObj FromPath(string path) => FromPath(path, path.StartsWith(Globals.ModPath));

      public static NewPathObj FromPath(string path, bool isModPath)
      {
         return new(isModPath 
            ? path.Remove(0, Globals.ModPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar) 
            : path.Remove(0, Globals.VanillaPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar)
            , isModPath);
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
      private static Dictionary<NewPathObj, List<NewSaveable>> AllSaveableFiles = [];
      
      public static void SaveAllChanges(bool onlyModified = true, SaveableType saveableType = SaveableType.All)
      {
         if (NeedsToBeHandled.Contains(NewPathObj.Empty))
            AddNewSaveables(saveableType);
         if (!onlyModified)
         {
            foreach (var path in AllSaveableFiles.Keys)
            {
               var singleModData = AllSaveableFiles[path][0].WhatAmI();
               if ((saveableType & singleModData) == 0)
                  continue;
               SaveFile(path);
               Globals.SaveableType &= ~singleModData;
            }
            return;
         }
         foreach (var path in NeedsToBeHandled)
         {
            var singleModData = AllSaveableFiles[path][0].WhatAmI();
            if ((saveableType & singleModData) == 0)
               continue;

            if (!path.IsModPath)
               SaveVanillaToMod(path);
            else
               SaveFile(path);
            Globals.SaveableType &= ~singleModData;
         }

         NeedsToBeHandled.Clear();
      }

      public static void AddNewSaveables(SaveableType saveableType)
      {
         Dictionary<string[], NewPathObj> pathGrouping = [];
         foreach (var saveable in AllSaveableFiles[NewPathObj.Empty])
         {
            if ((saveableType & saveable.WhatAmI()) == 0)
               continue;

            var filenameKVP = saveable.GetFileName();
            var fileEnding = saveable.GetFileEnding();
            var defaultPath = saveable.GetDefaultFolderPath();
            var path = saveable.Path;
            if (path != NewPathObj.Empty)
               continue;

            GetNewFileAt(defaultPath, filenameKVP, fileEnding, out defaultPath, out _, true);

            if (!pathGrouping.TryGetValue(defaultPath, out path))
            {
               GetNewFileAt(defaultPath, filenameKVP, fileEnding, out var pathString, out var useGrouping, false, saveable.GetSavePromptString());
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
               path = otherSaveables[0].Path;
               otherSaveables.Add(saveable);
            }

            saveable.SetPath(ref path);
            NeedsToBeHandled.Add(path);
         }
         NeedsToBeHandled.Remove(NewPathObj.Empty);
         AllSaveableFiles.Remove(NewPathObj.Empty);
      }

      public static void AddToDictionary(ref NewPathObj path, NewSaveable saveable)
      {
         lock (AllSaveableFiles)
         {
            if (!AllSaveableFiles.TryGetValue(path, out var saveables))
            {
               AllSaveableFiles.Add(path, []);
               saveables = AllSaveableFiles[path];
            }
            saveables.Add(saveable);
         }
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

      public static void AddToLocObject(NewSaveable saveable)
      {
         var path = saveable.Path;
         if (!path.IsLocalisation)
            throw new EvilActions($"Should not add a non localisation saveable {path} using AddToLocObject!");
         Globals.SaveableType |= SaveableType.Localisation;

         if (path.Equals(NewPathObj.Empty))
         {
            AddToDictionary(ref path, saveable);
         }
         else
         {
            path = path.Copy(true, true);

            if (!AllSaveableFiles.TryGetValue(path, out var list))
            {
               AllSaveableFiles.Add(path, [saveable]);
            }
            else
            {
               path = list[0].Path;
               list.Add(saveable);
            }
            saveable.SetPath(ref path);
         }
         NeedsToBeHandled.Add(path);
      }

      public static void AddToBeHandled(NewSaveable saveable)
      {
         var fileNameKVP = saveable.GetFileName();
         var fileEnding = saveable.GetFileEnding();
         var defaultPath = saveable.GetDefaultFolderPath();
         var path = saveable.Path;
         if (path != NewPathObj.Empty)
            NeedsToBeHandled.Add(path);
         else if (!fileNameKVP.Value)
         {
            AddToDictionary(ref path, saveable);
            NeedsToBeHandled.Add(path);
         }
         else
         {
            if (!GetNewFileAt(defaultPath, fileNameKVP, fileEnding, out var stringPath, out _))
               throw new EvilActions($"Not able to make directory: {defaultPath}!");

            path = new(stringPath, true);

            if (AllSaveableFiles.TryGetValue(path, out var saveables))
            {
               path = saveables[0].Path;
            }
            else
            {
               path.IsModPath = false;
               if (!AllSaveableFiles.TryGetValue(path, out saveables))
                  AddToDictionary(ref path, saveable);
               else
                  path = saveables[0].Path;
            }

            saveable.SetPath(ref path);
         }
         NeedsToBeHandled.Add(path);
         Globals.SaveableType |= saveable.WhatAmI();
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

         if (!string.IsNullOrWhiteSpace(changed[0].GetHeader()))
            sb.AppendLine(changed[0].GetHeader());

         // Save the unchanged first
         AddListToStringBuilder(ref sb, unchanged);
         sb.AppendLine($"### Modified {DateTime.Now} ###");
         AddListToStringBuilder(ref sb, changed);

         if (path.IsLocalisation)
            IO.WriteLocalisationFile(path.ToPath(), sb.ToString(), false);
         else
            IO.WriteToFile(path.ToPath(), sb.ToString(), false);
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<NewSaveable> items, int tabs = 0)
      {
         foreach (var item in items)
         {
            var saveComment = item.SavingComment();
            if (!string.IsNullOrWhiteSpace(saveComment))
               sb.AppendLine($"# {saveComment}");
            sb.AppendLine(item.GetSaveString(tabs));
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
         // TODO remove
         if (savables[0].Path != path)
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