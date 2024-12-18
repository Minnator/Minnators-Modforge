using Editor.DataClasses.GameDataClasses;
using Windows.ApplicationModel.Appointments.DataProvider;

namespace Editor.Helper
{
   public static class FormsHelper
   {
      public static string[] GetCompletionSuggestion(ModifierValueType type)
      {
         return type switch
         {
            ModifierValueType.Bool => ["yes", "no"],
            ModifierValueType.Int => ["-5", "-4", "-3", "-2", "-1", "0", "1", "2", "3", "4", "5"],
            ModifierValueType.Float => ["-0.50", "-0.20", "-0.15", "-0.10", "-0.05", "0.00", "0.05", "0.10", "0.15", "0.20", "0.50"],
            ModifierValueType.Percentile => ["-0.04", "-0.03", "-0.02", "-0.01", "0.00", "0.01", "0.02", "0.03", "0.04"],
            _ => [""]
         };
      }

      public static bool GetOpenForm<T>(out T f) where T : Form
      {
         foreach (Form form in Application.OpenForms)
            if (form is T form1)
            {
               f = form1;
               return true;
            }
         f = null!;
         return false;
      }

      public static void ShowForm<T>(ref T? form) where T : Form, new()
      {
         if (form == null || form.IsDisposed)
         {
            form = new ();
            form.Show();
         }
         else
         {
            form.BringToFront();
         }
      }

      public static Form GetIfAnyOpen<T>() where T : Form, new()
      {
         if (GetOpenForm(out T f))
            return f;
         return new T();
      }

      public static Form ShowIfAnyOpen<T>() where T : Form, new()
      {
         var form = GetIfAnyOpen<T>();
         form.Show();
         form.BringToFront();
         return form;
      }

      public static Form ShowDialogIfAnyOpen<T>() where T : Form, new()
      {
         var form = GetIfAnyOpen<T>();
         form.ShowDialog();
         return form;
      }

      public static Form ShowIfAnyOpen<T>(Point location) where T : Form, new()
      {
         var form = GetIfAnyOpen<T>();
         form.StartPosition = FormStartPosition.Manual;
         form.Location = location;
         form.Show();
         form.BringToFront();
         return form;
      }

      public static Form ShowDialogIfAnyOpen<T>(Point location) where T : Form, new()
      {
         var form = GetIfAnyOpen<T>();
         form.StartPosition = FormStartPosition.Manual;
         form.Location = location;
         form.ShowDialog();
         return form;
      }

   }
}