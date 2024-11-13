using System.Diagnostics;
using System.Text.RegularExpressions;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading;

public static class DefaultMapLoading
{
   public static void CreateProvinceGroups()
   {
      var sw = new Stopwatch();
      sw.Start();

      if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "default.map"))
      {
         Globals.ErrorLog.Write("Error: default.map not found!");
         return;
      }
      var content = IO.ReadAllInUTF8(path);
      
      // Regex could be replaced by Parsing.GetElements(0, content) but this is faster
      const string pattern = @"\bmax_provinces\b\s+=\s+(?<maxProv>\d*)\s*\bsea_starts\b\s+=\s+{(?<seaProvs>[^\}]*)}[.\s\S]*\bonly_used_for_random\b\s+=\s+{(?<RnvProvs>[^\}]*)}[.\s\S]*\blakes\b\s+=\s+{(?<LakeProvs>[^\}]*)}[.\s\S]*\bforce_coastal\b\s+=\s+{(?<CostalProvs>[^\}]*)";

      HashSet<Province> land = [];
      HashSet<Province> sea = [];
      HashSet<Province> rnv = [];
      HashSet<Province> lake = [];
      HashSet<Province> coastal = [];

      List<Province> nonLandProvinces = [];
      List<Province> landProvinces = [];

      var match = Regex.Match(content, pattern);

      AddProvincesToDictionary(match.Groups["seaProvs"].Value, sea);
      AddProvincesToDictionary(match.Groups["RnvProvs"].Value, rnv);
      AddProvincesToDictionary(match.Groups["LakeProvs"].Value, lake);
      AddProvincesToDictionary(match.Groups["CostalProvs"].Value, land);

      foreach (var p in Globals.Provinces)
      {
         if (sea.Contains(p) || lake.Contains(p))
         {
            nonLandProvinces.Add(p);
            continue;
         }
         if ( rnv.Contains(p) || coastal.Contains(p))
            continue;
         land.Add(p);
         landProvinces.Add(p);
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
      Globals.NonLandProvinces = [.. nonLandProvinces];
      Globals.LandProvinceIds = [.. landProvinces];

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Parsing default.map", sw.ElapsedMilliseconds);
   }

   private static void AddProvincesToDictionary(string provinceList, HashSet<Province> hashSet)
   {
      foreach (var item in Parsing.GetIntListFromString(provinceList))
      {
         hashSet.Add(Globals.ProvinceIdToProvince[item]);
      }
   }
}