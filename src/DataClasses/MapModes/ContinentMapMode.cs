using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public sealed class ContinentMapMode : MapMode
{
   public ContinentMapMode()
   {
      Continent.ItemsModified += UpdateProvinceCollection;
      Continent.ColorChanged += UpdateComposite<Province>;
   }

   public override MapModeType MapModeType => MapModeType.Continent;

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Provinces.TryGetValue(id, out var province))
         if (province.GetContinent() != Continent.Empty)
            return province.GetContinent().Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (province.GetContinent() != Continent.Empty)
            return $"Continent: {province.GetContinent().Name} ({Localisation.GetLoc(province.GetContinent().Name)})";
      return "Continent: [Unknown]";
   }

   public override bool ShouldProvincesMerge(Province p1, Province p2)
   {
      if (Globals.Provinces.TryGetValue(p1, out var province1) && Globals.Provinces.TryGetValue(p2, out var province2))
         return province1.GetContinent() == province2.GetContinent();
      return false;
   }
}