namespace Editor.Helper
{
   public static class FormHelper
   {

      public static T OpenOrBringToFront<T>(T? form) where T : Form, new()
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
         return form;
      }

   }
}