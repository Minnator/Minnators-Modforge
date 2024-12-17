namespace Editor.Helper
{
   public static class FormsHelper
   {
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

      public static void ShowIfAnyOpen<T>() where T : Form, new()
      {
         if (!GetOpenForm(out T form))
            ShowForm(ref form!);
         form.BringToFront();
      }
   }
}