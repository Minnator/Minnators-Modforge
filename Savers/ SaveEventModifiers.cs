using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms.SavingClasses;
using Editor.Helper;
using Editor.Interfaces;

namespace Editor.Savers;

public static class EventModifierSaver
{
   public static void SaveEventModifiers()
   {
      var path = Path.Combine(Globals.ModPath, "common", "event_modifiers");
      List<ISaveable> allModifiers = [];
      allModifiers.AddRange(Globals.EventModifiers.Values);

      var newModifiers = EditingHelper.GetObjectsWithStatus(allModifiers, ObjEditingStatus.New);
      var modifiedModifiers = EditingHelper.GetObjectsWithStatus(allModifiers, ObjEditingStatus.Modified);

      Dictionary<int, List<EventModifier>> modifiedFiles = []; // contains all modified files and their modifiers
      Dictionary<int, List<EventModifier>> unchangedModifiers = []; // contains all unchanged modifiers of the modified files
      foreach (var modMod in modifiedModifiers)
      {
         if (!modifiedFiles.TryAdd(modMod.FileIndex, [(EventModifier)modMod])) 
            modifiedFiles[modMod.FileIndex].Add((EventModifier)modMod);
      }

      foreach (var modMod in modifiedFiles)
         unchangedModifiers.Add(modMod.Key, ObjectSourceFiles.GetAllElementsOfFile<EventModifier>(modMod.Key, allModifiers));

      var newFileName = string.Empty;
      if (newModifiers.Count > 0)
      {
         if (Globals.Settings.SavingSettings.AlwaysAskBeforeCreatingFiles)
         {
            var inputForm = new GetSavingFile(Path.Combine(Globals.ModPath, "common", "event_modifiers"), "Please enter your input:");
            if (inputForm.ShowDialog() == DialogResult.OK)
               newFileName = inputForm.NewPath;
         }
         else
            newFileName = IO.GetDefaultPathForFolder("common", "event_modifiers");

         SaveEventModifiersToFile(newModifiers.Cast<EventModifier>().ToList(), [], newFileName);
      }

      if (modifiedFiles.Count > 0)
      {
         foreach (var modFile in modifiedFiles)
         {
            if (ObjectSourceFiles.GetFileFromIndex(modFile.Key, out var fileName))
            {
               var fName = Path.Combine(Globals.ModPath, fileName);
               if (File.Exists(fName))
               {
                  SaveEventModifiersToFile(modifiedFiles[modFile.Key], unchangedModifiers[modFile.Key], fName);
               }
            }
         }
      }

   }

   private static void SaveEventModifiersToFile(List<EventModifier> changed, List<EventModifier> unchanged, string newFileLocation)
   {
      var sb = new StringBuilder();
      var oldModifiers = unchanged.Except(changed).ToList(); // get all unchanged modifiers

      WriteUnchanged(ref sb, ref oldModifiers);
      WriteChanged(ref sb, ref changed);
      
      IO.WriteToFile(newFileLocation, sb.ToString(), false);
   }

   private static void WriteChanged(ref StringBuilder sb, ref List<EventModifier> changed)
   {
      sb.AppendLine($"### Modified: {DateTime.UtcNow} UTC ###");
      sb.AppendLine();

      foreach (var mod in changed)
      {
         sb.AppendLine($"# {Localisation.GetLoc(mod.Name)}");
         sb.AppendLine(mod.GetFormattedString());
      }
   }

   private static void WriteUnchanged(ref StringBuilder sb, ref List<EventModifier> oldModifiers)
   {
      foreach (var mod in oldModifiers)
      {
         sb.AppendLine($"# {Localisation.GetLoc(mod.Name)}");
         sb.AppendLine(mod.GetFormattedString());
      }
   }
}
