namespace Editor.NameGenerator
{
   internal class Model
   {
      private readonly int _order;
      private readonly double _smoothing;
      private readonly IList<char> _alphabet;
      private readonly Dictionary<string, List<char>> _observations;
      private Dictionary<string, List<double>> _chains;

      public Model(ICollection<string> trainingData, int order, double smoothing, IList<char> alphabet)
      {
         _order = order;
         _smoothing = smoothing;
         _alphabet = alphabet;
         _chains = [];
         _observations = [];
         Retrain(trainingData);
      }

      public char Generate(string context, Random rnd)
      {
         return _chains.TryGetValue(context, out var chain) ? _alphabet[SelectIndex(chain, rnd)] : '\0';
      }

      private void Retrain(ICollection<string> trainingData)
      {
         _observations.Clear();
         _chains.Clear();
         Train(trainingData);
         BuildChains();
      }

      private void Train(ICollection<string> trainingData)
      {
         foreach (var d in trainingData)
         {
            var data = new string('#', _order) + d + '#';
            for (var i = 0; i < data.Length - _order; i++)
            {
               var key = data.Substring(i, _order);
               if (!_observations.TryGetValue(key, out var value))
               {
                  value = [];
                  _observations[key] = value;
               }

               value.Add(data[i + _order]);
            }
         }
      }

      private void BuildChains()
      {
         foreach (var kvp in _observations)
         {
            var chain = new double[_alphabet.Count];
            for (var i = 0; i < _alphabet.Count; i++)
            {
               var prediction = _alphabet[i];
               var count = kvp.Value.Count(c => c == prediction);
               chain[i] = count + _smoothing; // Smoothing applied here
            }
            _chains[kvp.Key] = chain.ToList();
         }
      }

      private static int SelectIndex(List<double> chain, Random rnd)
      {
         var totals = new List<double>();
         double accumulator = 0f;

         foreach (var weight in chain)
         {
            accumulator += weight;
            totals.Add(accumulator);
         }

         var rand = rnd.NextDouble() * accumulator;

         for (var i = 0; i < totals.Count; i++)
         {
            if (rand < totals[i])
               return i;
         }

         return 0;
      }
   }
}