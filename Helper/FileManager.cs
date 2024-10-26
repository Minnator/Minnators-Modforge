using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Editor.Forms.SavingClasses;
using Editor.Interfaces;

namespace Editor.Helper
{
   public static class FileManager
   {
      private static HashSet<PathObj> NeedsToBeHandledMod = [];
      private static HashSet<PathObj> NeedsToBeHandledVanilla = [];
      // New ISavables will be saved with an empty string
      private static Dictionary<PathObj, List<Saveable>> AllSaveableFiles = [];

      public static void AddToDictionary(ref PathObj path, Saveable saveable)
      {
         if (!AllSaveableFiles.TryGetValue(path, out var saveables))
         {
            AllSaveableFiles.Add(path, []);
            saveables = AllSaveableFiles[path];
         }
         saveables.Add(saveable);
      }

      public static void AddRangeToDictionary(PathObj path, ICollection<Saveable> newSaveables)
      {
         if (!AllSaveableFiles.TryGetValue(path, out var saveables))
         {
            AllSaveableFiles.Add(path, []);
            saveables = AllSaveableFiles[path];
         }
         saveables.AddRange(newSaveables);
      }

      public static void AddToBeHandled(Saveable obj)
      {
         // It is a new object and will be handled via the PathObj.Empty
         if (obj.Path.Equals(PathObj.Empty))
         {
            if (!AllSaveableFiles.TryAdd(obj.Path, [obj])) 
               AllSaveableFiles[obj.Path].Add(obj);
         }
         else if (obj.Path.isModPath)
            NeedsToBeHandledMod.Add(obj.Path);
         else
            NeedsToBeHandledVanilla.Add(obj.Path);
      }

      /*
       
         vanilla
            theoretisches mod pathobj
            wenn pathobj in saveables (aka Mod file existiert)
               saveables[pathobj].Add(vanilla)
               saveables[oldpathobj].Remove(vanilla)
               entweder alle adden oder removen je nachdem ob replace oder modify
            sonst
               neues File direkt erstellen
       
         new mod (aka saveables[PathObj.Empty])
            alle benötigten pathObjs erstellen und hinzufügen zu Saveables
            PathObj.Empty aus dictionary entfernen
            und zu NeedsToBeHandledMod adden oder direkt bearbeiten

         mod
            schreiben
         
         finally
            needsToBeHandled Clearen
            Alle Saveables zurücksetzen auf unchanged
         
       */

      public static void SaveChanges(bool onlyModified = true)
      {
         // TODO safe all when only Modified is false

         if (onlyModified)
         {
            CalculateModifiedObjects();
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

      private static void CalculateModifiedObjects()
      {
         // TODO Order and save the position of unchanged, modified and new, so it can be saved neatly or use the SaveModifiedObjects version
         foreach (var change in NeedsToBeHandledVanilla)
         {
            var replace = change.ShouldReplace();
            var modPath = change.GetInverted(replace);

            // Check is the file already exists
            if (!AllSaveableFiles.TryGetValue(modPath, out var saveables))
            {
               if (replace)
               {
                  // No Mod file but replace, so any name
                  modPath = new(modPath.Path, true);
               }
               AllSaveableFiles.Add(modPath, []); // empty list as it is new
               saveables = AllSaveableFiles[modPath];
            }

            // Mod Replace localisation
            if (replace)
            {
               var vanilla = AllSaveableFiles[change];

               for (var i = vanilla.Count - 1; i >= 0; i--)
               {
                  var vanillaSavable = vanilla[i];
                  if (vanillaSavable.EditingStatus != ObjEditingStatus.Modified)
                     continue;
                  // Only add savables to the mod which are changed, since we are in replace
                  saveables.Add(vanillaSavable);
                  vanillaSavable.SetPath(ref modPath);
                  vanilla.RemoveAt(i);
               }
            }
            else
            {
               // Mod modify
               saveables.AddRange(AllSaveableFiles[change]);
               // Maybe use different solution idk :P (only centralized references on paths)

               var newPath = AllSaveableFiles[change][0];
               AllSaveableFiles.Remove(change);
               newPath.Path.isModPath = true;
            }

            NeedsToBeHandledMod.Add(modPath);

         }

         // Adding entirely new obj to the mod
         if (AllSaveableFiles.TryGetValue(PathObj.Empty, out var newSaveables))
         {
            foreach (var saveable in newSaveables)
            {
               // TODO group by folders to reduce pop up count
               PathObj modPath = new(GetNewFileAt(pathForFile: saveable.GetDefaultSavePath().Path), true);

               if (!AllSaveableFiles.TryGetValue(modPath, out var saveables))
               {
                  Debug.WriteLine($"New file: {modPath.GetHashCode()}");
                  foreach (var sav in AllSaveableFiles.Keys)
                  {
                     Debug.WriteLine($"Existing: {sav.GetHashCode()}");
                  }
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

            // Save the unchanged first
            AddListToStringBuilder(ref sb, unchanged);
            sb.AppendLine($"### Modified {DateTime.Now} ###");
            AddListToStringBuilder(ref sb, changed);

            IO.WriteToFile(obj.ToPath(), sb.ToString(), false);
         }
      }

      private static void AddListToStringBuilder(ref StringBuilder sb, List<Saveable> items)
      {
         foreach (var item in items)
         {
            sb.AppendLine($"# {item.SavingComment()}");
            sb.AppendLine(item.GetSaveString(0));
         }
      }

      /// <summary>
      /// Returns a new Path for a file in the mod folder with the given path either provided by the user or a default file
      /// </summary>
      /// <param name="ending"></param>
      /// <param name="pathForFile"></param>
      /// <returns></returns>
      public static string[] GetNewFileAt(string ending = ".txt", params string[] pathForFile)
      {
         if (Globals.Settings.SavingSettings.AlwaysAskBeforeCreatingFiles)
         {
            var inputForm = new GetSavingFileForm(Path.Combine(Globals.ModPath, Path.Combine(pathForFile)), "Please enter your input:", ending);
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
               return inputForm.NewPath.Split(Path.DirectorySeparatorChar);
            }

         }
         return GetDefaultPathForFolder(ending, pathForFile);
      }
      
      private static string[] GetDefaultPathForFolder(string ending, params string[] innerPath)
      {
         // retrieve the last folder in the path
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

      public PathObj(string[] path): this(path, true)
      {

      }

      public bool ShouldReplace()
      {  
         if (isModPath)
               return false;
         for (var i = 0; i < Path.Length; i++)
         {
            if (Path[i].Equals("localisation"))
               return true;
         }
         return false;
      }

      public static PathObj FromPath(string path, bool isModPath)
      {
         var internalPath = isModPath 
            ? path.Remove(0, Globals.ModPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar) 
            : path.Remove(0, Globals.VanillaPath.Length + System.IO.Path.DirectorySeparatorChar.ToString().Length).Split(System.IO.Path.DirectorySeparatorChar);
         return new (internalPath, isModPath);
      }

      public string ToPath()
      {
         if (!isModPath)
            throw new EvilActions("Trying to overwrite the vanilla resources is Evil!");
         return System.IO.Path.Combine(Globals.ModPath, System.IO.Path.Combine(Path));
      }

      public PathObj GetInverted(bool addReplacePath)
      {
         if (addReplacePath)
         {
            var pathParts = new string[Path.Length + 1];
            Array.Copy(Path, pathParts, Path.Length - 1);
            pathParts[^2] = "replace";
            pathParts[^1] = Path[^1];
            return new(pathParts, !isModPath);
         }

         return new (Path, !isModPath);
      }

      public static PathObj Empty => new ([], true);

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