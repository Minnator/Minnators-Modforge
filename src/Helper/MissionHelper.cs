using System;
using System.Diagnostics;
using System.Text;
using Windows.Media.Playback;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Helper
{
   public static class MissionHelper
   {
      public static List<MissionSlot> GetSlotsForFile(string fileName)
      {
         return Globals.MissionSlots.FindAll(slot => slot.PathObj.GetFileName().Equals(fileName));
      }

   }

   public static class MissionLayoutEngine
   {
      public static List<MissionSlot> LayoutSlotsOfFile(string fileName)
      {
         var slots = MissionHelper.GetSlotsForFile(fileName);
         if (slots.Count == 0)
            return [];

         foreach (var missionSlot in slots)
            SetMissionPositions(missionSlot);

         return slots;
      }

      private static void SetMissionDependencies(MissionSlot slot)
      {
         foreach (var mission in slot.Missions)
            SetMissionDependencyPosition(mission, slot.PathObj);
      }

      private static void SetMissionDependencyPosition(Mission mission, PathObj po)
      {
         if (mission.RequiredMissions.Length == 0 || mission.Position != -1)
            return;

         var allReqSamePos = true;
         var reqPos = -2;
         foreach (var requiredMission in mission.RequiredMissions)
         {
            if (!Globals.Missions.TryGetValue(requiredMission, out var reqMission))
            {
               _ = new LoadingError(po, $"Mission \"{requiredMission}\" not found!", -1, -1, ErrorType.ObjectNotFound);
               allReqSamePos = false;
               continue;
            }


            var reqMissionPos = Math.Max(0, reqMission.Position);

            if (reqPos == -2)
            {
               reqPos = reqMissionPos;
            }
            else if (reqPos != reqMissionPos)
               allReqSamePos = false;
         }

         if (allReqSamePos)
            mission.Position = reqPos + 1;
         else
            _ = new ErrorObject(ErrorType.MissingMissionReference,
                                $"Mission '{mission.Name}' has required missions of different y-positions. Is this intended behavior?", level: LogType.Warning);
      }

      public static MissionView?[][] SlotsToView(List<MissionSlot> layout, MissionView.CompletionType completion, MissionView.FrameType frame)
      {
         var missionViews = new MissionView[layout.Count][];
         var rows = 0;

         // find the number of rows
         foreach (var slot in layout)
            rows = Math.Max(rows, slot.Missions.Max(mission => mission.Position));

         // Convert to 'table'
         for (var i = 0; i < layout.Count; i++)
         {
            missionViews[i] = new MissionView[rows];
            for (var j = 0; j < layout[i].Missions.Count; j++)
            {
               MissionView view = new(layout[i].Missions[j])
               {
                  Completion = completion,
                  Frame = frame
               };
               missionViews[i][layout[i].Missions[j].Position - 1] = view;
            }
         }

         return missionViews;
      }

      public static void SetMissionPositions(MissionSlot slot)
      {
         var lastPosition = 0;

         foreach (var mission in slot.Missions)
         {
            if (mission.Position == -1)
            {
               mission.Position = lastPosition + 1;
               lastPosition++;
            }
            else
            {
               lastPosition = mission.Position;
            }
         }
      }

      [Flags]
      private enum ArrowType
      {
         None = 0,
         Left = 1,
         Middle = 2,
         Center = 4,
      }

      public static void LayoutToImage(List<MissionSlot> slots, MissionView.CompletionType completion, MissionView.FrameType frame)
      {
         /* Draw Order
          * Arrows
          * MissionFrame
          * CountryShield
          */

         const int missionIconHeight = 125;
         const int missionIconWidth = 109;
         const int missionIconVSpacing = 25;
         const int missionIconHSpacing = -5;

         var layout = SlotsToView(slots, completion, frame);
         if (layout.Length == 0)
            return;

         var expImgWidth = layout.Length * missionIconWidth + (layout.Length - 1) * missionIconHSpacing;
         var expImgHeight = layout[0].Length * missionIconHeight + (layout[0].Length - 1) * missionIconVSpacing;

         var expImg = new Bitmap(expImgWidth, expImgHeight);
         using var g = Graphics.FromImage(expImg);

         // create a rect for each mission
         for (var x = 0; x < layout.Length; x++)
         {
            var xCoord = x == 0 ? 0 : x * missionIconWidth + x * missionIconHSpacing;

            for (var y = 0; y < layout[x].Length; y++)
            {
               var mission = layout[x][y];
               if (mission == null) // empty slot
                  continue;

               var yCoord = y == 0 ? 0 : y * missionIconHeight + y * missionIconVSpacing;
               var iconRect = new Rectangle(xCoord, yCoord, missionIconWidth, missionIconHeight);

               var arrows = GetOutArrowType(mission.Mission);
               DrawArrows(g, arrows, iconRect);


               using var missionBmp = mission.ExportToImage();

               g.DrawImage(missionBmp, iconRect);
            }
         }

         expImg.SaveBmpToModforgeData("missionLayout.png");
      }

      private static void DrawArrows(Graphics g, ArrowType type, Rectangle missionRect)
      {
         //var center
      }

      private static ArrowType GetOutArrowType(Mission mission)
      {
         var arrowType = ArrowType.None;
         foreach (var reqMissionName in mission.RequiredMissions)
         {
            if (!Globals.Missions.TryGetValue(reqMissionName, out var reqMission))
               continue;

            if (reqMission.Slot.Slot < mission.Slot.Slot)
               arrowType |= ArrowType.Left;
            else if (reqMission.Slot.Slot > mission.Slot.Slot)
               arrowType |= ArrowType.Center;
            else
               arrowType |= ArrowType.Middle;
         }
         return arrowType;
      }
   }
}