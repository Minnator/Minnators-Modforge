﻿using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class CountryMapMode : MapMode
   {
      public CountryMapMode()
      {
         ProvinceEventHandler.OnProvinceOwnerChanged += UpdateProvince;
         ProvinceEventHandler.OnProvinceControllerChanged += UpdateProvince;
      }

      public override bool IsLandOnly => true;
      public override bool ShowOccupation => true;

      public override int GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
         {
            if (province.Owner == Tag.Empty)
               return Color.DimGray.ToArgb();
            if (Globals.Countries.TryGetValue(province.Owner, out var country))
               return country.Color.ToArgb();
         }
         return Color.DimGray.ToArgb();
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

      public override void RenderMapMode(Func<int, int> method)
      {
         base.RenderMapMode(method);
         MapDrawing.DrawAllCapitals(Color.Yellow.ToArgb());
      }
   }
}