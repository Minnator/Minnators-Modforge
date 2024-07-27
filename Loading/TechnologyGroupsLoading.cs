using System.Diagnostics;
using Editor.DataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class TechnologyGroupsLoading
   {
      public static void Load(ModProject project)
      {
         var sw = Stopwatch.StartNew();
         FilesHelper.GetFileUniquely(project.ModPath, project.VanillaPath, out var content, "common", "technology.txt");

         var blocks = Parsing.GetElements(0, ref content);

         if (blocks.Count < 1)
         {
            MessageBox.Show("Error parsing technology.txt. No Nodes found. Technology groups will not be loaded.");
            return;
         }
         
         foreach (var block in blocks)
         {
            if (block is Block { Name: "groups" } block2)
            {
               foreach (var blk in block2.GetBlockElements)
               {
                  Globals.TechnologyGroups.Add(blk.Name); // We only need the names, no editing supported
               }

               sw.Stop();
               Globals.LoadingLog.WriteTimeStamp("TechnologyGroups", sw.ElapsedMilliseconds);
               return;
            }
         }

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("TechnologyGroups", sw.ElapsedMilliseconds);

         MessageBox.Show("Error parsing technology.txt. No 'groups' block found. Technology groups will not be loaded.");
      }

   }
}