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
         HashSet<string> slotNames = [];


         var files = FilesHelper.GetAllFilesInFolder(searchPattern: "*.txt", "missions");
         
         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);

            var elements = po.LoadBaseOrder();

            foreach (var element in elements)
            {
               if (element is not EnhancedBlock block) 
                  continue;

               var slot = EnhancedParsing.GetMissionSlotFromBlock(block, po);
               if (!slotNames.Add(slot.Name))
               {
                  _ = new LoadingError(po, $"Mission slot \"{block.Name}\" already exists!", block.StartLine, -1, ErrorType.DuplicateObjectDefinition);
                  continue;
               }
               Globals.MissionSlots.Add(slot);
               foreach (var missionName in slot.Missions)
                  if (Globals.Missions.TryAdd(missionName.Name, missionName)) 
                     _ = new LoadingError(po, $"Mission \"{missionName}\" already exists!", block.StartLine, -1, ErrorType.DuplicateObjectDefinition);
            }
         }
      }
   }
}