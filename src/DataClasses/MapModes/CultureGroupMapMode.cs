﻿using System.Linq;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;

namespace Editor.DataClasses.MapModes;

public class CultureGroupMapMode : MapMode
{
   public CultureGroupMapMode()
   {
      // Subscribe to events to update the min and max values when a province's development changes
      ProvinceEventHandler.OnProvinceCultureChanged += UpdateProvince;
   }

   public override bool IsLandOnly => true;

   public override int GetProvinceColor(Province id)
   {
      if (Globals.Cultures.TryGetValue(id.Culture, out var culture))
         if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var group))
            return group.Color.ToArgb();
      return Color.DimGray.ToArgb();
   }

   public override MapModeType MapModeType => MapModeType.CultureGroup;

   public override string GetSpecificToolTip(Province provinceId)
   {
      if (Globals.Cultures.TryGetValue(provinceId.Culture, out var culture))
         if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var group))
            return $"Culture Group: {group.Name} ({Localisation.GetLoc(group.Name)})";
      return "Culture Group: [Unknown]";
   }

}