using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.DataClasses.MapModes;

public sealed class RegionsMapMode : MapMode
{
   public RegionsMapMode()
   {
      Region.ItemsModified += UpdateProvinceCollection;
      Region.ColorChanged += UpdateComposite<Province>;
   }

   public override MapModeType MapModeType => MapModeType.Regions;

   public override int GetProvinceColor(Province id)
   {
      if (id.GetArea() != Area.Empty)
         if (id.GetArea().Region != Region.Empty)
         {
            try
            {
               return id.GetArea().Region.Color.ToArgb();
            }
            catch (Exception)
            {

               throw;
            }
         }
      return Color.DarkGray.ToArgb();
   }

   public override string GetSpecificToolTip(Province province)
   {
      if (province.GetArea() != Area.Empty)
            return $"Region: {province.GetArea().Region.Name} ({Localisation.GetLoc(province.GetArea().Region.Name)})";
      return "Region: [Unknown]";
   }

   public override bool ShouldProvincesMerge(Province p1, Province p2)
   {
      return p1.GetArea().Region == p2.GetArea().Region;
   }
}