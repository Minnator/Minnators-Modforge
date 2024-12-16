using Editor.DataClasses.Settings;
using Editor.Saving;

namespace Editor.Helper
{
   public static class StartUpMetrics
   {
      private static List<KeyValuePair<string, int>>? _operationLoadingTimes = [];
      private static List<int>? _startTimes = [];


      /// <summary>
      /// Starts the metrics and initializes the static variables
      /// </summary>
      public static void StartMetrics()
      {
         _operationLoadingTimes ??= [];
         _startTimes ??= [];

         _startTimes = Globals.Settings.Metrics.StartTimes;

         if (_startTimes.Count >= Globals.Settings.Metrics.MaxNumOfMetrics) 
            _startTimes.RemoveAt(0);
      }

      /// <summary>
      /// Adds the operation loading time to the list
      /// </summary>
      /// <param name="operationName"></param>
      /// <param name="time"></param>
      /// <exception cref="EvilActions"></exception>
      public static void AddOperationLoadingTime(string operationName, int time)
      {
         if (_operationLoadingTimes == null)
            throw new EvilActions("Metrics list should never be called before Starting the metrics!");

         _operationLoadingTimes.Add(new(operationName, time));
      }

      /// <summary>
      /// Adds the total operation time to the list and cleans the static variables
      /// </summary>
      /// <param name="clean"></param>
      /// <exception cref="EvilActions"></exception>
      public static void EndMetrics(bool clean = true)
      {
         if (_startTimes == null || _operationLoadingTimes == null)
            throw new EvilActions("Metrics list should never be called before Starting the metrics!");
         
         _startTimes.Add(_operationLoadingTimes.Sum(x => x.Value));
         
         if (Globals.Settings.Metrics.OperationLoadingTimes.Count >= Globals.Settings.Metrics.MaxNumOfMetrics)
            Globals.Settings.Metrics.OperationLoadingTimes.RemoveAt(0);
         Globals.Settings.Metrics.OperationLoadingTimes.Add(_operationLoadingTimes);

         Globals.Settings.Metrics.StartTimes = _startTimes;
         Globals.Settings.Metrics.NumOfStarts++;

         SettingsSaver.Save(Globals.Settings);

         if (clean)
            Clean();
      }

      /// <summary>
      /// Cleans the static variables
      /// </summary>
      public static void Clean()
      {
         _operationLoadingTimes = null;
         _startTimes = null;
      }

   }
}