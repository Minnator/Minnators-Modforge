namespace Editor.NameGenerator
{

   // Sample from selection

   public class NameGenParams()
   {
      public int MaxIterationsPerAttempt = 20;
      public int MinLength = 5;
      public int MaxLength = 15;
      public int MaxDistance = 3;
      public string? SimilarTo = "Minnator";

   }

   // Enum Attribute for NameGenSource to tell if a custom range can be applied and if it samples from a selection
   [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
   public class TrainingsDataAttr(bool customRange, bool sampleSelection, int count) : Attribute
   {
      public bool CustomRange { get; } = customRange;
      public bool SampleSelection { get; } = sampleSelection;
      public int SampleCount { get; set; } = count;
   }


   /// <summary>
   /// Input modes for the trainings data
   /// </summary>
   public enum NameGenSource
   {
      [TrainingsDataAttr(true, true, 1000)]
      ProvinceNames,
      [TrainingsDataAttr(true, false, 500)]
      CustomByUser,
      [TrainingsDataAttr(true, true, 200)]
      MonarchNames,
      [TrainingsDataAttr(true, true, 1000)]
      LocWords
   }

   public class NameGenerator(ICollection<string> trainingData, int order, double smoothing)
      : Generator(trainingData, order, smoothing)
   {
      private const int MAX_ITERATIONS_PER_ATTEMPT = 20;
      private bool GenerateName(int minLength, int maxLength, int maxDistance, string? similarTo, Random rnd, out string name)
      {
         name = Generate(rnd).Replace("#", "");
         if (name.Length < minLength || name.Length > maxLength)
            return false;

         return similarTo == null || LevenshteinDistance.Compute(similarTo, name) <= maxDistance;
      }

      public List<string> GenerateNames(int count, int length, Random rnd)
      {
         return GenerateNames(count, length, length, 0, null, rnd);
      }

      public List<string> GenerateNames(int count, int minLength, int maxLength, Random rnd)
      {
         return GenerateNames(count, minLength, maxLength, 0, null, rnd);
      }

      public List<string> GenerateNames(int count, int minLength, int maxLength, int maxDistance, string? similarTo,
         Random rnd)
      {
         var names = new HashSet<string>();

         var iteration = 0;

         while (names.Count < count && iteration++ < MAX_ITERATIONS_PER_ATTEMPT * count)
         {
            if (GenerateName(minLength, maxLength, maxDistance, similarTo, rnd, out var name))
               names.Add(name);
         }

         return names.ToList();
      }
   }
}