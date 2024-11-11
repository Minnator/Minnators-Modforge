namespace Editor.Helper
{
   public static class InputHelper
   {
      public static bool GetStringIfNotEmpty(TextBox box, out string value)
      {
         value = box.Text.Trim();
         return !string.IsNullOrEmpty(value);
      }

      public static bool GetIntIfNotEmpty(TextBox box, out int value)
      {
         if (int.TryParse(box.Text, out value))
            return true;
         value = 0;
         return false;
      }

      public static bool GetIntIfNotEmpty(NumericUpDown num, out int value)
      {
         value = (int)num.Value;
         return true;
      }
   }
}