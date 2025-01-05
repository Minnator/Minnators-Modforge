namespace Editor.Controls
{
   public class ExtendedCheckBox : CheckBox
   {
      public readonly string PropertyName;

      public ExtendedCheckBox(string propName)
      {
         PropertyName = propName;
         Dock = DockStyle.Fill;
         Height = 21;
         AutoSize = true;
         TextAlign = ContentAlignment.MiddleLeft;
      }
   }
}