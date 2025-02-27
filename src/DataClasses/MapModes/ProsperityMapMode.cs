﻿using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Events;

namespace Editor.DataClasses.MapModes
{
   public class ProsperityMapMode : MapMode
   {
      public override bool IsLandOnly => true;

      public ProsperityMapMode()
      {
         // Subscribe to events to update the min and max values when a province's development changes
         // TODO FIX MAP MODE UPDATES ProvinceEventHandler.OnProvinceProsperityChanged += UpdateProvince;
      }

      public override MapModeType MapModeType => MapModeType.Prosperity;

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Prosperity: {province.Prosperity}";
         return "Prosperity: -";
      }

      public override int GetProvinceColor(Province id)
      {
         if (Globals.SeaProvinces.Contains(id))
            return id.Color.ToArgb();
         return Globals.ColorProvider.GetColorOnGreenRedShade(0, 100, id.Prosperity).ToArgb();
      }

   }
}