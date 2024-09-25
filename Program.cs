using System.Diagnostics;
using Editor.Forms;
using Editor.Forms.Loadingscreen;

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
      }
   }
}
