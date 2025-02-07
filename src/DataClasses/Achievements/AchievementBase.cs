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
      internal bool CheckCondition();
      internal Bitmap GetIcon();
   }

   public interface IAchievementCondition
   {
      float GetProgress();
      bool IsCompleted();
      void IncreaseProgress(float amount);
   }


   public class ProgressCondition(float goal, AchievementId id) : IAchievementCondition
   {
      private float _currentProgress;
      private AchievementId _id = id;

      public float CurrentProgress => _currentProgress;

      public float Goal { get; } = goal;
      public float GetProgress() => Math.Clamp(CurrentProgress / goal, 0f, 1f);
      public bool IsCompleted() => CurrentProgress >= goal;
      public void IncreaseProgress(float amount)
      {
         _currentProgress = Math.Min(Goal, _currentProgress += amount);
         if (IsCompleted()) 
            AchievementEvents.NotifyCompletion(id);
      }
   }


   public class Achievement : IAchievement
   {
      public Achievement(AchievementId id,
                         string name,
                         string description,
                         IAchievementCondition condition,
                         Bitmap icon,
                         bool isHidden = false,
                         int level = 0)
      {
         Id = id;
         Name = name;
         Description = description;
         Icon = AchievementImageHelper.ColorMask(icon, AchievementManager.GetAchievementColor(level));
         Level = level;
         IsHidden = isHidden;
         Condition = condition;
      }

      public AchievementId Id { get; }
      public string Name { get; }
      public string Description { get; }
      public Bitmap Icon { get; }
      public int Level { get; }
      public DateTime DateAchieved { get; private set; }
      public bool IsHidden { get; }
      public bool IsAchieved { get; private set; }
      public IAchievementCondition Condition { get; }

      public bool CheckCondition() => Condition.IsCompleted();
      public float GetProgress() => Condition.GetProgress();
      public void SetAchieved()
      {
         if (!IsAchieved)
         {
            IsAchieved = true;
            DateAchieved = DateTime.UtcNow;
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
   }
}