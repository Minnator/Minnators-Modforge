using System.Diagnostics;
using System.Drawing.Imaging;
using System.Media;
using System.Reflection;
using System.Runtime;
using System.Text;
using Editor.Controls;
using Editor.Controls.NewControls;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Forms.Feature.Crash_Reporter;
using Editor.Forms.Feature.SavingClasses;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading;
using Editor.NameGenerator;
using Editor.Parser;
using Editor.Saving;
using Editor.src.Controls.NewControls;
using Editor.src.Forms.Feature;
using Editor.src.Forms.GetUserInput;
using static Editor.Helper.ProvinceEnumHelper;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MapLoading = Editor.Loading.Enhanced.MapLoading;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Forms
{
   public partial class MapWindow : Form
   {
      #region CustomEditingControls

      private BindablePropertyComboBox<Province, Religion, string> _religionComboBox = null!;
      private PropertyComboBox<Province, int> _tradeCenterComboBox = null!;
      private BindablePropertyComboBox<Province, TradeGood, string> _tradeGoodsComboBox = null!;
      private BindablePropertyComboBox<Province, Culture, string> _cultureComboBox = null!;
      public ExtendedComboBox ModifierComboBox = null!;
      private ExtendedComboBox _modifierTypeComboBox = null!;

      private PropertyNumeric<Province, int> _taxNumeric = null!;
      private PropertyNumeric<Province, int> _prdNumeric = null!;
      private PropertyNumeric<Province, int> _mnpNumeric = null!;
      private PropertyNumeric<Province, float> _autonomyNumeric = null!;
      private PropertyNumeric<Province, float> _devastationNumeric = null!;
      private PropertyNumeric<Province, float> _prosperityNumeric = null!;
      private PropertyNumeric<Province, int> _extraCostNumeric = null!;

      private PropertyCheckBox<Province> _isCityCheckBox = null!;
      private PropertyCheckBox<Province> _isHreCheckBox = null!;
      private PropertyCheckBox<Province> _isParliamentSeatCheckbox = null!;
      private PropertyCheckBox<Province> _hasRevoltCheckBox = null!;

      private BindablePropertyComboBox<Province, Country, Tag> _tribalOwner = null!;
      private ExtendedComboBox _tradeCompanyInvestments = null!;
      private BindableFakePropertyComboBox<Province, Terrain, string> _terrainComboBox = null!;

      private PropertyNumeric<Province, int> _nativesSizeTextBox = null!;
      private PropertyNumeric<Province, float> _nativeFerocityTextBox = null!;
      private PropertyNumeric<Province, int> _nativeHostilityTextBox = null!;

      private PropertyTextBox<Province> _localisationTextBox = null!;
      private PropertyTextBox<Province> _provAdjTextBox = null!;
      private PropertyTextBox<Province> _capitalNameTextBox = null!;

      private ToolTip _savingButtonsToolTip = null!;

      private CollectionEditor2<Area, Province> _areaEditingGui = null!;
      private CollectionEditor2<Region, Area> _regionEditingGui = null!;
      private CollectionEditor2<SuperRegion, Region> _superRegionEditingGui = null!;
      private CollectionEditor2<TradeCompany, Province> _tradeCompanyEditingGui = null!;
      private CollectionEditor2<Country, Province> _countryEditingGui = null!;
      private CollectionEditor2<TradeNode, Province> _tradeNodeEditingGui = null!;
      private CollectionEditor2<ProvinceGroup, Province> _provinceGroupsEditingGui = null!;
      private CollectionEditor2<ColonialRegion, Province> _colonialRegionEditingGui = null!;

      private BindablePropertyComboBox<Province, Country, Tag> _ownerTagBox = null!;
      private BindablePropertyComboBox<Province, Country, Tag> _controllerTagBox = null!;

      private PropertyLabel<Province> LocalisationLabel = null!;
      private PropertyLabel<Province> ProvAdjLabel = null!;

      private PropertyCollectionSelector<Province, List<Building>, Building> _buildingsSelector = null!;
      private PropertyCollectionSelector<Province, List<Tag>, Tag> _coreSelector = null!;
      private PropertyCollectionSelector<Province, List<Tag>, Tag> _claimSelector = null!;
      private PropertyCollectionSelector<Province, List<Tag>, Tag> _permaClaimSelector = null!;
      private PropertyCollectionSelector<Province, List<string>, string> _discoveredBy = null!;

      public Screen? StartScreen = null;

      #endregion

      public readonly DateControl DateControl = new(Date.MinValue, DateControlLayout.Horizontal);
      private LoadingScreen.LoadingScreen _ls = null!;

      public MapWindow()
      {
         Globals.State = State.Loading;
         Globals.MapWindow = this;
         Globals.Random = new(Globals.Settings.Misc.RandomSeed);

         // Load the game data
         RunLoadingScreen();
         ThreadHelper.StartBWHeapThread();

         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Normal, Cursors.Default, this);

         if (StartScreen != null)
         {
            var screen = Globals.MapWindow.StartScreen;
            if (screen != null)
               Location = new(screen.Bounds.X + (screen.Bounds.Width - Width) / 2, screen.Bounds.Y + (screen.Bounds.Height - Height) / 2);
         }

         //TODO: solve all the issues regarding this
         //DiscordActivityManager.ActivateActivity();

#if DEBUG
         debugToolStripMenuItem.Enabled = true;
         debugToolStripMenuItem.Visible = true;
#endif
      }

      #region Initialize Application and Loadingscreen

      public void Initialize()
      {
         Hide();
         //pause gui updates
         SuspendLayout();

         InitGui();
         Text = $"{Text} | {Globals.DescriptorData.Name}";

         // MUST BE LAST in the loading sequence
         InitMapModes();
         HistoryManager.UndoDepthChanged += UpdateUndoDepth!;
         HistoryManager.RedoDepthChanged += UpdateRedoDepth!;

         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         //resume gui updates
         // Enable the Application
         Globals.LoadingLog.Close();
         ResourceUsageHelper.Initialize(this);
         HistoryManager.UpdateToolStrip();
         SetSelectedProvinceSum(0);

         // ALL LOADING COMPLETE - Set the application to running

         DateControl.Date = new(1444, 11, 11);
         CountryLoading.AssignProvinces();
         Globals.State = State.Running;
         MapModeManager.SetCurrentMapMode(MapModeType.Country);
         ResumeLayout();

         StartPosition = FormStartPosition.CenterScreen;
         Globals.ZoomControl.FocusOn(new(3100, 600), 1f);
         Show();

         // Activate this window
         Activate();
         Globals.ZoomControl.Invalidate();
      }


      private void RunLoadingScreen()
      {
         _ls = new();
         Eu4Cursors.SetEu4CursorIfEnabled(Eu4CursorTypes.Loading, Cursors.AppStarting, _ls);
         _ls.ShowDialog();
      }

      #endregion
      #region MapWindow GUI Init
      private void InitGui()
      {
         InitializeComponent();

         HistoryManager.UndoEvent += (sender, args) => UpdateGui();
         HistoryManager.RedoEvent += (sender, args) => UpdateGui();

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

         BookMarkComboBox.Items.AddRange([.. Globals.Bookmarks]);
         BookMarkComboBox.SelectedIndexChanged += OnBookMarkChanged;


         // Initalize Settings Events and Listeners
         SettingsHelper.InitializeEvent();
      }

      private void OnBookMarkChanged(object? sender, EventArgs e)
      {
         if (BookMarkComboBox.SelectedIndex == -1)
            return;

         var bookmark = Globals.Bookmarks[BookMarkComboBox.SelectedIndex];
         DateControl.Date = bookmark.Date;
      }
      #endregion
      #region GUI Updates
      private void UpdateGui()
      {
         UpdateTabIfSelected(DataTabPanel.SelectedIndex);
      }

      private void UpdateTabIfSelected(int i)
      {
         if (DataTabPanel.SelectedIndex != i)
            return;
         switch (i)
         {
            case 0:
               UpdateProvinceTab();
               break;
            case 1:
               UpdateCountryTab();
               break;
            case 2:
               //UpdateProvinceCollectionTab();
               break;
         }
      }

      private void UpdateProvinceTab()
      {
         if (DataTabPanel.SelectedIndex != 0)
            return;

         if (Selection.Count == 0)
         {
            DataTabPanel.TabPages[0].SuspendLayout();
            Globals.State = State.Loading;
            ClearProvinceGui();
            Globals.State = State.Running;
            DataTabPanel.TabPages[0].Enabled = false;
            DataTabPanel.TabPages[0].ResumeLayout();
         }
         else
         {
            DataTabPanel.TabPages[0].Enabled = true;
            //LoadSelectedProvincesToGui();
            LoadGuiEvents.ReloadProvinces();
         }
      }

      private void UpdateCountryTab()
      {
         if (DataTabPanel.SelectedIndex != 1)
            return;

         if (Selection.SelectedCountry == Country.Empty)
         {
            DataTabPanel.TabPages[1].SuspendLayout();
            Globals.State = State.Loading;
            ClearCountryGui();
            Globals.State = State.Running;
            DataTabPanel.TabPages[1].Enabled = false;
            DataTabPanel.TabPages[1].ResumeLayout();
         }
         else
         {
            DataTabPanel.TabPages[1].Enabled = true;
            //LoadCountryToGui(Selection.SelectedCountry);
            LoadGuiEvents.ReloadCountry();
         }
      }

      #endregion
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
         MapModeManager.MapModeChanged += (sender, mode) =>
         {
            MapModeTimesInfo.Text = $"MapMode times [ms]: last: {MapModeManager.LasMapModeTime} | Min: {MapModeManager.MinMapModeTime} | Max: {MapModeManager.MaxMapModeTime} | Avg: {MapModeManager.AverageMapModeTime}";
         };
         MapModeTimesInfo.ToolTipText =
            "Time only counts in the time it takes the Rendering Pipeline to render the map and Invalidate the control.\nOther GUI updates are not included.";
         RamUsageStrip.ToolTipText = "Current RAM usage of the application\nThis is inflated by HeapAllocation from the GC and will free memory if your PC needs some.";
         UndoDepthLabel.ToolTipText = "Undo tree depth\nCompacted \'undos\'(CTRL+Z) / Uncompacted \'undos\'(CTRL+ALT+Z)";
         RedoDepthLabel.ToolTipText = "Redo tree depth\nCompacted \'redos\'(CTRL+Y) / Uncompacted \'redos\'(CTRL+ALT+Y)";
         CpuUsageStrip.ToolTipText = "Current CPU usage of the application";
         SelectedProvinceSum.ToolTipText = "Sum of currently selected provinces";
      }

      private void InitializeMapModeButtons()
      {
         var button01 = ControlFactory.GetMapModeButton('q', 0);
         var button21 = ControlFactory.GetMapModeButton('w', 1);
         var button31 = ControlFactory.GetMapModeButton('e', 2);
         var button41 = ControlFactory.GetMapModeButton('r', 3);
         var button5 = ControlFactory.GetMapModeButton('t', 4);
         var button6 = ControlFactory.GetMapModeButton('y', 5);
         var button7 = ControlFactory.GetMapModeButton('u', 6);
         var button8 = ControlFactory.GetMapModeButton('i', 7);
         var button9 = ControlFactory.GetMapModeButton('o', 8);
         var button10 = ControlFactory.GetMapModeButton('p', 9);
         button01.Margin = new(0, 0, 3, 0);
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
         _countryEditingGui = new(ItemTypes.Id, SaveableType.Country, SaveableType.Province, MapModeType.Country) { AllowSeaTiles = false };
         Country.ItemsModified += _countryEditingGui.OnCorrespondingDataChange;
         _countryEditingGui._extendedComboBox.DataSource = new BindingSource
         {
            DataSource = Globals.Countries,
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
         DataTabPanel.TabPages[1].Enabled = false;
         Selection.OnProvinceSelectionChange += (sender, i) => UpdateTabIfSelected(0);
         Selection.OnCountrySelectionChange += (sender, i) => UpdateTabIfSelected(1);

         DataTabPanel.SelectedIndexChanged += (sender, args) => UpdateGui();


         // CustomTextBox
         _localisationTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.TitleLocalisation)));
         LocTableLayoutPanel.Controls.Add(_localisationTextBox, 1, 1);

         _provAdjTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.AdjectiveLocalisation)));
         LocTableLayoutPanel.Controls.Add(_provAdjTextBox, 1, 3);

         LocalisationLabel = ControlFactory.GetPropertyLabel(typeof(Province).GetProperty(nameof(Province.TitleKey)));
         LocTableLayoutPanel.Controls.Add(LocalisationLabel, 1, 0);

         ProvAdjLabel = ControlFactory.GetPropertyLabel(typeof(Province).GetProperty(nameof(Province.AdjectiveKey)));
         LocTableLayoutPanel.Controls.Add(ProvAdjLabel, 1, 2);

         // Tooltips for saving Buttons
         _savingButtonsToolTip = new();
         _savingButtonsToolTip.AutoPopDelay = 10000;
         _savingButtonsToolTip.InitialDelay = 100;
         _savingButtonsToolTip.ShowAlways = true;

         SaveAllProvincesButton.MouseEnter += OnSavingAllEnter;
         SaveCurrentSelectionButton.MouseEnter += OnSavingSelectionEnter;
         OpenProvinceFile.MouseEnter += OnOpenProvinceFileEnter;

         _ownerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.Owner))!, Globals.Countries);
         MisProvinceData.Controls.Add(_ownerTagBox, 1, 0);
         _controllerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.Controller))!, Globals.Countries);
         MisProvinceData.Controls.Add(_controllerTagBox, 1, 1);

         _coreSelector = new(typeof(Province).GetProperty(nameof(Province.Cores)),
                             ref LoadGuiEvents.ProvLoadAction,
                             () => Selection.GetSelectedProvinces,
                             Globals.Countries.Keys.ToList(),
                             typeof(Tag).GetProperty("TagValue")!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_coreSelector, 0, 2);

         _discoveredBy = new(typeof(Province).GetProperty(nameof(Province.DiscoveredBy)),
                             ref LoadGuiEvents.ProvLoadAction,
                             () => Selection.GetSelectedProvinces,
                             [.. Globals.Countries.Keys.Select(x => x.TagValue).ToList(), .. Globals.TechnologyGroups.Keys],
                             null!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_discoveredBy, 0, 4);


         _buildingsSelector = new(typeof(Province).GetProperty(nameof(Province.Buildings)),
                                  ref LoadGuiEvents.ProvLoadAction,
                                  () => Selection.GetSelectedProvinces,
                                  Globals.Buildings,
                                  typeof(Building).GetProperty(nameof(Building.Name))!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_buildingsSelector, 1, 2);

         _claimSelector = new(typeof(Province).GetProperty(nameof(Province.Claims)),
                              ref LoadGuiEvents.ProvLoadAction,
                              () => Selection.GetSelectedProvinces,
                              Globals.Countries.Keys.ToList(),
                              typeof(Tag).GetProperty("TagValue")!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_claimSelector, 0, 1);

         _permaClaimSelector = new(typeof(Province).GetProperty(nameof(Province.PermanentClaims)),
                                   ref LoadGuiEvents.ProvLoadAction,
                                   () => Selection.GetSelectedProvinces,
                                   Globals.Countries.Keys.ToList(),
                                   typeof(Tag).GetProperty("TagValue")!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_permaClaimSelector, 1, 1);


         _discoveredBy.Location = new(0, 18);

         _tradeCenterComboBox = ControlFactory.SimpleComboBoxProvince<int>(typeof(Province).GetProperty(nameof(Province.CenterOfTrade))!);
         _tradeCenterComboBox.Items.AddRange(["0", "1", "2", "3"]);
         _tradeCenterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         TradePanel.Controls.Add(_tradeCenterComboBox, 1, 0);

         _tradeGoodsComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.TradeGood))!, Globals.TradeGoods);
         TradePanel.Controls.Add(_tradeGoodsComboBox, 1, 1);

         _cultureComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Culture))!, Globals.Cultures);
         MisProvinceData.Controls.Add(_cultureComboBox, 1, 3);

         _religionComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Religion))!, Globals.Religions);
         MisProvinceData.Controls.Add(_religionComboBox, 1, 2);

         // TODO GEDANKEN MACHEN Wie man das hier am besten macht
         _terrainComboBox = ControlFactory.GetBindableFakePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Terrain))!, Globals.Terrains);
         _terrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MisProvinceData.Controls.Add(_terrainComboBox, 1, 5);


         _taxNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseTax)), 0);
         _taxNumeric.Minimum = 0;
         _taxNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_taxNumeric, 3, 0);

         _prdNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseProduction)), 0);
         _prdNumeric.Minimum = 0;
         _prdNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_prdNumeric, 3, 1);

         _mnpNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.BaseManpower)), 0);
         _mnpNumeric.Minimum = 0;
         _mnpNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_mnpNumeric, 3, 2);

         _autonomyNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.LocalAutonomy)), 0.0f);
         _autonomyNumeric.Minimum = 0;
         _autonomyNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_autonomyNumeric, 1, 0);

         _devastationNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.Devastation)), 0.0f);
         _devastationNumeric.Minimum = 0;
         _devastationNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_devastationNumeric, 1, 1);

         _prosperityNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.Prosperity)), 0.0f);
         _prosperityNumeric.Minimum = 0;
         _prosperityNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_prosperityNumeric, 1, 2);

         _extraCostNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ExtraCost)), 0);
         _extraCostNumeric.Minimum = 0;
         _extraCostNumeric.Maximum = 1000;
         TradePanel.Controls.Add(_extraCostNumeric, 1, 2);

         _capitalNameTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.Capital)));
         MisProvinceData.Controls.Add(_capitalNameTextBox, 1, 4);

         _isCityCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsCity))!);
         _isCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isCityCheckBox, 3, 3);

         _isHreCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsHre))!);
         _isHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isHreCheckBox, 3, 4);

         _isParliamentSeatCheckbox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.IsSeatInParliament))!);
         _isParliamentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isParliamentSeatCheckbox, 3, 5);

         _hasRevoltCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.HasRevolt))!);
         _hasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_hasRevoltCheckBox, 3, 6);

         MWAttirbuteCombobox.Items.AddRange([.. Enum.GetNames<ProvAttrGet>()]);

         // NATIVES TAB
         _tribalOwner = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.TribalOwner))!, Globals.Countries);
         NativesLayoutPanel.Controls.Add(_tribalOwner, 1, 0);

         _nativesSizeTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeSize)), 0, 100);
         _nativesSizeTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativesSizeTextBox, 1, 1);

         _nativeFerocityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeFerocity)), 0f, 100);
         _nativeFerocityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeFerocityTextBox, 1, 3);

         _nativeHostilityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.NativeHostileness)), 0, 100);
         _nativeHostilityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeHostilityTextBox, 1, 2);

         // TRADE_COMPANIES TAB
         _tradeCompanyInvestments = ControlFactory.GetExtendedComboBox(nameof(Province.TradeCompanyInvestments));
         _tradeCompanyInvestments.Items.AddRange([.. Globals.TradeCompanyInvestments.Keys]);
         _tradeCompanyInvestments.OnDataChanged += ProvinceEditingEvents.OnExtendedComboBoxSelectedIndexChanged;
         TradeCompaniesLayoutPanel.Controls.Add(_tradeCompanyInvestments, 1, 0);

         // MODIFIERS TAB
         InitializeModifierTab();


         ProvinceCustomToolStripLayoutPanel.Paint += TableLayoutBorder_Paint;
      }


      private void InitializeModifierTab()
      {
         ModifierComboBox = ControlFactory.GetExtendedComboBox(nameof(Province.ProvinceModifiers));
         ModifierComboBox.Items.AddRange([.. Globals.EventModifiers.Keys]);
         // No data changed here as they are added via the "Add" button
         ModifiersLayoutPanel.Controls.Add(ModifierComboBox, 1, 1);

         _modifierTypeComboBox = ControlFactory.GetExtendedComboBox("", ["CountryModifier", "ProvinceModifier"]);
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


      #endregion
      #region Province Gui
      public void ClearProvinceGui()
      {
         ExtendedComboBox.AllowEvents = false;
         _ownerTagBox.SetDefault();
         _controllerTagBox.SetDefault();
         _religionComboBox.SetDefault();
         _cultureComboBox.SetDefault();
         _capitalNameTextBox.SetDefault();
         _isCityCheckBox.Checked = false;
         _isHreCheckBox.Checked = false;
         _isParliamentSeatCheckbox.Checked = false;
         _hasRevoltCheckBox.Checked = false;
         _taxNumeric.SetDefault();
         _prdNumeric.SetDefault();
         _mnpNumeric.SetDefault();
         _claimSelector.SetDefault();
         _permaClaimSelector.SetDefault();
         _coreSelector.SetDefault();
         _buildingsSelector.SetDefault();
         _discoveredBy.SetDefault();
         _autonomyNumeric.SetDefault();
         _devastationNumeric.SetDefault();
         _prosperityNumeric.SetDefault();
         _tradeGoodsComboBox.SetDefault();
         _tradeCenterComboBox.SetDefault();
         _extraCostNumeric.SetDefault();
         _tribalOwner.Clear();
         _nativesSizeTextBox.SetDefault();
         _nativeFerocityTextBox.SetDefault();
         _nativeHostilityTextBox.SetDefault();
         ModifierComboBox.Text = string.Empty;
         ModifiersListView.Items.Clear();
         DurationTextBox.Text = string.Empty;
         _tradeCompanyInvestments.Text = string.Empty;
         _localisationTextBox.SetDefault();
         LocalisationLabel.SetDefault();
         _provAdjTextBox.SetDefault();
         ProvAdjLabel.SetDefault();
         _terrainComboBox.SetDefault();
         ExtendedComboBox.AllowEvents = true;
      }
      #endregion
      #region MapMode Init / Update

      public void UpdateMapModeButtons(bool buttonInv = true)
      {
         foreach (var mapModeButton in MMButtonsTLPanel.Controls)
         {
            if (mapModeButton is not MapModeButton button)
               continue;

            button.UpdateMapMode(buttonInv);
         }
      }

      private void InitMapModes()
      {
         MapModeComboBox.Items.Clear();
         MapModeComboBox.Items.AddRange([.. Enum.GetNames<MapModeType>()]);
      }
      private void MapModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         MapModeManager.SetCurrentMapMode(Enum.Parse<MapModeType>(MapModeComboBox.SelectedItem?.ToString() ?? string.Empty));
      }

      #endregion
      #region Resource Updater MethodInvokation
      public void SetSelectedProvinceSum(int sum)
      {
         SelectedProvinceSum.Text = $"Selected: {sum}";
      }

      public void UpdateCompactionToolStripTime(TimeSpan time)
      {
         CompactionToolStrip.Text = $"Compaction in: {time:mm\\:ss}";
      }
      
      #endregion
      #region HistoryManager Event Handlers

      private void UpdateRedoDepth(object sender, Func<(int, int)> valueTuple)
      {
         var (simple, full) = valueTuple();
         RedoDepthLabel.Text = $"Redos [{simple}/{full}]";
      }

      private void UpdateUndoDepth(object? sender, Func<(int, int)> valueTuple)
      {
         var (simple, full) = valueTuple();
         UndoDepthLabel.Text = $"Undos [{simple}/{full}]";
      }

      #endregion
      #region History interface interactions

      private void RevertInSelectionHistory(object sender, EventArgs e)
      {
         var historyTreeView = new HistoryTree(HistoryManager.RevertTo);
         historyTreeView.VisualizeFull(HistoryManager.Root);
         historyTreeView.ShowDialog();
      }

      private void DeleteHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var result = MessageBox.Show("Are you sure you want to delete the history?", "Delete History", MessageBoxButtons.OKCancel);
         if (result == DialogResult.OK)
            HistoryManager.Clear();
      }

      #endregion
      #region Toolstrip Interactions

      private void OneGBRAM(object sender, EventArgs e)
      {
         Theory.OneGBRamUsage();
      }
      private void searchToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<Search>();
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

      private void gCToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
         GC.Collect();
      }

      private void SaveCurrentMapModeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
         var form = new GetSavingFileForm(pictures, "Where to save the map mode as an image to", ".png");
         form.SetPlaceHolderText(MapModeManager.CurrentMapMode.MapModeType.ToString());
         form.RequireModDirectory = false;
         form.ShowDialog();
         if (form.DialogResult == DialogResult.OK)
         {
            using var bmp = Globals.ZoomControl.Map;
            bmp.Save(form.NewPath, ImageFormat.Png);
         }
      }
      private void graphicalElementsManagerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<GuiDrawings>();
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
      private void randomModifierToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ModifierSuggestion().ShowDialog();
      }


      #endregion
      #region Modifiers

      private void OpenAddModifierForm_Click(object sender, EventArgs e)
      {
         new EventModifierForm().ShowDialog();
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
         //TODO  ProvinceEditingEvents.OnModifierAdded(mod, type);
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

         // TODO  ProvinceEditingEvents.OnModifierRemoved(new ApplicableModifier(modifierName, duration), type);
         ModifiersListView.Items.Remove(item);
      }

      #endregion

      private void OnSavingAllEnter(object? sender, EventArgs e) => _savingButtonsToolTip.SetToolTip(SaveAllProvincesButton, $"Save all provinces ({Globals.Provinces.Count})");
      private void OnSavingSelectionEnter(object? sender, EventArgs e) => _savingButtonsToolTip.SetToolTip(SaveCurrentSelectionButton, $"Save selection ({Selection.Count})");

      private void OnOpenProvinceFileEnter(object? sender, EventArgs e)
      {
         _savingButtonsToolTip.SetToolTip(OpenProvinceFile,
            Selection.Count == 1
               ? $"Open the selected province file ({Selection.GetSelectedProvinces[0].TitleLocalisation})"
               : $"Open the selected province files ({Selection.Count})");
      }

      private static void OnDateChanged(object? sender, Date date)
      {
         //ProvinceHistoryManager.LoadDate(date);
      }

      public void UpdateHoveredInfo(Province? province)
      {
         if (province == null)
         {
            ProvinceNameLabel.Text = "Province: -";
            OwnerCountryNameLabel.Text = "Owner: -";
            return;
         }
         ProvinceNameLabel.Text = $"Province: {province.TitleLocalisation}";
         OwnerCountryNameLabel.Text = $"Owner: {Localisation.GetLoc(province.Owner.TitleKey)}";
      }

      private void MapWindow_KeyDown(object sender, KeyEventArgs e)
      {
         if ((ModifierKeys & Keys.Control) == Keys.Control)
         {
            if ((ModifierKeys & Keys.Alt) == Keys.Alt)
               switch (e.KeyCode)
               {
                  case Keys.Z:
                     HistoryManager.Undo(true);
                     break;
                  case Keys.Y:
                     HistoryManager.Redo(true);
                     e.SuppressKeyPress = true;
                     e.Handled = true;
                     break;
               }
            else
               switch (e.KeyCode)
               {
                  case Keys.F:
                     FormsHelper.ShowIfAnyOpen<Search>();
                     break;
                  case Keys.Z:
                     HistoryManager.Undo(false);
                     break;
                  case Keys.Y:
                     HistoryManager.Redo(false);
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
               FormsHelper.ShowIfAnyOpen<ConsoleForm>();
               break;
         }
      }
      private void MapWindow_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (!ShutDownMaster.DoWeShutDown())
            e.Cancel = true;

         // Terminate ResourceUsageHelper
         ResourceUsageHelper.Dispose();
      }



      // ------------------- COUNTRY EDITING TAB ------------------- \\
      public TagComboBox _tagSelectionBox = null!;

      internal FlagLabel CountryFlagLabel = null!;

      private BindableListPropertyComboBox<CommonCountry, string> _graphicalCultureBox = null!;
      private BindablePropertyComboBox<HistoryCountry, TechnologyGroup, string> _unitTypeBox = null!;
      private BindablePropertyComboBox<HistoryCountry, TechnologyGroup, string> _techGroupBox = null!;
      private BindablePropertyComboBox<HistoryCountry, Government, string> _governmentTypeBox = null!;
      private ListPropertyComboBox<HistoryCountry, int> _governmentRankBox = null!;
      private BindablePropertyComboBox<HistoryCountry, Culture, string> _primaryCultureBox = null!;
      private ListPropertyComboBox<HistoryCountry, Mana> _focusComboBox = null!;

      private PropertyColorButton<CommonCountry> _countryColorPickerButton = null!;
      public ThreeColorStripesButton RevolutionColorPickerButton = null!;

      private PropertyCollectionSelector<HistoryCountry, List<GovernmentReform>, GovernmentReform> _governmentReforms = null!;
      private PropertyCollectionSelector<HistoryCountry, List<Culture>, Culture> _acceptedCultures = null!;

      private PropertyNamesEditor<CommonCountry, List<string>> _leaderEditor = null!;
      private PropertyNamesEditor<CommonCountry, List<string>> _shipEditor = null!;
      private PropertyNamesEditor<CommonCountry, List<string>> _armyEditor = null!;
      private PropertyNamesEditor<CommonCountry, List<string>> _fleetEditor = null!;

      private PropertyLabel<HistoryCountry> _capitalTextBox = null!;

      private ExtendedNumeric _countryDevelopmentNumeric = null!;

      private PropertyQuickAssignControl<CommonCountry, List<string>, string> _historicalUnits = null!;
      private PropertyQuickAssignControl<CommonCountry, List<string>, string> _historicalIdeas = null!;
      private PropertyQuickAssignControl<HistoryCountry, List<Tag>, Tag> _historicRivals = null!;
      private PropertyQuickAssignControl<HistoryCountry, List<Tag>, Tag> _historicFriends = null!;
      private PropertyQuickAssignControl<HistoryCountry, List<string>, string> _estatePrivileges = null!;

      private PropertyTextBox<Country> CountryLoc = null!;
      private PropertyTextBox<Country> CountryADJLoc = null!;

      internal PropertyMonarchNamesControl<CommonCountry, List<MonarchName>> _monarchNames = null!;

      private void InitializeCountryEditGui()
      {
         Selection.OnCountrySelected += CountryGuiEvents.OnCountrySelected;
         Selection.OnCountryDeselected += CountryGuiEvents.OnCountryDeselected;
         _tagSelectionBox = new(null!)
         {
            Margin = new(1),
            Height = 25,
         };
         CountryFlagLabel = ControlFactory.GetFlagLabel();
         _tagSelectionBox.OnTagChanged += CountryGuiEvents.TagSelectionBox_OnTagChanged;
         _countryColorPickerButton = ControlFactory.GetColorPickerButtonCommonCountry(typeof(CommonCountry).GetProperty(nameof(CommonCountry.Color)));
         _countryColorPickerButton.Click += CountryGuiEvents.CountryColorPickerButton_Click;
         GeneralToolTip.SetToolTip(_countryColorPickerButton, "Set the <color> of the selected country");
         RevolutionColorPickerButton = ControlFactory.GetThreeColorsButton();
         RevolutionColorPickerButton.Margin = new(0);

         GeneralToolTip.SetToolTip(RevolutionColorPickerButton, "LMB: Set the <revolutionary_color> of the selected country\nRMB: randomize");
         _graphicalCultureBox = ControlFactory.GetBindableListPropertyComboBox(typeof(CommonCountry).GetProperty(nameof(CommonCountry.GraphicalCulture))!,
                                                                               Globals.GraphicalCultures,
                                                                               ControlFactory.DefaultMarginType.Slim);

         _unitTypeBox = ControlFactory.GetBindablePropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.UnitType))!,
                                                                                 Globals.TechnologyGroups,
                                                                                 margin: ControlFactory.DefaultMarginType.Slim);

         _techGroupBox = ControlFactory.GetBindablePropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.TechnologyGroup))!,
                                                                                  Globals.TechnologyGroups,
                                                                                  margin: ControlFactory.DefaultMarginType.Slim);

         _governmentTypeBox = ControlFactory.GetBindablePropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.Government))!,
                                                                                       Globals.GovernmentTypes,
                                                                                       margin: ControlFactory.DefaultMarginType.Default,
                                                                                       hasEmptyItemAt0: false);
         _governmentTypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
         _governmentTypeBox.SelectedIndexChanged += (_, _) =>
         {
            if (!Globals.GovernmentTypes.TryGetValue(_governmentTypeBox.Text, out var government))
               _governmentReforms.SetItems([]);
            _governmentReforms.SetItems(government.AllReforms.Select(x => x.Name).ToList());
         };


         _governmentRankBox = ControlFactory.GetListPropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.GovernmentRank))!,
                                                                                   Enumerable.Range(1, Globals.MaxGovRank).Select(i => i).ToList());
         _governmentRankBox.Margin = new(3, 0, 3, 2);
         _governmentRankBox.DropDownStyle = ComboBoxStyle.DropDownList;

         _governmentReforms = new(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.GovernmentReforms))!,
                                  ref LoadGuiEvents.HistoryCountryLoadAction,
                                  () => [Selection.SelectedCountry.HistoryCountry],
                                  Globals.GovernmentReforms.Values.ToList(),
                                  typeof(GovernmentReform).GetProperty(nameof(GovernmentReform.Name)));

         _capitalTextBox = ControlFactory.GetPropertyLabelHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.GetCapitalLoc)));

         _focusComboBox = ControlFactory.GetListPropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.NationalFocus))!,
                                                                               [.. Enum.GetValues<Mana>()],
                                                                               ControlFactory.DefaultMarginType.Slim);
         _focusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

         TagAndColorTLP.Controls.Add(_tagSelectionBox, 1, 0);
         TagAndColorTLP.Controls.Add(_countryColorPickerButton, 3, 0);
         TagAndColorTLP.Controls.Add(RevolutionColorPickerButton, 3, 3);
         TagAndColorTLP.Controls.Add(_graphicalCultureBox, 1, 2);
         TagAndColorTLP.Controls.Add(_unitTypeBox, 3, 2);
         TagAndColorTLP.Controls.Add(_techGroupBox, 1, 3);
         CapitalTLP.Controls.Add(_capitalTextBox, 0, 0);
         TagAndColorTLP.Controls.Add(_focusComboBox, 1, 4);
         CountryHeaderTLP.Controls.Add(CountryFlagLabel, 0, 0);

         GovernmentLayoutPanel.Controls.Add(_governmentTypeBox, 0, 1);
         GovernmentLayoutPanel.Controls.Add(_governmentRankBox, 0, 4);
         GovernmentLayoutPanel.Controls.Add(_governmentReforms, 1, 0);
         GovernmentLayoutPanel.SetRowSpan(_governmentReforms, 5);

         // Names
         _leaderEditor = ControlFactory.GetPropertyNamesEditorCommonCountry<List<string>>(typeof(CommonCountry).GetProperty(nameof(CommonCountry.LeaderNames)), "Add / Remove any names, separate with \",\"");
         _shipEditor = ControlFactory.GetPropertyNamesEditorCommonCountry<List<string>>(typeof(CommonCountry).GetProperty(nameof(CommonCountry.ShipNames)), "Add / Remove any names, separate with \",\" $PROVINCE$ can be used here.");
         _armyEditor = ControlFactory.GetPropertyNamesEditorCommonCountry<List<string>>(typeof(CommonCountry).GetProperty(nameof(CommonCountry.ArmyNames)), "Add / Remove any names, separate with \",\"");
         _fleetEditor = ControlFactory.GetPropertyNamesEditorCommonCountry<List<string>>(typeof(CommonCountry).GetProperty(nameof(CommonCountry.FleetNames)), "Add / Remove any names, separate with \",\" $PROVINCE$ can be used here.");

         LeaderNamesTab.Controls.Add(_leaderEditor);
         ShipNamesTab.Controls.Add(_shipEditor);
         ArmyNamesTab.Controls.Add(_armyEditor);
         FleetNamesTab.Controls.Add(_fleetEditor);

         // Cultures
         _primaryCultureBox = ControlFactory.GetBindablePropertyComboBoxHistoryCountry(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.PrimaryCulture))!,
                                                                                       Globals.Cultures,
                                                                                       margin: ControlFactory.DefaultMarginType.Slim);
         _primaryCultureBox.Dock = DockStyle.None;
         _primaryCultureBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
         _primaryCultureBox.Margin = new(3);

         _acceptedCultures = new(typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.AcceptedCultures)),
                                 ref LoadGuiEvents.HistoryCountryLoadAction,
                                 () => [Selection.SelectedCountry.HistoryCountry],
                                 Globals.Cultures.Values.ToList(),
                                 typeof(Culture).GetProperty("Name"));


         CulturesTLP.Controls.Add(_primaryCultureBox, 0, 1);
         CulturesTLP.Controls.Add(_acceptedCultures, 1, 0);
         CulturesTLP.SetRowSpan(_acceptedCultures, 2);

         // Development
         _countryDevelopmentNumeric = ControlFactory.GetExtendedNumeric("");
         _countryDevelopmentNumeric.Minimum = 0;
         _countryDevelopmentNumeric.Maximum = 100000;
         _countryDevelopmentNumeric.UpButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(1);
         _countryDevelopmentNumeric.UpButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(5);
         _countryDevelopmentNumeric.UpButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(10);
         _countryDevelopmentNumeric.DownButtonPressedSmall += (_, _) => AddDevToSelectedCountryIfValid(-1);
         _countryDevelopmentNumeric.DownButtonPressedMedium += (_, _) => AddDevToSelectedCountryIfValid(-5);
         _countryDevelopmentNumeric.DownButtonPressedLarge += (_, _) => AddDevToSelectedCountryIfValid(-10);
         _countryDevelopmentNumeric.OnValueChanged += (_, _) => SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
         DevelopmenTLP.Controls.Add(_countryDevelopmentNumeric, 1, 0);
         GeneralToolTip.SetToolTip(_countryDevelopmentNumeric, "LMB = +- 1\nSHIFT + LMB = +- 5\nCTRL + LMB = +-10\nThe development is only added to one random province in the selected country per click.");

         // Quick Assign
         _historicalUnits = ControlFactory.GetPropertyQuickAssignControlCC<List<string>, string>([..Globals.Units.Keys], typeof(CommonCountry).GetProperty(nameof(CommonCountry.HistoricUnits))!, null, "Historic Units", -1, LandUnit.AutoSelectFuncUnits);

         _historicalIdeas = ControlFactory.GetPropertyQuickAssignControlCC<List<string>, string>(Globals.Ideas.Select(x => x.Name).ToList(), typeof(CommonCountry).GetProperty(nameof(CommonCountry.HistoricIdeas))!, null, "Historic Ideas", 8, IdeaGroup.GetAutoAssignment);

         _historicRivals = ControlFactory.GetPropertyQuickAssignControlHC<List<Tag>, Tag>([.. Globals.Countries.Keys], typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.HistoricalRivals))!, typeof(Tag).GetProperty(nameof(DataClasses.GameDataClasses.Tag.TagValue)), "Historic Rivals", 3, Country.GetHistoricRivals);

         _historicFriends = ControlFactory.GetPropertyQuickAssignControlHC<List<Tag>, Tag>([.. Globals.Countries.Keys], typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.HistoricalFriends))!, typeof(Tag).GetProperty(nameof(DataClasses.GameDataClasses.Tag.TagValue)), "Historic Friends", 3, Country.GetHistoricFriends);
         
         _estatePrivileges = ControlFactory.GetPropertyQuickAssignControlHC<List<string>, string>([], typeof(HistoryCountry).GetProperty(nameof(HistoryCountry.EstatePrivileges))!, null, "Estate Privileges", 8, _ => []);
         _estatePrivileges.Enabled = false;

         MiscTLP.Controls.Add(_historicalUnits, 0, 1);
         MiscTLP.Controls.Add(_historicalIdeas, 0, 2);
         MiscTLP.Controls.Add(_historicRivals, 0, 3);
         MiscTLP.Controls.Add(_historicFriends, 0, 4);
         MiscTLP.Controls.Add(_estatePrivileges, 0, 5);

         CountryLoc = ControlFactory.GetPropertyTextBoxCountry(typeof(Country).GetProperty(nameof(Country.TitleLocalisation)), ControlFactory.DefaultMarginType.Slim);
         TagAndColorTLP.Controls.Add(CountryLoc, 1, 1);

         CountryADJLoc = ControlFactory.GetPropertyTextBoxCountry(typeof(Country).GetProperty(nameof(Country.AdjectiveLocalisation)), ControlFactory.DefaultMarginType.Slim);
         TagAndColorTLP.Controls.Add(CountryADJLoc, 3, 1);
         
         CountryCustomToolStripLayoutPanel.Paint += TableLayoutBorder_Paint;
         OpenCountryFileButton.Enter += SetSavingToolTipCountryFileButton;

         _monarchNames = new(typeof(CommonCountry).GetProperty(nameof(CommonCountry.MonarchNames)), ref LoadGuiEvents.CommonCountryLoadAction, () => [Selection.SelectedCountry.CommonCountry])
         {
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         MonarchNamesTLP.Controls.Add(_monarchNames, 0, 1);
      }

      private void SetSavingToolTipCountryFileButton(object? sender, EventArgs e)
      {
         var countries = Selection.GetSelectedProvinceOwners();
         _savingButtonsToolTip.SetToolTip(OpenCountryFileButton,
            countries.Count == 1
               ? $"Open the file of {countries.First().Tag} ({countries.First().TitleLocalisation})"
               : $"Open the files of {countries.Count} countries");
      }

      public void ClearCountryGui()
      {
         Globals.State = State.Loading;
         // Flag
         CountryFlagLabel.SetCountry(Country.Empty);

         // Misc
         _tagSelectionBox.SelectedItem = DataClasses.GameDataClasses.Tag.Empty;
         CountryNameLabel.Text = "Country: -";
         _countryColorPickerButton.BackColor = Color.Empty;
         _countryColorPickerButton.Text = "(//)";
         CountryADJLoc.Clear();
         CountryLoc.Clear();
         RevolutionColorPickerButton.SetDefault();
         _graphicalCultureBox.SetDefault();
         _unitTypeBox.SetDefault();
         _techGroupBox.SetDefault();
         _focusComboBox.SetDefault();
         //_capitalTextBox.Clear();

         // Names
         _leaderEditor.SetDefault();
         _shipEditor.SetDefault();
         _armyEditor.SetDefault();
         _fleetEditor.SetDefault();
         _monarchNames.SetDefault();

         // Cultures
         _primaryCultureBox.SetDefault();
         _acceptedCultures.SetDefault();

         // Government
         _governmentTypeBox.SelectedIndex = 0;
         _governmentRankBox.SetDefault();
         _governmentReforms.SetDefault();

         // Development
         _countryDevelopmentNumeric.Value = 3;

         // Quick Assign
         _historicalUnits.SetDefault();
         _historicalIdeas.SetDefault();
         _historicRivals.SetDefault();
         _historicFriends.SetDefault();
         _estatePrivileges.SetDefault();

         Globals.State = State.Running;
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

      private void quickSettingsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<SettingsWindow>();
      }

      private void CountryAdvancedEditorButton_Click(object sender, EventArgs e)
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


      private void AddNewMonarchNameButton_Click_1(object sender, EventArgs e)
      {
         var handle = Parsing.GetMonarchNameFromTextBoxes(NameTextBox, ChanceTextBox, out var mName);
         if (!handle.Log())
            return;
         
         _monarchNames.AddMonarchName(mName);
         NameTextBox.Clear();
         ChanceTextBox.Clear();
      }
      
      private void ShowMonarchNamesCB_CheckedChanged(object sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (ShowMonachrNamesCB.Checked)
         {
            _monarchNames.SetValue(Selection.SelectedCountry.CommonCountry.MonarchNames);
            NameTextBox.Enabled = true;
            AddNewMonarchNameButton.Enabled = true;
            ChanceTextBox.Enabled = true;
         }
         else
         {
            _monarchNames.SetDefault();
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


      private void infoToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new InformationForm().ShowDialog();
      }

      private void toolStripMenuItem4_Click(object sender, EventArgs e)
      {
#if DEBUG
         var sb = new StringBuilder();
         foreach (var terrain in Globals.Terrains.Values)
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


      private void browseEditedObjectsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new EditedObjectsExplorer().Show();
      }



      private void gameOfLiveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GameOfLive.RunGameOfLive(100);
      }

      private void ViewErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new ErrorLogExplorer()
         {
            StartPosition = FormStartPosition.CenterParent
         }.Show();
      }

      private void OpenProvinceFile_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFiles(Selection.GetSelectedProvincesAsSaveable());
      }

      private void OpenProvinceFolder_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvincesAsSaveable());
      }

      private void TableLayoutBorder_Paint(object? sender, PaintEventArgs e)
      {
         var tableLayout = (TableLayoutPanel)sender!;
         var pen = new Pen(Color.Black, 1);
         e.Graphics.DrawRectangle(pen, 0, 0, tableLayout.Width - 1, tableLayout.Height - 1);
      }

      private void OpenCountryFolder_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvinceOwners().Select(c => c.CommonCountry).Cast<Saveable>().ToList());
      }

      private void OpenCountryHistoryFolder(object? sender, EventArgs e)
      {
         ProcessHelper.OpenSaveableFolders(Selection.GetSelectedProvinceOwners().Select(c => c.HistoryCountry).Cast<Saveable>().ToList());
      }

      private void OpenCountryFileButton_Click(object sender, MouseEventArgs e)
      {
         ProcessHelper.OpenSaveableFiles(e.Button == MouseButtons.Left
            ? Selection.GetSelectedProvinceOwners().Select(c => c.CommonCountry).Cast<Saveable>().ToList()
            : Selection.GetSelectedProvinceOwners().Select(c => c.HistoryCountry).Cast<Saveable>().ToList());
      }

      private void SaveSelectedCountriesButton_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveSaveables(Selection.GetSelectedProvinceOwnersAsSaveable(), onlyModified: false);
      }

      private void SaveAllCountries_Click(object sender, EventArgs e)
      {
         SaveMaster.SaveAllChanges(false, SaveableType.Country);
      }

      private void testToolStripMenuItem_Click(object sender, EventArgs e)
      {
         _ = new RoughEditorForm(new GlobalsDynamicWrapper(), false).ShowDialog();
      }

      private void CountryHistoryEntry_Toggle(object sender, EventArgs e)
      {
         if (Globals.Settings.Gui.EnableDisableHistoryEntryCreationGlobally)
            ProvinceHistoryEntryToggleButton.State = CountryHistoryEntryToggleButton.State;
      }

      private void ProvinceHistoryEntryToggleButton_Click(object sender, EventArgs e)
      {
         if (Globals.Settings.Gui.EnableDisableHistoryEntryCreationGlobally)
            CountryHistoryEntryToggleButton.State = ProvinceHistoryEntryToggleButton.State;
      }

      private void runNameGenToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NameGenStarter.RunNameGen();
      }

      private void loadingToolStripMenuItem_Click(object sender, EventArgs e)
      {
         MapLoading.Load();
      }

      private void saveErrorLogsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         LogManager.SaveLogToFile();
      }

      private void saveErrorLogAscsvToolStripMenuItem_Click(object sender, EventArgs e)
      {
         LogManager.SaveLogAsCsv();
      }

      private void clearCrashLogsToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         CrashManager.ClearCrashLogs();
      }

      private void openCrashLogFolderToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenFolder(CrashManager.LogFolder);
      }

      private void openLastCrashLogsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         CrashManager.OpenLastCrashLogs();
      }

      private void collectionSelectorBaseToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new CollectionSelectorBase([.. Globals.Cultures.Keys]).ShowDialog();
      }

      private void propertyCollectionSelectorToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void generateTextureAtlasPacedToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //var path = Path.Combine(Globals.DebugPath, "atlas.png");
         //GameIconDefinition.CreateSpriteSheetPacked(GameIconDefinition.Icons.Select(x => x.Value.Icon).ToList(), path);


      }

      private void audioTestToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //SoundManager.StartPanning("C:\\Users\\david\\Downloads\\run-130bpm-190419.wav");
         SoundManager.PlayAllSoundsOnce();
      }

      private void tradegoodEditorToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new TradeGoodView().Show();
      }

      private void compactHistoryToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var min = int.MaxValue;
         var max = int.MinValue;
         var total = 0;

         foreach (var country in Globals.Countries.Values)
         {
            if (country == Country.Empty)
               continue;
            var count = country.CommonCountry.MonarchNames.Count;
            total += count;
            if (count < min) 
               min = count;
            if (count > max)
               max = count;
         }

         List<Tag> minCCs = [];
         List<Tag> maxCCs = [];

         foreach (var country in Globals.Countries.Values)
         {
            if (country.CommonCountry.MonarchNames.Count == min)
               minCCs.Add(country.Tag);
            if (country.CommonCountry.MonarchNames.Count == max)
               maxCCs.Add(country.Tag);
         }


         MessageBox.Show($"Min: {min}\nMax: {max}\nTotal: {total}\nAverage: {total / Globals.Countries.Count}\nMinVal: {string.Join(", ", minCCs)}\nMaxVal: {string.Join(", ", maxCCs)}");
      }

      private void compactHistoryToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         HistoryManager.Compact();
      }
   }
}
