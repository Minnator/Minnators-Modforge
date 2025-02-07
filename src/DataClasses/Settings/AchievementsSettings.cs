using System.ComponentModel;

namespace Editor.DataClasses.Settings
{
   public class AchievementsSettings : SubSettings
   {
      private bool _enableAchievements = true;
      private Color _achievementWindowBackColor = Color.FromArgb(14, 20, 27);
      private Color _achievementItemBackColor = Color.FromArgb(35, 38, 46);
      private Color _achievementProgressBarColor = Color.FromArgb(26, 159, 255);
      private Color _achievementProgressBarBackColor = Color.FromArgb(61, 68, 80);
      private Color _achievementTitleColor = Color.FromArgb(220, 222, 223);
      private Color _achievementDescColor = Color.FromArgb(149, 153, 158);
      private Color _achievementUncompletedBorderColor = Color.FromArgb(253, 253, 253);
      private Color _achievementCompletedBorderColor = Color.FromArgb(195, 143, 69);

      public AchievementsSettings()
      {
#if !DEBUG
         IsAvailable = false;
#endif
      }

      [Description("Determines if achievements should be enabled.")]
      [CompareInEquals]
      public bool EnableAchievements
      {
         get => _enableAchievements;
         set => SetField(ref _enableAchievements, value);
      }

      [Description("The background color of the achievement window.")]
      [CompareInEquals]
      public Color AchievementWindowBackColor
      {
         get => _achievementWindowBackColor;
         set => SetField(ref _achievementWindowBackColor, value);
      }


      [Description("The background color of the achievement items.")]
      [CompareInEquals]
      public Color AchievementItemBackColor
      {
         get => _achievementItemBackColor;
         set => SetField(ref _achievementItemBackColor, value);
      }

      [Description("The color of the progress bar in the achievement items.")]
      [CompareInEquals]
      public Color AchievementProgressBarColor
      {
         get => _achievementProgressBarColor;
         set => SetField(ref _achievementProgressBarColor, value);
      }

      [Description("The background color of the progress bar in the achievement items.")]
      [CompareInEquals]
      public Color AchievementProgressBarBackColor
      {
         get => _achievementProgressBarBackColor;
         set => SetField(ref _achievementProgressBarBackColor, value);
      }

      [Description("The color of the title in the achievement items.")]
      [CompareInEquals]
      public Color AchievementTitleColor
      {
         get => _achievementTitleColor;
         set => SetField(ref _achievementTitleColor, value);
      }

      [Description("The color of the description in the achievement items.")]
      [CompareInEquals]
      public Color AchievementDescColor
      {
         get => _achievementDescColor;
         set => SetField(ref _achievementDescColor, value);
      }

      [Description("The color of the border of uncompleted achievements.")]
      [CompareInEquals]
      public Color AchievementUncompletedBorderColor
      {
         get => _achievementUncompletedBorderColor;
         set => SetField(ref _achievementUncompletedBorderColor, value);
      }

      [Description("The color of the border of completed achievements.")]
      [CompareInEquals]
      public Color AchievementCompletedBorderColor
      {
         get => _achievementCompletedBorderColor;
         set => SetField(ref _achievementCompletedBorderColor, value);
      }
   }
}