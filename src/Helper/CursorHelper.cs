using System.Runtime.InteropServices;

namespace Editor.Helper
{
   public static class CursorHelper
   {
      public static bool SetCursor(Eu4CursorTypes type, Form form)
      {
         string file;
         switch (type)
         {
            case Eu4CursorTypes.Loading:
               if (!FilesHelper.GetVanillaPath(out file, "gfx", "cursors", "busy.ani"))
                  return false;
               form.Cursor = AdvancedCursors.Create(file);
               break;
            case Eu4CursorTypes.Normal:
               if (!FilesHelper.GetVanillaPath(out file, "gfx", "cursors", "normal.cur"))
                  return false;
               form.Cursor = new (file);
               break;
            case Eu4CursorTypes.Select:
               if (!FilesHelper.GetVanillaPath(out file, "gfx", "cursors", "dragselect.ani"))
                  return false;
               form.Cursor = AdvancedCursors.Create(file);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(type), type, null);
         }
         return true;
      }
   }
   public class AdvancedCursors
   {

      [DllImport("User32.dll")]
      private static extern IntPtr LoadCursorFromFile(String str);

      public static Cursor Create(string filename)
      {
         IntPtr hCursor = LoadCursorFromFile(filename);

         if (!IntPtr.Zero.Equals(hCursor))
         {
            return new (hCursor);
         }
         else
         {
            throw new ApplicationException("Could not create cursor from file " + filename);
         }
      }
   }
}