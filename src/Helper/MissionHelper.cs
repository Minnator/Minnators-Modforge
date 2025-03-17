using System;
using System.Diagnostics;
using System.Text;
using Windows.Media.Playback;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Properties;
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

         if (layout.Count == 0)
            return [];

         var missionViews = new MissionView[layout.Max(x => x.Slot)][];
         var rows = 0;

         // find the number of rows
         foreach (var slot in layout)
            if (slot.Missions.Count > 0)
               rows = Math.Max(rows, slot.Missions.Max(mission => mission.Position));

         // Convert to 'table'
         for (var i = 0; i < missionViews.Length; i++)
         {
            missionViews[i] = new MissionView[rows];
            var slot = layout.Find(x => x.Slot == i + 1);
            if (slot == null)
               continue;

            for (var j = 0; j < slot.Missions.Count; j++)
            {
               MissionView view = new(slot.Missions[j])
               {
                  Completion = completion,
                  Frame = frame,
                  HasCountryShield = slot.HasCountryShield
               };
               missionViews[i][slot.Missions[j].Position - 1] = view;
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
         Right = 2,
         Center = 4,
      }
      
      const int MISSION_ICON_HEIGHT = 125;
      const int MISSION_ICON_WIDTH = 109;
      const int MISSION_ICON_V_SPACING = 25;
      const int MISSION_ICON_H_SPACING = -5;

      private const int MISSION_ROW_HEIGHT_VSP = MISSION_ICON_HEIGHT + MISSION_ICON_V_SPACING;

      public static Bitmap DrawMissionOnBackGround(Bitmap missionBmp, bool squared, bool showMissionNames, List<MissionSlot> slots, Country missionOwner) 
         // if squared we only draw the square part of the bg image
         // if not we draw the entire window with close button and outlines
      {
         if (slots.Count == 0)
            return new (2, 2);
         
         var numOfColumns = slots.Max(x => x.Slot);
         var numOfRows = slots.Max(x => x.Missions.Count == 0 ? 0 : x.Missions.Max(m => m.Position));
         return DrawMissionOnBackGround(missionBmp, squared, showMissionNames, numOfRows, numOfColumns, missionOwner);
      }      
      
      public static Bitmap DrawMissionOnBackGround(Bitmap missionBmp, bool squared, bool showMissionNames, int numOfRows, int numOfColumns, Country missionOwner) 
         // if squared we only draw the square part of the bg image
         // if not we draw the entire window with close button and outlines
      {
         var background = GetBackgroundTrimmed(squared, showMissionNames, numOfRows, numOfColumns, $"{missionOwner.AdjectiveLocalisation} Missions", out var offset);

         using var g = Graphics.FromImage(background);
         g.DrawImage(missionBmp, offset);

         return background;
      }

      private static Bitmap GetBackgroundTrimmed(bool squared, bool showMissionNames, int numOfRows, int numOfColumns, string missionText, out Point offset)
      {
         Rectangle rect;
         var source = GameIconDefinition.GetIcon(GameIcons.MissionViewHuge);
         var missionWidth = numOfColumns * MISSION_ICON_WIDTH + (numOfColumns - 1) * MISSION_ICON_H_SPACING;
         var missionHeight = numOfRows * MISSION_ICON_HEIGHT + (numOfRows - 1) * MISSION_ICON_V_SPACING;
         int textYOffset;
         
         if (squared)
         {
            if (showMissionNames)
            {
               rect = new(14, 138 - 51, 535 + 12, 744 + 4 + 44); // 4 / 12 is the border 44 is the space for the mission names
               offset = new((rect.Width - missionWidth) / 2, 46 + (rect.Height - 46 - missionHeight + (numOfRows - 5) * MISSION_ROW_HEIGHT_VSP) / 2); // 44 is the space for the mission names
               textYOffset = 5;
            }  
            else
            {
               rect = new(14, 131, 535 + 12, 744 + 4); // 4 / 12 is the border
               offset = new((rect.Width - missionWidth) / 2, 2 + (rect.Height - 2 - missionHeight + (numOfRows - 5) * MISSION_ROW_HEIGHT_VSP) / 2);
               textYOffset = -200; // no text so we don't care
            }
         }
         else
         {
            rect = new(2, 5, source.Width - 15, source.Height - 17);
            offset = new(9 + (rect.Width - 9 - missionWidth) / 2, 125 + (rect.Height - 125 - 37 - missionHeight + (numOfRows - 5) * MISSION_ROW_HEIGHT_VSP) / 2);
            textYOffset = 87;
         }

         Bitmap background;
         Graphics g;

         if (numOfRows <= 5)
         {
            background = new (rect.Width, rect.Height);
            g = Graphics.FromImage(background);
            g.DrawImage(source, rect with { X = 0, Y = 0 }, rect, GraphicsUnit.Pixel);
         }
         else
         {
            background = new(rect.Width, rect.Height + (numOfRows - 5) * MISSION_ROW_HEIGHT_VSP);
            g = Graphics.FromImage(background);

            // we draw the top part
            // then we insert a tileable part numofrows - 5 times
            // then we draw the bottom part

            var tilePart = Resources.MissionRowSlotTile ?? new (577, 150);
            var topPart = rect with { Height = rect.Height - MISSION_ROW_HEIGHT_VSP };
            var bottomPart = rect with { Y = rect.Bottom - MISSION_ROW_HEIGHT_VSP, Height = MISSION_ROW_HEIGHT_VSP};

            g.DrawImage(source, topPart with { X = 0, Y = 0 }, topPart, GraphicsUnit.Pixel);

            for (var i = 0; i < numOfRows - 5; i++)
            {
               var y = topPart.Height + i * MISSION_ROW_HEIGHT_VSP;
               var tRect = new Rectangle(0, y, tilePart.Width, tilePart.Height);
               if (squared)
                  g.DrawImage(tilePart, tRect, new(0, 0, tilePart.Width, tilePart.Height), GraphicsUnit.Pixel);
               else
                  g.DrawImage(tilePart, tRect with {X = 12}, new(0, 0, tilePart.Width, tilePart.Height), GraphicsUnit.Pixel);
            }

            g.DrawImage(source, bottomPart with { X = 0, Y = background.Height - bottomPart.Height }, bottomPart, GraphicsUnit.Pixel);
         }



         // Draw mission text if needed
         if (showMissionNames)
         {
            var font = new Font("vic_18", 16, FontStyle.Regular);
            var textLength = MissionView.MeasureTextWidth(g, missionText, font);
            g.DrawString(missionText, font, Brushes.Beige, new Point((background.Width - textLength) / 2, textYOffset + 3));
         }

         if (!squared)
         {
            var close = GameIconDefinition.GetIcon(GameIcons.CloseButton);
            g.DrawImage(close, new Point(520, 5));
         }

         g.Dispose();

         return background;
      }

      public static Bitmap LayoutSlotsToTransparentImage(List<MissionSlot> slots, MissionView.CompletionType completion, MissionView.FrameType frame, Country country)
      {
         /* Draw Order
          * Arrows
          * MissionFrame
          * CountryShield
          */
         var layout = SlotsToView(slots, completion, frame);
         if (layout.Length == 0)
            return new (2, 2);

         var expImgWidth = layout.Length * MISSION_ICON_WIDTH + (layout.Length - 1) * MISSION_ICON_H_SPACING;
         var expImgHeight = layout[0].Length * MISSION_ICON_HEIGHT + (layout[0].Length - 1) * MISSION_ICON_V_SPACING;

         var expImg = new Bitmap(expImgWidth, expImgHeight);
         using var g = Graphics.FromImage(expImg);

         // create a rect for each mission
         for (var x = 0; x < layout.Length; x++)
         {
            var xCoord = x == 0 ? 0 : x * MISSION_ICON_WIDTH + x * MISSION_ICON_H_SPACING;

            for (var y = 0; y < layout[x].Length; y++)
            {
               var mission = layout[x][y];
               if (mission == null) // empty slot
                  continue;

               var yCoord = y == 0 ? 0 : y * MISSION_ICON_HEIGHT + y * MISSION_ICON_V_SPACING;
               var iconRect = new Rectangle(xCoord, yCoord, MISSION_ICON_WIDTH, MISSION_ICON_HEIGHT);

               // first draw all arrows
               DrawArrows(g, iconRect, GetArrows(mission.Mission));
            }
         }

         // create a rect for each mission
         for (var x = 0; x < layout.Length; x++)
         {
            var xCoord = x == 0 ? 0 : x * MISSION_ICON_WIDTH + x * MISSION_ICON_H_SPACING;

            for (var y = 0; y < layout[x].Length; y++)
            {
               var mission = layout[x][y];
               if (mission == null) // empty slot
                  continue;

               var yCoord = y == 0 ? 0 : y * MISSION_ICON_HEIGHT + y * MISSION_ICON_V_SPACING;
               var iconRect = new Rectangle(xCoord, yCoord, MISSION_ICON_WIDTH, MISSION_ICON_HEIGHT);

               // draw the mission frame
               using var missionBmp = mission.ExportToImage();
               g.DrawImage(missionBmp, iconRect);

               // draw the country shield
               using var flag = country.GetFlagBitmap();
               using var flagMasked = BmpLoading.ApplyMask(flag, GameIconDefinition.GetIcon(GameIcons.MissionFlagMask));
               var overlay = GameIconDefinition.GetIcon(GameIcons.SmallShieldOverlay);

               g.DrawImage(flagMasked, new Point(iconRect.X + (iconRect.Width - flagMasked.Width) / 2, iconRect.Top + 3));
               g.DrawImage(overlay, new Point(iconRect.X + (iconRect.Width - overlay.Width) / 2, iconRect.Top));
            }
         }

         return expImg;
      }



      // Returns the arrow type for the mission with x and y offset in missions with 1 being just above or next to the mission
      private static (ArrowType, int, int)[] GetArrows(Mission mission)
      {
         if (mission.RequiredMissions.Length == 0)
            return [];

         var arrows = new (ArrowType, int, int)[mission.RequiredMissions.Length];
         for (var i = 0; i < mission.RequiredMissions.Length; i++)
         {
            if (!Globals.Missions.TryGetValue(mission.RequiredMissions[i], out var reqMission))
               continue;

            var x = Math.Abs(reqMission.Slot.Slot - mission.Slot.Slot);
            var y = Math.Abs(reqMission.Position - mission.Position);
            var arrowType = ArrowType.None;
            if (mission.Slot.Slot > reqMission.Slot.Slot)
               arrowType |= ArrowType.Left;
            else if (mission.Slot.Slot < reqMission.Slot.Slot)
               arrowType |= ArrowType.Right;
            else
               arrowType |= ArrowType.Center;

            arrows[i] = (arrowType, x, y);
         }

         return arrows;
      }


      private static void DrawArrows(Graphics g, Rectangle missionRect, (ArrowType, int, int)[] arrows)
      {
         /* There are always 4 pieces of a right/left arrow:
          *    - out_part
          *    - skip_part/tile_part -- only used the skip part no idea what the other is for
          *    - in_part
          *    - arrow_end
          * The center arrow has only 2 parts:
          *    - vertical_tile
          *    - arrow_end
          */

         if (arrows.Length == 0)
            return;

         var centerLine = (int)Math.Round(missionRect.X + missionRect.Width / 2.0);
         var arrowEnd = GameIconDefinition.GetIcon(GameIcons.ArrowEnd);

         foreach (var (type, xOffset, yOffset) in arrows)
         {
            switch (type)
            {
               case ArrowType.Left:
                  var outRightImage = GameIconDefinition.GetIcon(GameIcons.ArrowRightOut);
                  var inRightImage = GameIconDefinition.GetIcon(GameIcons.ArrowRightIn);

                  // draw in image first
                  g.DrawImage(inRightImage, new Point(centerLine - 60, missionRect.Top - MISSION_ICON_V_SPACING - 2));
                  // draw out image
                  var x = centerLine - 54 - inRightImage.Width - xOffset * MISSION_ICON_H_SPACING - (xOffset - 1) * missionRect.Width;
                  g.DrawImage(outRightImage, new Point(x, 
                                                  missionRect.Top - MISSION_ICON_V_SPACING - 8));

                  if (xOffset > 1)
                  {
                     // draw horizontal skip
                     var skipImage = GameIconDefinition.GetIcon(GameIcons.ArrowHorizontalSkipSlot);
                     for (var i = 1; i < xOffset; i++)
                        g.DrawImage(skipImage, new Point(x - 21 + inRightImage.Width + (i - 1) * 124, missionRect.Top - MISSION_ICON_V_SPACING - 2));
                  }
                  g.DrawImage(arrowEnd, new Point(centerLine - arrowEnd.Width / 2 - 24, missionRect.Top - MISSION_ICON_V_SPACING + 12));
                  break;
               case ArrowType.Right:
                  var outLeftImage = GameIconDefinition.GetIcon(GameIcons.ArrowLeftOut);
                  var inLeftImage = GameIconDefinition.GetIcon(GameIcons.ArrowLeftIn);

                  // draw in image first
                  g.DrawImage(inLeftImage, new Point(centerLine + 60 - inLeftImage.Width, missionRect.Top - MISSION_ICON_V_SPACING - 1));
                  // draw out image
                  var xL = centerLine + 54 + xOffset * MISSION_ICON_H_SPACING + (xOffset - 1) * missionRect.Width;
                  g.DrawImage(outLeftImage, new Point(xL, missionRect.Top - MISSION_ICON_V_SPACING - 7));

                  if (xOffset > 1)
                  {
                     // draw horizontal skip
                     var skipImage = GameIconDefinition.GetIcon(GameIcons.ArrowHorizontalSkipSlot);
                     for (var i = 1; i < xOffset; i++)
                     {
                        var xLl = xL + 21 - skipImage.Width + (i - 1) * -124;
                        g.DrawImage(skipImage, new Point(xLl, missionRect.Top - MISSION_ICON_V_SPACING - 1));
                     }
                  }
                  g.DrawImage(arrowEnd, new Point(centerLine - arrowEnd.Width / 2 + 23, missionRect.Top - MISSION_ICON_V_SPACING + 12));

                  break;
               case ArrowType.Center: // works
                  if (yOffset >= 1)
                  {
                     // draw vertical tile
                     var vertImage = GameIconDefinition.GetIcon(GameIcons.ArrowVerticalTile);
                     g.DrawImage(vertImage, new Point(centerLine - vertImage.Width / 2, missionRect.Top - MISSION_ICON_V_SPACING - 8));
                  }
                  if (yOffset > 1)
                  {
                     // draw vertical skip
                     var vertImage = GameIconDefinition.GetIcon(GameIcons.ArrowVerticalSkipTier);
                     for (var i = 1; i < yOffset; i++)
                        g.DrawImage(vertImage, new Point(centerLine - vertImage.Width / 2, missionRect.Top - MISSION_ICON_V_SPACING - 8 - i * 152));
                  }
                  if (yOffset < 1)
                  {
                     _ = new ErrorObject(ErrorType.MissingMissionReference, "Mission has a negative y offset!", level: LogType.Warning);
                     continue;
                  }
                  g.DrawImage(arrowEnd, new Point(centerLine - arrowEnd.Width / 2, missionRect.Top - MISSION_ICON_V_SPACING + 12));
                  break;
               case ArrowType.None:
                  throw new EvilActions("None should never be put in layout here!");
            }
         }
      }

   }
}