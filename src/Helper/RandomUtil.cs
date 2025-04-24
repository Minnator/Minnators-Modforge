using System;
using System.Diagnostics;

namespace Editor.Helper;

public static class RandomUtil
{
   public static int GetRandomNumber(int max, int seed = -1)
   {
      Debug.Assert(max > 0, "max > 0");
      if (max <= 0)
         throw new ArgumentOutOfRangeException(nameof(max), "Max must be greater than 0");
      if (seed == -1)
         return Globals.Random.Next(max);
      return new Random(seed).Next(max);
   }
   public static int GetRandomNumber(int min, int max, int seed = -1)
   {
      Debug.Assert(min <= max, "min <= max");
      if (min >= max)
         throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than Max");
      if (seed == -1)
         return Globals.Random.Next(min, max);
      return new Random(seed).Next(min, max);
   }

   public static int[] GetRandomNumbers(int min, int max, int count, int seed = -1)
   {
      Debug.Assert(min <= max, "min <= max");
      if (min >= max)
         throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than Max");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(min, max - min).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(min, max - min).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static int[] GetRandomNumbers(int max, int count, int seed = -1)
   {
      Debug.Assert(max > 0, "max > 0");
      if (max <= 0)
         throw new ArgumentOutOfRangeException(nameof(max), "Max must be greater than 0");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(0, max).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(0, max).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static int[] GetRandomNumbers(int min, int max, int count, ICollection<int> exclude, int seed = -1)
   {
      Debug.Assert(min <= max, "min <= max");
      if (min >= max)
         throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than Max");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(min, max - min).Where(x => !exclude.Contains(x)).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(min, max - min).Where(x => !exclude.Contains(x)).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static int[] GetRandomNumbers(int max, int count, ICollection<int> exclude, int seed = -1)
   {
      Debug.Assert(max > 0, "max > 0");
      if (max <= 0)
         throw new ArgumentOutOfRangeException(nameof(max), "Max must be greater than 0");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(0, max).Where(x => !exclude.Contains(x)).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(0, max).Where(x => !exclude.Contains(x)).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static int[] GetRandomNumbers(int min, int max, int count, ICollection<int> exclude, ICollection<int> include, int seed = -1)
   {
      Debug.Assert(min <= max, "min <= max");
      if (min >= max)
         throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than Max");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(min, max - min).Where(x => !exclude.Contains(x) && include.Contains(x)).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(min, max - min).Where(x => !exclude.Contains(x) && include.Contains(x)).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static int[] GetRandomNumbers(int max, int count, ICollection<int> exclude, ICollection<int> include, int seed = -1)
   {
      Debug.Assert(max > 0, "max > 0");
      if (max <= 0)
         throw new ArgumentOutOfRangeException(nameof(max), "Max must be greater than 0");
      if (count < 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");
      if (seed == -1)
         return Enumerable.Range(0, max).Where(x => !exclude.Contains(x) && include.Contains(x)).OrderBy(_ => Globals.Random.Next()).Take(count).ToArray();
      var random = new Random(seed);
      return Enumerable.Range(0, max).Where(x => !exclude.Contains(x) && include.Contains(x)).OrderBy(_ => random.Next()).Take(count).ToArray();
   }

   public static T[] GetRandomItems<T>(int count, IList<T> collection, bool unique = false)
   {
      Debug.Assert(count > 0, "count > 0");
      if (count <= 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");

      if (unique)
      {
         var result = new HashSet<T>(count);
         var collCopy = new List<T>(collection);
         for (var i = 0; i < count; i++)
         {
            var index = Globals.Random.Next(0, collCopy.Count);
            result.Add(collCopy[index]);
            collCopy.RemoveAt(index);
         }
         return result.ToArray();
      }
      var resultList = new List<T>(count);
      for (var i = 0; i < count; i++)
      {
         var index = Globals.Random.Next(0, collection.Count);
         resultList.Add(collection[index]);
      }
      return resultList.ToArray();
   }

   public static T[] GetRandomItems<T>(int count, IList<T> collection, ICollection<T> exclude, bool unique = false)
   {
      Debug.Assert(count > 0, "count > 0");
      if (count <= 0)
         throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than 0");

      if (unique)
      {
         var result = new HashSet<T>(count);
         var collCopy = new List<T>(collection);
         foreach (var item in exclude)
            collCopy.Remove(item);
         for (var i = 0; i < count; i++)
         {
            var index = Globals.Random.Next(0, collCopy.Count);
            result.Add(collCopy[index]);
            collCopy.RemoveAt(index);
         }
         return result.ToArray();
      }
      var resultList = new List<T>(count);
      foreach (var item in exclude)
         collection.Remove(item);
      for (var i = 0; i < count; i++)
      {
         var index = Globals.Random.Next(0, collection.Count);
         resultList.Add(collection[index]);
      }
      return resultList.ToArray();
   }

   // ########################### Gaussian ###########################
   /// <summary>
   /// stdDev = (max - min) / 6 // ~99.7% of values in [min, max]
   /// </summary>
   /// <param name="mean"></param>
   /// <param name="stdDev"></param>
   /// <param name="min"></param>
   /// <param name="max"></param>
   /// <returns></returns>
   public static int GaussianInt(int mean, int stdDev, int min, int max)
   {
      var u1 = 1.0 - Globals.Random.NextDouble();
      var u2 = 1.0 - Globals.Random.NextDouble();
      var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                          Math.Sin(2.0 * Math.PI * u2);
      var value = mean + stdDev * randStdNormal;
      return Math.Clamp((int)Math.Round(value), min, max);
   }

   public static int GaussianInt(int mean, int min, int max)
   {
      return GaussianInt(mean, (max - min) / 6, min, max);
   }

   public static double GaussianDouble(double mean, double stdDev, double min, double max)
   {
      var u1 = 1.0 - Globals.Random.NextDouble();
      var u2 = 1.0 - Globals.Random.NextDouble();
      var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                          Math.Sin(2.0 * Math.PI * u2);
      var value = mean + stdDev * randStdNormal;
      return Math.Clamp(value, min, max);
   }

   public static double GaussianDouble(double mean, double stdDev)
   {
      var u1 = 1.0 - Globals.Random.NextDouble();
      var u2 = 1.0 - Globals.Random.NextDouble();
      var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                          Math.Sin(2.0 * Math.PI * u2);
      return mean + stdDev * randStdNormal;
   }

   public static double GaussianDouble(double mean, double stdDev, int seed)
   {
      var u1 = 1.0 - new Random(seed).NextDouble();
      var u2 = 1.0 - new Random(seed).NextDouble();
      var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                          Math.Sin(2.0 * Math.PI * u2);
      return mean + stdDev * randStdNormal;
   }

   public static double GaussianDouble(double mean, double stdDev, double min, double max, int seed)
   {
      var u1 = 1.0 - new Random(seed).NextDouble();
      var u2 = 1.0 - new Random(seed).NextDouble();
      var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                          Math.Sin(2.0 * Math.PI * u2);
      var value = mean + stdDev * randStdNormal;
      return Math.Clamp(value, min, max);
   }
}