using Editor.Saving;

namespace Editor.Controls.PROPERTY;

public interface ICopyable
{
   public void SetClipboard();

   public void Paste();
}

public static class CopyPasteHandler
{
   public static bool OnMouseDown(object? sender, MouseEventArgs e, Keys modifierKeys)
   {
      if (sender is not ICopyable copyPasteControl)
         return false;

      if (e.Button == MouseButtons.Right && modifierKeys.HasFlag(Keys.Shift))
      {
         copyPasteControl.SetClipboard();
         return false;
      }
      if (e.Button == MouseButtons.Left && modifierKeys.HasFlag(Keys.Shift))
      {
         copyPasteControl.Paste();
         return true;
      }
      return false;
   }
}