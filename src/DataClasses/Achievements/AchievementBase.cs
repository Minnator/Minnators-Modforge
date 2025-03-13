using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editor.DataClasses.Achievements
{

   public interface IAchievement
   {
      public AchievementId Id { get; }
      internal string Name { get; }
      internal string Description { get; }
      internal Bitmap Icon{ get; }
      internal int Level { get; }
      internal DateTime DateAchieved { get; }

      internal bool IsHidden { get; }
      internal bool IsAchieved { get; }

      internal IAchievementCondition Condition { get; }

      internal void SetAchieved();
      internal float GetProgress();
      internal float GetProgressValue();
      internal bool CheckCondition();
      internal Bitmap GetIcon();
      internal void Reset();
   }

   public interface IAchievementCondition
   {
      float GetProgressValue();
      float GetProgress();
      bool IsCompleted();
      void IncreaseProgress(float amount);
      void Reset();
   }


   public class ProgressCondition(float goal, AchievementId id) : IAchievementCondition
   {
      private float _currentProgress;
      private readonly AchievementId _id = id;

      public float CurrentProgress => _currentProgress;

      [JsonIgnore]
      public float Goal { get; } = goal;

      [JsonIgnore]
      public AchievementId Id => _id;

      public float GetProgressValue() => CurrentProgress;

      public float GetProgress() => Math.Clamp(CurrentProgress / goal, 0f, 1f);
      public bool IsCompleted() => CurrentProgress >= goal;
      public void IncreaseProgress(float amount)
      {
         if (IsCompleted()) 
            return;
         _currentProgress = Math.Min(Goal, _currentProgress += amount);
         if (IsCompleted()) 
            AchievementManager.GetAchievement(Id).SetAchieved();
      }

      public void Reset() => _currentProgress = 0;
   }


   public class Achievement : IAchievement
   {
      public Achievement(AchievementId id,
                         string name,
                         string description,
                         IAchievementCondition condition,
                         AchievementImage icon,
                         bool isHidden = false,
                         int level = 0)
      {
         Id = id;
         Name = name;
         Description = description;
         Image = icon;
         Icon = AchievementImageHelper.ColorMask(icon, AchievementManager.GetAchievementColor(level));
         Level = level;
         IsHidden = isHidden;
         Condition = condition;
      }

      public AchievementId Id { get; set; }
      [JsonIgnore]
      public string Name { get; }
      [JsonIgnore]
      public string Description { get; }
      [JsonIgnore]
      public Bitmap Icon { get; }
      [JsonIgnore]
      public int Level { get; }
      public DateTime DateAchieved { get; internal set; } = DateTime.MinValue;
      [JsonIgnore]
      public bool IsHidden { get; }
      public bool IsAchieved { get; internal set; }
      [JsonIgnore]
      public AchievementImage Image { get; }

      public IAchievementCondition Condition { get; }

      public bool CheckCondition() => Condition.IsCompleted();
      public float GetProgress() => Condition.GetProgress();
      public float GetProgressValue() => Condition.GetProgressValue();
      public void SetAchieved()
      {
         if (!IsAchieved)
         {
            IsAchieved = true;
            DateAchieved = DateTime.Now;
            Condition.IncreaseProgress(float.MaxValue);
            AchievementEvents.NotifyCompletion(Id);
         }
      }

      public void IncreaseProgress(float progress)
      {
         Condition.IncreaseProgress(progress);
         if (CheckCondition()) 
            SetAchieved();
      }

      public Bitmap GetIcon()
      {
         return Icon;
      }

      public void LoadProgress(float progress)
      {
         Condition.IncreaseProgress(progress);
         if (CheckCondition()) 
            IsAchieved = true;
      }

      public void Reset()
      {
         IsAchieved = false;
         DateAchieved = DateTime.MinValue;
         Condition.Reset();
      }
   }
   
}