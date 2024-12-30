using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.MapModes;

public sealed class ProvinceMapMode : MapMode
{
   public ProvinceMapMode()
   {
      Province.ColorChanged += UpdateComposite<Province>;
   }

   public override MapModeType MapModeType => MapModeType.Province;

   public override int GetProvinceColor(Province provinceId)
   {
      return provinceId.Color.ToArgb();
   }

   public override string GetSpecificToolTip(Province provinceId)
   {
      return $"Province: {provinceId.Id} ({provinceId.TitleLocalisation})";
   }

   public override bool ShouldProvincesMerge(Province p1, Province p2)
   {
      return false;
   }
}