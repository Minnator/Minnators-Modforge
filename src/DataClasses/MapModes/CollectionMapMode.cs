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
            if (Globals.MapModeManager.PreviousLandOnly)
            {
               foreach (var collection in Collections)
               {
                  var color = collection.Color.ToArgb();
                  foreach (var province in collection.GetProvinces())
                     MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels);
               }
            }
            else
            {
               foreach (var collection in Collections)
               {
                  var color = collection.Color.ToArgb();
                  foreach (var province in collection.GetProvinces())
                     MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels);
               }
               MapDrawing.DrawOnMap(Globals.NonLandProvinces, GetSeaProvinceColor, Globals.ZoomControl, PixelsOrBorders.Pixels);
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
                  MapDrawing.DrawOnMap(province, color, Globals.ZoomControl, PixelsOrBorders.Pixels);
                  drawnProvinces.Remove(province);
               }
            }

            foreach (var province in drawnProvinces)
               MapDrawing.DrawOnMap(province, Color.DimGray.ToArgb(), Globals.ZoomControl, PixelsOrBorders.Pixels);
         }
         if (ShowOccupation)
            MapDrawing.DrawOccupations(false, Globals.ZoomControl);

         Selection.RePaintSelection();
         Globals.ZoomControl.Invalidate();
         Globals.MapModeManager.PreviousLandOnly = IsLandOnly;
      }
   }
}