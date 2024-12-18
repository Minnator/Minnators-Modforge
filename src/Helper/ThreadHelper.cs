using System.Runtime;

namespace Editor.Helper
{
   public static class ThreadHelper
   {
      public static void StartBWHeapThread()
      {
         Thread backgroundThread = new(() =>
         {
            for (var i = 0; i < 30; i++)
            {
               Thread.Sleep(1000);
               GC.Collect();
               GC.WaitForPendingFinalizers();
            }
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            GC.WaitForPendingFinalizers();
         })
         {
            IsBackground = true // Make it a background thread
         };
         backgroundThread.Start();
      }
   }
}