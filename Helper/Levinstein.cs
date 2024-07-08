namespace Editor.Helper
{
   public static class Levinstein
   {

      // Method to calculate the Levenshtein distance between two strings
      public static int CalculateLevenshteinDistance(string source, string target)
      {
         if (string.IsNullOrEmpty(source))
            return string.IsNullOrEmpty(target) ? 0 : target.Length;

         if (string.IsNullOrEmpty(target))
            return source.Length;

         var sourceLength = source.Length;
         var targetLength = target.Length;

         var distanceMatrix = new int[sourceLength + 1, targetLength + 1];

         for (var i = 0; i <= sourceLength; distanceMatrix[i, 0] = i++) { }
         for (var j = 0; j <= targetLength; distanceMatrix[0, j] = j++) { }

         for (var i = 1; i <= sourceLength; i++)
         {
            for (var j = 1; j <= targetLength; j++)
            {
               var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

               distanceMatrix[i, j] = Math.Min(
                  Math.Min(distanceMatrix[i - 1, j] + 1, distanceMatrix[i, j - 1] + 1),
                  distanceMatrix[i - 1, j - 1] + cost);
            }
         }

         return distanceMatrix[sourceLength, targetLength];
      }

      // Method to get the Levenshtein distance for an array of words and a target word
      public static int GetLevinsteinDistance(string[] words, string word)
      {
         if (string.IsNullOrEmpty(word))
            return int.MaxValue;

         var minDistance = int.MaxValue;

         foreach (var w in words)
         {
            var distance = CalculateLevenshteinDistance(w, word);
            if (distance < minDistance)
            {
               minDistance = distance;
            }
         }

         return minDistance;
      }



   }
}