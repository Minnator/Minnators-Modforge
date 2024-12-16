using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Media;
using System.Runtime;
using System.Text;
using Editor.Controls;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Forms.Feature.Crash_Reporter;
using Editor.Forms.Feature.SavingClasses;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading;
using Editor.Saving;
using Editor.src.Forms.GetUserInput;
using Editor.src.Forms.PopUps;
using Editor.Testing;
using static Editor.Helper.ProvinceEnumHelper;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Forms
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private ItemList _claims = null!;
      private ItemList _permanentClaims = null!;
      private ItemList _cores = null!;
      private ItemList _buildings = null!;
      private ItemList _discoveredBy = null!;

      private ExtendedComboBox _religionComboBox = null!;
      private ExtendedComboBox _cultureComboBox = null!;
      public ExtendedComboBox ModifierComboBox = null!;
      private ExtendedComboBox _modifierTypeComboBox = null!;

      private ExtendedNumeric _taxNumeric = null!;
      private ExtendedNumeric _prdNumeric = null!;
      private ExtendedNumeric _mnpNumeric = null!;

      private ExtendedNumeric _autonomyNumeric = null!;
      private ExtendedNumeric _devastationNumeric = null!;
      private ExtendedNumeric _prosperityNumeric = null!;
      private ExtendedNumeric _extraCostNumeric = null!;

      private TagComboBox _tribalOwner = null!;
      private ExtendedComboBox _tradeCompanyInvestments = null!;
      private ExtendedComboBox _terrainComboBox = null!;

      private TextSaveableTextBox<string, CProvinceAttributeChange> _nativesSizeTextBox = null!;
      private TextSaveableTextBox<string, CProvinceAttributeChange> _nativeFerocityTextBox = null!;
      private TextSaveableTextBox<string, CProvinceAttributeChange> _nativeHostilityTextBox = null!;

      private TextSaveableTextBox<string, CModifyLocalisation> _localisationTextBox = null!;
      private TextSaveableTextBox<string, CModifyLocalisation> _provAdjTextBox = null!;
      private TextSaveableTextBox<string, CProvinceAttributeChange> _capitalNameTextBox = null!;

      private ToolTip _savingButtonsToolTip = null!;

      private CollectionEditor2<Area, Province> _areaEditingGui = null!;
      private CollectionEditor2<Region, Area> _regionEditingGui = null!;
      private CollectionEditor2<SuperRegion, Region> _superRegionEditingGui = null!;
      private CollectionEditor2<TradeCompany, Province> _tradeCompanyEditingGui = null!;
      private CollectionEditor2<Country, Province> _countryEditingGui = null!;
      private CollectionEditor2<TradeNode, Province> _tradeNodeEditingGui = null!;
      private CollectionEditor2<ProvinceGroup, Province> _provinceGroupsEditingGui = null!;
      private CollectionEditor2<ColonialRegion, Province> _colonialRegionEditingGui = null!;

      public Screen? StartScreen = null;

      #endregion

      public readonly DateControl DateControl = new(Date.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen.LoadingScreen _ls = null!;

      public MapWindow()
      {
         Globals.State = State.Loading;
         Globals.MapWindow = this;

         Localisation.Initialize();
         Globals.Random = new(Globals.Settings.Misc.RandomSeed);

         RunLoadingScreen();
         StartBWHeapThread();

         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, this);

         if (StartScreen != null)
         {
            // Center the new form on the StartScreen; Cannot use StartPosition.CenterScreen because it centers on the current screen not the screen it was started on
            var screen = Globals.MapWindow.StartScreen;
            if (screen != null)
               Location = new(screen.Bounds.X + (screen.Bounds.Width - Width) / 2, screen.Bounds.Y + (screen.Bounds.Height - Height) / 2);
         }

         DiscordActivityManager.ActivateActivity();
      }

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();

         InitGui();
         Text = $"{Text} | {Globals.DescriptorData.Name}";

         // MUST BE LAST in the loading sequence
         InitMapModes();
         Globals.HistoryManager.UndoDepthChanged += UpdateUndoDepth!;
         Globals.HistoryManager.RedoDepthChanged += UpdateRedoDepth!;


         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);

         // ALL LOADING COMPLETE - Set the application to running

         DateControl.Date = new(1444, 11, 11);
         CountryLoading.AssignProvinces();
         Globals.State = State.Running;
         Globals.MapModeManager.SetCurrentMapMode(MapModeType.Country);
         ResumeLayout();

         StartPosition = FormStartPosition.CenterScreen;
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);
         Show();

         // Activate this window
         Activate();
         Globals.ZoomControl.Invalidate();
         AfterLoad();

      }

      private void StartBWHeapThread()
      {
         Thread backgroundThread = new(() =>
         {
            for (var i = 0; i < 30; i++)
            {
               Thread.Sleep(1000);
               GC.Collect();
               GC.WaitForPendingFinalizers();
            }
         })
         {
            IsBackground = true // Make it a background thread
         };
         backgroundThread.Start();
      }

      private void AfterLoad()
      {
         GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
         GC.Collect();
         GC.WaitForPendingFinalizers();

      }

      public void InitMapModes()
      {
         Globals.MapModeManager = new(); // Initialize the MapModeManager
         MapModeComboBox.Items.Clear();
         MapModeComboBox.Items.AddRange([.. Enum.GetNames<MapModeType>()]);
      }

      private void RunLoadingScreen()
      {
         _ls = new();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Loading, Cursors.AppStarting, _ls);
         _ls.ShowDialog();
      }

      private void InitGui()
      {
         InitializeComponent();

         Globals.HistoryManager.UndoEvent += (sender, args) => UpdateGui();
         Globals.HistoryManager.RedoEvent += (sender, args) => UpdateGui();

         Globals.ZoomControl = new(new(Globals.MapWidth, Globals.MapHeight))
         {
            BorderWidth = Globals.Settings.Rendering.MapBorderWidth,
            Border = Globals.Settings.Rendering.ShowMapBorder,
            BorderColor = Globals.Settings.Rendering.MapBorderColor,
            MinVisiblePixels = Globals.Settings.Rendering.MinVisiblePixels,
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         MapLayoutPanel.Controls.Add(Globals.ZoomControl, 0, 0);
         Selection.Initialize();
         GuiDrawing.Initialize();

         TopStripLayoutPanel.Controls.Add(DateControl, 4, 0);
         DateControl.OnDateChanged += OnDateChanged;

         SelectionTypeBox.Items.AddRange([.. Enum.GetNames<SelectionType>()]);
         SelectionTypeBox.SelectedIndex = 0;

         // Initalize Settings Events and Listeners
         SettingsHelper.InitializeEvent();
      }

      private void UpdateGui()
      {
         var currentTab = DataTabPanel.SelectedIndex;
         switch (currentTab)
         {
            case 0:
               UpdateProvinceTab();
               break;
            case 1:
               UpdateCountryTab();
               break;
            case 2:
               UpdateProvinceCollectionTab();
               break;
         }
      }

      private void UpdateProvinceTab() => LoadSelectedProvincesToGui();
      private void UpdateCountryTab() => LoadCountryToGui(Selection.SelectedCountry);

      private void UpdateProvinceCollectionTab()
      {

      }

      #region EditGui
      /// <summary>
      /// Initializes all Tabs of the EditGui
      /// </summary>
      private void InitializeEditGui()
      {
         InitializeProvinceEditGui();
         InitializeProvinceCollectionEditGui();
         InitializeCountryEditGui();
         InitializeMapModeButtons();
         InitializeInfoStrip();
      }

      private void InitializeInfoStrip()
      {
         Globals.MapModeManager.MapModeChanged += (sender, mode) =>
         {
            MapModeTimesInfo.Text = $"MapMode times [ms]: last: {Globals.MapModeManager.LasMapModeTime} | Min: {Globals.MapModeManager.MinMapModeTime} | Max: {Globals.MapModeManager.MaxMapModeTime} | Avg: {Globals.MapModeManager.AverageMapModeTime}";
         };
         MapModeTimesInfo.ToolTipText =
            "Time only counts in the time it takes the Rendering Pipeline to render the map and Invalidate the control.\nOther GUI updates are not included.";
         RamUsageStrip.ToolTipText = "Current RAM usage of the application\nThis is inflated by HeapAllocation from the GC and will free memory if your PC needs some.";
         UndoDepthLabel.ToolTipText = "Undo stack depth\nThis is the amount of actions you can undo/redo";
         RedoDepthLabel.ToolTipText = "Redo stack depth\nThis is the amount of actions you can redo/undo";
         CpuUsageStrip.ToolTipText = "Current CPU usage of the application";
         SelectedProvinceSum.ToolTipText = "Sum of selected provinces";
      }

      private void InitializeMapModeButtons()
      {
         var button01 = ControlFactory.GetMapModeButton('q', 0);
         button01.Margin = new(0, 0, 3, 0);
         var button21 = ControlFactory.GetMapModeButton('w', 1);
         var button31 = ControlFactory.GetMapModeButton('e', 2);
         var button41 = ControlFactory.GetMapModeButton('r', 3);
         var button5 = ControlFactory.GetMapModeButton('t', 4);
         var button6 = ControlFactory.GetMapModeButton('y', 5);
         var button7 = ControlFactory.GetMapModeButton('u', 6);
         var button8 = ControlFactory.GetMapModeButton('i', 7);
         var button9 = ControlFactory.GetMapModeButton('o', 8);
         var button10 = ControlFactory.GetMapModeButton('p', 9);
         button10.Margin = new(3, 0, 0, 0);
         MMButtonsTLPanel.Controls.Add(button01, 0, 0);
         MMButtonsTLPanel.Controls.Add(button21, 1, 0);
         MMButtonsTLPanel.Controls.Add(button31, 2, 0);
         MMButtonsTLPanel.Controls.Add(button41, 3, 0);
         MMButtonsTLPanel.Controls.Add(button5, 4, 0);
         MMButtonsTLPanel.Controls.Add(button6, 5, 0);
         MMButtonsTLPanel.Controls.Add(button7, 6, 0);
         MMButtonsTLPanel.Controls.Add(button8, 7, 0);
         MMButtonsTLPanel.Controls.Add(button9, 8, 0);
         MMButtonsTLPanel.Controls.Add(button10, 9, 0);

      }

      private void InitializeProvinceCollectionEditGui()
      {
         _countryEditingGui = new(ItemTypes.Id, SaveableType.Country, SaveableType.Province, MapModeType.Country);
         Country.ItemsModified += _countryEditingGui.OnCorrespondingDataChange;
         _countryEditingGui._extendedComboBox.DataSource = new BindingSource
         {
            DataSource = Globals.Countries
         };
         _countryEditingGui._extendedComboBox.DisplayMember = "Key";
         _countryEditingGui._extendedComboBox.ValueMember = "Value";

         _areaEditingGui = new(ItemTypes.Id, SaveableType.Area, SaveableType.Province, MapModeType.Area);
         Area.ItemsModified += _areaEditingGui.OnCorrespondingDataChange;

         _regionEditingGui = new(ItemTypes.String, SaveableType.Region, SaveableType.Area, MapModeType.Regions);
         DataClasses.GameDataClasses.Region.ItemsModified += _regionEditingGui.OnCorrespondingDataChange;

         _superRegionEditingGui = new(ItemTypes.String, SaveableType.SuperRegion, SaveableType.Region, MapModeType.SuperRegion);
         SuperRegion.ItemsModified += _superRegionEditingGui.OnCorrespondingDataChange;

         _tradeCompanyEditingGui = new(ItemTypes.Id, SaveableType.TradeCompany, SaveableType.Province, MapModeType.TradeCompany);
         TradeCompany.ItemsModified += _tradeCompanyEditingGui.OnCorrespondingDataChange;

         _tradeNodeEditingGui = new(ItemTypes.Id, SaveableType.TradeNode, SaveableType.Province, MapModeType.TradeNode);
         TradeNode.ItemsModified += _tradeNodeEditingGui.OnCorrespondingDataChange;

         _provinceGroupsEditingGui = new(ItemTypes.Id, SaveableType.ProvinceGroup, SaveableType.Province, MapModeType.ProvinceGroup);
         ProvinceGroup.ItemsModified += _provinceGroupsEditingGui.OnCorrespondingDataChange;

         _colonialRegionEditingGui = new(ItemTypes.Id, SaveableType.ColonialRegion, SaveableType.Province, MapModeType.ColonialRegions);
         ColonialRegion.ItemsModified += _colonialRegionEditingGui.OnCorrespondingDataChange;

         ProvinceCollectionsLayoutPanel.Controls.Add(_countryEditingGui, 0, 0);
         ProvinceCollectionsLayoutPanel.Controls.Add(_regionEditingGui, 0, 2);
         ProvinceCollectionsLayoutPanel.Controls.Add(_areaEditingGui, 0, 1);
         ProvinceCollectionsLayoutPanel.Controls.Add(_superRegionEditingGui, 0, 3);
         ProvinceCollectionsLayoutPanel.Controls.Add(_tradeCompanyEditingGui, 0, 5);
         ProvinceCollectionsLayoutPanel.Controls.Add(_tradeNodeEditingGui, 0, 4);
         ProvinceCollectionsLayoutPanel.Controls.Add(_provinceGroupsEditingGui, 0, 6);
         ProvinceCollectionsLayoutPanel.Controls.Add(_colonialRegionEditingGui, 0, 7);

         FocusSelectionCheckBox.Checked = Globals.Settings.Gui.JumpToSelectedProvinceCollection;
      }


      private void InitializeProvinceEditGui()
      {

         DataTabPanel.TabPages[0].Enabled = false;
         Selection.OnProvinceSelectionChange += (sender, i) =>
         {


            if (i == 0)
            {
               DataTabPanel.TabPages[0].SuspendLayout();
               Globals.EditingStatus = EditingStatus.LoadingInterface;
               ClearProvinceGui();
               Globals.EditingStatus = EditingStatus.Idle;
               DataTabPanel.TabPages[0].Enabled = false;
               DataTabPanel.TabPages[0].ResumeLayout();
            }
            else
            {
               DataTabPanel.TabPages[0].Enabled = true;
               LoadSelectedProvincesToGui();
            }
         };

         DataTabPanel.TabPages[1].Enabled = false;
         Selection.OnCountrySelectionChange += (_, _) =>
         {
            if (Selection.SelectedCountry == Country.Empty)
            {
               DataTabPanel.TabPages[1].SuspendLayout();
               Globals.EditingStatus = EditingStatus.LoadingInterface;
               ClearCountryGui();
               Globals.EditingStatus = EditingStatus.Idle;
               DataTabPanel.TabPages[1].Enabled = false;
               DataTabPanel.TabPages[1].ResumeLayout();
            }
            else
            {
               DataTabPanel.TabPages[1].Enabled = true;
               LoadCountryToGui(Selection.SelectedCountry);
            }
         };

         // CustomTextBox
         _localisationTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CLocObjFactory(true))
         {
            Margin = new(3, 1, 3, 3),
            Dock = DockStyle.Fill
         };
         LocTableLayoutPanel.Controls.Add(_localisationTextBox, 1, 1);

         _provAdjTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CLocObjFactory(false))
         {
            Margin = new(3, 1, 3, 3),
            Dock = DockStyle.Fill
         };
         LocTableLayoutPanel.Controls.Add(_provAdjTextBox, 1, 3);

         // Tooltips for saving Buttons
         _savingButtonsToolTip = new();
         _savingButtonsToolTip.AutoPopDelay = 10000;
         _savingButtonsToolTip.InitialDelay = 100;
         _savingButtonsToolTip.ShowAlways = true;

         SaveAllModifiedButton.MouseEnter += OnSavingModifiedEnter;
         SaveAllProvincesButton.MouseEnter += OnSavingAllEnter;
         SaveCurrentSelectionButton.MouseEnter += OnSavingSelectionEnter;

         OwnerTagBox = ControlFactory.GetTagComboBox();
         OwnerTagBox.OnTagChanged += ProvinceEditingEvents.OnOwnerChanged;
         ControllerTagBox = ControlFactory.GetTagComboBox();
         ControllerTagBox.OnTagChanged += ProvinceEditingEvents.OnControllerChanged;
         MisProvinceData.Controls.Add(OwnerTagBox, 1, 0);
         MisProvinceData.Controls.Add(ControllerTagBox, 1, 1);

         _cores = ControlFactory.GetItemList(ItemTypes.Tag, [], "Cores");
         _cores.OnItemAdded += ProvinceEditingEvents.OnCoreAdded;
         _cores.OnItemRemoved += ProvinceEditingEvents.OnCoreRemoved;
         _claims = ControlFactory.GetItemList(ItemTypes.Tag, [], "Regular");
         _claims.OnItemAdded += ProvinceEditingEvents.OnClaimAdded;
         _claims.OnItemRemoved += ProvinceEditingEvents.OnClaimRemoved;
         _permanentClaims = ControlFactory.GetItemList(ItemTypes.Tag, [], "Permanent");
         _permanentClaims.OnItemAdded += ProvinceEditingEvents.OnPermanentClaimAdded;
         _permanentClaims.OnItemRemoved += ProvinceEditingEvents.OnPermanentClaimRemoved;

         _buildings = ControlFactory.GetItemListObjects(ItemTypes.String, [.. Globals.Buildings], "Building");
         _buildings.OnItemAdded += ProvinceEditingEvents.OnBuildingAdded;
         _buildings.OnItemRemoved += ProvinceEditingEvents.OnBuildingRemoved;
         _discoveredBy = ControlFactory.GetItemList(ItemTypes.String, [.. Globals.TechnologyGroups.Keys], "TechGroup");
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
         MisProvinceData.Controls.Add(_cultureComboBox, 1, 3);
         _cultureComboBox.Items.AddRange([.. culturesString]);
         _cultureComboBox.OnDataChanged += ProvinceEditingEvents.OnCultureChanged;

         List<string> religionsString = [.. Globals.Religions.Keys];
         religionsString.Sort();
         _religionComboBox = ControlFactory.GetExtendedComboBox();
         MisProvinceData.Controls.Add(_religionComboBox, 1, 2);
         _religionComboBox.Items.AddRange([.. religionsString]);
         _religionComboBox.OnDataChanged += ProvinceEditingEvents.OnReligionChanged;

         List<string> terrainString = [string.Empty];
         terrainString.AddRange([.. Globals.Terrains.Select(x => x.Name).ToList()]);
         terrainString.Sort();
         _terrainComboBox = ControlFactory.GetExtendedComboBox();
         _terrainComboBox.Items.AddRange([.. terrainString]);
         _terrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         _terrainComboBox.OnDataChanged += (_, args) =>
         {
            if (args.Value is not string val)
               return;

            if (val.Equals(string.Empty))
            {
               Terrain.RemoveFromAll(args.Provinces);
            }
            else
            {
               Terrain.GetTerrainFroString(val).SetOverride(args.Provinces);
            }
         };
         MisProvinceData.Controls.Add(_terrainComboBox, 1, 5);

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
         MisProvinceData.Controls.Add(_taxNumeric, 3, 0);

         _prdNumeric = ControlFactory.GetExtendedNumeric();
         _prdNumeric.Minimum = 0;
         _prdNumeric.Maximum = 1000;
         _prdNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseProductionChanged;
         _prdNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseProductionChanged;
         _prdNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumProduction;
         _prdNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeProduction;
         _prdNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseProductionChanged;
         _prdNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumProduction;
         _prdNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeProduction;
         MisProvinceData.Controls.Add(_prdNumeric, 3, 1);

         _mnpNumeric = ControlFactory.GetExtendedNumeric();
         _mnpNumeric.Minimum = 0;
         _mnpNumeric.Maximum = 1000;
         _mnpNumeric.OnTextValueChanged += ProvinceEditingEvents.OnTextBaseManpowerChanged;
         _mnpNumeric.UpButtonPressedSmall += ProvinceEditingEvents.OnUpBaseManpowerChanged;
         _mnpNumeric.UpButtonPressedMedium += ProvinceEditingEvents.OnUpButtonPressedMediumManpower;
         _mnpNumeric.UpButtonPressedLarge += ProvinceEditingEvents.OnUpButtonPressedLargeManpower;
         _mnpNumeric.DownButtonPressedSmall += ProvinceEditingEvents.OnDownBaseManpowerChanged;
         _mnpNumeric.DownButtonPressedMedium += ProvinceEditingEvents.OnDownButtonPressedMediumManpower;
         _mnpNumeric.DownButtonPressedLarge += ProvinceEditingEvents.OnDownButtonPressedLargeManpower;
         MisProvinceData.Controls.Add(_mnpNumeric, 3, 2);

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

         _capitalNameTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CProvinceAttributeFactory(ProvAttrGet.capital, ProvAttrSet.capital))
         {
            Dock = DockStyle.Fill,
            Margin = new(3, 1, 3, 3),
            Input = InputType.Text,
         };
         MisProvinceData.Controls.Add(_capitalNameTextBox, 1, 4);

         IsCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsCityChanged;
         IsHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnIsHreChanged;
         IsParlimentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnIsSeatInParliamentChanged;
         HasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnHasRevoltChanged;

         MWAttirbuteCombobox.Items.AddRange([.. Enum.GetNames<ProvAttrGet>()]);

         // NATIVES TAB
         _tribalOwner = ControlFactory.GetTagComboBox();
         _tribalOwner.OnTagChanged += ProvinceEditingEvents.OnTribalOwnerChanged;
         NativesLayoutPanel.Controls.Add(_tribalOwner, 1, 0);

         _nativesSizeTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CProvinceAttributeFactory(ProvAttrGet.native_size, ProvAttrSet.native_size))
         {
            Input = InputType.UnsignedNumber
         };
         NativesLayoutPanel.Controls.Add(_nativesSizeTextBox, 1, 1);

         _nativeFerocityTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CProvinceAttributeFactory(ProvAttrGet.native_ferocity, ProvAttrSet.native_ferocity))
         {
            Input = InputType.DecimalNumber
         };
         NativesLayoutPanel.Controls.Add(_nativeFerocityTextBox, 1, 3);

         _nativeHostilityTextBox = new(Selection.GetSelectedProvincesAsSaveable, new CProvinceAttributeFactory(ProvAttrGet.native_hostileness, ProvAttrSet.native_hostileness))
         {
            Input = InputType.UnsignedNumber
         };
         NativesLayoutPanel.Controls.Add(_nativeHostilityTextBox, 1, 2);

         // TRADE_COMPANIES TAB
         _tradeCompanyInvestments = ControlFactory.GetExtendedComboBox();
         _tradeCompanyInvestments.Items.AddRange([.. Globals.TradeCompanyInvestments.Keys]);
         _tradeCompanyInvestments.OnDataChanged += ProvinceEditingEvents.OnTradeCompanyInvestmentChanged;
         TradeCompaniesLayoutPanel.Controls.Add(_tradeCompanyInvestments, 1, 0);

         // MODIFIERS TAB
         InitializeModifierTab();

         // Localisation GroupBox

      }

      #endregion

      private void InitializeModifierTab()
      {
         ModifierComboBox = ControlFactory.GetExtendedComboBox();
         ModifierComboBox.Items.AddRange([.. Globals.EventModifiers.Keys]);
         // No data changed here as they are added via the "Add" button
         ModifiersLayoutPanel.Controls.Add(ModifierComboBox, 1, 1);

         _modifierTypeComboBox = ControlFactory.GetExtendedComboBox(["CountryModifier", "ProvinceModifier"]);
         _modifierTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         _modifierTypeComboBox.Margin = new(2, 1, 2, 0);
         ModTypeSubLayout.Controls.Add(_modifierTypeComboBox, 0, 0);

         ModifiersListView.View = View.Details;
         ModifiersListView.FullRowSelect = true;
         ModifiersListView.Columns.Add("Name");
         ModifiersListView.Columns.Add("Duration");
         ModifiersListView.Columns.Add("Type");
         // Space the columns out
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
         ExtendedComboBox.AllowEvents = false;
         if (Selection.GetSharedAttribute(ProvAttrGet.claims, out var result) && result is List<Tag> tags)
            _claims.AddItemsUnique([.. tags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.permanent_claims, out result) && result is List<Tag> permanentTags)
            _permanentClaims.AddItemsUnique([.. permanentTags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.cores, out result) && result is List<Tag> coreTags)
            _cores.AddItemsUnique([.. coreTags]);
         if (Selection.GetSharedAttribute(ProvAttrGet.buildings, out result) && result is List<string> buildings)
            _buildings.AddItemsUnique(buildings);
         if (Selection.GetSharedAttribute(ProvAttrGet.discovered_by, out result) && result is List<string> techGroups)
            _discoveredBy.AddItemsUnique(techGroups);
         if (Selection.GetSharedAttribute(ProvAttrGet.owner, out result) && result is Tag owner)
            OwnerTagBox.Text = owner;
         if (Selection.GetSharedAttribute(ProvAttrGet.controller, out result) && result is Tag controller)
            ControllerTagBox.Text = controller;
         if (Selection.GetSharedAttribute(ProvAttrGet.religion, out result) && result is string religion)
            _religionComboBox.Text = religion;
         if (Selection.GetSharedAttribute(ProvAttrGet.culture, out result) && result is string culture)
            _cultureComboBox.Text = culture;
         if (Selection.GetSharedAttribute(ProvAttrGet.capital, out result) && result is string capital)
            _capitalNameTextBox.Text = capital;
         if (Selection.GetSharedAttribute(ProvAttrGet.is_city, out result) && result is bool isCity)
            IsCityCheckBox.Checked = isCity;
         if (Selection.GetSharedAttribute(ProvAttrGet.hre, out result) && result is bool isHre)
            IsHreCheckBox.Checked = isHre;
         if (Selection.GetSharedAttribute(ProvAttrGet.seat_in_parliament, out result) && result is bool isSeatInParliament)
            IsParlimentSeatCheckbox.Checked = isSeatInParliament;
         if (Selection.GetSharedAttribute(ProvAttrGet.revolt, out result) && result is bool hasRevolt)
            HasRevoltCheckBox.Checked = hasRevolt;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_tax, out result) && result is int baseTax)
            _taxNumeric.Value = baseTax;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_production, out result) && result is int baseProduction)
            _prdNumeric.Value = baseProduction;
         if (Selection.GetSharedAttribute(ProvAttrGet.base_manpower, out result) && result is int baseManpower)
            _mnpNumeric.Value = baseManpower;
         if (Selection.GetSharedAttribute(ProvAttrGet.local_autonomy, out result) && result is float localAutonomy)
            _autonomyNumeric.Value = (int)localAutonomy;
         if (Selection.GetSharedAttribute(ProvAttrGet.devastation, out result) && result is float devastation)
            _devastationNumeric.Value = (int)devastation;
         if (Selection.GetSharedAttribute(ProvAttrGet.prosperity, out result) && result is float prosperity)
            _prosperityNumeric.Value = (int)prosperity;
         if (Selection.GetSharedAttribute(ProvAttrGet.trade_good, out result) && result is string tradeGood)
            TradeGoodsComboBox.Text = tradeGood;
         if (Selection.GetSharedAttribute(ProvAttrGet.center_of_trade, out result) && result is int centerOfTrade)
            TradeCenterComboBox.Text = centerOfTrade.ToString();
         if (Selection.GetSharedAttribute(ProvAttrGet.extra_cost, out result) && result is int extraCost)
            _extraCostNumeric.Value = extraCost;
         if (Selection.GetSharedAttribute(ProvAttrGet.tribal_owner, out result) && result is Tag tribalOwner)
            _tribalOwner.Text = tribalOwner;
         if (Selection.GetSharedAttribute(ProvAttrGet.native_size, out result) && result is int nativeSize)
            _nativesSizeTextBox.Text = nativeSize.ToString();
         if (Selection.GetSharedAttribute(ProvAttrGet.native_ferocity, out result) && result is float nativeFerocity)
            _nativeFerocityTextBox.Text = nativeFerocity.ToString(CultureInfo.InvariantCulture);
         if (Selection.GetSharedAttribute(ProvAttrGet.native_hostileness, out result) && result is float nativeHostileness)
            _nativeHostilityTextBox.Text = nativeHostileness.ToString(CultureInfo.InvariantCulture);
         if (Selection.GetSharedAttribute(ProvAttrGet.terrain, out result) && result is Terrain terrain)
            _terrainComboBox.Text = terrain.Name;
         // TODO The Gui needs to be able to represent several trade company investments
         //if (Selection.GetSharedAttribute(ProvAttrGet.trade_company_investment, out result) && result is string tradeCompanyInvestments)
         //   _tradeCompanyInvestments.Text = tradeCompanyInvestments;
         if (Selection.GetSelectedProvinces.Count == 1)
         {
            AddAllModifiersToListView(Selection.GetSelectedProvinces[0]);
            _localisationTextBox.Text = Selection.GetSelectedProvinces[0].GetLocalisation();
            LocalisationLabel.Text = Selection.GetSelectedProvinces[0].GetTitleKey();
            _provAdjTextBox.Text = Localisation.GetLoc(Selection.GetSelectedProvinces[0].GetAdjectiveKey());
            ProvAdjLabel.Text = Selection.GetSelectedProvinces[0].GetAdjectiveKey();
         }
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
         ExtendedComboBox.AllowEvents = true;
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
         _capitalNameTextBox.Text = province.Capital;
         IsCityCheckBox.Checked = province.IsCity;
         IsHreCheckBox.Checked = province.IsHre;
         IsParlimentSeatCheckbox.Checked = province.IsSeatInParliament;
         HasRevoltCheckBox.Checked = province.HasRevolt;
         _taxNumeric.Value = province.BaseTax;
         _prdNumeric.Value = province.BaseProduction;
         _mnpNumeric.Value = province.BaseManpower;
         _claims.AddItemsUnique([.. province.Claims]);
         _permanentClaims.AddItemsUnique([]);
         _cores.AddItemsUnique([.. province.Cores]);
         _buildings.AddItemsUnique(province.Buildings);
         _discoveredBy.AddItemsUnique(province.DiscoveredBy);
         _autonomyNumeric.Value = (int)province.LocalAutonomy;
         _devastationNumeric.Value = (int)province.Devastation;
         _prosperityNumeric.Value = (int)province.Prosperity;
         TradeGoodsComboBox.Text = province.TradeGood;
         TradeCenterComboBox.Text = province.CenterOfTrade.ToString();
         _extraCostNumeric.Value = province.ExtraCost;
         _tribalOwner.Text = province.TribalOwner;
         _nativesSizeTextBox.Text = province.NativeSize.ToString();
         _nativeFerocityTextBox.Text = province.NativeFerocity.ToString(CultureInfo.InvariantCulture);
         _nativeHostilityTextBox.Text = province.NativeHostileness.ToString(CultureInfo.InvariantCulture);
         _terrainComboBox.Text = province.Terrain.Name;
         AddAllModifiersToListView(province);
         ResumeLayout();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      public void ClearProvinceGui()
      {
         ExtendedComboBox.AllowEvents = false;
         OwnerTagBox.Clear();
         ControllerTagBox.Clear();
         _religionComboBox.Clear();
         _cultureComboBox.Clear();
         _capitalNameTextBox.Clear();
         IsCityCheckBox.Checked = false;
         IsHreCheckBox.Checked = false;
         IsParlimentSeatCheckbox.Checked = false;
         HasRevoltCheckBox.Checked = false;
         _taxNumeric.Value = 1;
         _prdNumeric.Value = 1;
         _mnpNumeric.Value = 1;
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
         _tribalOwner.Clear();
         _nativesSizeTextBox.Clear();
         _nativeFerocityTextBox.Clear();
         _nativeHostilityTextBox.Clear();
         ModifierComboBox.Text = string.Empty;
         ModifiersListView.Items.Clear();
         DurationTextBox.Text = string.Empty;
         _tradeCompanyInvestments.Text = string.Empty;
         _localisationTextBox.Clear();
         LocalisationLabel.Text = string.Empty;
         _provAdjTextBox.Clear();
         ProvAdjLabel.Text = string.Empty;
         _terrainComboBox.SelectedItem = string.Empty;
         ExtendedComboBox.AllowEvents = true;
      }
      #endregion

      public void UpdateMapModeButtons(bool buttonInv = true)
      {
         foreach (var mapModeButton in MMButtonsTLPanel.Controls)
         {
            if (mapModeButton is not MapModeButton button)
               continue;

            button.UpdateMapMode(buttonInv);
         }
      }


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

      private void UpdateRedoDepth(object sender, int e) => RedoDepthLabel.Text = $"Redos [{e}]";
      private void UpdateUndoDepth(object sender, int e) => UndoDepthLabel.Text = $"Undos [{e}]";
      #endregion
      #endregion

      public void SetEditingMode()
      {
         EditingModeLabel.Text = Selection.Count <= 1
            ? "Idle Mode: Single Province"
            : $"Idle Mode: Multi Province ({Selection.Count})";
      }
      private void SetFinishEditingOnEnter(TextBox tb, Action<object?, EventArgs> handler)
      {
         tb.KeyDown += (sender, e) =>
         {
            if (e.KeyCode == Keys.Enter)
               handler(sender, e);
         };
      }

      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         ResourceUsageHelper.Dispose();
      }

      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(Globals.HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(Globals.HistoryManager.Root);
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var result = MessageBox.Show("Are you sure you want to delete the history?", "Delete History", MessageBoxButtons.OKCancel);
         if (result == DialogResult.OK)
            Globals.HistoryManager.Clear();
      }

      #endregion

      private void OnSavingAllEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllProvincesButton, $"Save all provinces ({Globals.LandProvinceIds.Length})");
      }

      private void OnSavingModifiedEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveAllModifiedButton, $"Save modified provinces ({SaveMaster.GetNumOfModifiedObjects(SaveableType.Province)})");
      }

      private void OnSavingSelectionEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(SaveCurrentSelectionButton, $"Save selection ({Selection.Count})");
      }

      private static void OnDateChanged(object? sender, Date date)
      {
         ProvinceHistoryManager.LoadDate(date);
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

      private void AddModifiersToList(ModifierAbstract modifier, ModifierType type)
      {
         var item = new ListViewItem(modifier.Name);
         item.SubItems.Add(modifier.Duration.ToString());
         item.SubItems.Add(type.ToString());
         ModifiersListView.Items.Add(item);

         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
         ModifiersListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }

      private void AddAllModifiersToListView(Province province)
      {
         ModifiersListView.Items.Clear();

         foreach (var modifier in province.ProvinceModifiers)
            AddModifiersToList(modifier, ModifierType.ProvinceModifier);

         foreach (var modifier in province.PermanentProvinceModifiers)
            AddModifiersToList(modifier, ModifierType.PermanentProvinceModifier);
      }

      private void debugToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         Globals.MapModeManager.SetCurrentMapMode(Enum.Parse<MapModeType>(MapModeComboBox.SelectedItem?.ToString() ?? string.Empty));
      }

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
         var form = new GetSavingFileForm(pictures, "Where to save the map mode as an image to", ".png");
         form.SetPlaceHolderText(Globals.MapModeManager.CurrentMapMode.GetMapModeName().ToString());
         form.RequireModDirectory = false;
         form.ShowDialog();
         if (form.DialogResult == DialogResult.OK)
         {
            using var bmp = Globals.ZoomControl.Map;
            bmp.Save(form.NewPath, ImageFormat.Png);
         }
      }

      private void openCustomizerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var toolTipCustomizer = new ToolTipCustomizer();
         toolTipCustomizer.Show();
      }

      private void ShowToolTipMenuItem_Click(object sender, EventArgs e)
      {
         Selection.ShowToolTip = ShowToolTipMenuItem.Checked;
      }

      private void telescopeToolStripMenuItem_Click(object sender, EventArgs e)
      {
#if DEBUG
         var sb = new StringBuilder();
         foreach (var province in Globals.Provinces)
         {
            sb.Append($"{province.GetLocalisation()}: ");
            foreach (var building in province.Buildings)
            {
               sb.Append($"{building}, ");
            }
            sb.AppendLine();
         }
         File.WriteAllText(Path.Combine(Globals.DebugPath, "buildings.txt"), sb.ToString());
#endif

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
                  e.SuppressKeyPress = true;
                  e.Handled = true;
                  break;
               // Tabs
               case Keys.D1:
                  DataTabPanel.SelectedIndex = 0;
                  break;
               case Keys.D2:
                  DataTabPanel.SelectedIndex = 1;
                  break;
               case Keys.D3:
                  DataTabPanel.SelectedIndex = 2;
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

      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormHelper.OpenOrBringToFront(Globals.SearchForm);
      }

      private void bestPointsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var tnsSortedTopological = TradeNodeHelper.TopologicalSort(Globals.TradeNodes.Values.ToList());
         DebugPrints.VerifyTopologicalSort(tnsSortedTopological);
      }

      private void provDiffToolStripMenuItem_Click(object sender, EventArgs e)
      {
#if DEBUG
         var sb = new StringBuilder();
         foreach (var collision in Globals.LocalisationCollisions)
         {
            sb.AppendLine($"{collision.Key} : {collision.Value}");
         }

         File.WriteAllText(Path.Combine(Globals.DebugPath, "localisationCollisions.txt"), sb.ToString());
#endif
      }

      private void saveAllProvincesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables([.. Globals.LandProvinces]);
      }

      // PE Button saves all changes
      private void SaveAllModifiedButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables([.. Globals.Provinces], onlyModified: true);
      }

      // Menu item saves all changes
      private void SaveAllMenuItemClick(object sender, EventArgs e)
      {
         var numOfChanges = SaveMaster.GetNumOfModifiedObjects();
         if (numOfChanges == 0)
         {
            MessageBox.Show("No changes to save", "No Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         var result = ImprovedMessageBox.Show($"Are you sure you want to save all {numOfChanges} modified objects?", "Confirm Saving", ref Globals.Settings.PopUps.AskWhenSavingAllChangesRef, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
         if (result == DialogResult.OK)
            SaveMaster.SaveAllChanges();
      }

      private void saveSelectionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new SelectionDrawerForm().Show();
      }

      private void SaveCurrentSelectionButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables([.. Selection.GetSelectedProvinces]);
      }


      private void SaveAllProvincesButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables([.. Globals.LandProvinces]);
      }

      private void saveEuropeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var inputForm = new GetSavingFileForm(Globals.ModPath, "Please enter your input:", ".txt");
         if (inputForm.ShowDialog() == DialogResult.OK)
         {
            string userInput = inputForm.NewPath;
            System.Diagnostics.Debug.WriteLine($"Selected path: {userInput}");
            MessageBox.Show("You entered: " + userInput);
         }

      }
      private void AddModifierButton_Click(object sender, EventArgs e)
      {
         var error = string.Empty;
         if (!int.TryParse(DurationTextBox.Text, out var duration))
            error += "Duration must be a number\n";
         if (ModifierComboBox.Text == string.Empty)
            error += "Modifier must be selected\n";
         if (_modifierTypeComboBox.Text == string.Empty)
            error += "ModifierType must be selected\n";

         if (error != string.Empty)
         {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }

         var mod = new ApplicableModifier(ModifierComboBox.Text, duration);
         var type = Enum.Parse<ModifierType>(_modifierTypeComboBox.Text);
         ProvinceEditingEvents.OnModifierAdded(mod, type);
         AddModifiersToList(mod, type);
      }

      private void DeleteModifierButton_Click(object sender, EventArgs e)
      {
         if (ModifiersListView.SelectedItems.Count == 0)
            return;
         var item = ModifiersListView.SelectedItems[0];
         var modifierName = item.SubItems[0].Text.Trim();

         if (!int.TryParse(item.SubItems[1].Text, out var duration) || !Enum.TryParse(item.SubItems[2].Text, out ModifierType type))
            return;

         if (!Globals.EventModifiers.TryGetValue(modifierName, out _))
            return;

         ProvinceEditingEvents.OnModifierRemoved(new ApplicableModifier(modifierName, duration), type);
         ModifiersListView.Items.Remove(item);
      }

      private void IsaveableClick(object sender, EventArgs e)
      {
      }
      private void yoloToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sw = Stopwatch.StartNew();
         for (var i = 0; i < 100; i++)
         {
            LocalisationLoading.Load();
         }
         sw.Stop();
         System.Diagnostics.Debug.WriteLine($"Time: {sw.ElapsedMilliseconds / 100}");
      }

      private void graphicalElementsManagerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (Globals.GuiDrawings == null || Globals.GuiDrawings.IsDisposed)
            Globals.GuiDrawings = new();
         Globals.GuiDrawings.Show();
      }

      private void OpenAddModifierForm_Click(object sender, EventArgs e)
      {
         new EventModifierForm().ShowDialog();
      }


      private void saveManualToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ManualSaving().ShowDialog();
      }

      private void checkForCyclesInTradenodesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var result = TradeNodeHelper.FindCycle(Globals.TradeNodes.Values.ToList());
         if (result.Count == 0)
            MessageBox.Show("TradeNodes does not contain a cycle", "No Cycle Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
         else
         {
            var sb = new StringBuilder();
            foreach (var tradeNode in result)
            {
               sb.AppendLine($"-> {tradeNode.Name}");
            }
            MessageBox.Show(sb.ToString(), "Cycle Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
         }
      }

      private void helpToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void randomModifierToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ModifierSuggestion().ShowDialog();
      }
      private void crashReportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         CrashManager.EnterCrashHandler(new());
      }

      private void loadDDSFilesTestToolStripMenuItem_Click(object sender, EventArgs e)
      {
#if DEBUG
         var path = @"S:\SteamLibrary\steamapps\common\Europa Universalis IV\gfx\interface\mapmode_military_access.dds";
         var bmp = ImageReader.ReadImage(path);

         bmp.Save(Globals.DebugPath + "\\dds_test.png", ImageFormat.Png);
#endif
      }

      private void roughEditorToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new RoughEditorForm(Globals.Countries["TUR"]).ShowDialog();
      }



      // ------------------- COUNTRY EDITING TAB ------------------- \\
      public TagComboBox _tagSelectionBox = null!;

      internal FlagLabel CountryFlagLabel = null!;

      private ComboBox _graphicalCultureBox = null!;
      private ComboBox _unitTypeBox = null!;
      private ComboBox _techGroupBox = null!;
      private ComboBox _governmentTypeBox = null!;
      private ComboBox _governmentRankBox = null!;
      private ComboBox _primaryCultureBox = null!;
      private ComboBox _focusComboBox = null!;

      private ColorPickerButton _countryColorPickerButton = null!;
      public ThreeColorStripesButton RevolutionColorPickerButton = null!;

      private ItemList _governmentReforms = null!;
      private ItemList _acceptedCultures = null!;

      private NamesEditor _leaderEditor = null!;
      private NamesEditor _shipEditor = null!;
      private NamesEditor _armyEditor = null!;
      private NamesEditor _fleetEditor = null!;

      private TextSaveableTextBox<Province, CModifyProperty<Province>> _capitalTextBox = null!;

      private ExtendedNumeric _countryDevelopmentNumeric = null!;

      private QuickAssignControl<string> _historicalUnits = null!;
      private QuickAssignControl<string> _historicalIdeas = null!;
      private QuickAssignControl<Tag> _historicRivals = null!;
      private QuickAssignControl<Tag> _historicFriends = null!;
      private QuickAssignControl<string> _estatePrivileges = null!;

      private TextSaveableTextBox<string, CModifyLocalisation> CountryLoc = null!;
      private TextSaveableTextBox<string, CModifyLocalisation> CountryADJLoc = null!;

      private void InitializeCountryEditGui()
      {
         Selection.OnCountrySelected += CountryGuiEvents.OnCountrySelected;
         Selection.OnCountryDeselected += CountryGuiEvents.OnCountryDeselected;
         _tagSelectionBox = new()
         {
            Margin = new(1),
            Height = 25,
         };
         CountryFlagLabel = ControlFactory.GetFlagLabel();
         _tagSelectionBox.OnTagChanged += CountryGuiEvents.TagSelectionBox_OnTagChanged;
         _countryColorPickerButton = ControlFactory.GetColorPickerButton();
         _countryColorPickerButton.Click += CountryGuiEvents.CountryColorPickerButton_Click;
         GeneralToolTip.SetToolTip(_countryColorPickerButton, "Set the <color> of the selected country");
         RevolutionColorPickerButton = ControlFactory.GetThreeColorsButton();
         RevolutionColorPickerButton.Margin = new(0);
         RevolutionColorPickerButton.MouseUp += CountryGuiEvents.RevolutionColorPickerButton_Click;

         GeneralToolTip.SetToolTip(RevolutionColorPickerButton, "LMB: Set the <revolutionary_color> of the selected country\nRMB: randomize");
         _graphicalCultureBox = ControlFactory.GetListComboBox(Globals.GraphicalCultures, new(1));
         _graphicalCultureBox.SelectedIndexChanged += CountryGuiEvents.GraphicalCultureBox_SelectedIndexChanged;
         _unitTypeBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         _unitTypeBox.SelectedIndexChanged += CountryGuiEvents.UnitTypeBox_SelectedIndexChanged;
         _techGroupBox = ControlFactory.GetListComboBox([.. Globals.TechnologyGroups.Keys], new(1));
         _techGroupBox.SelectedIndexChanged += CountryGuiEvents.TechGroupBox_SelectedIndexChanged;
         _governmentTypeBox = ControlFactory.GetListComboBox([.. Globals.GovernmentTypes.Keys], new(1, 1, 6, 1));
         _governmentTypeBox.SelectedIndexChanged += GovernmentTypeBox_SelectedIndexChanged;
         _governmentRankBox = ControlFactory.GetListComboBox(["1", "2", "3"], new(1)); // TODO read in the defines to determine range
         _governmentRankBox.SelectedIndexChanged += CountryGuiEvents.GovernmentRankBox_SelectedIndexChanged;
         _governmentReforms = ControlFactory.GetItemList(ItemTypes.FullWidth, [], "Government Reforms");
         _governmentReforms.Width = 117;
         _governmentReforms.OnItemAdded += CountryGuiEvents.GovernmentReforms_OnItemAdded;
         _governmentReforms.OnItemRemoved += CountryGuiEvents.GovernmentReforms_OnItemRemoved;
         _capitalTextBox = new(Selection.GetHistoryCountryAsList, new CCountryPropertyChangeFactory<Province>(nameof(HistoryCountry.Capital)))
         {
            Margin = new(1), Dock = DockStyle.Fill,
            Input = InputType.UnsignedNumber
         };
         
         _focusComboBox = ControlFactory.GetListComboBox([.. Enum.GetNames<Mana>()], new(1), false);
         _focusComboBox.SelectedIndexChanged += CountryGuiEvents.FocusComboBox_SelectedIndexChanged;

         TagAndColorTLP.Controls.Add(_tagSelectionBox, 1, 0);
         TagAndColorTLP.Controls.Add(_countryColorPickerButton, 3, 0);
         TagAndColorTLP.Controls.Add(RevolutionColorPickerButton, 3, 3);
         TagAndColorTLP.Controls.Add(_graphicalCultureBox, 1, 2);
         TagAndColorTLP.Controls.Add(_unitTypeBox, 3, 2);
         TagAndColorTLP.Controls.Add(_techGroupBox, 1, 3);
         CapitalTLP.Controls.Add(_capitalTextBox, 0, 0);
         TagAndColorTLP.Controls.Add(_focusComboBox, 1, 4);
         CountryHeaderTLP.Controls.Add(CountryFlagLabel, 0, 0);

         GovernmentLayoutPanel.Controls.Add(_governmentTypeBox, 3, 0);
         GovernmentLayoutPanel.Controls.Add(_governmentRankBox, 1, 0);
         GovernmentLayoutPanel.Controls.Add(_governmentReforms, 0, 1);
         GovernmentLayoutPanel.SetColumnSpan(_governmentReforms, 4);

         // Names
         _leaderEditor = new([], "Add / Remove any names, separate with \",\"");
         _shipEditor = new([], "Add / Remove any names, separate with \",\"  $PROVINCE$ can be used here.");
         _armyEditor = new([], "Add / Remove any names, separate with \",\"");
         _fleetEditor = new([], "Add / Remove any names, separate with \",\"  $PROVINCE$ can be used here.");

         LeaderNamesTab.Controls.Add(_leaderEditor);
         ShipNamesTab.Controls.Add(_shipEditor);
         ArmyNamesTab.Controls.Add(_armyEditor);
         FleetNamesTab.Controls.Add(_fleetEditor);

         // Cultures
         _primaryCultureBox = new() { Dock = DockStyle.Fill };
         _primaryCultureBox.Items.AddRange([.. Globals.Cultures.Keys]);
         _primaryCultureBox.Margin = new(1, 1, 6, 1);
         _acceptedCultures = ControlFactory.GetItemList(ItemTypes.FullWidth, [.. Globals.Cultures.Keys], "Accepted Cultures");
         _acceptedCultures.OnItemAdded += CountryGuiEvents.AcceptedCultures_OnItemAdded;
         _acceptedCultures.OnItemRemoved += CountryGuiEvents.AcceptedCultures_OnItemRemoved;

         CulturesTLP.Controls.Add(_primaryCultureBox, 1, 0);
         CulturesTLP.Controls.Add(_acceptedCultures, 0, 1);
         CulturesTLP.SetColumnSpan(_acceptedCultures, 2);

         // Development
         _countryDevelopmentNumeric = ControlFactory.GetExtendedNumeric();
         _countryDevelopmentNumeric.Minimum = 0;
         _countryDevelopmentNumeric.Maximum = 100000;
         _countryDevelopmentNumeric.UpButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(1);
         _countryDevelopmentNumeric.UpButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(5);
         _countryDevelopmentNumeric.UpButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(10);
         _countryDevelopmentNumeric.DownButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(-1);
         _countryDevelopmentNumeric.DownButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(-5);
         _countryDevelopmentNumeric.DownButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(-10);
         _countryDevelopmentNumeric.OnTextValueChanged += (_, _) => SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
         DevelopmenTLP.Controls.Add(_countryDevelopmentNumeric, 1, 0);
         GeneralToolTip.SetToolTip(_countryDevelopmentNumeric, "LMB = +- 1\nSHIFT + LMB = +- 5\nCTRL + LMB = +-10\nThe development is only added to one random province in the selected country per click.");

         // Quick Assign
         List<string> unitsList = [];
         foreach (var unit in Globals.Units)
            unitsList.Add(unit.UnitName);
         _historicalUnits = new(unitsList, CommonCountry.Empty, nameof(CommonCountry.HistoricUnits), "Historic Units", -1);
         _historicalUnits.SetAutoSelectFunc(LandUnit.AutoSelectFuncUnits);

         List<string> ideaGroups = [];
         foreach (var idea in Globals.Ideas)
            if (idea.Type == IdeaType.IdeaGroup)
               ideaGroups.Add(idea.Name);
         _historicalIdeas = new(ideaGroups, CommonCountry.Empty, nameof(CommonCountry.HistoricIdeas), "Historic Ideas", 8);
         _historicalIdeas.SetAutoSelectFunc(IdeaGroup.GetAutoAssignment);

         _historicRivals = new([.. Globals.Countries.Keys], HistoryCountry.Empty, nameof(HistoryCountry.HistoricalRivals), "Historic Rivals", 8);
         _historicRivals.SetAutoSelectFunc(Country.GetHistoricRivals);
         _historicFriends = new([.. Globals.Countries.Keys], HistoryCountry.Empty, nameof(HistoryCountry.HistoricalFriends), "Historic Friends", 8);
         _historicFriends.SetAutoSelectFunc(Country.GetHistoricFriends);
         _estatePrivileges = new([], HistoryCountry.Empty, nameof(HistoryCountry.EstatePrivileges), "Estate Privileges", 8);
         _estatePrivileges.Enabled = false;

         MiscTLP.Controls.Add(_historicalUnits, 0, 1);
         MiscTLP.Controls.Add(_historicalIdeas, 0, 2);
         MiscTLP.Controls.Add(_historicRivals, 0, 3);
         MiscTLP.Controls.Add(_historicFriends, 0, 4);
         MiscTLP.Controls.Add(_estatePrivileges, 0, 5);

         CountryLoc = new(Selection.GetCountryAsList, new CLocObjFactory(true))
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         TagAndColorTLP.Controls.Add(CountryLoc, 1, 1);

         CountryADJLoc = new(Selection.GetCountryAsList, new CLocObjFactory(false))
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         TagAndColorTLP.Controls.Add(CountryADJLoc, 3, 1);

         AddNewMonarchNameButton.Click += CountryGuiEvents.AddMonarchName_Click;

      }

      public void ClearCountryGui()
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         // Flag
         CountryFlagLabel.SetCountry(Country.Empty);

         // Misc
         _tagSelectionBox.SelectedItem = DataClasses.GameDataClasses.Tag.Empty;
         CountryNameLabel.Text = "Country: -";
         _countryColorPickerButton.BackColor = Color.Empty;
         _countryColorPickerButton.Text = "(//)";
         CountryADJLoc.Clear();
         CountryLoc.Clear();
         RevolutionColorPickerButton.Clear();
         _graphicalCultureBox.SelectedIndex = 0;
         _unitTypeBox.SelectedIndex = 0;
         _techGroupBox.SelectedIndex = 0;
         _focusComboBox.SelectedIndex = 0;
         _capitalTextBox.Clear();

         // Names
         _leaderEditor.Clear();
         _shipEditor.Clear();
         _armyEditor.Clear();
         _fleetEditor.Clear();
         MonarchNamesFlowPanel.Controls.Clear();

         // Cultures
         _primaryCultureBox.SelectedIndex = -1;
         _acceptedCultures.Clear();

         // Government
         _governmentTypeBox.SelectedIndex = 0;
         _governmentRankBox.SelectedIndex = 0;
         _governmentReforms.Clear();

         // Development
         _countryDevelopmentNumeric.Value = 3;

         // Quick Assign
         _historicalUnits.Clear();
         _historicalIdeas.Clear();
         _historicRivals.Clear();
         _historicFriends.Clear();
         _estatePrivileges.Clear();
         
         Globals.EditingStatus = EditingStatus.Idle;
      }

      internal void LoadCountryToGui(Country country)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         if (country == Country.Empty)
            return;
         SuspendLayout();
         _tagSelectionBox.SelectedItem = country.Tag;
         if (Globals.Settings.Gui.ShowCountryFlagInCE)
         {
            CountryFlagLabel.SetCountry(country);
            GeneralToolTip.SetToolTip(CountryFlagLabel, $"{country.GetLocalisation()} ({country.Tag})");
         }
         CountryNameLabel.Text = $"{country.GetLocalisation()} ({country.Tag}) | (Total Dev: {country.GetDevelopment()})";
         _countryColorPickerButton.BackColor = country.Color;
         _countryColorPickerButton.Text = $"({country.Color.R}/{country.Color.G}/{country.Color.B})";
         CountryLoc.Text = country.GetLocalisation();
         CountryADJLoc.Text = country.GetAdjectiveLocalisation();
         RevolutionColorPickerButton.SetColorIndexes(country.CommonCountry.RevolutionaryColor.R, country.CommonCountry.RevolutionaryColor.G, country.CommonCountry.RevolutionaryColor.B);
         _graphicalCultureBox.Text = country.CommonCountry.GraphicalCulture;
         _unitTypeBox.Text = country.HistoryCountry.UnitType;
         _techGroupBox.Text = country.HistoryCountry.TechnologyGroup.Name;
         _capitalTextBox.Text = country.HistoryCountry.Capital.Id.ToString();
         _focusComboBox.Text = country.HistoryCountry.NationalFocus.ToString();

         // Names
         _leaderEditor.SetNames(country.CommonCountry.LeaderNames);
         _leaderEditor.TextBox.ContentModified += CountryGuiEvents.LeaderNames_ContentModified;
         _shipEditor.SetNames(country.CommonCountry.ShipNames);
         _shipEditor.TextBox.ContentModified += CountryGuiEvents.ShipNames_ContentModified;
         _armyEditor.SetNames(country.CommonCountry.ArmyNames);
         _armyEditor.TextBox.ContentModified += CountryGuiEvents.ArmyNames_ContentModified;
         _fleetEditor.SetNames(country.CommonCountry.FleetNames);
         _fleetEditor.TextBox.ContentModified += CountryGuiEvents.FleetNames_ContentModified;
         if (ShowMonachrNamesCB.Checked)
            AddMonarchNamesToGui(country.CommonCountry.MonarchNames);
         ResumeLayout();

         // Government
         _governmentTypeBox.SelectedItem = country.HistoryCountry.Government;
         _governmentRankBox.SelectedItem = country.HistoryCountry.GovernmentRank.ToString();
         if (Globals.GovernmentTypes.TryGetValue(country.HistoryCountry.Government, out var government))
            _governmentReforms.InitializeItems([.. government.AllReformNames]);

         _governmentReforms.Clear();
         _governmentReforms.Initializing = true;// we dont want to fire events that modify this collection when adding stuff as we are not editing it just loading.
         _governmentReforms.AddItemsUnique(country.HistoryCountry.GovernmentReforms);
         _governmentReforms.Initializing = false;

         // Cultures
         _primaryCultureBox.SelectedItem = country.HistoryCountry.PrimaryCulture;
         _acceptedCultures.Clear();
         foreach (var cult in country.HistoryCountry.AcceptedCultures)
            _acceptedCultures.AddItem(cult);

         // Development
         _countryDevelopmentNumeric.Value = country.GetDevelopment();

         _historicalUnits.UpdateCountry(country.CommonCountry);
         _historicalIdeas.UpdateCountry(country.CommonCountry);
         _historicRivals.UpdateCountry(country.HistoryCountry);
         _historicFriends.UpdateCountry(country.HistoryCountry);
         _estatePrivileges.UpdateCountry(country.HistoryCountry);


         Globals.EditingStatus = EditingStatus.Idle;
      }

      private void GovernmentTypeBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (_governmentTypeBox.SelectedItem == null)
            return;
         if (Globals.GovernmentTypes.TryGetValue(_governmentTypeBox.SelectedItem?.ToString()!, out var government))
         {
            _governmentReforms.InitializeItems([.. government.AllReformNames]);
            if (Selection.SelectedCountry != Country.Empty)
               Selection.SelectedCountry.HistoryCountry.Government = _governmentTypeBox.SelectedItem!.ToString()!;
         }
      }

      private void AddDevToSelectedCountryIfValid(int dev)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         Selection.SelectedCountry.AddDevToRandomProvince(dev);
      }

      private void SpreadDevInSelectedCountryIfValid(int value)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         var provinces = Selection.SelectedCountry.GetProvinces();
         var pieces = MathHelper.SplitIntoNRandomPieces(provinces.Count, value, Globals.Settings.Misc.MinDevelopmentInGeneration, Globals.Settings.Misc.MaxDevelopmentInGeneration);
         if (pieces.Count != provinces.Count)
            return;
         var index = 0;
         foreach (var province in provinces)
         {
            var devParts = MathHelper.SplitIntoNRandomPieces(3, pieces[index], 1,
               Globals.Settings.Misc.MaxDevelopmentInGeneration);

            province.BaseTax = devParts[0];
            province.BaseProduction = devParts[1];
            province.BaseManpower = devParts[2];

            index++;
         }
      }

      private void CreateFilesByDefault_Click(object sender, EventArgs e)
      {
         CreateFilesByDefault.Checked = !CreateFilesByDefault.Checked;
         Globals.Settings.Saving.AlwaysAskBeforeCreatingFiles = CreateFilesByDefault.Checked;
      }

      private void save1ToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new CrashReporter().ShowDialog();
      }

      private void quickSettingsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new SettingsWindow().ShowDialog();
      }

      private void OpenProvinceFileButton_Click(object sender, EventArgs e)
      {
         foreach (var province in Selection.GetSelectedProvinces)
         {
            var path = province.GetHistoryFilePath();
            if (File.Exists(path))
            {
               Process.Start(new ProcessStartInfo
               {
                  FileName = path,
                  UseShellExecute = true
               });
            }
            else
            {
               MessageBox.Show($"File not found: {path}");
            }
         }
      }


      private void button2_Click(object sender, EventArgs e)
      {
         var editor = new RoughEditorForm(Selection.SelectedCountry, false);
         editor.ExpandObjectsOfType(nameof(Country.HistoryCountry), nameof(Country.CommonCountry));
         editor.ShowDialog();
      }

      private void AdvancedProvinceEditing_Click(object sender, EventArgs e)
      {
         if (Selection.Count != 1)
         {
            MessageBox.Show("Only one province can be selected for advanced editing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }
         new RoughEditorForm(Selection.GetSelectedProvinces[0], false).ShowDialog();
      }



      private void AddMonarchNamesToGui(List<MonarchName> names)
      {
         MonarchNamesFlowPanel.FlowDirection = FlowDirection.TopDown;
         MonarchNamesFlowPanel.AutoScroll = true;
         MonarchNamesFlowPanel.WrapContents = false;

         MonarchNamesFlowPanel.Controls.Clear();
         SuspendLayout();
         foreach (var monName in names)
            AddMonarchNameToGui(monName);
         ResumeLayout();
      }

      internal void AddMonarchNameToGui(MonarchName monarchName)
      {
         MonarchNameControl monarchNameControl = new(monarchName, new(MonarchNamesFlowPanel.Width - 28, 29));
         MonarchNamesFlowPanel.Controls.Add(monarchNameControl);
      }

      private void ShowMonarchNamesCB_CheckedChanged(object sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (ShowMonachrNamesCB.Checked)
         {
            AddMonarchNamesToGui(Selection.SelectedCountry.CommonCountry.MonarchNames);
            NameTextBox.Enabled = true;
            AddNewMonarchNameButton.Enabled = true;
            ChanceTextBox.Enabled = true;
         }
         else
         {
            MonarchNamesFlowPanel.Controls.Clear();
            NameTextBox.Enabled = false;
            AddNewMonarchNameButton.Enabled = false;
            ChanceTextBox.Enabled = false;
         }
      }

      private void SetCapitalToSelected_Click(object sender, EventArgs e)
      {
         if (Selection.Count != 1)
            return;

         _capitalTextBox.Text = Selection.GetSelectedProvinces[0].Id.ToString();
         _capitalTextBox.Focus();
      }


      private void RedistributeDev_Click(object sender, EventArgs e)
      {
         SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
      }

      private void newSavingToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges();
      }

      private void refStackToolStripMenuItem_Click(object sender, EventArgs e)
      {

      }

      private void infoToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new InformationForm().ShowDialog();
      }

      private void clearCrashLogsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         CrashManager.ClearCrashLogs();
      }

      private void toolStripMenuItem4_Click(object sender, EventArgs e)
      {
#if DEBUG
         var sb = new StringBuilder();
         foreach (var terrain in Globals.Terrains)
         {
            sb.AppendLine($"{terrain.Name} : {terrain.SubCollection.Count}");
            foreach (var or in terrain.SubCollection)
            {
               sb.Append($"{or.Id}, ");
            }
            sb.AppendLine();
         }

         File.WriteAllText(Path.Combine(Globals.DebugPath, "terrainOverrides.txt"), sb.ToString());
#endif
      }

      private void deleteProvinceHistoryEntriesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GetDateInput dateInputForm = new("Select the date:");
         dateInputForm.ShowDialog();
         if (dateInputForm.DialogResult == DialogResult.OK)
         {
            if (!dateInputForm.GetDate(out var date))
               return;

            foreach (var province in Globals.Provinces)
               for (var i = province.History.Count - 1; i >= 0; i--)
                  if (province.History[i].Date >= date)
                     province.History.RemoveAt(i);
         }

      }

      private void FocusSelectionCheckBox_CheckedChanged(object sender, EventArgs e)
      {

      }

      private void AddNewCountryButton_Click(object sender, EventArgs e)
      {
         new CreateCountryForm().ShowDialog();
      }

      private void fileNamesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         foreach (var country in Globals.Countries.Values)
         {
            sb.AppendLine($"{country.Tag} : {country.CountryFilePath.FilePathArr.ToString()}");
         }


      }

      private void emptyCOlorInCountryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         foreach (var country in Globals.Countries.Values)
         {
            if (country.Color == Color.Empty)
               Debug.WriteLine(country.Tag);
         }
      }

      private void iMBTESTToolStripMenuItem_Click(object sender, EventArgs e)
      {
         SystemSounds.Question.Play();
      }

      private void browseEditedObjectsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new EditedObjectsExplorer().Show();
      }

      private void provinceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         ClearProvinceGui();
         Globals.EditingStatus = EditingStatus.Idle;
      }

      private void countryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Globals.EditingStatus = EditingStatus.LoadingInterface;
         ClearCountryGui();
         Globals.EditingStatus = EditingStatus.Idle;
      }


      private void AddNewMonarchNameButton_Click_1(object sender, EventArgs e)
      {
         CountryGuiEvents.AddMonarchName_Click(sender, e);
         NameTextBox.Clear();
         ChanceTextBox.Clear();
      }

      private void gameOfLiveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GameOfLive.RunGameOfLive(100);
      }

      private void benchmarkMapModesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sw = Stopwatch.StartNew();
         for (var i = 0; i < 1000; i++)
         {
            Globals.MapModeManager.SetCurrentMapMode(MapModeType.Area);
            Globals.MapModeManager.SetCurrentMapMode(MapModeType.Country);
         }
         sw.Stop();
         System.Diagnostics.Debug.WriteLine($"Time: {sw.ElapsedMilliseconds / 1000 * 2}");
      }
   }
}
