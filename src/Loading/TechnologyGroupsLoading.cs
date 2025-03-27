using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class TechnologyGroupsLoading
   {
      public static void Load()
      {
         PathManager.GetFilePathUniquely(out var path, "common", "technology.txt");
         if (!IO.ReadAllInANSI(path, out var content))
         {
            Globals.ErrorLog.Write("Could not read \"technology.txt\"!");
            return;
         }

         var blocks = Parsing.GetElements(0, content);
         
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
                  var group = new TechnologyGroup(blk.Name);
                  Globals.TechnologyGroups.Add(blk.Name, group);
               }
               return;
            }
         }

         MessageBox.Show("Error parsing technology.txt. No 'groups' block found. Technology groups will not be loaded.");
      }

   }
}