using System;
using System.Diagnostics;
using System.Text;
using Windows.Media.Playback;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;

namespace Editor.Helper
{
   public static class MissionHelper
   {
      public static List<MissionSlot> GetSlotsForFile(string fileName)
      {
         return Globals.MissionSlots.FindAll(slot => slot.File.Equals(fileName));
      }

   }

   public static class MissionLayoutEngine
   {
      /// <summary>
      /// Returns a list of missions and their positions in the file
      /// </summary>
      /// <param name="fileName"></param>
      /// <returns></returns>
      public static List<KeyValuePair<Mission, Point>>[] LayoutFile(string fileName)
      {
         var slots = MissionHelper.GetSlotsForFile(fileName);
         if (slots.Count == 0)
            return [];

         var positions = new List<KeyValuePair<Mission, Point>>[slots.Max(x => x.Slot)];

         foreach (var missionSlot in slots) 
            InsertMissionsIntoSlot(missionSlot, positions);

         return positions;
      }

      public static void InsertMissionsIntoSlot(MissionSlot slot, List<KeyValuePair<Mission, Point>>[] positions)
      {
         /* 3 cases:
          * 1. All missions are positioned
          * 2. All missions are unpositioned
          *    2.1 No prior missions in the slot
          *    2.2 Prior missions in the slot
          * 3. Some missions are positioned and some are not
          */

         var isSlotEmpty = positions[slot.Slot - 1] == null! || positions[slot.Slot - 1].Count > 0;

         var orderedMissions = slot.Missions.OrderBy(mission => mission.Position).ToList();
         var numOfUnPositioned = orderedMissions.Count(mission => mission.Position == -1);


         if (isSlotEmpty) // we have nomissions in this slot prior
         {
            if (numOfUnPositioned == orderedMissions.Count) // we have only un-positioned missions
            {
               for (var i = 0; i < orderedMissions.Count; i++) 
                  positions[slot.Slot - 1].Add(new(orderedMissions[i], new(slot.Slot - 1, i + 1)));
            }
            else // We have a mix: if there is a free slot in the positioned ones we put in one of the un-positioned ones
            {
               var leftUnPositioned = numOfUnPositioned;
               var lastPositioned = 0;

               for (var i = numOfUnPositioned - 1; i < orderedMissions.Count; i++)
               {
                  if (lastPositioned + 1 != orderedMissions[i].Position) // We have a gap in positioned missions
                  {
                     positions[slot.Slot - 1].Add(new(orderedMissions[numOfUnPositioned - leftUnPositioned], new(slot.Slot - 1, lastPositioned + 1)));
                     leftUnPositioned--;
                     lastPositioned++;
                  }
                  else
                  {
                     lastPositioned = orderedMissions[i].Position;
                     positions[slot.Slot - 1].Add(new(orderedMissions[i], new(slot.Slot - 1, lastPositioned)));
                  }
               }
            }
         }
         else
         {
            if (numOfUnPositioned == orderedMissions.Count) // we have only un-positioned missions
            {
               var lastFoundGap = 0;
               for (var i = 0; i < orderedMissions.Count; i++)
               {
                  while (positions[slot.Slot - 1][lastFoundGap].Value.Y == lastFoundGap + 1) // positions are not 0 indexed
                     lastFoundGap++; // we skip the already positioned missions

                  // we insert the un-positioned mission at the first gap we find
                  positions[slot.Slot - 1].Insert(lastFoundGap, new(orderedMissions[i], new(slot.Slot - 1, lastFoundGap + 1)));
               }
            }
            else
            {
               // insert the positioned missions first 
               for (var i = numOfUnPositioned; i < orderedMissions.Count; i++)
               {
                  if (positions[slot.Slot - 1].Any(x => x.Value.Y == orderedMissions[i].Position))
                     _ = new LoadingError(null!, $"Mission \"{orderedMissions[i].Name}\" already exists in slot \"{slot.Name}\"!", -1, -1, ErrorType.DuplicateObjectDefinition);
                  else
                  {
                     var numOfSlotsToIndex = 0;
                     for (var j = 0; j < positions[slot.Slot - 1].Count; j++)
                     {
                        if (positions[slot.Slot - 1][j].Value.Y < orderedMissions[i].Position)
                           numOfSlotsToIndex++;
                     }

                     positions[slot.Slot - 1].Insert(numOfSlotsToIndex, new(orderedMissions[i], new(slot.Slot - 1, orderedMissions[i].Position)));
                  }

               }

               // fill up with un-positioned missions
               var lastFoundGap = 0;
               for (var i = 0; i < numOfUnPositioned; i++)
               {
                  while (positions[slot.Slot - 1][lastFoundGap].Value.Y == lastFoundGap + 1) // positions are not 0 indexed
                     lastFoundGap++; // we skip the already positioned missions

                  // we insert the un-positioned mission at the first gap we find
                  positions[slot.Slot - 1].Insert(lastFoundGap, new(orderedMissions[i], new(slot.Slot - 1, lastFoundGap + 1)));
               }
            }
         }
      }
   }
}