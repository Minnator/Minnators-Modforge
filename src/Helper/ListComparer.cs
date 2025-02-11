using System.Collections.Generic;

namespace Editor.Helper
{
   public class ListComparer<T> : IEqualityComparer<List<T>>
   {
      public bool Equals(List<T>? x, List<T>? y)
      {
         if (x == null || y == null) return false;
         if (x.Count != y.Count) return false;
         for (var i = 0; i < x.Count; i++)
         {
            if (!EqualityComparer<T>.Default.Equals(x[i], y[i])) return false;
         }
         return true;
      }

      public int GetHashCode(List<T> obj)
      {
         unchecked
         {
            var hash = 19;
            foreach (var item in obj) 
               hash = hash * 31 + (item != null ? item.GetHashCode() : 1);
            return hash;
         }
      }
   }
}