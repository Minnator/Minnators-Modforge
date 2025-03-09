using Editor.ErrorHandling;

namespace Editor.NameGenerator
{
   public class Generator
   {
      private readonly int _order;
      public double Smoothing { get; set; }

      private readonly List<Model> _models;

      public bool IsTrained => _models is { Count: > 0 };

      /// <summary>
      /// 
      /// </summary>
      /// <param name="trainingData">training data for the generator, array of words</param>
      /// <param name="order">number of models to use, will be of orders up to and including "order"</param>
      /// <param name="smoothing">the dirichlet prior/additive smoothing "randomness" factor</param>
      protected Generator(ICollection<string> trainingData, int order, double smoothing)
      {
         if (trainingData == null || trainingData.Count == 0)
         {
            _ = new LogEntry(LogType.Error, "No training data provided for generator");
            return;
         }

         _order = order;
         Smoothing = smoothing;
         _models = [];

         // Identify and sort the alphabet used in the training data
         SortedSet<char> domain = ['#'];
         foreach (var word in trainingData)
            if (word != null)
               foreach (var c in word) 
                  domain.Add(c);

         for (var i = 0; i < order; i++) 
            _models.Add(new (trainingData, order - i, smoothing, domain.ToList()));
      }

      /// <summary>
      /// Generates a word
      /// </summary>
      /// <returns></returns>
      protected string Generate(Random rnd)
      {
         if (_models.Count == 0)
         {
            _ = new LogEntry(LogType.Error, "No models available for generator. Please retrain the Generator!");
            return string.Empty;
         }
         var name = new string('#', _order);
         var letter = GetLetter(name, rnd);
         while (letter != '#' && letter != '\0')
         {
            name += letter;
            letter = GetLetter(name, rnd);
         }
         return name;
      }

      private char GetLetter(string name, Random rnd)
      {
         var letter = '\0';
         var context = name.Substring(name.Length - _order);
         foreach (var model in _models)
         {
            letter = model.Generate(context, rnd);
            if (letter == '\0')
               context = context.Substring(1);
            else
               break;
         }
         return letter;
      }
   }
}