using Editor.Forms;
using Editor.Forms.Feature.Crash_Reporter;
using Editor.Forms.GetUserInput;

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

         

         Application.Run(new EnterPathForm());
         if (Globals.VanillaPath != string.Empty && Globals.ModPath != string.Empty)
            Application.Run(new MapWindow());
         return;
         Application.Run(new CrashReporter());

      }
   }
}
