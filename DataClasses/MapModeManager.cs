using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Editor.Controls;
using Editor.Loading;
using Editor.MapModes;

namespace Editor.DataClasses;

public class MapModeManager
{
   private List<IMapMode> MapModes { get; } = [];
   private IMapMode CurrentMapMode { get; set; } = null!;
   private IMapMode IdMapMode { get; set; } = null!;
   private PannablePictureBox PictureBox { get; set; }

   public MapModeManager(PannablePictureBox pictureBox)
   {
      PictureBox = pictureBox;
      InitializeAllMapModes();
   }

   private void InitializeAllMapModes()
   {
      MapModes.Add(new ProvinceMapMode());

      // We set the default map mode to retrieve province colors
      IdMapMode = GetMapMode("Provinces");
   }

   public List<IMapMode> GetMapModes()
   {
      return MapModes;
   }

   public IMapMode GetMapMode(string name)
   {
      return MapModes.Find(mode => mode.GetMapModeName() == name);
   }

   public List<string> GetMapModeNames()
   {
      var names = new List<string>();
      foreach (var mode in MapModes)
         names.Add(mode.GetMapModeName());
      return names;
   }

   public void SetCurrentMapMode(string name)
   {
      CurrentMapMode = GetMapMode(name);
      PictureBox.Image = CurrentMapMode.Bitmap; // We point the PictureBox to the new bitmap
      PictureBox.Invalidate(); // We force the PictureBox to redraw
   }

   public bool GetProvince(Point point, out Province province)
   {
      if (Globals.ColorToProvId.TryGetValue (IdMapMode.Bitmap.GetPixel(point.X, point.Y), out var provinceId))
      {
         province = Globals.Provinces[provinceId];
         return true;
      }
      province = null!;
      return false;
   }

}