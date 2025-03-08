using System.Diagnostics;
using Editor;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.CountryScope;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;

public static class Executor // Vader is operating it
{
   /*
    * DONE No Commands when running files
    * Optimize ITarget list -> after history release
    * DONE Make command take in TAG / provinceId
    * History Parsing
    *
    */

   public static void ExecuteFile(string filePath, ITarget target)
   {
      var po = PathObj.FromExternalPath(filePath);
      var content = IO.ReadAllInUTF8(filePath);

      var errorCnt = LogManager.TotalErrorCount;
      var tokens = target switch
      {
         Country => GeneralFileParser.ParseSomeFile(content, CountryScope.Scope, po),
         Province => GeneralFileParser.ParseSomeFile(content, ProvinceScopes.Scope, po),
         _ => throw new EvilActions("Not yet...")
      };
      if (errorCnt != LogManager.TotalErrorCount)
      {
         var retVal = ImprovedMessageBox.Show("Execution failed due to an error in the file. Please check the error log (F10) for more information!", "Execution failed", ref Globals.Settings.PopUps.TryContinueExecutionPopUp, MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

         if (retVal != DialogResult.Retry)
            return;

         // continue
      }

      Globals.State = State.Loading; //TODO option for compacting
      foreach (var token in tokens)
      {
         token.Activate(target);
      }
      Globals.State = State.Running;
      MapModeManager.RenderCurrent();
   }
}