using System;
using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Helper
{
   public static class GameOfLive
   {
      public static Dictionary<Province, CellState> IsAlive = [];
      public static Dictionary<Province, double> SurvivalProbability = [];

      private static Timer _timer = new();
      private static readonly Random Random = new();
      private const bool USE_RANDOM_STATE_CHANGE = false;

      private static readonly int MinSurvival = 2;
      private static readonly int MaxSurvival = 3;
      private static readonly int BirthThreshold = 3;

      private static readonly double SurvivalProbabilityThreshold = 0.2;

      private static readonly SurvivalRules Rules = SurvivalRules.SurvivalProbability;

      enum SurvivalRules
      {
         NeighbourLimits,
         Default,
         SurvivalProbability
      }

      public enum CellState
      {
         Dead, 
         Alive, 
         Infected
      }

      public static void Initialize()
      {
         IsAlive.Clear();
         foreach (var province in Globals.Provinces)
         {
            IsAlive[province] = CellState.Dead;
            SurvivalProbability[province] = Random.NextDouble();
         }
      }

      public static void NextGeneration()
      {
         var tempState = new Dictionary<Province, CellState>(IsAlive);
         foreach (var province in Globals.Provinces)
         {
            var aliveNeighbours = GetAliveNeighbours(province);
            switch (Rules)
            {
               case SurvivalRules.NeighbourLimits:
                  if (IsAlive[province] == CellState.Alive)
                  {
                     if (aliveNeighbours >= MinSurvival && aliveNeighbours <= MaxSurvival)
                        tempState[province] = CellState.Alive;
                     else
                        tempState[province] = CellState.Dead;
                  }
                  else
                  {
                     if (aliveNeighbours == BirthThreshold)
                        tempState[province] = CellState.Alive;
                     else
                        tempState[province] = CellState.Dead;
                  }
                  break;
               case SurvivalRules.Default:
                  if (IsAlive[province] == CellState.Alive)
                  {
                     if (aliveNeighbours is 2 or 3)
                        tempState[province] = CellState.Alive;
                     else
                        tempState[province] = CellState.Dead;
                  }
                  else
                  {
                     if (aliveNeighbours == 3)
                        tempState[province] = CellState.Alive;
                     else
                        tempState[province] = CellState.Dead;
                  }
                  break;
               case SurvivalRules.SurvivalProbability:
                  var survives = IsAlive[province] == CellState.Alive && aliveNeighbours is >= 2 and <= 3;
                  var born = IsAlive[province] != CellState.Alive  && aliveNeighbours == 3;

                  if (survives || (born && SurvivalProbability[province] > SurvivalProbabilityThreshold))
                     tempState[province] = CellState.Alive;
                  else
                     tempState[province] = CellState.Dead;
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }

            if (USE_RANDOM_STATE_CHANGE && Random.Next(0, 100) < 5)
               if (tempState[province] == CellState.Alive)
                  tempState[province] = CellState.Dead;
               else
                  tempState[province] = CellState.Alive;
         }
         IsAlive = tempState;
      }

      private static int GetAliveNeighbours(Province province)
      {
         var aliveNeighbours = 0;
         foreach (var neighbour in Globals.AdjacentProvinces[province])
            if (IsAlive[neighbour] == CellState.Alive)
               aliveNeighbours++;
         return aliveNeighbours;
      }

      public static void InitializeRandomValues()
      {
         foreach (var province in Globals.Provinces)
         {
            if (Random.Next(0, 2) == 1)
               IsAlive[province] = CellState.Alive;
            else
               IsAlive[province] = CellState.Dead;
         }
      }

      public static void RunGameOfLive(int iterations, long deltaTime = 100)
      {
         _timer.Stop();
         _timer = new Timer {Interval = (int)deltaTime};

         if (IsAlive.Count != Globals.Provinces.Count)
            Initialize();
         InitializeRandomValues();
         Globals.MapModeManager.SetCurrentMapMode(MapModeType.GameOfLive);

         _timer.Tick += (sender, args) =>
         {
            NextGeneration();
            Globals.MapModeManager.RenderCurrent();
            iterations--;
            if (iterations == 0)
               _timer.Stop();
         };

         _timer.Start();
      }

   }
}