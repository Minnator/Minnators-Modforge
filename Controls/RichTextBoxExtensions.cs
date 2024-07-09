namespace Editor.Controls
{
   public static class RichTextBoxExtensions
   {
      public static void AppendText(this RichTextBox box, string text, Color color)
      {
         box.SelectionStart = box.TextLength;
         box.SelectionLength = 0;

         box.SelectionColor = color;
         box.AppendText(text);
         box.SelectionColor = box.ForeColor;
      }

      public static void WriteError(this RichTextBox box, string text)
      {
         box.SelectionStart = box.TextLength;
         box.SelectionLength = 0;

         box.SelectionColor = Color.Red;
         box.AppendText($"Error: {text}");
         box.SelectionColor = box.ForeColor;
      }

      public static void Write(this RichTextBox box, string text)
      {
         box.AppendText($"-> {text}");
      }

      public static void WriteLine(this RichTextBox box, string text)
      {
         box.AppendText($"{text}\n");
      }
   }
}