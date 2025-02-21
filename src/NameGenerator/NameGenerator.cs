using System.ComponentModel;
using Editor.DataClasses.Settings;

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


   public class NameGenConfig
   {
      [Description("The parameters for the generator")]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public class GeneratorParams
      {
         [Description("The order of the models to use")]
         public int Order { get; set; } = 3;

         [Description("The number of models to use, will be of orders up to and including 'Order'")]
         public double Smoothing { get; set; } = 0.01;

         [Description("The source of the training data")]
         public NameGenSource Source { get; set; } = NameGenSource.ProvinceNames;

         [Description("Whether to use the seeded random from the Settings or a seedless random")]
         public bool DefaultRandom { get; set; } = true;
      }

      // Generation parameters

      [Description("The parameters for the generation")]
      [TypeConverter(typeof(CEmptyStringConverter))]
      public class GenerationParams
      {

         [Description("The number of names to generate")]
         public int Count { get; set; } = 10;

         [Description("The minimum length of the generated word")]
         public int MinLength { get; set; } = 5;

         [Description("The maximum length of the generated word")]
         public int MaxLength { get; set; } = 15;

         [Description("The maximum Levinstein Distance a generated word must can have to the simmilar word")]
         public int MaxDistance { get; set; } = 5;

         [Description("Will not be used if empty. The word the generated word should be similar to")]
         public string? SimilarTo { get; set; } = "Minnator";
      }

      public GeneratorParams Generator { get; set; } = new();
      public GenerationParams Generation { get; set; } = new();
   }


}