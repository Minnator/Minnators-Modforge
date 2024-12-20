namespace Editor.NameGenerator
{
   public static class LevenshteinDistance
   {
      public static int Compute(string? s, string? t)
      {
         if (s == null || t == null) return Math.Max(s?.Length ?? 0, t?.Length ?? 0);

         int n = s.Length, m = t.Length;

         // Reduce memory to O(min(s, t)) instead of O(n * m)
         var prev = new int[m + 1];
         var curr = new int[m + 1];

         for (var j = 0; j <= m; j++) 
            prev[j] = j;

         for (var i = 1; i <= n; i++)
         {
            curr[0] = i;
            for (var j = 1; j <= m; j++)
            {
               var cost = s[i - 1] == t[j - 1] ? 0 : 1;
               curr[j] = Math.Min(Math.Min(curr[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
            }
            Array.Copy(curr, prev, m + 1); 
         }
         return curr[m];
      }
   }


}