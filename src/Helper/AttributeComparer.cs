using System.Diagnostics;
using System.Reflection;

namespace Editor.Helper
{

   public static class AttributeComparer
   {
      public static bool GetSharedAttribute<T, Q>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects)
      {
         Debug.Assert(objects.Count > 0, "This function must only be called when there are more than 1 element selected!");

         var commonValue = propertyInfo.GetValue(objects.First());
         var result = objects.All((obj) => propertyInfo.GetValue(obj)!.Equals(commonValue));
         value = (Q)commonValue!;
         return result;
      }

      public static bool GetSharedAttributeList<T, Q, R>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects) where Q : ICollection<R> where R : notnull
      {
         List<Q> lists = [];
         foreach (var obj in objects)
            lists.Add((Q)propertyInfo.GetValue(obj)!);

         if (AreAllListsEquals<Q, R>(lists))
         {
            value = lists[0];
            return true;
         }
         value = default!;
         return false;
      }

      public static bool ScrambledListsEquals<T>(ICollection<T> list1, ICollection<T> list2) where T : notnull
      {
         var dict = new Dictionary<T, int>(list1.Count);
         foreach (var s in list1)
            if (!dict.TryAdd(s, 1))
               dict[s]++;
         foreach (var s in list2)
            if (dict.TryGetValue(s, out var cnt))
               dict[s] = --cnt;
            else
               return false;
         return dict.Values.All(c => c == 0);
      }

      public static bool ScrambledListsEquals<T>(ICollection<T> list1, ICollection<T> list2, Dictionary<T, int> compDictionary) where T : notnull
      {
         foreach (var s in list1)
            if (!compDictionary.TryAdd(s, 1))
               compDictionary[s]++;
         foreach (var s in list2)
            if (compDictionary.TryGetValue(s, out var cnt))
               compDictionary[s] = --cnt;
            else
               return false;
         return compDictionary.Values.All(c => c == 0);
      }

      public static bool AreAllListsEquals<T, Q>(List<T> lists) where T : ICollection<Q> where Q : notnull
      {
         if (lists.Count < 2)
            return true;

         var compDictionary = new Dictionary<Q, int>(lists[0].Count);
         foreach (var s in lists[0])
            if (!compDictionary.TryAdd(s, 1))
               compDictionary[s]++;

         compDictionary.TrimExcess();

         for (var i = 1; i < lists.Count; i++)
            if (!ScrambledListsEquals(lists[0], lists[i], new(compDictionary)))
               return false;

         return true;
      }
   }
}