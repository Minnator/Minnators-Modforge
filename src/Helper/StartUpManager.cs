using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.Achievements;
using Editor.DataClasses.Commands;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Forms.Loadingscreen;
using Editor.Loading;
using Editor.Parser;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Helper
{
   public static class StartUpManager
   {
      public static event EventHandler OnStartUpFinished = delegate {};

      internal static Timer RuntimeUpdateTimer = new(){Interval = 60000 };
      internal static readonly Stopwatch Sw;
      private static LoadingScreen ls;
      static StartUpManager()
      {
         Sw = new ();
         RuntimeUpdateTimer.Tick += (sender, args) =>
         {
            Globals.Settings.Misc.RunTime += Sw.Elapsed;
            Sw.Restart();
         };
         RuntimeUpdateTimer.Tick += (sender, args) =>
         {
            AchievementManager.IncreaseAchievementProgress(1, AchievementId.UsedFor1hour);
            AchievementManager.IncreaseAchievementProgress(1, AchievementId.UsedFor10hours);
            AchievementManager.IncreaseAchievementProgress(1, AchievementId.UsedFor100hours);
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
         ParseStartEndDate();
         Globals.MapWindow.DateControl.Date = Globals.StartDate;
         CountryLoading.AssignProvinces();
         Globals.MapWindow.UpdateErrorCounts(LogManager.TotalErrorCount, LogManager.TotalLogCount);
         Globals.State = State.Running;
         MapModeManager.SetCurrentMapMode(MapModeType.Country);
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);

         //ls.Close();
         Globals.MapWindow.Show();
         Globals.MapWindow.Activate();
         Globals.ZoomControl.Invalidate();

         RaiseOnStartUpFinished();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, Globals.MapWindow);
      }

      private static void ParseStartEndDate()
      {
         if (!Date.TryParse(Globals.Defines["NDefines.NGame.START_DATE"].GetValueAsText.TrimQuotes(), out Globals.StartDate).Log()) 
            Globals.StartDate = new(1444, 11, 11);
         if (!Date.TryParse(Globals.Defines["NDefines.NGame.END_DATE"].GetValueAsText.TrimQuotes(), out Globals.EndDate).Log())
            Globals.EndDate = new(1821, 1, 1);
      }

      private static void RunLoadingScreen()
      {
         ls = new();
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