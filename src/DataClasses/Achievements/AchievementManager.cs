﻿using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Editor.Controls;
using Editor.ErrorHandling;
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
      Create10Countries,
      Create50Countries,
      Create100Countries,
      Create250Countries,
      Edit100Provinces,
      Edit500Provinces,
      Edit1000Provinces,
      Edit5000Provinces,
   }

   public enum AchievementImage
   {
      Default,
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
      };

      private static string FilePath => Path.Combine(Globals.AppDirectory, "achievements.json");

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

      public static bool GetImage(AchievementImage image, [NotNullWhen(true)] out Bitmap? bmp)
      {
         return _achievementImages.TryGetValue(image, out bmp);
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

      public static IEnumerable<Achievement> GetAchievements()
      {
         return _achievements.Values;
      }

      public static void GenerateAchievements()
      {
         _achievements[AchievementId.Create10Countries] = new(
                                                              AchievementId.Create10Countries,
                                                              "Create 10 Countries",
                                                              "Create 10 countries in the editor.",
                                                              new ProgressCondition(10, AchievementId.Create10Countries),
                                                              AchievementImage.Default,
                                                              level: 0
                                                             );

         return;

         _achievements[AchievementId.Create50Countries] = new(
                                                              AchievementId.Create50Countries,
                                                              "Create 50 Countries",
                                                              "Create 50 countries in the editor.",
                                                              new ProgressCondition(50, AchievementId.Create50Countries),
                                                              AchievementImage.Default,
                                                              level: 1
                                                             );

         _achievements[AchievementId.Create100Countries] = new(
                                                               AchievementId.Create100Countries,
                                                               "Create 100 Countries",
                                                               "Create 100 countries in the editor.",
                                                               new ProgressCondition(100, AchievementId.Create100Countries),
                                                               AchievementImage.Default,
                                                               level: 2
                                                              );

         _achievements[AchievementId.Create250Countries] = new(
                                                               AchievementId.Create250Countries,
                                                               "Create 250 Countries",
                                                               "Create 250 countries in the editor.",
                                                               new ProgressCondition(250, AchievementId.Create250Countries),
                                                               AchievementImage.Default,
                                                               level: 3
                                                              );

         _achievements[AchievementId.Edit100Provinces] = new(
                                                             AchievementId.Edit100Provinces,
                                                             "Edit 100 Provinces",
                                                             "Edit 100 provinces in the editor.",
                                                             new ProgressCondition(100, AchievementId.Edit100Provinces),
                                                             AchievementImage.Default,
                                                             level: 0
                                                            );

         _achievements[AchievementId.Edit500Provinces] = new(
                                                             AchievementId.Edit500Provinces,
                                                             "Edit 500 Provinces",
                                                             "Edit 500 provinces in the editor.",
                                                             new ProgressCondition(500, AchievementId.Edit500Provinces),
                                                             AchievementImage.Default,
                                                             level: 1
                                                            );

         _achievements[AchievementId.Edit1000Provinces] = new(
                                                              AchievementId.Edit1000Provinces,
                                                              "Edit 1000 Provinces",
                                                              "Edit 1000 provinces in the editor.",
                                                              new ProgressCondition(1000, AchievementId.Edit1000Provinces),
                                                              AchievementImage.Default,
                                                              level: 2
                                                             );

         _achievements[AchievementId.Edit5000Provinces] = new(
                                                              AchievementId.Edit5000Provinces,
                                                              "Edit 5000 Provinces",
                                                              "Edit 5000 provinces in the editor.",
                                                              new ProgressCondition(5000, AchievementId.Edit500Provinces),
                                                              AchievementImage.Default,
                                                              level: 3
                                                             );
      }

      public static void SaveAchievements()
      {
         AchievementData data = new()
         {
            Achievements = _achievements.Values.ToList()
         };
         
         var settings = new JsonSerializerSettings
         {
            Converters = GetConverters(),
            Formatting = Formatting.Indented
         };

         data.HMAC = GenerateHMAC(JsonConvert.SerializeObject(data, Formatting.Indented), HMAC_KEY);

         File.WriteAllText(FilePath, JsonConvert.SerializeObject(data, settings));
      }

      public static void LoadAchievements()
      {
         if (!File.Exists(FilePath))
         {
            _ = new LoadingError(PathObj.Empty, "No valid achievement file found!");
            GenerateAchievements();
            return;
         }


         var settings = new JsonSerializerSettings
         {
            Converters = GetConverters(),
            Formatting = Formatting.Indented
         };

         var json = File.ReadAllText(FilePath);
         var secureData = JsonConvert.DeserializeObject<AchievementData>(json, settings);

         if (secureData == null || secureData.HMAC == null!)
         {
            _ = new LoadingError(PathObj.Empty, "No valid achievement file found!");
            GenerateAchievements();
            return;
         }

         var savedHmac = secureData.HMAC;
         secureData.HMAC = string.Empty; 
         var recomputedHmac = GenerateHMAC(JsonConvert.SerializeObject(secureData, Formatting.Indented), HMAC_KEY);

         if (savedHmac != recomputedHmac)
         {
            _ = new LoadingError(PathObj.Empty, "Achievement file has been tampered with!", level:LogType.Error);
            MessageBox.Show("The achievements file is either corrupted or has been tampered with.\nAll achievements are reset!", "Achievements Reset!");
            GenerateAchievements();
            return;
         }

         foreach (var achievement in secureData.Achievements) 
            _achievements[achievement.Id] = achievement;
      }

      private static List<JsonConverter> GetConverters()
      {
         return [new AchievementConditionConverter()];
      }
   

      private static string GenerateHMAC(string data, string key)
      {
         using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
         var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
         return Convert.ToBase64String(hash);
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

   public class AchievementData
   {
      public List<Achievement> Achievements { get; set; } = new();
      public string HMAC { get; set; } = string.Empty;
   }

}