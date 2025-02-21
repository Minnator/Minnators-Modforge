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
      private readonly AchievementId _id = id;

      public float CurrentProgress => _currentProgress;

      public float Goal { get; } = goal;

      public AchievementId Id => _id;

      public float GetProgress() => Math.Clamp(CurrentProgress / goal, 0f, 1f);
      public bool IsCompleted() => CurrentProgress >= goal;
      public void IncreaseProgress(float amount)
      {
         _currentProgress = Math.Min(Goal, _currentProgress += amount);
         if (IsCompleted()) 
            AchievementEvents.NotifyCompletion(_id);
      }
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
      public string Name { get; }
      public string Description { get; }
      [JsonIgnore]
      public Bitmap Icon { get; }
      public int Level { get; }
      public DateTime DateAchieved { get; private set; } = DateTime.MinValue;
      public bool IsHidden { get; }
      public bool IsAchieved { get; private set; }
      public AchievementImage Image { get; }

      [JsonConverter(typeof(AchievementConditionConverter))]
      public IAchievementCondition Condition { get; }

      public bool CheckCondition() => Condition.IsCompleted();
      public float GetProgress() => Condition.GetProgress();
      public void SetAchieved()
      {
         if (!IsAchieved)
         {
            IsAchieved = true;
            DateAchieved = DateTime.Now;
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

   public class AchievementConditionConverter : JsonConverter<IAchievementCondition>
   {
      public override void WriteJson(JsonWriter writer, IAchievementCondition value, JsonSerializer serializer)
      {
         if (value is ProgressCondition progressCondition)
         {
            writer.WriteStartObject();

            writer.WritePropertyName("goal");
            writer.WriteValue(progressCondition!.Goal);

            writer.WritePropertyName("currentProgress");
            writer.WriteValue(progressCondition.CurrentProgress);

            writer.WritePropertyName("id");
            serializer.Serialize(writer, progressCondition.Id);

            writer.WriteEndObject();
         }
         else
         {
            throw new JsonSerializationException("Unknown IAchievementCondition type");
         }
      }

      public override IAchievementCondition ReadJson(JsonReader reader, Type objectType, IAchievementCondition existingValue, bool hasExistingValue, JsonSerializer serializer)
      {
         var jsonObject = JObject.Load(reader);

         if (jsonObject["goal"] != null) // Identify ProgressCondition by its fields
         {
            var goal = jsonObject["goal"]!.Value<float>();
            var currentProgress = jsonObject["currentProgress"]!.Value<float>();
            var id = jsonObject["id"]!.ToObject<AchievementId>(serializer);

            var condition = new ProgressCondition(goal, id);
            condition.IncreaseProgress(currentProgress);
            return condition;
         }

         throw new JsonSerializationException("Unknown IAchievementCondition type");
      }
   }
   
}