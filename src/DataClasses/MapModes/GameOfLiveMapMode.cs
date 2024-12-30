using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class GameOfLiveMapMode : MapMode
   {
      public override MapModeType MapModeType => MapModeType.GameOfLive;

      public override void SetActive()
      {
         GameOfLive.RunGameOfLive(Globals.Settings.Rendering.GameOfLiveGenerations);
      }

      public override void SetInactive()
      {
         GameOfLive.Stop();
      }

      public override bool ShouldProvincesMerge(Province p1, Province p2)
      {
         return false;
      }

      public override int GetProvinceColor(Province id)
      {
         if (GameOfLive.Rules == GameOfLive.SurvivalRules.PopulationDynamics)
         {
            if (GameOfLive.Population.TryGetValue(id, out var population))
            {
               switch (population)
               {
                  case 0:
                     return System.Drawing.Color.OliveDrab.ToArgb();
                  case 1:
                     return System.Drawing.Color.DarkOrange.ToArgb();
                  case 2:
                     return System.Drawing.Color.DarkSlateBlue.ToArgb();
                  default:
                     return 0x000000;
               }
            }
         }

         if (GameOfLive.CellStates.TryGetValue(id, out var alive))
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
         if (GameOfLive.CellStates.TryGetValue(provinceId, out var alive))
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