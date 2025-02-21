using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using Editor.Controls;
using Editor.Forms;
using Editor.Forms.GetUserInput;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Saving;

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

#if false
         var testForm = new Form()
         {
            Size = new (800, 600),
            Text = "Test Form",
            StartPosition = FormStartPosition.CenterScreen,
            BackColor = Globals.Settings.Achievements.AchievementWindowBackColor,
         };

         var testBox = new MmfTextBox
         {
            BackColor = Globals.Settings.Achievements.AchievementItemBackColor,
            ForeColor = Globals.Settings.Achievements.AchievementDescColor,
            Margin = new(10),
            Location = new (10, 10),
            BorderWidth = 2,
            FocusMode = MmfTextBox.FocusModeEnum.Fade,
            CornerRadius = 3,
            PlaceHolderText = "Search",
         };

         var testButton = new Button()
         {
            Text = "Test",
            Location = new (10, 50),
         };

         testForm.Controls.Add(testBox);
         testForm.Controls.Add(testButton);

         Application.Run(testForm);

         return;
#endif

         Application.Run(new EnterPathForm());
         if (Globals.VanillaPath != string.Empty && Globals.ModPath != string.Empty)
         {
            try
            {
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
