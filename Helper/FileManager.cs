using System.Collections;
using System.Text;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms.SavingClasses;
using Editor.Interfaces;

namespace Editor.Helper
{
   public static class FileManager
   {
      private static HashSet<PathObj> NeedsToBeHandledMod = [];
      private static HashSet<PathObj> NeedsToBeHandledVanilla = [];
      // New ISavables will be saved with an empty string
      private static Dictionary<PathObj, List<Saveable>> AllSaveableFiles = []; //Saves AllFiles except vanilla localisation files and saveables


      public static void AddToDictionary(ref PathObj path, Saveable saveable)
      {
         if (!AllSaveableFiles.TryGetValue(path, out var saveables))
         {
            AllSaveableFiles.Add(path, []);
            saveables = AllSaveableFiles[path];
         }
         saveables.Add(saveable);
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

      public static void AddLocObject(LocObject loc)
      {
         Globals.SaveableType |= loc.WhatAmI();
         if (loc.Path.Equals(PathObj.Empty))
         {
            if (!AllSaveableFiles.TryAdd(loc.Path, [loc]))
               AllSaveableFiles[loc.Path].Add(loc);
            return;
         }

         if (loc.Path.isModPath)
         {
            NeedsToBeHandledMod.Add(loc.Path);
            return;
         }

         var path = loc.Path.GetInverted(true);

         if (!AllSaveableFiles.TryGetValue(path, out var list))
         {
            AllSaveableFiles.Add(path, [loc]);
         }
         else
         {
            path = list[0].Path;
            list.Add(loc);
         }
         loc.SetPath(ref path);
         NeedsToBeHandledMod.Add(path);
      }

      public static void AddToBeHandled(Saveable obj)
      {
         Globals.SaveableType |= obj.WhatAmI();
         // It is a new object and will be handled via the PathObj.Empty
         if (obj.Path.Equals(PathObj.Empty))
         {
            if (obj is ProvinceComposite and not Province)
            {
               // Does it exist in mod or vanilla?
               var path = obj.GetDefaultSavePath(); // default file name is modPath
               if (AllSaveableFiles.TryGetValue(path, out var saveables))
               {
                  path = saveables[0].Path;
                  NeedsToBeHandledMod.Add(obj.Path);
               }
               else
               {
                  path = AllSaveableFiles[path.GetInverted(false)][0].Path;
                  NeedsToBeHandledVanilla.Add(path);
               }
               obj.SetPath(ref path);
               AllSaveableFiles[path].Add(obj);
               return;
            }
            else if (!AllSaveableFiles.TryAdd(obj.Path, [obj]))
               AllSaveableFiles[obj.Path].Add(obj);
         }
         else if (obj.Path.isModPath)
            NeedsToBeHandledMod.Add(obj.Path);
         else
            NeedsToBeHandledVanilla.Add(obj.Path);
      }

      /// <summary>
      /// onlyModified: If true, only the modified objects will be saved, if false all objects will be saved
      /// </summary>
      /// <param name="onlyModified"></param>
      /// <param name="saveableType"></param>
      public static void SaveChanges(bool onlyModified = true, SaveableType saveableType = SaveableType.All)
      {
         // TODO safe all when only Modified is false

         if (onlyModified)
         {
            CalculateModifiedObjects(saveableType);
            SaveModifiedObjects();
         }
         else
         {
            CalculateAllObjects();
            SaveAllObjects();
         }


         NeedsToBeHandledMod.Clear();
         NeedsToBeHandledVanilla.Clear();
      }

      private static void SaveAllObjects()
      {
         throw new NotImplementedException();
      }

      private static void CalculateAllObjects()
      {
         throw new NotImplementedException();
      }

      private static void CalculateModifiedObjects(SaveableType saveableType)
      {
         // TODO Order and save the position of unchanged, modified and new, so it can be saved neatly or use the SaveModifiedObjects version
         foreach (var change in NeedsToBeHandledVanilla)
         {
            var singleModData = AllSaveableFiles[change][0].WhatAmI();
            if ((saveableType & singleModData) == 0)
               continue;
            var replace = change.ShouldReplace;
            var modPath = change.GetInverted(replace);
            
            // Check is the file already exists
            if (!AllSaveableFiles.TryGetValue(modPath, out var saveables))
            {
               AllSaveableFiles.Add(modPath, []); // empty list as it is new
               saveables = AllSaveableFiles[modPath];
            }

            // Mod modify
            saveables.AddRange(AllSaveableFiles[change]);

            var newPath = AllSaveableFiles[change][0];
            AllSaveableFiles.Remove(change);
            newPath.Path.isModPath = true;

            NeedsToBeHandledMod.Add(modPath);

         }

         // Adding entirely new obj to the mod
         if (AllSaveableFiles.TryGetValue(PathObj.Empty, out var newSaveables))
         {
            Dictionary<SaveableType, PathObj> pathGrouping = [];
            foreach (var saveable in newSaveables)
            {
               var singleModData = saveable.WhatAmI();
               if ((saveableType & singleModData) == 0)
                  continue;

               // TODO group by folders to reduce pop up count
               var ending = ".txt";
               if (saveable is LocObject) 
                  ending = $"_l_{Globals.Language.ToString().ToLower()}.yml";
               if (!pathGrouping.TryGetValue(singleModData, out var modPath))
               {
                  modPath = new(GetNewFileAt(saveable, ending), true);
                  pathGrouping.Add(singleModData, modPath);
               }

               if (!AllSaveableFiles.TryGetValue(modPath, out var saveables))
               {
                  AllSaveableFiles.Add(modPath, []);
                  saveables = AllSaveableFiles[modPath];
               }
               saveables.Add(saveable);
               NeedsToBeHandledMod.Add(modPath);
            }
            AllSaveableFiles.Remove(PathObj.Empty);
         }
      }

      private static void SaveModifiedObjects()
      {
         // Since all changes have been added to NeedsToBeHandledMod in CalculateModifiedObjects, only one foreach over the Files is necessary

         foreach (var obj in NeedsToBeHandledMod)
         {
            var sb = new StringBuilder();
            List<Saveable> unchanged = [];
            List<Saveable> changed = [];
            
            foreach (var item in AllSaveableFiles[obj])
            {
               if (item.EditingStatus == ObjEditingStatus.Modified)
                  changed.Add(item);
               else
                  unchanged.Add(item);

               item.EditingStatus = ObjEditingStatus.Unchanged;
            }
            if (!string.IsNullOrWhiteSpace(changed[0].GetHeader()))
               sb.AppendLine(changed[0].GetHeader());

            // Save the unchanged first
            AddListToStringBuilder(ref sb, unchanged);
            sb.AppendLine($"### Modified {DateTime.Now} ###");
            AddListToStringBuilder(ref sb, changed);

            if (obj.IsLocalisation)
               IO.WriteLocalisationFile(obj.ToPath(), sb.ToString(), false);
            else
               IO.WriteToFile(obj.ToPath(), sb.ToString(), false);
         }
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<Saveable> items)
      {
         foreach (var item in items)
         {
            var saveComment = item.SavingComment();
            if (!string.IsNullOrWhiteSpace(saveComment))
               sb.AppendLine($"# {saveComment}");
            sb.AppendLine(item.GetSaveString(0));
         }
      }

      /// <summary>
      /// Returns a new Path for a file in the mod folder with the given path either provided by the user or a default file
      /// </summary>
      /// <param name="saveable"></param>
      /// <param name="ending"></param>
      /// <returns></returns>
      public static string[] GetNewFileAt(Saveable saveable, string ending = ".txt")
      {
         if (Globals.Settings.SavingSettings.AlwaysAskBeforeCreatingFiles)
         {
            var inputForm = new GetSavingFileForm(Path.Combine(Globals.ModPath, Path.Combine(saveable.GetDefaultSavePath().Path)), $"Please enter your input for: {saveable.GetSavePromptString()}", ending);
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
               return inputForm.NewPath.Split(Path.DirectorySeparatorChar);
            }

         }
         return GetDefaultPathForFolder(ending, saveable.GetDefaultSavePath().Path);
      }

