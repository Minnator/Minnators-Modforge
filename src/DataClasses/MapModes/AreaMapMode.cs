using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public sealed class AreaMapMode : MapMode
{
   public AreaMapMode()
   {
      Area.ItemsModified += UpdateProvinceCollection;
      Area.AreaColorChanged += UpdateComposite<Province>;
   }


   public override MapModeType MapModeType => MapModeType.Area;

   public override int GetProvinceColor(Province id)
   {
      if (id.GetArea() != Area.Empty )
         return id.GetArea().Color.ToArgb();
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Area: {province.GetArea().Name} ({Localisation.GetLoc(province.GetArea().Name)})";
      return "Area: [Unknown]";
   }

}