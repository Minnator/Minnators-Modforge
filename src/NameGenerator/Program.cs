using System.Diagnostics;

namespace Editor.NameGenerator
{
   /// <summary>
   /// C# implementation of http://www.samcodes.co.uk/project/markov-namegen/
   /// </summary>
   public static class NameGenStarter
   {
      public static void RunNameGen()
      {
         var stopwatch = new System.Diagnostics.Stopwatch();

         stopwatch.Start();
         NameGenerator generator = new NameGenerator(TrainingData.EnglishTowns, 3, 0.01);
         stopwatch.Stop();
         Debug.WriteLine($"Generator creation took: {stopwatch.ElapsedMilliseconds}ms");
         stopwatch.Restart();
         Debug.Assert(generator != null, nameof(generator) + " != null");
         var task = generator.GenerateNames(150, 5, 15, 3, "chest", Globals.Random);
         stopwatch.Stop();
         Debug.WriteLine($"Name generation took: {stopwatch.ElapsedMilliseconds}ms");

         foreach (var name in task)
         {
            Debug.WriteLine(name);
         }
      }
   }
}