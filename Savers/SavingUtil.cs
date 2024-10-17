using System.Text;
using Editor.Forms.SavingClasses;
using Editor.Interfaces;
using Microsoft.VisualBasic;

namespace Editor.Savers
{
   public static class SavingUtil
   {
      public static int MaxLineWidth = 125;
      public static bool SortIntLists = true;
      public static bool SameSpacePerInt = true;

      public static void SaveAllModified(ModifiedData modifiedData)
      {
         if (modifiedData.SaveProvinces)
            ProvinceSaver.SaveAllModifiedProvinces();
         if (modifiedData.SaveAreas)
         {
            List<IProvinceCollection> areas = [];
            foreach (var area in Globals.Areas.Values)
               areas.Add(area);
            CollectionSavers.SaveIProvinceCollection(areas, true, "map", "area.txt");
         }
         if (modifiedData.SaveRegions)
         {
            List<KeyValuePair<string, List<string>>> regions = [];
            foreach (var region in Globals.Regions.Values)
               regions.Add(new(region.Name, region.Areas));
            CollectionSavers.SaveStringCollection(regions, "areas", true, "map", "region.txt");
         }

         if (modifiedData.SuperRegions)
         {
            List<KeyValuePair<string, List<string>>> superRegions = [];
            foreach (var superRegion in Globals.SuperRegions.Values)
               superRegions.Add(new(superRegion.Name, superRegion.Regions));
            CollectionSavers.SaveStringCollection(superRegions,true, "map", "superregion.txt");
         }
         if (modifiedData.Continents)
         {
            List<IProvinceCollection> continents = [];
            foreach (var continent in Globals.Continents.Values)
               continents.Add(continent);
            CollectionSavers.SaveIProvinceCollection(continents, true, "map", "continent.txt");
         }
         if (modifiedData.ProvinceGroups)
         {
            List<IProvinceCollection> provinceGroups = [];
            foreach (var provinceGroup in Globals.ProvinceGroups.Values)
               provinceGroups.Add(provinceGroup);
            CollectionSavers.SaveIProvinceCollection(provinceGroups, true, "map", "provincegroup.txt");
         }


      }

      public static string GetYesNo(bool value)
      {
         return value ? "yes" : "no";
      }

      private static void AddFormattedList(string blockName, ICollection<string> strings, int tabCount, bool isNumber, ref StringBuilder sb)
      {
         AddTabs(tabCount, ref sb);

         sb.Append(blockName).Append(" = {\n");
         AddTabs(tabCount + 1, ref sb);
         var lineLength = 0;
         foreach (var i in strings)
         {
            if (lineLength > MaxLineWidth)
            {
               sb.Append('\n');
               AddTabs(tabCount + 1, ref sb);
               lineLength = 0;
            }

            if (isNumber && SameSpacePerInt)
            {
               sb.Append($"{i,4}").Append(' ');
               lineLength += 5;
            }
            else if (!isNumber)
            {
               sb.AppendLine($"{i} ");
               AddTabs(tabCount + 1, ref sb);
            }
            else
            {
               sb.Append(i).Append(' ');
               lineLength += i.Length + 1;
            }
         }
         if (isNumber)
            sb.Append('\n');
         else
            sb.Remove(sb.Length - 2, 2);
         AddTabs(tabCount, ref sb);
         sb.Append("}\n\n");
      }

      public static void AddFormattedIntList(string blockName, ICollection<int> ints, int tabCount, ref StringBuilder sb)
      {
         if (SortIntLists)
         {
            var intsList = ints.ToList();
            intsList.Sort();
            ints = intsList;
         }
         AddFormattedList(blockName, ints.Select(i => i.ToString()).ToList(), tabCount, true, ref sb);
      }

      public static void AddFormattedStringList(string blockName, ICollection<string> strings, int tabCount, ref StringBuilder sb)
      {
         AddFormattedList(blockName, strings, tabCount, false, ref sb);
      }

      public static void AddTabs(int tabCount, ref StringBuilder sb)
      {
         for (var i = 0; i < tabCount; i++) 
            sb.Append('\t');
      }
   }
}