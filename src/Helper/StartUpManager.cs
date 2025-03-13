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
      
      private static bool _doEvents = true;

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
         Globals.State = State.Loading;
         Sw.Start();
         RuntimeUpdateTimer.Start();

         Globals.Random = new(Globals.Settings.Generator.RandomSeed);

         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Loading, Cursors.AppStarting, ls);
         ls = new();
         ls.Show();

         ls.LoadingComplete += (sender, args) =>
         {
            _doEvents = false;
            // Initialize everything AFTER loading completes
            
            Globals.MapWindow.Initialize();

            HistoryManager.UndoDepthChanged += Globals.MapWindow.UpdateUndoDepth;
            HistoryManager.RedoDepthChanged += Globals.MapWindow.UpdateRedoDepth;

            Globals.LoadingLog.Close();
            ResourceUsageHelper.Initialize(Globals.MapWindow);
            HistoryManager.UpdateToolStrip();
            Globals.MapWindow.SetSelectedProvinceSum(0);

            ParseStartEndDate();
            Globals.MapWindow.DateControl.Date = Globals.StartDate;
            CountryLoading.AssignProvinces();
            Globals.MapWindow.UpdateErrorCounts(LogManager.TotalErrorCount, LogManager.TotalLogCount);

            ls.Invoke(() => ls.Close()); // Close safely on UI thread
            
            Globals.MapWindow.Show();
            Globals.MapWindow.Activate();
            
            FixComboBoxSelection(Globals.MapWindow);
               

            Globals.State = State.Running;
            MapModeManager.SetCurrentMapMode(MapModeType.Country);
            Globals.ZoomControl.FocusOn(new(3100, 600), 1f);

            RaiseOnStartUpFinished();
            Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, Globals.MapWindow);
         };


         // Keep UI responsive while waiting for LoadingScreen
         while (ls.Visible)
         {
            if (_doEvents)
               Application.DoEvents();
            Thread.Sleep(10); // Avoid 100% CPU usage
         }
      }

      private static void FixComboBoxSelection(Control parent)
      {
         Queue<Control> queue = new();
         queue.Enqueue(parent);

         while (queue.Count > 0)
         {
            var current = queue.Dequeue();

            if (current is ComboBox comboBox)
            {
               if (!comboBox.IsHandleCreated)
                  continue; // Skip if handle is not created

               comboBox.SelectionLength = 0;
               comboBox.SelectionStart = comboBox.Text.Length;
            }

            foreach (Control child in current.Controls)
               queue.Enqueue(child);
         }
      }


      private static void ParseStartEndDate()
      {
         if (!Date.TryParse(Globals.Defines["NDefines.NGame.START_DATE"].GetValueAsText.TrimQuotes(), out Globals.StartDate).Log()) 
            Globals.StartDate = new(1444, 11, 11);
         if (!Date.TryParse(Globals.Defines["NDefines.NGame.END_DATE"].GetValueAsText.TrimQuotes(), out Globals.EndDate).Log())
            Globals.EndDate = new(1821, 1, 1);
      }

      internal static void SetProvinceInitials()
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