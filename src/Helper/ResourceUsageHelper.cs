﻿using System.Diagnostics;
using Editor.Forms;
using Timer = System.Threading.Timer;

#nullable enable

namespace Editor.Helper
{
   public static class ResourceUsageHelper
   {
      private static float _memoryUsage;
      private static float _cpuUsage;

      private static MapWindow _mapWindow = null!;
      private static PerformanceCounter? cpuUsage;
      private static PerformanceCounter? memoryUsage;

      private static Timer Updater { get; set; } = null!;
      private static string _appName = string.Empty;

      // Initialize the resource usage helper by setting the application name and starting the timer
      // Add PerformanceCounters for CPU and memory usage
      public static void Initialize(MapWindow mapWindow)
      {
         _appName = Process.GetCurrentProcess().ProcessName;

         _mapWindow = mapWindow;
         Updater = new(OnTimerTick, null, 0, 1000);
         //var sw = Stopwatch.StartNew();
         var initThread = new Thread(() =>
         {
            cpuUsage = new ("Process", "% Processor Time", _appName, true);
            memoryUsage = new ("Process", "Private Bytes", _appName, true);
         });
         initThread.Start();
         //sw.Stop();
         //Debug.WriteLine("ResourceUsageHelper took: " + sw.ElapsedMilliseconds + "ms");
      }

      // Update the CPU and memory usage
      private static void UpdateResources()
      {
         if (cpuUsage == null || memoryUsage == null)
            return;

         _cpuUsage = cpuUsage.NextValue() / Environment.ProcessorCount;
         _memoryUsage = memoryUsage.NextValue() / 1024 / 1024; // Convert bytes to MB
      }

      private static void OnTimerTick(object? state)
      {
         Task.Run(() =>
         {
            UpdateResources();

            // If the window is disposed, stop updating
            if (_mapWindow.Disposing || _mapWindow.IsDisposed)
            {
               Updater.Dispose();
               return;
            }


            _mapWindow.UpdateMemoryUsage(_memoryUsage);
            _mapWindow.UpdateCpuUsage(_cpuUsage);
         });
      }

      public static void Dispose()
      {
         Updater?.Dispose();
      }
   }
}
