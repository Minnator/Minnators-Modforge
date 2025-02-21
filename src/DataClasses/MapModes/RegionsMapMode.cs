using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using Region = Editor.DataClasses.Saveables.Region;

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
      if (id.Area != Area.Empty)
         if (id.Area.Region != Region.Empty)
         {
            try
            {
               return id.Area.Region.Color.ToArgb();
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
      if (province.Area != Area.Empty)
            return $"Region: {province.Area.Region.Name} ({Localisation.GetLoc(province.Area.Region.Name)})";
      return "Region: [Unknown]";
   }

}