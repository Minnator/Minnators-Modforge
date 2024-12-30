using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class WeatherMapMode : MapMode
   {
      public override MapModeType MapModeType { get; } = MapModeType.Weather;
      public override bool IsLandOnly { get; } = true;
      public override int GetProvinceColor(Province id)
      {
         foreach (var weather in Globals.Weather)
         {
            if (weather.Value.SubCollection.Contains(id))
               return weather.Value.Color.ToArgb();
         }
         return Color.FromArgb(94, 34, 34).ToArgb();
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         foreach (var weather in Globals.Weather)
         {
            if (weather.Value.SubCollection.Contains(provinceId))
               return $"Weather: {weather.Value.Name} ({Localisation.GetLoc(weather.Value.Name)})";
         }
         return "No unusual Weather";
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         foreach (var weather in Globals.Weather)
            if (weather.Value.SubCollection.Contains(p1) && weather.Value.SubCollection.Contains(p2))
               return true;
         return false;
      }
   }
}