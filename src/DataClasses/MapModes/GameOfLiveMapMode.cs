using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class GameOfLiveMapMode : MapMode
   {
      public override MapModeType GetMapModeName()
      {
         return MapModeType.GameOfLive;
      }

      public override int GetProvinceColor(Province id)
      {
         if (GameOfLive.IsAlive.TryGetValue(id, out var alive))
            return alive switch             {
               GameOfLive.CellState.Alive => 0x00FF00,
               GameOfLive.CellState.Dead => 0x000000,
               GameOfLive.CellState.Infected => 0xFF0000,
               _ => 0x000000
            };
         return 0x000000;
      }

      public override string GetSpecificToolTip(Province provinceId)
      {
         if (GameOfLive.IsAlive.TryGetValue(provinceId, out var alive))
            return alive switch
            {
               GameOfLive.CellState.Alive => "Alive",
               GameOfLive.CellState.Dead => "Dead",
               GameOfLive.CellState.Infected => "Infected",
               _ => "Dead"
            };
         return "Dead";
      }
   }
}