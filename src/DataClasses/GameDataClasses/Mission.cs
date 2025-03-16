using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses;

public class MissionView(Mission mission)
{
   public enum FrameType
   {
      Locked,
      Unlocked,
      Completed
   }

   public enum CompletionType
   {
      NotStarted,
      Started,
      Completed
   }

   public bool HasCountryShield { get; set; } = false;
   public Mission Mission { get; set; } = mission;
   public FrameType Frame { get; set; } = FrameType.Completed;
   public CompletionType Completion { get; set; } = CompletionType.NotStarted;

   public Bitmap ExportToImage()
   {
      var backGround = GameIconDefinition.GetIcon(GetFrameType(this));
      var effect = GetCompletionIcon(this);
      var trigger = GameIconDefinition.GetIcon(GameIcons.MissionTrigger);
      var missionIcon = GetMissionIcon(this);

      var image = new Bitmap(backGround.Width, backGround.Height, PixelFormat.Format32bppArgb);
      using var g = Graphics.FromImage(image);

      g.Clear(Color.Transparent);
      //g.CompositingMode = CompositingMode.SourceCopy;  // Preserve alpha

      g.DrawImage(missionIcon, 25, 20);
      g.DrawImage(backGround, 0, 0);

      g.DrawImage(effect, 73, 7);
      g.DrawImage(trigger, 0, 4);

      // Draw the mission text
      // check if the font Vic18 is available
      var font = new Font("vic_18", 10, FontStyle.Regular);
      // we have 84 pixels per line, we split at whole words
      const int maxCharsPerLine = 88;

      List<string> lines = [];
      List<int> lineWidths = [];

      var words = Mission.TitleLocalisation.Split(' ');
      var line = new StringBuilder();
      var lineWidth = 0;

      for (var i = 0; i < words.Length; i++)
      {
         var word = words[i];
         var wordWidth = MeasureTextWidth(g, word, font);
         // the word fits in the current line, we try to add another one
         if (wordWidth + lineWidth < maxCharsPerLine)
         {
            if (line.Length > 0)
            {
               line.Append(' ');
               line.Append(word);
               lineWidth = MeasureTextWidth(g, line.ToString(), font);
            }
            else
            {
               line.Append(word);
               lineWidth += wordWidth;
            }
            continue;
         }

         // the word does not fit but would fit in an empty line, we add the line and start a new one
         if (wordWidth < maxCharsPerLine)
         {
            lines.Add(line.ToString());
            lineWidths.Add(lineWidth);
            line.Clear();
            lineWidth = 0;
            i--;
            continue;
         }

         // the word is too long for a single line, we split it up
         var chars = word.ToCharArray();
         foreach (var c in chars)
         {
            var charWidth = MeasureTextWidth(g, c.ToString(), font);
            if (charWidth + lineWidth < maxCharsPerLine)
            {
               line.Append(c);
               lineWidth += charWidth;
            }
            else
            {
               lines.Add(line.ToString());
               lineWidths.Add(lineWidth);
               line.Clear();
               lineWidth = 0;
            }
         }
      }

      lines.Add(line.ToString());
      lineWidths.Add(lineWidth);

      for (var i = 0; i < lines.Count; i++)
      {
         TextRenderer.DrawText(g, lines[i], font, new Point(7 + (int)Math.Round((maxCharsPerLine - lineWidths[i]) / 2.0), 83 + i * 19), Color.Beige);
      }

      return image;
   }



   public static int MeasureTextWidth(Graphics graphics, string text, Font font)
   {
      var format = new StringFormat();
      var rect = new RectangleF(0, 0, 1000, 1000);
      CharacterRange[] ranges = [new (0, text.Length)];
      format.SetMeasurableCharacterRanges(ranges);

      var regions = graphics.MeasureCharacterRanges(text, font, rect, format);
      if (regions.Length == 0)
      {
         return 0; // or handle the error appropriately
      }

      rect = regions[0].GetBounds(graphics);

      return (int)(rect.Right + 1.0f);
   }

   public static GameIcons GetFrameType(MissionView missionView)
   {
      return missionView.Completion switch
      {
         CompletionType.Completed => GameIcons.MissionIconFrameComplete,
         CompletionType.Started => GameIcons.MissionIconFrame,
         _ => GameIcons.MissionIconFrameLocked
      };
   }

   public static Bitmap GetCompletionIcon(MissionView missionView)
   {
      var definition = GameIconDefinition.GetIconDefinition(GameIcons.MissionEffect);
      Debug.Assert(definition is GameIconStrip, "definition is GameIconStrip strip");
      return ((GameIconStrip)definition).IconStrip[
                                                   missionView.Completion switch
                                                   {
                                                      CompletionType.Completed => 2,
                                                      CompletionType.Started => 0,
                                                      _ => 1
                                                   }
                                                  ];
   }

   public static Bitmap GetMissionIcon(MissionView missionView)
   {
      if (Globals.SpriteTypes.TryGetValue(missionView.Mission.Icon, out var spriteType))
         return spriteType.Icon;
      return GameIconDefinition.GetIcon(GameIcons.MissionPlaceHolder);
   }
}

public class Mission(string name, MissionSlot slot) : ITitleAdjProvider
{
   public int Position { get; set; } = -1;
   public string Name { get; } = name;
   public string[] RequiredMissions { get; set; } = [];
   public string Icon { get; set; } = string.Empty;

   public MissionSlot Slot { get; set; } = slot;
   
   public string TitleKey => $"{Name}_title";
   public string TitleLocalisation => Localisation.GetLoc(TitleKey);
   public string AdjectiveKey => $"{Name}_desc";
   public string AdjectiveLocalisation => Localisation.GetLoc(AdjectiveKey);
   public override string ToString() => $"[{Position}] {Name}";
   public override int GetHashCode() => Name.GetHashCode();
   public override bool Equals(object? obj) => obj is Mission mission && mission.Name == Name;
}

public class MissionSlot(string name, PathObj po)
{
   public string Name { get; } = name;
   public int Slot { get; set; } = -1;
   public bool IsGeneric { get; set; } = false;
   public bool HasCountryShield { get; set; } = false;
   public List<Mission> Missions { get; set; } = [];
   public PathObj PathObj { get; } = po;

   public override int GetHashCode() => Name.GetHashCode();
   public override bool Equals(object? obj) => obj is MissionSlot slot && slot.Name == Name;
   public override string ToString() => Name;
}