using System.Diagnostics;
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
      private static Dictionary<SaveableType, int> _cache;

      static SaveMaster()
      {
         var names = EnumHelper.GetSaveableNames();
         _cache = new(names.Count);
         foreach (var name in names) 
            _cache.Add(Enum.Parse<SaveableType>(name), 0);
      }

      public static void SaveSaveables(ICollection<Saveable> saveables, SaveableType saveableType = SaveableType.All)
      {
         HashSet<PathObj> paths = [..saveables.Select((x)=>x.Path)];
         SavePaths(ref paths, saveableType);
      }

      public static void SaveAllChanges(bool onlyModified = true, SaveableType saveableType = SaveableType.All)
      {
         if (!onlyModified)
         {
            List<PathObj> toBeSaved = [];
            foreach (var path in AllSaveableFiles.Keys)
            {
               var singleModData = AllSaveableFiles[path][0].WhatAmI();
               if ((saveableType & singleModData) == 0)
                  continue;
               toBeSaved.Add(path);  
            }

            foreach (var path in toBeSaved)
            {
               var singleModData = AllSaveableFiles[path][0].WhatAmI();
               if (!path.IsModPath)
                  SaveVanillaToMod(path);
               else
                  SaveFile(path);
               Globals.SaveableType &= ~singleModData;
               _cache[singleModData] = 0;
            }

            return;
         }
         SavePaths(ref NeedsToBeHandled, saveableType);
      }

      public static void SavePaths(ref HashSet<PathObj> localToBeHandled, SaveableType saveableType = SaveableType.All)
      {
         if (localToBeHandled.Contains(PathObj.Empty))
            AddNewSaveables(saveableType, ref localToBeHandled);
         foreach (var path in localToBeHandled)
         {
            var singleModData = AllSaveableFiles[path][0].WhatAmI();
            if ((saveableType & singleModData) == 0)
               continue;

            if (!path.IsModPath)
               SaveVanillaToMod(path);
            else
               SaveFile(path);
            // Remove the saved type from the still to save types
            // TODO FIX

            if (_cache[singleModData] == 0)
            {
               NeedsToBeHandled.Remove(path);
               Globals.SaveableType &= ~singleModData;
            }
            // The cache has to be always above 0
            else if (_cache[singleModData] < 0)
               throw new EvilActions("The cache value is below 0 this should never happen!");
         }   
      }

      public static void AddNewSaveables(SaveableType saveableType, ref HashSet<PathObj> localToBeHandled)
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
            localToBeHandled.Add(path);
         }
         localToBeHandled.Remove(PathObj.Empty);
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
            if (!newSaveables.Any())
               throw new EvilActions("There needs to be at least one element added to the path.");
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
         _cache[SaveableType.Localisation]++;

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
               if (AllSaveableFiles.TryGetValue(path, out saveables))
                  path = saveables[0].Path;
            }
            AddToDictionary(ref path, saveable);
            saveable.SetPath(ref path);
         }
         NeedsToBeHandled.Add(path);
         var type = saveable.WhatAmI();
         Globals.SaveableType |= type;
         _cache[type]++;
      }

      private static void SaveFile(PathObj path)
      {
         var sb = new StringBuilder();
         List<Saveable> unchanged = [];
         List<Saveable> changed = [];

         if (!path.IsModPath)
            throw new EvilActions($"The file {path} is a vanilla file and cannot be overwritten!");

         var saveIndividually = true;
         Saveable? lastItem = null;
         var saveabletype = AllSaveableFiles[path].First().WhatAmI();
         foreach (var item in AllSaveableFiles[path])
         {
            if (lastItem is null)
            {
               lastItem = item;
               saveIndividually = item.GetSaveStringIndividually;
            }
            
            if (item.EditingStatus == ObjEditingStatus.Modified)
               changed.Add(item);
            else if (item.EditingStatus != ObjEditingStatus.Deleted)
               unchanged.Add(item);

            if (item.EditingStatus is ObjEditingStatus.Modified or ObjEditingStatus.Deleted)
            {
               _cache[saveabletype]--;
            }

            item.EditingStatus = ObjEditingStatus.Unchanged;
         }

         if (saveIndividually)
         {
            if (changed.Count > 0 && !string.IsNullOrWhiteSpace(changed[0].GetHeader()))
               sb.AppendLine(changed[0].GetHeader());

            // Save the unchanged first
            AddListToStringBuilder(ref sb, unchanged);
            if (Globals.Settings.Saving.AddModifiedCommentToFilesWhenSaving)
               sb.AppendLine($"### Modified {DateTime.Now} ###");
            AddListToStringBuilder(ref sb, changed);

            if (changed.Count > 0 && !string.IsNullOrWhiteSpace(changed[0].GetFooter()))
               sb.AppendLine(changed[0].GetFooter());
         }
         else
         {
            sb.Append(lastItem?.GetFullFileString(changed, unchanged));
         }

         if (path.IsLocalisation)
            IO.WriteLocalisationFile(path.ToPath(), sb.ToString(), false);
         else
            IO.WriteAllInANSI(path.ToPath(), sb.ToString(), false);
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<Saveable> items, int tabs = 0)
      {
         foreach (var item in items)
         {
            if (Globals.Settings.Saving.AddCommentAboveObjectsInFiles)
            {
               var saveComment = item.SavingComment();
               if (!string.IsNullOrWhiteSpace(saveComment))
                  sb.AppendLine($"# {saveComment}");
            }
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
               path[^1] = SavingUtil.ApplyModPrefix(path[^1]);
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