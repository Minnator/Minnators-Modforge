using Editor.Controls;
using Editor.Controls.MMF_DARK;
using Editor.DataClasses.Achievements;

namespace Editor.Forms.Feature
{
   public partial class AchievementsWindow : Form
   {
      MmfComboBox SortByDropDown = new();


      public AchievementsWindow()
      {
         InitializeComponent();

         SetTheme();
         SetAchievements();
         ActiveControl = null;
         SortByDropDown.Enabled = true;

         Load += OnFormLoad;
      }

      public void OnFormLoad(object? sender, EventArgs e) => ActiveControl = null;

      private void SetAchievements()
      {
         foreach (var achievement in AchievementManager.GetAchievements()) 
            AchievementFlowPanel.Controls.Add(new AchievementControl(achievement, AchievementFlowPanel.Width - 30));
      }

      private void SetTheme()
      {
         AchievementFlowPanel.WrapContents = false;
         AchievementFlowPanel.FlowDirection = FlowDirection.TopDown;
         AchievementFlowPanel.VerticalScroll.Enabled = true;

         MainTLP.BackColor = Globals.Settings.Achievements.AchievementWindowBackColor;
         BackColor = Globals.Settings.Achievements.AchievementWindowBackColor;
         
         SearchTextBox.BackColor = Globals.Settings.Achievements.AchievementItemBackColor;

         TitleLabel.BackColor = Globals.Settings.Achievements.AchievementWindowBackColor;
         TitleLabel.ForeColor = Globals.Settings.Achievements.AchievementTitleColor;

         MainTLP.Controls.Add(SortByDropDown, 2, 0);
         SortByDropDown.Height = 30;
         SortByDropDown.Items.AddRange(["All", "Progress", "Completed"]);
         SortByDropDown.SelectedIndex = 0;
         SortByDropDown.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
         SortByDropDown.BackColor = Globals.Settings.Achievements.AchievementItemBackColor;
         SortByDropDown.ForeColor = Globals.Settings.Achievements.AchievementTitleColor;
         SortByDropDown.Margin = new(3, 3, 28, 3);
         SortByDropDown.CornerRadius = 3;
         SortByDropDown.BorderWidth = 1;
         SortByDropDown.ShowBorder = true;
         
      }
   }
}
