using Editor.DataClasses.Commands;
using Editor.Forms;
using Editor.Forms.GetUserInput;
using Editor.Helper;

namespace Editor
{
   internal static class Program
   {

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      private static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         var filter = new ThumbButtonFilter();
         filter.OnXButton1 += shift => HistoryManager.Undo(shift);
         filter.OnXButton2 += shift => HistoryManager.Redo(shift);
         
         Application.Run(new EnterPathForm());
         if (Globals.VanillaPath != string.Empty && Globals.ModPath != string.Empty)
         {
            try
            {
               Application.AddMessageFilter(filter);
               Application.Run(new MapWindow());
            }
            catch (Exception e)
            {
               CrashManager.EnterCrashHandler(e);
            }
         }
      }
   }
}
