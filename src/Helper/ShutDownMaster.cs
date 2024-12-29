using Editor.DataClasses.Settings;
using Editor.Saving;

namespace Editor.Helper
{
   public static class ShutDownMaster
   {
      /// <summary>
      /// returns true if it shuts down the application
      /// </summary>
      /// <returns></returns>
      /// <exception cref="ArgumentOutOfRangeException"></exception>
      public static bool DoWeShutDown()
      {
         if (SaveMaster.Cache.Count == 0)
         {
            ShutDownInternal();
            return true;
         }

         switch (Globals.Settings.Saving.SaveOnExit)
         {
            case SavingSettings.SaveOnExitType.Discard:
               ShutDownInternal();
               return true;
               break;
            case SavingSettings.SaveOnExitType.AskToSave:
               var num = SaveMaster.GetNumOfModifiedObjects();
               if (num == 0)
               {
                  ShutDownInternal();
                  return true;
               }
               switch (
                  MessageBox.Show(
                     $"You have {num} unsaved changes!\nDo you want to save them?", 
                     "Save changes", 
                     MessageBoxButtons.YesNoCancel, 
                     MessageBoxIcon.Warning)
                  )
               {
                  case DialogResult.Yes:
                     SaveMaster.SaveAllChanges();
                     ShutDownInternal();
                     return true;
                  case DialogResult.No:
                     ShutDownInternal();
                     return true;
                  case DialogResult.Cancel:
                     return false;
                  default:
                     throw new ArgumentOutOfRangeException();
               }
            case SavingSettings.SaveOnExitType.Save:
               SaveMaster.SaveAllChanges();
               ShutDownInternal();
               return true;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private static void ShutDownInternal()
      {
         ResourceUsageHelper.Dispose();
      }
   }
}