using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class ProvinceGroupMapMode : MapMode
   {
      public ProvinceGroupMapMode()
      {
         ProvinceGroup.ColorChanged += UpdateComposite<Province>;
         ProvinceGroup.ItemsModified += UpdateProvinceCollection;
      }

      public override MapModeType MapModeType => MapModeType.ProvinceGroup;
      public override bool IsLandOnly => false;
      

      public override int GetProvinceColor(Province id)
      {
         foreach (var provinceGroup in Globals.ProvinceGroups.Values)
         {
            var ids = provinceGroup.GetProvinceIds();
            if (ids.Contains(id.Id))
               return provinceGroup.Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var provinceGroup in Globals.ProvinceGroups.Values)
         {
            if (provinceGroup.GetProvinceIds().Contains(provinceId.Id))
               return Localisation.GetLoc(provinceGroup.Name);
         }
         return "No province group";
      }

   }
}