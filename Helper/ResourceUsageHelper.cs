using System;
using System.Diagnostics;
using System.Threading;

namespace Editor.Helper
{
   public static class ResourceUsageHelper
   {
      private static float _memoryUsage;
      private static float _cpuUsage;

      private static MapWindow _mapWindow = null!;
      private static PerformanceCounter? cpuUsage;
      private static PerformanceCounter? memoryUsage;

      public static Timer Updater { get; set; } = null!;
      private static string _appName = string.Empty;

      public static void Initialize(MapWindow mapWindow)
      {
         _appName = Process.GetCurrentProcess().ProcessName;
         
         _mapWindow = mapWindow;
         Updater = new Timer(OnTimerTick, null, 0, 1000);
         cpuUsage = new PerformanceCounter("Process", "% Processor Time", _appName, true);
         memoryUsage = new PerformanceCounter("Process", "Private Bytes", _appName, true);
      }

      public static void UpdateResources()
      {
         if (cpuUsage == null || memoryUsage == null)
            return;

         _cpuUsage = cpuUsage.NextValue() / Environment.ProcessorCount;
         _memoryUsage = memoryUsage.NextValue() / 1024 / 1024; // Convert bytes to MB
      }

      private static void OnTimerTick(object sender)
      {
         UpdateResources();

         if (_mapWindow.Disposing || _mapWindow.IsDisposed)
            Updater.Dispose();

         _mapWindow.UpdateMemoryUsage(_memoryUsage);
         _mapWindow.UpdateCpuUsage(_cpuUsage);
      }

      public static void Dispose()
      {
         Updater.Dispose();
      }
   }
}