      private static string[] GetDefaultPathForFolder(string ending, params string[] innerPath)
      {
         if (innerPath.Length < 1)
         {
            MessageBox.Show("Failed to create default path!", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return [];
         }

         var lastFolder = innerPath[^1];
         if (string.IsNullOrWhiteSpace(lastFolder))
         {
            MessageBox.Show("Failed to create default path!", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return [];
         }

         return Path.Combine(Globals.ModPath, Path.Combine(innerPath), $"Modforge_{lastFolder}{ending}").Split(Path.DirectorySeparatorChar);
      }

      // Completely new file in mod
      // Modify file in mod

      public static void ModModify(PathObj path, List<Saveable> objToSave)
      {

      }

      public static void ModNew(PathObj path, List<Saveable> objToSave)
      {

      }

      public static bool ExistsInMod(params string[] path)
      {
         return File.Exists(Path.Combine(Globals.ModPath, Path.Combine(path)));
      }
   }

   public class PathObj(string[] path, bool isModPath)
   {
      public string[] Path = path;
      public bool isModPath = isModPath;

      public PathObj(string[] path) : this(path, true)
      {

      }

      public bool ShouldReplace => !isModPath && IsLocalisation;
      public bool IsLocalisation => Path.Contains("localisation");

      public static PathObj FromPath(string path, bool isModPath)
      {
         var internalPath = isModPath
            ? path.Remove(0, Globals.ModPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar)
            : path.Remove(0, Globals.VanillaPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar);
         return new(internalPath, isModPath);
      }

      public string ToPath()
      {
         if (!isModPath)
            throw new EvilActions("Trying to overwrite the vanilla resources is Evil!");
         return System.IO.Path.Combine(Globals.ModPath, System.IO.Path.Combine(Path));
      }

      public PathObj GetInverted(bool addReplacePath)
      {
         if (!addReplacePath)
            return new(Path, !isModPath);
         var pathParts = new string[Path.Length + 1];
         Array.Copy(Path, pathParts, Path.Length - 1);
         pathParts[^2] = "replace";
         pathParts[^1] = Path[^1];
         return new(pathParts, !isModPath);
      }

      public static PathObj Empty => new([], true);

      public override int GetHashCode()
      {
         return HashCode.Combine(isModPath.GetHashCode(), StructuralComparisons.StructuralEqualityComparer.GetHashCode(Path));
      }

      public override bool Equals(object? obj)
      {
         if (obj is not PathObj other)
            return false;
         return isModPath == other.isModPath && Path.SequenceEqual(other.Path);
      }

      public override string ToString()
      {
         return (isModPath ? $"Mod: " : $"Vanilla: ") + System.IO.Path.Combine(Path);
      }
   }

   public class EvilActions : Exception
   {
      public EvilActions(string message) : base(message)
      {
      }
   }
}