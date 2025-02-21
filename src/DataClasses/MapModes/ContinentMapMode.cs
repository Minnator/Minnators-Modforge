using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
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
         if (province.Continent != Continent.Empty)
            return province.Continent.Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
         if (province.Continent != Continent.Empty)
            return $"Continent: {province.Continent.Name} ({Localisation.GetLoc(province.Continent.Name)})";
      return "Continent: [Unknown]";
   }

}