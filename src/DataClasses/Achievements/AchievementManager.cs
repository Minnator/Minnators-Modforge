using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Editor.Controls;
using Editor.ErrorHandling;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Properties;
using Editor.Saving;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Editor.DataClasses.Achievements
{

   public enum AchievementId
   {
#if DEBUG
      ExampleAchievement,
#endif
      UseForTheFirstTime,
      UsedFor1hour,
      UsedFor10hours,
      UsedFor100hours,
   }

   public enum AchievementImage
   {
      Default,
      UseForTheFirstTime,
   }

   public static class AchievementManager
   {
      private const string HMAC_KEY = "ThErAnDoMhMaCkEy";

      private static readonly Dictionary<AchievementId, Achievement> _achievements = new();
      private static readonly Color[] _achievementColors =
      [
         Color.FromArgb(255, 215, 0),
         Color.FromArgb(163, 53, 238),
         Color.FromArgb(30, 144, 255),
         Color.FromArgb(169, 169, 169)
      ];

      private static readonly Dictionary<AchievementImage, Bitmap> _achievementImages = new()
      {
         [AchievementImage.Default] = Resources.AchievementExample,
         [AchievementImage.UseForTheFirstTime] = Resources.UseForTheFirstTime,
      };

      private const string ACHIEVEMENT_FILE_NAME = "achievements.json";

      static AchievementManager()
      {
         AchievementEvents.OnAchievementCompleted += id =>
         {
            if (_achievements.TryGetValue(id, out var achievement))
               AchievementPopup.Show(achievement);
            else
               throw new EvilActions($"Achievement {achievement} does not exist in the Manager. This is an illegal state.");
         };

         GenerateAchievements();
      }

      public static bool GetImage(AchievementImage image, [NotNullWhen(true)] out Bitmap? bmp)
      {
         return _achievementImages.TryGetValue(image, out bmp);
      }

      public static void AddAchievement(Achievement achievement)
      {
         _achievements[achievement.Id] = achievement;
      }

      public static Achievement? GetAchievement(AchievementId id)
      {
         if (_achievements.TryGetValue(id, out var achievement)) 
            return achievement;
         throw new EvilActions($"Achievement {achievement} does not exist in the Manager. This is an illegal state.");
      }
      
      public static void IncreaseAchievementProgress(float amount, params AchievementId[] id)
      {
         foreach (var aId in id)
            if (_achievements.TryGetValue(aId, out var achievement))
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

      public static IEnumerable<Achievement> GetAchievements()
      {
         return _achievements.Values;
      }

      public static void ResetAchievements()
      {
         foreach (var achievement in _achievements.Values)
            achievement.Reset();
      }

      public static void GenerateAchievements()
      {
         _achievements[AchievementId.UseForTheFirstTime] = new(
                                                              AchievementId.UseForTheFirstTime,
                                                              "Swing the hammer",
                                                              "Use Minnator's Modforge for the first time",
                                                              new ProgressCondition(1, AchievementId.UseForTheFirstTime),
                                                              AchievementImage.UseForTheFirstTime,
                                                              level: 0
                                                             );

         _achievements[AchievementId.UsedFor1hour] = new (
                                                         AchievementId.UsedFor1hour,
                                                         "Getting the hang of it",
                                                         "Use Minnator's Modforge for 1 hour",
                                                         new ProgressCondition(60, AchievementId.UsedFor1hour),
                                                         AchievementImage.Default,
                                                         level: 1
                                                        );

         _achievements[AchievementId.UsedFor10hours] = new (
                                                          AchievementId.UsedFor10hours,
                                                          "Master of the Forge",
                                                          "Use Minnator's Modforge for 10 hours",
                                                          new ProgressCondition(600, AchievementId.UsedFor10hours),
                                                          AchievementImage.Default,
                                                          level: 2
                                                         );

         _achievements[AchievementId.UsedFor100hours] = new (
                                                           AchievementId.UsedFor100hours,
                                                           "The Forge is your home",
                                                           "Use Minnator's Modforge for 100 hours",
                                                           new ProgressCondition(6000, AchievementId.UsedFor100hours),
                                                           AchievementImage.Default,
                                                           level: 3
                                                          );


      }

      public static void SaveAchievements()
      {
         AchievementData data = new();
         data.SetAchievements(_achievements.Values.ToList());
         data.HMAC = GenerateHMAC(JSONWrapper.Serialize(data), HMAC_KEY);
         JSONWrapper.SaveToModforgeData(data, ACHIEVEMENT_FILE_NAME);
      }

      public static void LoadAchievements()
      {
         if (!JSONWrapper.LoadFromModforgeData<AchievementData>(ACHIEVEMENT_FILE_NAME, out var secureData))
         {
            GenerateAchievements();
            return;
         }
         
         if (secureData.HMAC == null!)
         {
            _ = new LoadingError(PathObj.Empty, "No valid achievement file found!");
            GenerateAchievements();
            return;
         }

         var savedHmac = secureData.HMAC;
         secureData.HMAC = string.Empty; 

         if (savedHmac != GenerateHMAC(JSONWrapper.Serialize(secureData), HMAC_KEY))
         {
            _ = new LoadingError(PathObj.Empty, "Achievement file has been tampered with!", level:LogType.Error);
            MessageBox.Show("The achievements file is either corrupted or has been tampered with.\nAll achievements are reset!", "Achievements Reset!");
            GenerateAchievements();
            return;
         }

         foreach (var achievement in secureData.Achievements)
         {
            var local = _achievements[achievement.Id];
            local.LoadProgress(achievement.Progress);
            local.DateAchieved = achievement.DateAchieved;
         }
      }
      
      private static string GenerateHMAC(string data, string key)
      {
         using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
         var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
         return Convert.ToBase64String(hash);
      }

      public static Achievement? GetAchievementFromIdName(string name) => _achievements.Values.FirstOrDefault(a => a.Id.ToString().Equals(name));

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

   public class AchievementData
   {
      public void SetAchievements(List<Achievement> achievements)
      {
         foreach (var achievement in achievements)
         {
            Achievements.Add(new AchievementInfo
            {
               Id = achievement.Id,
               Progress = achievement.GetProgress(),
               DateAchieved = achievement.DateAchieved
            });
         }
      }

      public List<AchievementInfo> Achievements { get; set; } = new();
      public string HMAC { get; set; } = string.Empty;
   }

   public class AchievementInfo
   {
      public AchievementId Id { get; set; }
      public float Progress { get; set; }
      public DateTime DateAchieved { get; set; }
   }

}