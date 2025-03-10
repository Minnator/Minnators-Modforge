using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.Saveables;
using Editor.Saving;

namespace Editor.DataClasses.MapModes
{
   public class BaseTaxMapMode : MapMode
   {
      private int _max = int.MinValue;
      
      private static void UpdateMinMax(MapMode mapMode, ref int max)
      {
         if (!CalculateMinMax(ref max, typeof(Province).GetProperty(nameof(Province.BaseTax))!))
            return;

         if (MapModeManager.CurrentMapMode != mapMode)
            return;
         mapMode.RenderMapMode();
      }

      /// <summary>
      /// Returns if a new max was found
      /// </summary>
      /// <returns></returns>
      public static bool CalculateMinMax(ref int max, PropertyInfo info) 
      {
         Debug.Assert(info.PropertyType == typeof(int), "only integers can be used in dynamic min max calc for mapmodes");

         var newMaxFound = false;
         var newMax = int.MinValue;
         foreach (var province in Globals.Provinces)
         {
            var totalDev = (int)province.GetProperty(info);
            if (totalDev > newMax)
            {
               newMax = totalDev;
               if (newMax > max)
               {
                  max = newMax;
                  newMaxFound = true;
               }
            }
         }

         // If the new max is smaller than the current max, update the max
         if (newMax < max)
         {
            max = newMax;
            newMaxFound = true;
         }
         return newMaxFound;
      }

      public override int GetProvinceColor(Province id)
      {
         if (!Globals.LandProvinces.Contains(id))
            return id.Color.ToArgb();

         return Globals.ColorProvider.GetColorOnGreenRedShade(0, _max, id.BaseTax).ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (Globals.Provinces.TryGetValue(provinceId, out var province))
            return $"Base tax: {province.BaseTax}";
         return "nBaseTax: Unknown";
      }

      public override void SetActive()
      {
         CalculateMinMax(ref _max, typeof(Province).GetProperty(nameof(Province.BaseTax))!);
         base.SetActive();
      }
      public override MapModeType MapModeType { get; } = MapModeType.BaseTax;

   }
}