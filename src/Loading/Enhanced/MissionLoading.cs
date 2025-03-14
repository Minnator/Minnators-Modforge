using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static class MissionLoading
   {
      
      public static void LoadMissions()
      {
         HashSet<string> missionNames = [];
         HashSet<string> slotNames = [];


         var files = FilesHelper.GetAllFilesInFolder(searchPattern: "*.txt", "missions");
         
         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);

            var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

            foreach (var block in blocks)
            {
               var slot = EnhancedParsing.GetMissionSlotFromBlock(block, po);
               if (!slotNames.Add(slot.Name))
               {
                  _ = new LoadingError(po, $"Mission slot \"{block.Name}\" already exists!", block.StartLine, -1, ErrorType.DuplicateObjectDefinition);
                  continue;
               }

               Globals.MissionSlots.Add(slot);
               foreach (var missionName in slot.Missions)
                  if (!missionNames.Add(missionName.Name)) 
                     _ = new LoadingError(po, $"Mission \"{missionName}\" already exists!", block.StartLine, -1, ErrorType.DuplicateObjectDefinition);
            }
         }

         var exampleView = new MissionView(Globals.MissionSlots[12].Missions[0]);
         var bmp = exampleView.ExportToImage();
         bmp.SaveBmpToModforgeData("missionExample.png");
      }
   }
}