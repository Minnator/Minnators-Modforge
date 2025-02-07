using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Editor.Controls;
using Editor.Properties;
using Editor.Saving;

namespace Editor.DataClasses.Achievements
{
   public enum AchievementId
   {
#if DEBUG
      ExampleAchievement,
#endif
      Create10Countries,
      Create50Countries,
      Create100Countries,
      Create250Countries,
      Edit100Provinces,
      Edit500Provinces,
      Edit1000Provinces,
      Edit5000Provinces,
   }

   public static class AchievementManager
   {
      private static readonly Dictionary<AchievementId, Achievement> _achievements = new();
      private static readonly Color[] _achievementColors =
      [
         Color.FromArgb(255, 215, 0),
         Color.FromArgb(163, 53, 238),
         Color.FromArgb(30, 144, 255),
         Color.FromArgb(169, 169, 169)
      ];

      static AchievementManager()
      {
         AchievementEvents.OnAchievementCompleted += id =>
         {
            if (_achievements.TryGetValue(id, out var achievement)) 
               Debug.WriteLine($"🎉 Achievement Unlocked: {achievement.Name}");
            else
               throw new EvilActions($"Achievement {achievement} does not exist in the Manager. This is an illegal state.");
         };

         GenerateAchievements();
      }
      public static void AddAchievement(Achievement achievement)
      {
         _achievements[achievement.Id] = achievement;
      }

      public static Achievement GetAchievement(AchievementId id)
      {
         if (_achievements.TryGetValue(id, out var achievement)) 
            return achievement;
         throw new EvilActions($"Achievement {achievement} does not exist in the Manager. This is an illegal state.");
      }

      public static void IncreaseAchievementProgress(AchievementId id, float amount)
      {
         if (_achievements.TryGetValue(id, out var achievement)) 
            achievement.IncreaseProgress(amount);
         else
            throw new EvilActions($"Achievement {achievement} does not exist in the Manager. This is an illegal state.");
      }

      public static Color GetAchievementColor(int level)
      {
         if (level < 0 || level >= _achievementColors.Length)
            throw new ArgumentOutOfRangeException(nameof(level), level, "Achievement level out of range.");
         return _achievementColors[level];
      }

      public static void GenerateAchievements()
      {
         _achievements[AchievementId.Create10Countries] = new Achievement(
            AchievementId.Create10Countries,
            "Create 10 Countries",
            "Create 10 countries in the editor.",
            new ProgressCondition(10, AchievementId.Create10Countries),
            Resources.AchievementExample,
            level: 0
         );

         _achievements[AchievementId.Create50Countries] = new Achievement(
            AchievementId.Create50Countries,
            "Create 50 Countries",
            "Create 50 countries in the editor.",
            new ProgressCondition(50, AchievementId.Create50Countries),
            Resources.AchievementExample,
            level: 1
         );

         _achievements[AchievementId.Create100Countries] = new Achievement(
            AchievementId.Create100Countries,
            "Create 100 Countries",
            "Create 100 countries in the editor.",
            new ProgressCondition(100, AchievementId.Create100Countries),
            Resources.AchievementExample,
            level: 2
         );

         _achievements[AchievementId.Create250Countries] = new Achievement(
            AchievementId.Create250Countries,
            "Create 250 Countries",
            "Create 250 countries in the editor.",
            new ProgressCondition(250, AchievementId.Create250Countries),
            Resources.AchievementExample,
            level: 3
         );

         _achievements[AchievementId.Edit100Provinces] = new Achievement(
            AchievementId.Edit100Provinces,
            "Edit 100 Provinces",
            "Edit 100 provinces in the editor.",
            new ProgressCondition(100, AchievementId.Edit100Provinces),
            Resources.AchievementExample,
            level: 0
         );

         _achievements[AchievementId.Edit500Provinces] = new Achievement(
            AchievementId.Edit500Provinces,
            "Edit 500 Provinces",
            "Edit 500 provinces in the editor.",
            new ProgressCondition(500, AchievementId.Edit500Provinces),
            Resources.AchievementExample,
            level: 1
         );

         _achievements[AchievementId.Edit1000Provinces] = new Achievement(
            AchievementId.Edit1000Provinces,
            "Edit 1000 Provinces",
            "Edit 1000 provinces in the editor.",
            new ProgressCondition(1000, AchievementId.Edit1000Provinces),
            Resources.AchievementExample,
            level: 2
         );

         _achievements[AchievementId.Edit5000Provinces] = new Achievement(
            AchievementId.Edit5000Provinces,
            "Edit 5000 Provinces",
            "Edit 5000 provinces in the editor.",
            new ProgressCondition(5000, AchievementId.Edit500Provinces),
            Resources.AchievementExample,
            level: 3
         );
      }

#if DEBUG
      public static void DebugVisualize()
      {
         var form = new Form
         {
            Text = "Achievements",
            Size = new (800, 600),
            Padding = new (10),
         };

         var panel = new FlowLayoutPanel
         {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BorderStyle = BorderStyle.FixedSingle,
            VerticalScroll = { Enabled = true},
            BackColor = Globals.Settings.Achievements.AchievementWindowBackColor,
            AutoScroll = true,
         };
         form.Controls.Add(panel);

         const int margin = 1;

         foreach (var achievement in _achievements.Values) 
            panel.Controls.Add(new AchievementControl(achievement, panel.Width - 30));


         form.ShowDialog();
      }
#endif
   }



   public static class AchievementEvents
   {
      public static event Action<AchievementId, float>? OnAchievementProgress;
      public static event Action<AchievementId>? OnAchievementCompleted;

      public static void NotifyProgress(AchievementId id, float progress)
      {
         OnAchievementProgress?.Invoke(id, progress);
      }

      public static void NotifyCompletion(AchievementId id)
      {
         AchievementManager.GetAchievement(id).SetAchieved();
         OnAchievementCompleted?.Invoke(id);
      }
   }



}