using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using Editor.Controls;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms;
using Editor.Forms.Loadingscreen;
using Editor.Helper;
using Editor.Loading;
using Editor.Savers;

namespace Editor
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private ItemList _claims;
      private ItemList _permanentClaims;
      private ItemList _cores;
      private ItemList _buildings;
      private ItemList _discoveredBy;

      private ExtendedComboBox _religionComboBox;
      private ExtendedComboBox _cultureComboBox;

      private ExtendedNumeric _taxNumeric;
      private ExtendedNumeric _prodNumeric;
      private ExtendedNumeric _manpNumeric;

      private ExtendedNumeric _autonomyNumeric;
      private ExtendedNumeric _devastationNumeric;
      private ExtendedNumeric _prosperityNumeric;
      private ExtendedNumeric _extraCostNumeric;

      #endregion

      public PannablePictureBox MapPictureBox = null!;
      public readonly DateControl DateControl = new(DateTime.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen ls;

      public readonly ModProject Project = new()
      {
         Name = "DAVID",
         ModPath = Consts.MOD_PATH,
         VanillaPath = Consts.VANILLA_PATH
      };

      public MapWindow()
      {
         Globals.State = State.Loading;
         Globals.MapWindow = this;
         RunLoadingScreen();
      }

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();
         InitGui();


         // MUST BE LAST in the loading sequence
         LoadingManager.InitMapModes(this);
         Globals.LoadingStage++;
         LoadingManager.InitializeComponents(this);
         Globals.LoadingStage++;


         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         Globals.LoadingStage++;
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);

         // ALL LOADING COMPLETE - Set the application to running

         DateControl.Date = new(1444, 11, 11);
         Globals.State = State.Running;
         Globals.LoadingStage++;
         MapModeComboBox.SelectedIndex = 11;
         ResumeLayout();
         Globals.LoadingStage++;
         Show();
         MapPictureBox.FocusOn(new(3100, 600));

         AfterLoad();
      }

      private void AfterLoad()
      {
      }

      private void RunLoadingScreen()
      {
         ls = new();
         ls.ShowDialog();
      }

      // Loads the map into the created PannablePictureBox
      private void InitGui()
      {
         InitializeComponent();
         MapPictureBox = ControlFactory.GetPannablePictureBox(ref MapPanel, this);
         MapPanel.Controls.Add(MapPictureBox);
         Globals.Selection = new(MapPictureBox);

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         ProvincePreviewMode.Items.AddRange([.. Enum.GetNames(typeof(ProvinceEditingStatus))]);
         ProvincePreviewMode.SelectedIndex = 2;

      }

      private void InitializeEditGui()
      {
         OwnerTagBox = ControlFactory.GetTagComboBox();
         OwnerTagBox.OnTagChanged += ProvinceEditingEvents.OnOwnerChanged;
         ControllerTagBox = ControlFactory.GetTagComboBox();
         ControllerTagBox.OnTagChanged += ProvinceEditingEvents.OnControllerChanged;
         OwnerControllerLayoutPanel.Controls.Add(OwnerTagBox, 1, 0);
         OwnerControllerLayoutPanel.Controls.Add(ControllerTagBox, 1, 1);

         _cores = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Cores");
         _cores.OnItemAdded += ProvinceEditingEvents.OnCoreAdded;
         _cores.OnItemRemoved += ProvinceEditingEvents.OnCoreRemoved;
         _claims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Regular");
         _claims.OnItemAdded += ProvinceEditingEvents.OnClaimAdded;
         _claims.OnItemRemoved += ProvinceEditingEvents.OnClaimRemoved;
         _permanentClaims = ControlFactory.GetItemList(ItemTypes.Tag, [.. Globals.Countries.Keys], "Permanent");
         _permanentClaims.OnItemAdded += ProvinceEditingEvents.OnPermanentClaimAdded;
         _permanentClaims.OnItemRemoved += ProvinceEditingEvents.OnPermanentClaimRemoved;
         //TODO: Implement PermanentClaims.OnItemAdded += ProvinceEditingEvents.OnPermanentClaimAdded;

         _buildings = ControlFactory.GetItemListObjects(ItemTypes.String, [.. Globals.Buildings], "Building");
         _buildings.OnItemAdded += ProvinceEditingEvents.OnBuildingAdded;
         _buildings.OnItemRemoved += ProvinceEditingEvents.OnBuildingRemoved;
         _discoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups], "TechGroup");
         _discoveredBy.OnItemAdded += ProvinceEditingEvents.OnDiscoveredByAdded;
         _discoveredBy.OnItemRemoved += ProvinceEditingEvents.OnDiscoveredByRemoved;

         TradeCenterComboBox.SelectedIndexChanged += ProvinceEditingEvents.OnTradeCenterChanged;

         CoresAndClaimLayoutPanel.Controls.Add(_permanentClaims, 0, 0);
         CoresAndClaimLayoutPanel.Controls.Add(_claims, 1, 0);
         CoresGroupBox.Controls.Add(_cores);
         _cores.Location = new(0, 18);
         BuildingsGroupBox.Controls.Add(_buildings);
         _buildings.Location = new(0, 18);
         DiscoveredByGroupBox.Controls.Add(_discoveredBy);
         _discoveredBy.Location = new(0, 18);

         List<string> tradeGoodsString = [.. Globals.TradeGoods.Keys];
         tradeGoodsString.Sort();
         TradeGoodsComboBox.Items.AddRange([.. tradeGoodsString]);
         TradeGoodsComboBox.SelectedIndexChanged += ProvinceEditingEvents.OnTradeGoodChanged;

         List<string> culturesString = [.. Globals.Cultures.Keys];
         culturesString.Sort();
         _cultureComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(_cultureComboBox, 1, 1);
         _cultureComboBox.Items.AddRange([.. culturesString]);
         _cultureComboBox.OnDataChanged += ProvinceEditingEvents.OnCultureChanged;

         List<string> religionsString = [.. Globals.Religions.Keys];
         religionsString.Sort();
         _religionComboBox = ControlFactory.GetExtendedComboBox();
         RCCLayoutPanel.Controls.Add(_religionComboBox, 1, 0);
         _religionComboBox.Items.AddRange([.. religionsString]);
         _religionComboBox.OnDataChanged += ProvinceEditingEvents.OnReligionChanged;

         _taxNumeric = ControlFactory.GetExtendedNumeric();
         _taxNumeric.Minimum = 0;
         _taxNumeric.Maximum = 1000;
         _taxNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseTaxChanged;
         _taxNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseTaxChanged;
         _taxNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpTaxButtonButtonPressedMedium;
         _taxNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpBaseTaxButtonPressedLarge;
         _taxNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseTaxChanged;
         _taxNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownBaseTaxButtonPressedMedium;
         _taxNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownBaseTaxButtonPressedLarge;
         DevelopmentLayoutPanel.Controls.Add(_taxNumeric, 1, 0);

         _prodNumeric = ControlFactory.GetExtendedNumeric();
         _prodNumeric.Minimum = 0;
         _prodNumeric.Maximum = 1000;
         _prodNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseProductionChanged;
         _prodNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseProductionChanged;
         _prodNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProduction;
         _prodNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProduction;
         _prodNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseProductionChanged;
         _prodNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProduction;
         _prodNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProduction;
         DevelopmentLayoutPanel.Controls.Add(_prodNumeric, 1, 1);

         _manpNumeric = ControlFactory.GetExtendedNumeric();
         _manpNumeric.Minimum = 0;
         _manpNumeric.Maximum = 1000;
         _manpNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseManpowerChanged;
         _manpNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseManpowerChanged;
         _manpNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumManpower;
         _manpNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeManpower;
         _manpNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseManpowerChanged;
         _manpNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumManpower;
         _manpNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeManpower;
         DevelopmentLayoutPanel.Controls.Add(_manpNumeric, 1, 2);

         _autonomyNumeric = ControlFactory.GetExtendedNumeric();
         _autonomyNumeric.Minimum = 0;
         _autonomyNumeric.Maximum = 100;
         _autonomyNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextLocalAutonomyChanged;
         _autonomyNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpLocalAutonomyChanged;
         _autonomyNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumLocalAutonomy;
         _autonomyNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeLocalAutonomy;
         _autonomyNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownLocalAutonomyChanged;
         _autonomyNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumLocalAutonomy;
         _autonomyNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeLocalAutonomy;
         FloatLayoutPanel.Controls.Add(_autonomyNumeric, 1, 0);

         _devastationNumeric = ControlFactory.GetExtendedNumeric();
         _devastationNumeric.Minimum = 0;
         _devastationNumeric.Maximum = 100;
         _devastationNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextDevastationChanged;
         _devastationNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpDevastationChanged;
         _devastationNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumDevastation;
         _devastationNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeDevastation;
         _devastationNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownDevastationChanged;
         _devastationNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumDevastation;
         _devastationNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeDevastation;
         FloatLayoutPanel.Controls.Add(_devastationNumeric, 1, 1);

         _prosperityNumeric = ControlFactory.GetExtendedNumeric();
         _prosperityNumeric.Minimum = 0;
         _prosperityNumeric.Maximum = 100;
         _prosperityNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextProsperityChanged;
         _prosperityNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpProsperityChanged;
         _prosperityNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProsperity;
         _prosperityNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProsperity;
         _prosperityNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownProsperityChanged;
         _prosperityNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProsperity;
         _prosperityNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProsperity;
         FloatLayoutPanel.Controls.Add(_prosperityNumeric, 1, 2);

         _extraCostNumeric = ControlFactory.GetExtendedNumeric();
         _extraCostNumeric.Minimum = 0;
         _extraCostNumeric.Maximum = 1000;
         _extraCostNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextExtraCostChanged;
         _extraCostNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpExtraCostChanged;
         _extraCostNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumExtraCost;
         _extraCostNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeExtraCost;
         _extraCostNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownExtraCostChanged;
         _extraCostNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumExtraCost;
         _extraCostNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeExtraCost;
         TradePanel.Controls.Add(_extraCostNumeric, 1, 2);

         CapitalNameTextBox.Leave += ProvinceEditingEvents.OnCapitalNameChanged;

         IsCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsCityChanged;
         IsHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsHreChanged;
         IsParlimentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnIsSeatInParliamentChanged;
         HasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnHasRevoltChanged;

         AttirbuteCombobox.Items.AddRange([.. Enum.GetNames(typeof(ProvAttr))]);
      }

      // ======================== Province GUI Update Methods ========================
      #region Province Gui
      /// <summary>
      /// This will only load the province attributes to the gui which are shared by all provinces
      /// </summary>
      public void LoadSelectedProvincesToGui()
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         SuspendLayout();
         ClearProvinceGui();
         if (Globals.Selection.GetSharedAttribute(ProvAttr.claims, out var result) && result is List<Tag> tags)
            _claims.AddItemsUnique([.. tags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.permanent_claims, out result) && result is List<Tag> permanentTags)
            _permanentClaims.AddItemsUnique([.. permanentTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.cores, out result) && result is List<Tag> coreTags)
            _cores.AddItemsUnique([.. coreTags]);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.buildings, out result) && result is List<string> buildings)
            _buildings.AddItemsUnique(buildings);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.discovered_by, out result) && result is List<string> techGroups)
            _discoveredBy.AddItemsUnique(techGroups);
         if (Globals.Selection.GetSharedAttribute(ProvAttr.owner, out result) && result is Tag owner)
            OwnerTagBox.Text = owner;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.controller, out result) && result is Tag controller)
            ControllerTagBox.Text = controller;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.religion, out result) && result is string religion)
            _religionComboBox.Text = religion;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.culture, out result) && result is string culture)
            _cultureComboBox.Text = culture;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.capital, out result) && result is string capital)
            CapitalNameTextBox.Text = capital;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.is_city, out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.hre, out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.seat_in_parliament, out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.revolt, out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_tax, out result) && result is int baseTax)
            _taxNumeric.Value = baseTax;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_production, out result) && result is int baseProduction)
            _prodNumeric.Value = baseProduction;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.base_manpower, out result) && result is int baseManpower)
            _manpNumeric.Value = baseManpower;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.local_autonomy, out result) && result is float localAutonomy)
            _autonomyNumeric.Value = (int)localAutonomy;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.devastation, out result) && result is float devastation)
            _devastationNumeric.Value = (int)devastation;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.prosperity, out result) && result is float prosperity)
            _prosperityNumeric.Value = (int)prosperity;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.trade_good, out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (Globals.Selection.GetSharedAttribute(ProvAttr.center_of_trade, out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (Globals.Selection.GetSharedAttribute(ProvAttr.extra_cost, out result) && result is int extraCost)
            _extraCostNumeric.Value = extraCost;
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      public void LoadProvinceToGui(Province province)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         SuspendLayout();
         ClearProvinceGui();
         OwnerTagBox.Text = province.Owner;
         ControllerTagBox.Text = province.Controller;
         _religionComboBox.Text = province.Religion;
         _cultureComboBox.Text = province.Culture;
         CapitalNameTextBox.Text = province.Capital;
         IsCityCheckBox.Checked = province.IsCity;
         IsHreCheckBox.Checked = province.IsHre;
         IsParlimentSeatCheckbox.Checked = province.IsSeatInParliament;
         HasRevoltCheckBox.Checked = province.HasRevolt;
         _taxNumeric.Value = province.BaseTax;
         _prodNumeric.Value = province.BaseProduction;
         _manpNumeric.Value = province.BaseManpower;
         _claims.AddItemsUnique([.. province.Claims]);
         _permanentClaims.AddItemsUnique([]); //TODO what is wrong here why no Province.PermanentClaims
         _cores.AddItemsUnique([.. province.Cores]);
         _buildings.AddItemsUnique(province.Buildings);
         _discoveredBy.AddItemsUnique(province.DiscoveredBy);
         _autonomyNumeric.Value = (int)province.LocalAutonomy;
         _devastationNumeric.Value = (int)province.Devastation;
         _prosperityNumeric.Value = (int)province.Prosperity;
         TradeGoodsComboBox.Text = province.TradeGood;
         TradeCenterComboBox.Text = province.CenterOfTrade.ToString();
         _extraCostNumeric.Value = province.ExtraCost;
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      public void ClearProvinceGui()
      {
         OwnerTagBox.Clear();
         ControllerTagBox.Clear();
         _religionComboBox.Clear();
         _cultureComboBox.Clear();
         CapitalNameTextBox.Clear();
         IsCityCheckBox.Checked = false;
         IsHreCheckBox.Checked = false;
         IsParlimentSeatCheckbox.Checked = false;
         HasRevoltCheckBox.Checked = false;
         _taxNumeric.Value = 1;
         _prodNumeric.Value = 1;
         _manpNumeric.Value = 1;
         _claims.Clear();
         _permanentClaims.Clear();
         _cores.Clear();
         _buildings.Clear();
         _discoveredBy.Clear();
         _autonomyNumeric.Value = 0;
         _devastationNumeric.Value = 0;
         _prosperityNumeric.Value = 0;
         TradeGoodsComboBox.Clear();
         TradeCenterComboBox.Clear();
         _extraCostNumeric.Value = 0;
      }
      #endregion


      #region ToolStrip update methods
      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"ProvSum: [{sum}]";
      }

      #region Resource Updater MethodInvokation

      public void UpdateMemoryUsage(float memoryUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { RamUsageStrip.Text = $"RAM: [{Math.Round(memoryUsage)} MB]"; }));
      }

      public void UpdateCpuUsage(float cpuUsage)
      {
         if (InvokeRequired) Invoke(new MethodInvoker(delegate { CpuUsageStrip.Text = $"CPU: [{Math.Round(cpuUsage, 2):F2}%]"; }));
      }

      #endregion

      #region HistoryManager Event Handlers

      public void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      public void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #endregion

      public void SetEditingMode()
      {
         EditingModeLabel.Text = Globals.Selection.Count <= 1
            ? "Idle Mode: Single Province"
            : $"Idle Mode: Multi Province ({Globals.Selection.Count})";
      }

      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         ResourceUsageHelper.Dispose();
      }

      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(Globals.HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(Globals.HistoryManager.GetRoot());
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.HistoryManager.Clear();
      }

      #endregion

      private void OnDateChanged(object? sender, EventArgs e)
      {
         ProvinceHistoryManager.LoadDate(Globals.Date);
      }

      public void UpdateHoveredInfo(Province? province)
      {
         if (province == null)
         {
            ProvinceNameLabel.Text = "Province: -";
            OwnerCountryNameLabel.Text = "Owner: -";
            return;
         }
         ProvinceNameLabel.Text = $"Province: {province.GetLocalisation()}";
         OwnerCountryNameLabel.Text = $"Owner: {Localisation.GetLoc(province.Owner)}";
      }

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (MapPictureBox.IsPainting)
            return;
         Globals.MapModeManager.SetCurrentMapMode(MapModeComboBox.SelectedItem.ToString());
         GC.Collect(); // We force the garbage collector to collect the old bitmap
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
         MapPictureBox.Image.Save($@"{downloadFolder}{MapModeComboBox.SelectedItem}.png", ImageFormat.Png);
      }

      private void openCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var toolTipCustomizer = new ToolTipCustomizer();
         toolTipCustomizer.Show();
      }

      private void ShowToolTipMenuItem_Click(object sender, EventArgs e)
      {
         MapPictureBox.ShowToolTip = ShowToolTipMenuItem.Checked;
      }

      private void testToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //var content = File.ReadAllText("C:\\Users\\david\\Downloads\\NestedBLocks.txt");
         var content = File.ReadAllText("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\common\\cultures\\00_cultures.txt");
         var sw = Stopwatch.StartNew();
         var blocks = Parsing.GetElements(0, ref content);
         sw.Stop();
         Debug.WriteLine("Parsing cultures took: " + sw.ElapsedMilliseconds + "ms");

         var sb = new StringBuilder();
         foreach (var block in blocks)
         {
            DebugPrints.BuildBlockString(0, block, ref sb);
         }
         File.WriteAllText("C:\\Users\\david\\Downloads\\NestedBLocksOutput2.txt", sb.ToString());
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.TelescopeImageBenchmark();
      }

      private void MapWindow_KeyDown(object sender, KeyEventArgs e)
      {
         if (ModifierKeys == Keys.Control)
         {
            switch (e.KeyCode)
            {
               case Keys.F:
                  Globals.SearchForm = FormHelper.OpenOrBringToFront(Globals.SearchForm);
                  break;
               case Keys.Z:
                  Globals.HistoryManager.Undo();
                  break;
               case Keys.Y:
                  Globals.HistoryManager.Redo();
                  break;
            }
         }

         switch (e.KeyCode)
         {
            case Keys.F1:
               Globals.ConsoleForm = FormHelper.OpenOrBringToFront(Globals.ConsoleForm);
               break;
         }
      }

      private void refStackToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.GridMap();
      }

      private void DateSelector_SelectedIndexChanged(object sender, EventArgs e)
      {

      }

      private void MapWindow_Load(object sender, EventArgs e)
      {
      }

      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormHelper.OpenOrBringToFront(Globals.SearchForm);
      }

      private void bestPointsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DebugMaps.TestCenterPoints();
      }

      private void provDiffToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var res = EditingManager.GetModifiedProvinces();

         Debug.WriteLine($"Modified Provinces: {res.Count}");
         foreach (var province in res)
         {
            Debug.WriteLine(province.Id);
         }
      }

      private void ProvincePreviewMode_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.ProvinceEditingStatus = (ProvinceEditingStatus)ProvincePreviewMode.SelectedIndex;
         // Close the menu strip// Close the menu when an item is selected
         filesToolStripMenuItem.DropDown.Close();
      }

      private void MagicWandToolButton_Click(object sender, EventArgs e)
      {
         Globals.Selection.State = Globals.Selection.State != SelectionState.MagicWand ? SelectionState.MagicWand : SelectionState.Single;
      }

      private void yoloToolStripMenuItem_Click(object sender, EventArgs e)
      {
         string[] dateStrings = { "1.1.1", "9999.11.11", "2024.09.16" };

         foreach (string dateString in dateStrings)
         {
            if (Parsing.TryParseDate(dateString, out var dateValue))
            {
               Debug.WriteLine($"Parsed date: {dateValue:yyyy-MM-dd}");
            }
            else
            {
               Debug.WriteLine($"Failed to parse the date: {dateString}");
            }
         }
      }


      private void saveAllProvincesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProvinceSaver.SaveAllLandProvinces();
      }

      private void save1ToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.Provinces[269].SaveToHistoryFile();
      }

      private void saveEuropeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         foreach (var id in Globals.Continents["europe"].Provinces)
         {
            Globals.Provinces[id].SaveToHistoryFile();
         }
      }
   }
}
