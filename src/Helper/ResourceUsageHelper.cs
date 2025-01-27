using System.Diagnostics;
using Editor.Forms;
using Timer = System.Windows.Forms.Timer;

#nullable enable

namespace Editor.Helper
{
   public static class ResourceUsageHelper
   {
      private static float _memoryUsage;
      private static float _cpuUsage;

      private static PerformanceCounter? cpuUsage;
      private static PerformanceCounter? memoryUsage;

      private static Timer Updater { get; set; } = null!;
      private static string _appName = string.Empty;

      // Initialize the resource usage helper by setting the application name and starting the timer
      // Add PerformanceCounters for CPU and memory usage
      public static void Initialize(MapWindow mapWindow)
      {
         _appName = Process.GetCurrentProcess().ProcessName;

         Updater = new(){Interval = 1000 };
         Updater.Tick += OnTimerTick;
         var initThread = new Thread(() =>
         {
            cpuUsage = new ("Process", "% Processor Time", _appName, true);
            memoryUsage = new ("Process", "Private Bytes", _appName, true);
         });
         initThread.Start();
         Updater.Start();
      }

      // Update the CPU and memory usage
      private static void UpdateResources()
      {
         if (cpuUsage == null || memoryUsage == null)
            return;

         _cpuUsage = cpuUsage.NextValue() / Environment.ProcessorCount;
         _memoryUsage = memoryUsage.NextValue() / 1024 / 1024; // Convert bytes to MB
      }

      private static void OnTimerTick(object? state, EventArgs eventArgs)
      {
         UpdateResources(); 
         Globals.MapWindow.RamUsageStrip.Text = _memoryUsage > 1024 ? $"RAM: [{Math.Round(_memoryUsage / 1024, 2):F2} GB]" : $"RAM: [{Math.Round(_memoryUsage)} MB]";
         Globals.MapWindow.CpuUsageStrip.Text = $"CPU: [{Math.Round(_cpuUsage, 2):F2}%]";
      }

      public static void Dispose()
      {
         cpuUsage?.Dispose();
         memoryUsage?.Dispose();
         Updater?.Stop();
         Updater?.Dispose();
      }
   }
}
