using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class CountryMapMode : MapMode
   {
      public CountryMapMode()
      {
         ProvinceEventHandler.OnProvinceOwnerChanged += UpdateProvince;
      }

      public override bool IsLandOnly => true;
      public override bool ShowOccupation => true;

      public override Color GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (province.Owner == Tag.Empty)
               return Color.DimGray;
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return country.Color;
         }
         return Color.DimGray;
      }

      public override string GetMapModeName()
      {
         return "Country";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
         {
            if (province.Owner == Tag.Empty)
               return "Country: [Unknown]";
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return $"Country: {country.Tag} ({country.GetLocalisation()})";
         }
         return "Country: [Unknown]";
      }

      public override void RenderMapMode(Func<int, Color> method)
      {
         base.RenderMapMode(method);
         if (!Globals.Settings.MapModeSettings.ShowCountryCapitals)
            return;
         Graphics g;
         if (Globals.MapModeRendering == MapModeRendering.Live)
         {
            g = Graphics.FromImage(Globals.MapModeManager.ShareLiveBitmap);
         }
         else
         {
            g = Graphics.FromImage(Bitmap);
         }
         
         foreach (var country in Globals.Countries.Values)
         {
            if (country.Exists && Globals.Provinces.TryGetValue(country.Capital, out var province))
            {
               g.DrawEllipse(new (Color.Black, 1), province.Center.X - 2, province.Center.Y - 2, 4, 4);
               g.DrawEllipse(Pens.Yellow, province.Center.X - 1, province.Center.Y - 1, 2, 2);
            }
         }
      }
   }
}