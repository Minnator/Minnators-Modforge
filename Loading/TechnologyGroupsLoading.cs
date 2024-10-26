﻿using System.Diagnostics;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Loading
{
   public static class TechnologyGroupsLoading
   {
      public static void Load()
      {
         var sw = Stopwatch.StartNew();
         FilesHelper.GetFileUniquely(out var content, "common", "technology.txt");
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
                  var group = new TechnologyGroup(blk.Name);
                  // TODO: Load group properties if needed
                  Globals.TechnologyGroups.Add(blk.Name, group);
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