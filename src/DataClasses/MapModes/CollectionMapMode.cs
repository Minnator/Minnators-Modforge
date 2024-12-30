using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public abstract class CollectionMapMode : MapMode
   {
      public List<ProvinceCollection<Province>> Collections { get; set; } = [];
      public override bool IsCollectionMapMode => true;

      public virtual void RenderMapMode()
      {
         if (IsLandOnly)
         {
            if (MapModeManager.PreviousLandOnly)
            {
               foreach (var collection in Collections)
               {
                  var color = collection.Color.ToArgb();
                  foreach (var province in collection.GetProvinces())
                     MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels, ShouldProvincesMerge);
               }
            }
            else
            {
               foreach (var collection in Collections)
               {
                  var color = collection.Color.ToArgb();
                  foreach (var province in collection.GetProvinces())
                     MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels, ShouldProvincesMerge);
               }
               MapDrawing.DrawOnMap(Globals.NonLandProvinces, GetSeaProvinceColor, Globals.ZoomControl, PixelsOrBorders.Pixels, ShouldProvincesMerge);
            }
         }
         else
         {
            var drawnProvinces = new HashSet<Province>(Globals.Provinces);

            foreach (var node in Globals.TradeNodes.Values)
            {
               var color = node.Color.ToArgb();
               foreach (var province in node.GetProvinces())
               {
                  MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels, ShouldProvincesMerge);
                  drawnProvinces.Remove(province);
               }
            }

            foreach (var province in drawnProvinces)
               MapDrawing.DrawOnMap(province, Color.DimGray.ToArgb(), Globals.ZoomControl, PixelsOrBorders.Pixels, ShouldProvincesMerge);
         }
         if (ShowOccupation)
            MapDrawing.DrawOccupations(false, Globals.ZoomControl);

         Selection.RePaintSelection();
         Globals.ZoomControl.Invalidate();
         MapModeManager.PreviousLandOnly = IsLandOnly;
      }
   }
}