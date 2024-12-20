using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.NameGenerator
{
   public static class DataSampler
   {

      public static string[] Sample(NameGenSource source, int count = 1000)
      {
         var attr = source.GetAttributeOfType<TrainingsDataAttr>();
         if (attr == null)
            throw new EvilActions($"{nameof(NameGenSource)}:({source}) Attributes are null but required!");
         if (!attr.CustomRange)
            count = attr.SampleCount;
         if (attr.SampleSelection)
            return SampleFromSelection(source, count);
         return SampleFromRange(source, count);
      }

      private static string[] SampleFromRange(NameGenSource source, int count)
      {
         switch (source)
         {
            case NameGenSource.ProvinceNames:
               return SampleArray(GetLocProviderLocValues(Globals.Provinces.Cast<ITitleAdjProvider>().ToList()), count, true);
            case NameGenSource.CustomByUser:
               // The user can provide a file path to a .txt file with a list of names separated by commas or new lines which will be used as the source
               break;
            case NameGenSource.MonarchNames:

            case NameGenSource.LocWords:
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(source), source, null);
         }
         return [];
         
      }

      private static string[] SampleFromSelection(NameGenSource source, int count)
      {
         switch (source)
         {
            case NameGenSource.ProvinceNames:
            case NameGenSource.CustomByUser:
            case NameGenSource.MonarchNames:
            case NameGenSource.LocWords:
            default:
               throw new ArgumentOutOfRangeException(nameof(source), source, null);
         }
      }

      private static string[] SampleArray(string[] source, int count, bool random)
      {
         if (source.Length < count)
            _ = new LogEntry(LogType.Warning, "Source array for string generator. Sampling will have duplicate strings!");

         var result = new string[count];
         if (random)
         {
            var sourceList = source.ToList();
            for (var i = count - 1; i >= 0; i--)
            {
               result[i] = sourceList[Globals.Random.Next(0, sourceList.Count)];
               if (source.Length >= count)
                  sourceList.RemoveAt(i); // if the source array is smaller than the count, we will have duplicates so we do not remove the item we just added
            }
         }
         else
         {
            if (source.Length >= count)
               Array.Copy(source, result, count);
            else
            {
               // Copy the source array to the result array and fill the rest with the source array
               Array.Copy(source, result, source.Length);
               Array.Copy(source, 0, result, source.Length, count - source.Length);
            }
         }

         return result;
      }

      private static string[] GetLocProviderLocValues(ICollection<ITitleAdjProvider> provinceSource, bool title = true)
      {
         var prvNameArr = new string[provinceSource.Count];
         var cnt = 0;
         foreach (var province in provinceSource) 
            if (title)
               prvNameArr[cnt++] = province.TitleLocalisation;
            else
               prvNameArr[cnt++] = province.AdjectiveLocalisation;
         return prvNameArr;
      }

      private static string[] GetMonarchNames()
      {
         // there are way to many monarch names, but we just include the option bc we can
         var totalNames = new string[ProvColHelper.CountListItemsOfAllCountries(nameof(Country.CommonCountry.MonarchNames))];
         var cnt = 0;
         foreach (var country in Globals.Countries.Values)
         {
            var names = country.GetProperty(nameof(country.CommonCountry.MonarchNames));
            if (names == null || names.GetValue(country) is not ICollection<MonarchName> mNames)
               continue;
            foreach (var name in mNames)
               totalNames[cnt++] = name.Name;
         }
         return totalNames;
      }

      

   }
}