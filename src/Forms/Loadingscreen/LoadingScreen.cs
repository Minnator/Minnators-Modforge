﻿using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using Editor.Controls;
using Editor.Helper;
using Editor.Loading;
using Editor.Loading.Enhanced;
using Editor.Parser;
using DefaultMapLoading = Editor.Loading.Enhanced.DefaultMapLoading;
using MapLoading = Editor.Loading.Enhanced.MapLoading;
using RegionLoading = Editor.Loading.Enhanced.RegionLoading;
using SuperRegionLoading = Editor.Loading.Enhanced.SuperRegionLoading;
using TradeNodeLoading = Editor.Loading.TradeNodeLoading;

namespace Editor.Forms.LoadingScreen
{
   public partial class LoadingScreen : Form
   {
      private readonly CustomProgressBar _progressBar;
      private readonly MemoryStream _ms = null!;

      public int LoadingStage
      {
         get => _loadingStage;
         set
         {
            _loadingStage = value;
            LoadingStageChanged.Invoke(this, value);
         }
      }

      public EventHandler<int> LoadingStageChanged = delegate{};
      private readonly List<Action> _loadingActions =
      [
         StartUpMetrics.StartMetrics,
         UnitTypeLoading.Load,
         DefinesLoading.Load,
         BookMarksLoading.Load,
         SettingsHelper.LoadSettingsToComponents,
         FactionsLoading.Load,
         GovernmentMechanicsLoading.Load,
         LoadEstateModifiers.Load,
         ModifierParser.Initialize,
         CustomColorsLoading.Load,
         DescriptorLoading.Load,
         GraphicalCulturesLoading.Load,
         ScriptedEffectLoading.Load,
         GovernmentReformsLoading.Load,
         GovernmentLoading.Load,
         // Insert here
         RiverLoading.Load,
         ModifierLoading.Load,
         LocalisationLoading.Load,
         TradeGoodsLoading.Load,
         TechnologyGroupsLoading.Load,
         BuildingsLoading.Load,
         TradeCompanyInvestmentsLoading.Load,
         ReligionLoading.Load,
         DefinitionLoading.LoadDefinition,
         CultureLoading.Load,
         CountryLoading.Load,
         DefaultMapLoading.Load,
         MapLoading.Load,
         PositionsLoading.Load,
         ProvinceParser.ParseAllUniqueProvinces,
         TerrainLoading.Load, // Requires Provinces
         ClimateLoading.Load,
         AreaParsing.Load,
         TradeNodeLoading.Load,
         RegionLoading.Load,
         SuperRegionLoading.Load,
         ContinentLoading.Load,
         ScopeParser.GenerateRuntimeScopes,
         TradeCompanyLoading.Load,
         ColonialRegionsLoading.Load,
         CustomProvinceNames.Load,
         AdjacenciesLoading.Load,
         IdeasLoading.Load,
         AutoTerrainCalculations.Load,
         HeightMapLoading.Load,
         ProvinceGroupsLoading.Load,
         GameIcon.Initialize,
         Eu4Cursors.LoadCursors,


         // Must be last
         ModifierParser.Demilitarize,
      ];

      public LoadingScreen()
      {
         InitializeComponent();

         Globals.LoadingStages = _loadingActions.Count;
         LoadingStageChanged += LoadingScreen_LoadingStageChanged;
         
         _progressBar = new();
         _progressBar.Dock = DockStyle.Fill;
         _progressBar.TaskCount = Globals.LoadingStages;
         _progressBar.DisplayStyle = ProgressBarDisplayText.CustomPercentage;
         tableLayoutPanel1.Controls.Add(_progressBar, 0, 1);

         LoadingAnimation.Click += TotallyNormalClickEvent;

         StartLoadingAnimation();

         StartPosition = FormStartPosition.CenterScreen;

         //create a background worker to load the data
         var bw = new BackgroundWorker();
         bw.WorkerReportsProgress = true;
         bw.DoWork += OnBwOnDoWork;
         bw.RunWorkerCompleted += (s, e) => LoadingCompleted();
         bw.ProgressChanged += (s, e) =>
         {
            LoadingStage++;
         };
         bw.RunWorkerAsync();

         Globals.MapWindow.StartScreen = Screen.FromControl(this);
      }

      
      ~LoadingScreen()
      {
         _ms.Dispose();
      }
      
      private void LoadingScreen_LoadingStageChanged(object? sender, int e)
      {
         var percent = (int)(((double)e / Globals.LoadingStages) * 100);
         _progressBar.Value = Math.Min(100, percent);
         if (e < _loadingActions.Count)
            _progressBar.CustomText = _loadingActions[e].Method.DeclaringType?.Name ?? string.Empty;
         _progressBar.TaskCompletedCount = e;
         _progressBar.Invalidate();
      }

      private void StartLoadingAnimation()
      {
         LoadingAnimation.Image = Properties.Resources.LoadingGif;
         LoadingAnimation.SizeMode = PictureBoxSizeMode.Zoom;
      }
      
      private int count;
      private DateTime end = DateTime.Today;
      private int _loadingStage;

      private void TotallyNormalClickEvent(object? s, EventArgs e)
      {
         if (count == 0)
         {
            end = DateTime.Now.AddSeconds(2);
            count = 1;
            return;
         }

         if (DateTime.Now < end)
         {
            if (count < 2)
            {
               count += 1;
               return;
            }

            using var player = new SoundPlayer(Properties.Resources.BonkSoundEffect);
            player.Play();
         }
         count = 0;
      }

      private void OnBwOnDoWork(object? s, DoWorkEventArgs e)
      {
         if (s is not BackgroundWorker bw)
            return;
         var sw = Stopwatch.StartNew();
         var progress = 0;

         try
         {
            foreach (var task in _loadingActions)
            {
               sw.Restart();
               task.Invoke();
               sw.Stop();
               if (Globals.Settings.Metrics.EnableLoadingMetrics)
                  StartUpMetrics.AddOperationLoadingTime(task.Method.DeclaringType?.Name ?? "Unknown", (int)sw.ElapsedMilliseconds);
               Globals.LoadingLog.WriteTimeStamp(task.Method.DeclaringType?.Name ?? "Unknown", sw.ElapsedMilliseconds);
               bw.ReportProgress(++progress);
            }
         }
         catch (Exception exception)
         {
            CrashManager.EnterCrashHandler(exception);
            throw;
         }
      }

      private void LoadingCompleted()
      {
         StartUpMetrics.EndMetrics();
         Globals.MapWindow.Initialize();
         Close();
      }
   }
}
