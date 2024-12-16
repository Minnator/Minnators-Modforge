using System.ComponentModel;

namespace Editor.DataClasses.Settings
{
   public class MetricDummySetting : SubSettings
   {
      private int _numOfStarts = 0;
      private List<int> _startTimes = [];
      private List<List<KeyValuePair<string, int>>> _operationLoadingTimes = new();
      private bool _enableLoadingMetrics = true;
      private int _maxNumOfMetrics = 5;

      [Description("Determines if the loading metrics should be enabled, If true the last 10 loading times will be saved")]
      [CompareInEquals]
      public bool EnableLoadingMetrics
      {
         get => _enableLoadingMetrics;
         set => SetField(ref _enableLoadingMetrics, value);
      }

      [Description("The number of times the application has been started")]
      [CompareInEquals]
      [ReadOnly(true)]
      public int NumOfStarts
      {
         get => _numOfStarts;
         set => SetField(ref _numOfStarts, value);
      }

      [Description("The total time the application has been started")]
      [CompareInEquals]
      [ReadOnly(true)]
      public List<int> StartTimes
      {
         get => _startTimes;
         set => SetField(ref _startTimes, value);
      }

      [Description("The amount of time it took each loading operation to complete")]
      [CompareInEquals]
      [ReadOnly(true)]
      public List<List<KeyValuePair<string, int>>> OperationLoadingTimes   
      {
         get => _operationLoadingTimes;
         set => SetField(ref _operationLoadingTimes, value);
      }

      [Description("The maximum number of metrics that will be saved")]
      [CompareInEquals]
      [ReadOnly(true)]
      public int MaxNumOfMetrics
      {
         get => _maxNumOfMetrics;
         set => SetField(ref _maxNumOfMetrics, value);
      }
   }
}