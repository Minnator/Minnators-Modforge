using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;

namespace Editor.Helper
{

   public enum SharedListMode
   {
      Equals,
      Intersect,
      Union
   }

   public static class AttributeHelper
   {
      public static TAttribute? GetAttribute<TAttribute>(object? obj) where TAttribute : Attribute
      {
         return (TAttribute?)obj?.GetType().GetCustomAttributes(typeof(TAttribute), inherit: true).FirstOrDefault();
      }

      public static TAttribute? GetAttribute<TAttribute>(PropertyInfo? propertyInfo) where TAttribute : Attribute
      {
         return (TAttribute?)propertyInfo?.GetCustomAttributes(typeof(TAttribute), inherit: true).FirstOrDefault();
      }

      public static TEnum GetEnumFromAttribute<TEnum>(Attribute? attribute, int defaultEnumIndex = -1) where TEnum : Enum
      {
         if (attribute == null)
            return GetDefaultEnumValue<TEnum>(defaultEnumIndex);

         var enumType = typeof(TEnum);
         foreach (var property in attribute.GetType().GetProperties())
            if (property.PropertyType == enumType)
               if (property.GetValue(attribute) is TEnum enumValue)
                  return enumValue;

         return GetDefaultEnumValue<TEnum>(defaultEnumIndex);
      }

      private static TEnum GetDefaultEnumValue<TEnum>(int defaultEnumIndex) where TEnum : Enum
      {
         var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
         if (defaultEnumIndex >= 0 && defaultEnumIndex < enumValues.Length)
            return enumValues[defaultEnumIndex];
         return enumValues[0]; 
      }

      public static bool GetSharedAttribute<T, Q>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects)
      {
         Debug.Assert(objects.Count > 0, "This function must only be called when there are more than 1 element selected!");

         var commonValue = propertyInfo.GetValue(objects.First());
         var result = objects.All((obj) => propertyInfo.GetValue(obj)!.Equals(commonValue));
         value = (Q)commonValue!;
         return result;
      }

      public static bool GetSharedAttributeList<T, Q, R>(PropertyInfo propertyInfo, out Q value, ICollection<T> objects, SharedListMode mode = SharedListMode.Equals) where Q : ICollection<R>, new() where R : notnull
      {
         List<Q> lists = [];
         if (objects.Count < 1)
         {
            value = default!;
            return false;
         }

         foreach (var obj in objects)
            lists.Add((Q)propertyInfo.GetValue(obj)!);

         switch (mode)
         {
            case SharedListMode.Equals:
               // Check if all lists are deeply equal
               if (AreAllListsEquals<Q, R>(lists))
               {
                  value = lists[0];
                  return true;
               }
               value = default!;
               return false;

            case SharedListMode.Intersect:
               // Find intersection of all lists
               var intersection = new HashSet<R>(lists[0]);
               foreach (var list in lists.Skip(1))
               {
                  intersection.IntersectWith(list);
               }
               value = new Q();
               foreach (var item in intersection)
               {
                  value.Add(item);
               }
               return value.Count > 0;

            case SharedListMode.Union:
               // Combine all unique elements from all lists
               var union = new HashSet<R>();
               foreach (var list in lists)
               {
                  union.UnionWith(list);
               }
               value = new Q();
               foreach (var item in union)
               {
                  value.Add(item);
               }
               return value.Count > 0;

            default:
               throw new ArgumentOutOfRangeException(nameof(mode), "Invalid mode specified.");
         }
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

      public static bool ScrambledListsEquals<T>(ICollection<T> list2, Dictionary<T, int> compDictionary) where T : notnull
      {
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
            if (!ScrambledListsEquals(lists[i], new Dictionary<Q, int>(compDictionary)))
               return false;

         return true;
      }

      public static string GetStringWithoutCamelCase(string camelString)
      {
         var sb = new StringBuilder();
         var first = true;
         foreach (var c in camelString)
         {
            if (char.IsUpper(c))
               if (!first)
                  sb.Append(' ');
               else
                  first = false;
            sb.Append(c);
         }
         return sb.ToString();
      }

      public static int HashKeyValuePair<T, Q>(this KeyValuePair<T, Q> pair) where T : notnull where Q : notnull
      {
         Debug.Assert(pair.Key != null && pair.Value != null, "pair.Key != null && pair.Value != null");
         return pair.Key.GetHashCode() ^ pair.Value.GetHashCode();
      }


      internal static List<string> GetDisplayMember<T>(List<T> items, PropertyInfo displayMember)
      {
         var displayMembers = new List<string>();
         foreach (var item in items)
            displayMembers.Add(displayMember.GetValue(item)!.ToString()!);
         return displayMembers;
      }

   }
}