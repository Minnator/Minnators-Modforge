using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation.Collections;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;
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
               if (!IO.Exists(Globals.Settings.Saving.CustomWordsLocation) || !IO.ReadAllInANSI(Globals.Settings.Saving.CustomWordsLocation, out var content))
                  return [];
               return SampleArray(Parsing.GetStringListWithoutQuotes(content), count, true);
            case NameGenSource.MonarchNames:
               return SampleArray(GetMonarchNames(), count, true);
            case NameGenSource.LocWords:
               return SampleLocalisation(count);
            default:
               throw new ArgumentOutOfRangeException(nameof(source), source, null);
         }
      }

      private static string[] SampleFromSelection(NameGenSource source, int count)
      {
         switch (source)
         {
            case NameGenSource.ProvinceNames:
               var data = new List<string>();
               foreach (var province in Selection.GetSelectedProvinces)
               {
                  if (Globals.RNWProvinces.Contains(province))
                     continue;

                  var value = province.TitleLocalisation;
                  if (value.Contains("PROV"))
                     continue;
                  data.Add(value);
               }
               return SampleArray(data.ToArray(), count, true);
            case NameGenSource.CustomByUser:
               if (!File.Exists(Globals.Settings.Misc.NameGenConfig.CustomNamesFile))
                  return new string[count];
               if (!IO.ReadAllInANSI(Globals.Settings.Misc.NameGenConfig.CustomNamesFile, out var content))
                  return new string[count];
               return SampleArray(Parsing.GetStringListWithoutQuotes(content), count, true);
            case NameGenSource.MonarchNames:
               HashSet<Country> countriesFromProvinces = [];
               foreach (var sp in Selection.GetSelectedProvinces)
                  if (sp.Owner != Country.Empty)
                     countriesFromProvinces.Add(sp.Owner);
               List<string> monarchNames = [];
               foreach (var country in countriesFromProvinces)
                  monarchNames.AddRange(country.CommonCountry.MonarchNames.Select(x => x.PureName).Where(name => !name.Equals(string.Empty)));
               return SampleArray(monarchNames.ToArray(), count, true);
            case NameGenSource.LocWords:
               var allLocObjects = SaveMaster.GetAllLocObjects();
               List<string> locs = [];
               foreach (var locObject in allLocObjects) 
                  locs.Add(locObject.Value);
               return SampleArray(locs.ToArray(), count, true);
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
            var names = country.GetPropertyInfo(nameof(country.CommonCountry.MonarchNames));
            if (names == null || names.GetValue(country) is not ICollection<MonarchName> mNames)
               continue;
            foreach (var name in mNames)
               totalNames[cnt++] = name.Name;
         }
         return totalNames;
      }

      private static string[] SampleLocalisation(int count)
      {
         var locs = new string[count];
         if (count > Globals.Localisation.Count)
         {
            _ = new LogEntry(LogType.Warning, "too little Localisation array for string generator. Sampling will have duplicate strings!");
            Array.Copy(Globals.Localisation.ToArray(), locs, count);
            while (count > Globals.Localisation.Count)
               Array.Copy(Globals.Localisation.ToArray(), 0, locs, Globals.Localisation.Count, count - Globals.Localisation.Count);
            return locs;
         }

         var resultCount = 0;
         foreach (var item in Globals.Localisation)
         {
            // Probability of selecting an item depends on remaining capacity and elements
            var remainingSlots = count - resultCount;

            // Select item if within probability bounds
            if (Globals.Random.Next(Globals.Localisation.Count - resultCount) < remainingSlots) 
               locs[resultCount++] = item.Value;

            if (resultCount == count)
               break;
         }

         return locs;
      }

   }
}