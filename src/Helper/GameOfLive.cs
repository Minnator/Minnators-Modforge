using System;
using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Helper
{
   public static class GameOfLive
   {
      public static Dictionary<Province, CellState> CellStates = [];
      public static Dictionary<Province, double> SurvivalProbability = [];
      public static Dictionary<Province, int> Population = [];

      private static Timer _timer = new();
      private static readonly Random Random = new();

      private static readonly int MinSurvival = 2;
      private static readonly int MaxSurvival = 3;
      private static readonly int BirthThreshold = 3;

      private static readonly double SurvivalProbabilityThreshold = 0.2;
      private static readonly double ResistanceChance = 0.5;

      public static SurvivalRules Rules = SurvivalRules.PopulationDynamics;

      public enum SurvivalRules
      {
         NeighbourLimits,
         Default,
         SurvivalProbability,
         MultipleCellStates,
         PopulationDynamics
      }

      public enum CellState
      {
         Dead, 
         Alive, 
         Infected
      }

      public static void Initialize()
      {
         CellStates.Clear();
         SurvivalProbability.Clear();
         Population.Clear();
         foreach (var province in Globals.Provinces)
         {
            CellStates[province] = CellState.Dead;
            SurvivalProbability[province] = Random.NextDouble();
            Population[province] = 0;
         }
      }

      public static void NextGeneration()
      {
         var tempState = new Dictionary<Province, CellState>(CellStates);
         var tempPopulation = new Dictionary<Province, int>(Population);

         foreach (var province in Globals.Provinces)
         {
            var aliveNeighbours = GetAliveNeighbours(province);

            switch (Rules)
            {
               case SurvivalRules.NeighbourLimits:
                  ApplyNeighbourLimitsRule(province, tempState, aliveNeighbours);
                  break;
               case SurvivalRules.Default:
                  ApplyDefaultRule(province, tempState, aliveNeighbours);
                  break;
               case SurvivalRules.SurvivalProbability:
                  ApplySurvivalProbabilityRule(province, tempState, aliveNeighbours);
                  break;
               case SurvivalRules.MultipleCellStates:
                  ApplyMultipleCellStatesRule(province, tempState, aliveNeighbours);
                  break;
               case SurvivalRules.PopulationDynamics:
                  ApplyPopulationDynamicsRule(province, tempPopulation);
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }

            if (Globals.Settings.Rendering.EasterEggs.GameOfLiveUseRandomCellChanges && Random.Next(0, 100) < 5)
               tempState[province] = tempState[province] == CellState.Alive ? CellState.Dead : CellState.Alive;
         }

         CellStates = tempState;
         Population = tempPopulation;
      }

      private static void ApplyNeighbourLimitsRule(Province province, Dictionary<Province, CellState> tempState, int aliveNeighbours)
      {
         if (CellStates[province] == CellState.Alive)
         {
            tempState[province] = aliveNeighbours >= MinSurvival && aliveNeighbours <= MaxSurvival
                ? CellState.Alive
                : CellState.Dead;
         }
         else
         {
            tempState[province] = aliveNeighbours == BirthThreshold
                ? CellState.Alive
                : CellState.Dead;
         }
      }

      private static void ApplyDefaultRule(Province province, Dictionary<Province, CellState> tempState, int aliveNeighbours)
      {
         if (CellStates[province] == CellState.Alive)
         {
            tempState[province] = aliveNeighbours is 2 or 3 ? CellState.Alive : CellState.Dead;
         }
         else
         {
            tempState[province] = aliveNeighbours == 3 ? CellState.Alive : CellState.Dead;
         }
      }

      private static void ApplySurvivalProbabilityRule(Province province, Dictionary<Province, CellState> tempState, int aliveNeighbours)
      {
         var survives = CellStates[province] == CellState.Alive && aliveNeighbours is >= 2 and <= 3;
         var born = CellStates[province] != CellState.Alive && aliveNeighbours == 3;

         tempState[province] = survives || (born && SurvivalProbability[province] > SurvivalProbabilityThreshold)
             ? CellState.Alive
             : CellState.Dead;
      }

      private static void ApplyMultipleCellStatesRule(Province province, Dictionary<Province, CellState> tempState, int aliveNeighbours)
      {
         if (CellStates[province] == CellState.Alive)
         {
            tempState[province] = aliveNeighbours is 2 or 3 ? CellState.Alive : CellState.Infected;
         }
         else if (CellStates[province] == CellState.Dead)
         {
            tempState[province] = aliveNeighbours == 3 ? CellState.Alive : CellState.Dead;
         }
         else if (CellStates[province] == CellState.Infected)
         {
            tempState[province] = CellState.Dead;
         }
      }

      private static void ApplyPopulationDynamicsRule(Province province, Dictionary<Province, int> tempPopulation)
      {
         var neighborCounts = Globals.AdjacentProvinces[province]
             .GroupBy(neighbour => Population[neighbour])
             .ToDictionary(g => g.Key, g => g.Count());
         
         var topPopulations = neighborCounts
            .Where(kvp => kvp.Value == neighborCounts.Values.Max())
            .Select(kvp => kvp.Key)
            .ToList();

         var dominantPopulation = topPopulations[Random.Next(topPopulations.Count)];

         if (Random.NextDouble() < ResistanceChance)
         {
            tempPopulation[province] = Population[province];
            return;
         }


         tempPopulation[province] = dominantPopulation; // Province adopts the dominant neighbor's population
         
      }

      private static int GetAliveNeighbours(Province province)
      {
         var aliveNeighbours = 0;
         foreach (var neighbour in Globals.AdjacentProvinces[province])
            if (CellStates[neighbour] == CellState.Alive)
               aliveNeighbours++;
         return aliveNeighbours;
      }

      public static void InitializeRandomValues()
      {
         foreach (var province in Globals.Provinces)
         {
            CellStates[province] = Random.Next(0, 2) == 1 ? CellState.Alive : CellState.Dead;
            Population[province] = Random.Next(0, 3);
         }
      }

      public static void RunGameOfLive(int iterations, long deltaTime = 100)
      {
         _timer.Stop();
         _timer = new Timer {Interval = (int)deltaTime};

         if (CellStates.Count != Globals.Provinces.Count)
            Initialize();
         InitializeRandomValues();
         MapModeManager.SetCurrentMapMode(MapModeType.GameOfLive);

         _timer.Tick += (sender, args) =>
         {
            NextGeneration();
            MapModeManager.ConstructCache();
            MapModeManager.RenderCurrent();
            iterations--;
            if (iterations == 0)
               _timer.Stop();
         };

         _timer.Start();
      }

      public static void Stop()
      {
         _timer.Stop();
      }
   }
}