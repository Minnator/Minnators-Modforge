using static Editor.Events.ProvinceEventHandler;
using System;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Events;

public static class GlobalEventHandlers
{
   public class ProvinceCollectionModified(string name, bool add) : EventArgs;

   public static event EventHandler<ProvinceCollectionModified> OnCountryListChanged = delegate { };
   public static void RaiseCountryListChanged()
   {
      OnCountryListChanged.Invoke(null, new (string.Empty, false));
   }

   public static event EventHandler<ProvinceCollectionModified> OnSuperRegionListChanged = delegate { };
   public static void RaiseSuperRegionListChanged(string name, bool add)
   {
      OnSuperRegionListChanged.Invoke(null, new(name, add));
   }


   // ----------- MapModePaintEvent ----------------

   public class MapModePaintEventArgs : EventArgs
   {

      public Graphics Graphics { get; set; }
      public Rectangle ClipRectangle { get; set; }
      public ICollection<Province> VisibleProvinces { get; set; }
      public MapModePaintEventArgs(Graphics graphics, Rectangle clipRectangle, ICollection<Province> visibleProvinces)
      {
         Graphics = graphics;
         ClipRectangle = clipRectangle;
         VisibleProvinces = visibleProvinces;
      }
   }


}
