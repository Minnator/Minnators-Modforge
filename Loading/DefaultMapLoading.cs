using Editor.Helper;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Editor.Loading;

public static class DefaultMapLoading
{
   public static void Load(string folder)
   {
      var sw = new Stopwatch();
      sw.Start();
      var path = Path.Combine(folder, "map", "default.map");
      var content = IO.ReadAllInUTF8(path);
      const string pattern = @"\bmax_provinces\b\s+=\s+(?<maxProv>\d*)\s*\bsea_starts\b\s+=\s+{(?<seaProvs>[^\}]*)}[.\s\S]*\bonly_used_for_random\b\s+=\s+{(?<RnvProvs>[^\}]*)}[.\s\S]*\blakes\b\s+=\s+{(?<LakeProvs>[^\}]*)}[.\s\S]*\bforce_coastal\b\s+=\s+{(?<CostalProvs>[^\}]*)";

      HashSet<int> land = [];
      HashSet<int> sea = [];
      HashSet<int> rnv = [];
      HashSet<int> lake = [];
      HashSet<int> coastal = [];

      List<int> nonLandProvinces = [];
      List<int> landProvinces = [];

      var match = Regex.Match(content, pattern);

      AddProvincesToDictionary(match.Groups["seaProvs"].Value, sea);
      AddProvincesToDictionary(match.Groups["RnvProvs"].Value, rnv);
      AddProvincesToDictionary(match.Groups["LakeProvs"].Value, lake);
      AddProvincesToDictionary(match.Groups["CostalProvs"].Value, land);

      foreach (var p in Globals.Provinces.Values)
      {
         if (sea.Contains(p.Id) || lake.Contains(p.Id))
         {
            nonLandProvinces.Add(p.Id);
            continue;
         }
         if ( rnv.Contains(p.Id) || coastal.Contains(p.Id))
            continue;
         land.Add(p.Id);
         landProvinces.Add(p.Id);
      }

      foreach (var p in rnv)
      {
         sea.Remove(p);
         lake.Remove(p);
         coastal.Remove(p);
         land.Remove(p);
      }

      Globals.LandProvinces = land;
      Globals.SeaProvinces = sea;
      Globals.LakeProvinces = lake;
      Globals.CoastalProvinces = coastal;
      Globals.NonLandProvinceIds = [.. nonLandProvinces];
      Globals.LandProvinceIds = [.. landProvinces];

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing default.map", sw.ElapsedMilliseconds);
   }

   private static void AddProvincesToDictionary(string provinceList, HashSet<int> hashSet)
   {
      foreach (var item in Parsing.GetIntListFromString(provinceList))
         hashSet.Add(item);
   }
}