using Editor.DataClasses;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Editor.Helper;

namespace Editor.Loading;

public static class DefaultMapLoading
{
   private static void Load(string folder)
   {
      var path = Path.Combine(folder, "map", "default.map");
      var content = IO.ReadAllInUTF8(path);
      const string pattern = @"\bmax_provinces\b\s+=\s+(?<maxProv>\d*)\s*\bsea_starts\b\s+=\s+{(?<seaProvs>[^\}]*)}[.\s\S]*\bonly_used_for_random\b\s+=\s+{(?<RnvProvs>[^\}]*)}[.\s\S]*\blakes\b\s+=\s+{(?<LakeProvs>[^\}]*)}[.\s\S]*\bforce_coastal\b\s+=\s+{(?<CostalProvs>[^\}]*)";

      var land = new HashSet<int>();
      var sea = new HashSet<int>();
      var rnv = new HashSet<int>();
      var lake = new HashSet<int>();
      var coastal = new HashSet<int>();

      var match = Regex.Match(content, pattern);

      AddProvincesToDictionary(match.Groups["seaProvs"].Value, sea);
      AddProvincesToDictionary(match.Groups["RnvProvs"].Value, rnv);
      AddProvincesToDictionary(match.Groups["LakeProvs"].Value, lake);
      AddProvincesToDictionary(match.Groups["CostalProvs"].Value, land);

      foreach (var p in Data.Provinces)
      {
         if (sea.Contains(p.Id) || rnv.Contains(p.Id) || lake.Contains(p.Id) || coastal.Contains(p.Id))
            continue;
         land.Add(p.Id);
      }

      foreach (var p in rnv)
      {
         sea.Remove(p);
         lake.Remove(p);
         coastal.Remove(p);
         land.Remove(p);
      }


   }

   private static void AddProvincesToDictionary(string provinceList, HashSet<int> hashSet)
   {
      foreach (var item in Parsing.GetProvincesList(provinceList))
         hashSet.Add(item);
   }
}