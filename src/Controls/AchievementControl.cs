using System.Drawing.Printing;
using Editor.Controls.NewControls;
using Editor.DataClasses.Achievements;

namespace Editor.Controls
{
   public class AchievementControl : TableLayoutPanel
   {
      private const int MARGIN = 5;

      private Label _titleLabel;
      private Label _descLabel;
      private Label _progressLabel;

      private BorderedPictureBox _pb;

      private SlimProgressBar _progressBar;

      private Achievement _achievement;


      public AchievementControl(Achievement achievement, int width)
      {
         _achievement = achievement;

         _titleLabel = new()
         {
            Text = achievement.Name,
            ForeColor = Globals.Settings.Achievements.AchievementTitleColor,
            Font = new("Arial", 12, FontStyle.Bold),
            Margin = new(1, MARGIN, MARGIN, 1),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left,
            Padding = new(0),
            TextAlign = ContentAlignment.BottomLeft
         };

         _descLabel = new()
         {
            Text = achievement.Description,
            ForeColor = Globals.Settings.Achievements.AchievementDescColor,
            Font = new("Arial", 9, FontStyle.Regular),
            Margin = new(2, 1, MARGIN, 0),
            Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left,
            Padding = new(0),
            TextAlign = ContentAlignment.TopLeft,
         };

         _progressLabel = new()
         {
            ForeColor = Globals.Settings.Achievements.AchievementDescColor,
            Font = new("Arial", 10, FontStyle.Regular),
            Margin = new(1, MARGIN, 5, 1),
            Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
            Padding = new(0),
            TextAlign = ContentAlignment.BottomRight
         };


         if (achievement.Condition is ProgressCondition pg)
         {
            pg.IncreaseProgress(new Random().Next(0, (int)pg.Goal));
            _progressLabel.Text = $"{pg.CurrentProgress}/{pg.Goal}";
         }
         else
            _progressLabel.Text = string.Empty;

         _pb = new()
         {
            Image = achievement.GetIcon(),
            Dock = DockStyle.Fill,
            Margin = new(7),
            ShowBorder = true,
            BorderColor = achievement.IsAchieved ? Globals.Settings.Achievements.AchievementCompletedBorderColor : Globals.Settings.Achievements.AchievementUncompletedBorderColor,
            Padding = new(0),
            SizeMode = PictureBoxSizeMode.StretchImage,
         };

         _progressBar = new()
         {
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            Margin = new(MARGIN, MARGIN, 5, MARGIN),
            Padding = new(0),
            ForeColor = Globals.Settings.Achievements.AchievementProgressBarColor,
            BackColor = Globals.Settings.Achievements.AchievementProgressBarBackColor,
            Maximum = 100,
            Value = (int)(achievement.GetProgress() * 100),
            Width = 200,
            Height = 8,
         };

         ColumnCount = 3;
         RowCount = 2;
         Margin = new(5);
         Width = width;
         Height = 70;
         base.BackColor = Globals.Settings.Achievements.AchievementItemBackColor;


         Paint += (sender, args) =>
         {
            var p = new Pen(Color.Black, 1);
            args.Graphics.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
         };

         ColumnStyles.Add(new(SizeType.Absolute, 70));
         ColumnStyles.Add(new(SizeType.Percent, 100));
         ColumnStyles.Add(new(SizeType.Absolute, 220));

         RowStyles.Add(new(SizeType.Percent, 50));
         RowStyles.Add(new(SizeType.Percent, 50));

         Controls.Add(_pb, 0, 0);
         Controls.Add(_titleLabel, 1, 0);
         Controls.Add(_descLabel, 1, 1);
         Controls.Add(_progressLabel, 2, 0);
         Controls.Add(_progressBar, 2, 1);

         SetRowSpan(_pb, 2);
      }

   }
}