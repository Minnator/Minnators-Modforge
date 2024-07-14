namespace Editor.Helper
{
   public static class FormHelper
   {

      public static void OpenOrBringToFront<T>(T? form) where T : Form, new()
      {
         if (form == null || form.IsDisposed)
         {
            form = new T();
            form.Show();
         }
         else
         {
            form.BringToFront();
         }
      }

   }
}