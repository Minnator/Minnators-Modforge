using System.Text;
using Editor.DataClasses.Misc;
using Editor.Forms.Feature.SavingClasses;
using Editor.Helper;

namespace Editor.Saving
{
   public class EvilActions(string message) : Exception(message);

   public static class SaveMaster
   {
      private static HashSet<PathObj> NeedsToBeHandled = [];
      private static Dictionary<PathObj, List<Saveable>> AllSaveableFiles = [];
      
      public static void SaveAllChanges(bool onlyModified = true, SaveableType saveableType = SaveableType.All)
      {
         if (NeedsToBeHandled.Contains(PathObj.Empty))
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
            // Remove the saved type from the still to save types
            Globals.SaveableType &= ~singleModData;
         }

         NeedsToBeHandled.Clear();
      }

      public static void AddNewSaveables(SaveableType saveableType)
      {
         Dictionary<string, PathObj> pathGrouping = [];
         foreach (var saveable in AllSaveableFiles[PathObj.Empty])
         {
            if ((saveableType & saveable.WhatAmI()) == 0)
               continue;

            var filenameKVP = saveable.GetFileName();
            var fileEnding = saveable.GetFileEnding();
            var preDefaultPath = saveable.GetDefaultFolderPath();
            var path = saveable.Path;
            if (path != PathObj.Empty)
               continue;

            var groupingString = Path.Combine(preDefaultPath);

            if (!pathGrouping.TryGetValue(groupingString, out path))
            {
               GetNewFileAt(preDefaultPath, filenameKVP, fileEnding, out var pathString, out var useGrouping, false, saveable.GetSavePromptString());
               path = new(pathString, true);
               if (useGrouping)
                  pathGrouping.Add(groupingString, path);
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
         NeedsToBeHandled.Remove(PathObj.Empty);
         AllSaveableFiles.Remove(PathObj.Empty);
      }

      public static void AddToDictionary(ref PathObj path, Saveable saveable)
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

      public static void RemoveFromDictionary(Saveable saveable)
      {
         if (AllSaveableFiles.TryGetValue(saveable.Path, out var saveables))
            saveables.Remove(saveable);
      }

      public static void AddRangeToDictionary(PathObj path, IEnumerable<Saveable> newSaveables)
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

      public static void AddLocObject(Saveable saveable)
      {
         if (saveable is not LocObject)
            throw new EvilActions($"Should not add a non Loc Object {saveable} using AddLocObject!");
         var path = saveable.Path;
         Globals.SaveableType |= SaveableType.Localisation;

         if (path.Equals(PathObj.Empty))
         {
            AddToDictionary(ref path, saveable);
         }
         else if (!path.IsModPath)
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

      public static void AddToBeHandled(Saveable saveable)
      {
         var fileNameKVP = saveable.GetFileName();
         var fileEnding = saveable.GetFileEnding();
         var defaultPath = saveable.GetDefaultFolderPath();
         var path = saveable.Path;
         if (path != PathObj.Empty)
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

      private static void SaveFile(PathObj path)
      {
         var sb = new StringBuilder();
         List<Saveable> unchanged = [];
         List<Saveable> changed = [];

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
            else
            {
               item.Dispose();
            }
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
            IO.WriteAllInANSI(path.ToPath(), sb.ToString(), false);
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<Saveable> items, int tabs = 0)
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
      private static void SaveVanillaToMod(PathObj path)
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

            var form = new GetSavingFileForm(Path.Combine(folderPath), $"Please enter your input for: {savePromptString}", ending);
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