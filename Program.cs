using System.ComponentModel;
using System.Diagnostics;
using Editor.Forms;
using Editor.Forms.GetUserInput;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading.Enhanced;
using Editor.Saving;

namespace Editor
{
   internal static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      private static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         var po = new PathObj("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\history\\provinces\\4-Bergslagen.txt".Split("\""), false);
         IO.ReadAllInANSI(po.GetPath(), out var content);
         var sw = Stopwatch.StartNew();
         var elements = EnhancedParser.GetElements(ref po, content);
         sw.Stop();
         Debug.WriteLine($"{sw.ElapsedMilliseconds} for {elements.Count}");

         var blks = elements.Where(x => x.IsBlock).ToList();
         BindingList<EnhancedBlock> blocks = new ();
         foreach (var block in blks)
            blocks.Add((EnhancedBlock)block);
         var conts = elements.Where(x => !x.IsBlock).ToList();
         BindingList<EnhancedContent> contents = new ();
         foreach (var contentElement in conts)
            contents.Add((EnhancedContent)contentElement);
         Application.Run(new ObjectCollectionEditor<EnhancedBlock>(blocks));
         Application.Run(new ObjectCollectionEditor<EnhancedContent>(contents));

         return;

         Application.Run(new EnterPathForm());
         if (Globals.VanillaPath != string.Empty && Globals.ModPath != string.Empty)
         {
            try
            {
               Application.Run(new MapWindow());
            }
            catch (Exception e)
            {
               CrashManager.EnterCrashHandler(e);
            }
         }
      }
   }
}
