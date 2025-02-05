using System.Runtime.InteropServices;

namespace Editor.Helper
{
   public static class Eu4Cursors
   {
      private static readonly Dictionary<Eu4CursorTypes, Cursor> Cursors = new();

      public static void LoadCursors()
      {
         try
         {
            if (!FilesHelper.GetVanillaPath(out var file, "gfx", "cursors", "busy.ani"))
               return;
            Cursors.TryAdd(Eu4CursorTypes.Loading, AdvancedCursors.Create(file));
            if (!FilesHelper.GetVanillaPath(out file, "gfx", "cursors", "normal.cur"))
               return;
            Cursors.TryAdd(Eu4CursorTypes.Normal, AdvancedCursors.Create(file));
            if (!FilesHelper.GetVanillaPath(out file, "gfx", "cursors", "dragselect.ani"))
               return;
            Cursors.TryAdd(Eu4CursorTypes.Select, AdvancedCursors.Create(file));
         }
         catch (ApplicationException e)
         {
            MessageBox.Show("Eu4 Cursors won't be available in the application\n" + e.Message, "Error loading Cursors", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private static bool SetCursor(Eu4CursorTypes type, Form form)
      {
         if (Cursors.TryGetValue(type, out var cursor))
         {
            form.Cursor = cursor;
            return true;
         }
         return false;
      }

      public static void SetEu4CursorIfEnabled(Eu4CursorTypes eu4Type, Cursor generic, Form? form)
      {
         if (form == null)
            return;
         if (Globals.Settings.Misc.CustomizationOptions.UseEu4Cursor)
            SetCursor(eu4Type, form);
         else
            form.Cursor = generic;
      }
   }
   public class AdvancedCursors
   {

      [DllImport("User32.dll")]
      private static extern IntPtr LoadCursorFromFile(String str);

      public static Cursor Create(string filename)
      {
         var hCursor = LoadCursorFromFile(filename);

         if (!IntPtr.Zero.Equals(hCursor))
            return new(hCursor);
         throw new ApplicationException("Could not create cursor from file " + filename);
      }
   }
}