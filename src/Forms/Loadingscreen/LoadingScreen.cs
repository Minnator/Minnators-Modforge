﻿using System.ComponentModel;
using System.Media;
using System.Reflection;
using Editor.Controls;
using Editor.Loading;
using Editor.Parser;

namespace Editor.Forms.Loadingscreen
{
   public partial class LoadingScreen : Form
   {
      private const bool SKIP_LOADING = true;
      private readonly CustomProgressBar ProgressBar;
      private MemoryStream _ms = null!;

      public LoadingScreen()
      {
         InitializeComponent();

         // Calculate the number of loading stages
        Globals.LoadingStages = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: true, IsSealed: true }) // Static classes
            .Where(t => t.GetCustomAttribute<LoadingAttribute>() != null)
            .ToList().Count;
         

         Globals.LoadingStageChanged += LoadingScreen_LoadingStageChanged;
         
         ProgressBar = new();
         ProgressBar.Dock = DockStyle.Fill;
         tableLayoutPanel1.Controls.Add(ProgressBar, 0, 2);

         LoadingAnimation.Click += TotallyNormalClickEvent;

         StartLoadingAnimation();

         StartPosition = FormStartPosition.CenterScreen;

         LoadButton_Click(null!, null!);
      }

      
      ~LoadingScreen()
      {
         _ms.Dispose();
      }
      
      private void LoadingScreen_LoadingStageChanged(object? sender, int e)
      {
         ProgressBar.Value = Math.Min(100, (int)((float)e / Globals.LoadingStages * 100));
         ProgressBar.Invalidate();
      }

      private void StartLoadingAnimation()
      {
         LoadingAnimation.Image = Properties.Resources.LoadingGif;
         LoadingAnimation.SizeMode = PictureBoxSizeMode.Zoom;
      }
      
      private void LoadButton_Click(object sender, EventArgs e)
      {
         LoadButton.Enabled = false;
         //create a background worker to load the data
         var bw = new BackgroundWorker();
         bw.WorkerReportsProgress = true;
         bw.DoWork += OnBwOnDoWork;
         bw.RunWorkerCompleted += (s, e) => LoadingCompleted();
         bw.ProgressChanged += (s, e) =>
         {
            ProgressBar.Value = Math.Min(100, (int)((float)e.ProgressPercentage / Globals.LoadingStages * 100));
            ProgressBar.Invalidate();
         };
         bw.RunWorkerAsync();

         Globals.MapWindow.StartScreen = Screen.FromControl(this);
      }

      private int count = 0;
      private DateTime end = DateTime.Today;

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
         var progress = 0;

         try
         {
            FactionsLoading.Load();
            bw.ReportProgress(++progress);
            GovernmentMechanicsLoading.Load();
            bw.ReportProgress(++progress);
            LoadEstateModifiers.Load();
            bw.ReportProgress(++progress);
            ModifierParser.Initialize();
            bw.ReportProgress(++progress);
            CustomColorsLoading.Load();
            bw.ReportProgress(++progress);
            DescriptorLoading.Load();
            bw.ReportProgress(++progress);
            GraphicalCulturesLoading.Load();
            bw.ReportProgress(++progress);
            ScriptedEffectLoading.Load();
            bw.ReportProgress(++progress);
            GovernmentReformsLoading.Load();
            bw.ReportProgress(++progress);
            GovernmentLoading.Load();
            bw.ReportProgress(++progress);
            RiverLoading.Load();
            bw.ReportProgress(++progress);
            ModifierLoading.Load();
            bw.ReportProgress(++progress);
            LocalisationLoading.Load();
            bw.ReportProgress(++progress);
            TradeGoodsLoading.Load();
            bw.ReportProgress(++progress);
            TechnologyGroupsLoading.Load();
            bw.ReportProgress(++progress);
            BuildingsLoading.Load();
            bw.ReportProgress(++progress);
            TradeCompanyInvestmentsLoading.Load();
            bw.ReportProgress(++progress);
            ReligionLoading.Load();
            bw.ReportProgress(++progress);
            MapLoading.Load();
            bw.ReportProgress(++progress);
            DefaultMapLoading.Load();
            bw.ReportProgress(++progress);
            PositionsLoading.Load();
            bw.ReportProgress(++progress);
            CultureLoading.Load();
            bw.ReportProgress(++progress);
            ProvinceParser.ParseAllUniqueProvinces(); // Also marked as loading
            bw.ReportProgress(++progress);
            AreaLoading.Load();
            bw.ReportProgress(++progress);
            TradeNodeLoading.Load();
            bw.ReportProgress(++progress);
            RegionLoading.Load();
            bw.ReportProgress(++progress);
            SuperRegionLoading.Load();
            bw.ReportProgress(++progress);
            ContinentLoading.Load();
            bw.ReportProgress(++progress);
            ScopeParser.GenerateRuntimeScopes(); // Also marked as loading
            bw.ReportProgress(++progress);
            TradeCompanyLoading.Load();
            bw.ReportProgress(++progress);
            ColonialRegionsLoading.Load();
            bw.ReportProgress(++progress);
            CountryLoading.Load();
            bw.ReportProgress(++progress);
            AdjacenciesLoading.Load();
            bw.ReportProgress(++progress);
            UnitTypeLoading.Load();
            bw.ReportProgress(++progress);
            IdeasLoading.Load();
            bw.ReportProgress(++progress);

            // Disable loading specific stuff
            ModifierParser.Demilitarize(); // Also marked as loading
            bw.ReportProgress(++progress);
         }
         catch (Exception exception)
         {
            // Create a message box to inform the user that an error occurred
            MessageBox.Show(exception.Message + "\n\nTry restarting the application. There ia a run condition error in the code I could not find yet. Restarting resolves it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
         }
         

         GC.Collect();
         Globals.LoadingStage = progress;
      }

      private void LoadingCompleted()
      {
         Globals.MapWindow.Initialize();
         Close();
      }
   }
}