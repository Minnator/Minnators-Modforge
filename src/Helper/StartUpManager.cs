using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.Achievements;
using Editor.DataClasses.Commands;
using Editor.DataClasses.MapModes;
using Editor.ErrorHandling;
using Editor.Forms.Loadingscreen;
using Editor.Loading;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Helper
{
   public static class StartUpManager
   {
      public static event EventHandler OnStartUpFinished = delegate {};

      internal static Timer RuntimeUpdateTimer = new(){Interval = 60000 };
      internal static readonly Stopwatch Sw;

      static StartUpManager()
      {
         Sw = new ();
         RuntimeUpdateTimer.Tick += (sender, args) =>
         {
            Globals.Settings.Misc.RunTime += Sw.Elapsed;
            Sw.Restart();
         };
      }

      public static void StartUp()
      {
         Sw.Start();
         RuntimeUpdateTimer.Start();
         RunLoadingScreen();

         SetProvinceInitials();

         Globals.MapWindow.Initialize();

         HistoryManager.UndoDepthChanged += Globals.MapWindow.UpdateUndoDepth;
         HistoryManager.RedoDepthChanged += Globals.MapWindow.UpdateRedoDepth;

         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(Globals.MapWindow);
         HistoryManager.UpdateToolStrip();
         Globals.MapWindow.SetSelectedProvinceSum(0);

         // ALL LOADING COMPLETE - Set the application to running
         Globals.MapWindow.DateControl.Date = new(1444, 11, 11);
         CountryLoading.AssignProvinces();
         Globals.MapWindow.UpdateErrorCounts(LogManager.TotalErrorCount, LogManager.TotalLogCount);
         Globals.State = State.Running;
         MapModeManager.SetCurrentMapMode(MapModeType.Country);
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);

         Globals.MapWindow.Show();
         Globals.MapWindow.Activate();
         Globals.ZoomControl.Invalidate();

         RaiseOnStartUpFinished();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, Globals.MapWindow);
      }


      private static void RunLoadingScreen()
      {
         LoadingScreen ls = new();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Loading, Cursors.AppStarting, ls);
         ls.ShowDialog();
      }

      private static void SetProvinceInitials()
      {
         foreach (var province in Globals.Provinces)
            province.SetInit();
      }

      internal static void SetRunTimes()
      {
         Sw.Stop();
         Globals.Settings.Misc.RunTime += Sw.Elapsed;
         RuntimeUpdateTimer.Stop();
      }

      private static void RaiseOnStartUpFinished()
      {
         ThreadHelper.StartBWHeapThread();
         AchievementManager.IncreaseAchievementProgress(1, AchievementId.UseForTheFirstTime);
         StartUpMetrics.EndMetrics();

         OnStartUpFinished?.Invoke(null, EventArgs.Empty);
      }
   }
}