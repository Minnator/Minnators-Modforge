using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;

namespace Editor.Saving
{
   public static class CollectionSavers
   {
      public static void SaveIProvinceCollection(List<ProvinceCollection<Province>> provinceCollections, bool locComment, params string[] internalPath)
      {
         var path = Path.Combine(Globals.ModPath, Path.Combine(internalPath));
         StringBuilder sb = new ();
         foreach (var provinceCollection in provinceCollections)
         {
            if (locComment)
               sb.AppendLine($"# {Localisation.GetLoc(provinceCollection.Name)}");
            SavingUtil.AddFormattedIntList(provinceCollection.Name, provinceCollection.GetProvinceIds(), 0, ref sb);
         }
         IO.WriteToFile(path, sb.ToString(), false);
      }

      public static void SaveStringCollection(List<KeyValuePair<string, List<string>>> collection, bool locComment, params string[] internalPath)
      {
         var path = Path.Combine(Globals.ModPath, Path.Combine(internalPath));
         StringBuilder sb = new ();
         foreach (var item in collection)
         {
            if (locComment)
               sb.AppendLine($"# {Localisation.GetLoc(item.Key)}");
            SavingUtil.AddFormattedStringList(item.Key, item.Value, 0, ref sb);
         }
         IO.WriteToFile(path, sb.ToString(), false);
      }

      public static void SaveStringCollection(List<KeyValuePair<string, List<string>>> collection, string elementsGroup,
         bool locComment, params string[] internalPath)
      {
         var path = Path.Combine(Globals.ModPath, Path.Combine(internalPath));
         StringBuilder sb = new ();
         foreach (var item in collection)
         {
            if (locComment)
               sb.AppendLine($"# {Localisation.GetLoc(item.Key)}");
            sb.AppendLine($"{item.Key} = {{");
            SavingUtil.AddFormattedStringList(elementsGroup, item.Value, 1, ref sb);
            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine("}\n");
         }
         IO.WriteToFile(path, sb.ToString(), false);
      }


   }
}