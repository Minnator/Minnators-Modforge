using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime;
using System.Text;
using Editor.Controls;
using Editor.Controls.PROPERTY;
using Editor.Controls.PRV_HIST;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Forms.Feature.SavingClasses;
using Editor.Forms.GetUserInput;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Parser;
using Editor.Saving;
using Editor.src.Forms.Console;
using Editor.src.Forms.Feature;
using Editor.src.Forms.GetUserInput;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Region = Editor.DataClasses.Saveables.Region;
using RevoltEffect = Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope.RevoltEffect;

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

      private ProvinceHistoryEntryTreeView _provinceHistoryTreeView = null!;
      #endregion

      #region ProvHistory Editing



      #endregion

      public readonly DateControl DateControl = new(Date.MinValue, DateControlLayout.Horizontal);

      public bool ShowHistoryEntries = false;

      public MapWindow()
      {
         StartPosition = FormStartPosition.Manual;
         Hide();
         Load += OnMainWindowLoad;

         Globals.MapWindow = this;

         Size = new(1527, 966);
         var _startScreen = Screen.FromPoint(Cursor.Position);
         Globals.MapWindow.Location = new((_startScreen.WorkingArea.Width - Globals.MapWindow.Width) / 2,
                                          (_startScreen.WorkingArea.Height - Globals.MapWindow.Height) / 2);
      }

      private void OnMainWindowLoad(object? sender, EventArgs e)
      {
         StartUpManager.StartUp();
      }

      #region Initialize Application and Loadingscreen

      public void Initialize()
      {
         SuspendLayout();

         InitGui();

         Text = $"{Text} | {Globals.DescriptorData.Name}";

#if DEBUG
         debugToolStripMenuItem.Enabled = true;
         debugToolStripMenuItem.Visible = true;
#endif

         // MUST BE LAST in the loading sequence
         InitMapModes();
         //Needs to be after loading the game data to populate the gui with it
         InitializeEditGui();
         // Enable the Application
         ResumeLayout();
      }

      private void SetDynamicContent(Control control, int width = -1)
      {
         MapSplitContainer.SuspendLayout();
         MapSplitContainer.Panel2Collapsed = false;
         MapSplitContainer.IsSplitterFixed = false;
         MapSplitContainer.SplitterDistance = width == -1 ? MapSplitContainer.Width - 300 : MapSplitContainer.Width - width;
         MapSplitContainer.Panel2.Controls.Add(control);
         MapSplitContainer.ResumeLayout();
      }

      private void RemoveDynamicContent()
      {
         MapSplitContainer.SuspendLayout();
         MapSplitContainer.Panel2Collapsed = true;
         MapSplitContainer.Panel2.Controls.Clear();
         MapSplitContainer.IsSplitterFixed = true;
         MapSplitContainer.ResumeLayout();
      }

      private void RenderHistoryIfNeeded()
      {
         if (ShowHistoryEntries)
            SetDynamicContent(_provinceHistoryTreeView);
         else
            RemoveDynamicContent();
      }


      #endregion
      #region MapWindow GUI Init
      private void InitGui()
      {
         InitializeComponent();

         HistoryManager.UndoEvent += (_, _) => UpdateGui();
         HistoryManager.RedoEvent += (_, _) => UpdateGui();

         Globals.ZoomControl = new(new(Globals.MapWidth, Globals.MapHeight))
         {
            BorderWidth = Globals.Settings.Rendering.Map.MapBorderWidth,
            Border = Globals.Settings.Rendering.Map.ShowMapBorder,
            BorderColor = Globals.Settings.Rendering.Map.MapBorderColor,
            MinVisiblePixels = Globals.Settings.Rendering.Map.MinVisiblePixels,
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         _provinceHistoryTreeView = ControlFactory.GetProvinceHistoryTreeView();
         _provinceHistoryTreeView.Dock = DockStyle.Fill;
         _provinceHistoryTreeView.Margin = new(0);
         MapLayoutPanel.Controls.Add(_provinceHistoryTreeView, 1, 0);

         MapSplitContainer.Panel1.Controls.Add(Globals.ZoomControl);
         RenderHistoryIfNeeded();
         Selection.Initialize();
         GuiDrawing.Initialize();

         TopStripLayoutPanel.Controls.Add(DateControl, 8, 0);
         DateControl.OnDateChanged += OnDateChanged;

         SelectionTypeBox.Items.AddRange([.. Enum.GetNames<SelectionType>()]);
         SelectionTypeBox.SelectedIndex = 0;

         BookMarkComboBox.Items.AddRange(["Scenario", .. Globals.Bookmarks]);
         BookMarkComboBox.SelectedIndex = 0;
         BookMarkComboBox.SelectedIndexChanged += OnBookMarkChanged;

         // TODO why does this only work when doing it like this? Why do the map mode buttons not render unless this is done once
         ShowHistoryEntries = true;
         RenderHistoryIfNeeded();
         ShowHistoryEntries = false;
         RenderHistoryIfNeeded();

         // Initialize Settings Events and Listeners
         SettingsHelper.InitializeEvent();
      }

      private void OnBookMarkChanged(object? sender, EventArgs e)
      {
         Debug.Assert(BookMarkComboBox.SelectedIndex != -1, "BookMarkComboBox.SelectedIndex == -1 must never be reached!");
         if (BookMarkComboBox.SelectedIndex == 0)
         {
            DateControl.Date = Globals.StartDate;
            ProvinceHistoryManager.ResetProvinceHistory();
            MapModeManager.RenderCurrent();
            return;
         }

         var bookmark = Globals.Bookmarks[BookMarkComboBox.SelectedIndex-1];
         DateControl.Date = bookmark.Date;
         ProvinceHistoryManager.LoadDate(bookmark.Date);
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
            case 3:
               UpdatePrvHistTab();
               break;
         }
      }

      public IEnumerable<Saveable> GetCurrentSaveables()
      {
         if (DataTabPanel.SelectedIndex == 0)
            return Selection.GetSelectedProvinces;
         if (DataTabPanel.SelectedIndex == 1)
            return [Selection.SelectedCountry];
         return [];
      }

      private void UpdateProvinceTab()
      {
         if (DataTabPanel.SelectedIndex != 0)
            return;

         if (Selection.Count == 0)
         {
            if (!DataTabPanel.TabPages[0].Enabled)
               return;
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
            LoadGuiEvents.ReloadProvinces();
         }
      }
      // TODO OVERTHINK THIS AND HOW IT IS CALLED, MOREOVER HOW THE CALLER OF THIS IS CALLED AND WHY IT IS INITIATED THAT WAY
      private void UpdatePrvHistTab()
      {
         if (DataTabPanel.SelectedIndex != 3)
            return;

         if (Selection.Count == 0)
         {
            if (!DataTabPanel.TabPages[3].Enabled)
               return;
            DataTabPanel.TabPages[3].SuspendLayout();
            Globals.State = State.Loading;
            ClearProvinceGui();
            Globals.State = State.Running;
            DataTabPanel.TabPages[3].Enabled = false;
            DataTabPanel.TabPages[3].ResumeLayout();
         }
         else
         {
            DataTabPanel.TabPages[3].Enabled = true;
            LoadGuiEvents.ReloadHistoryProvinces();
         }
      }

      private void UpdateCountryTab()
      {
         if (DataTabPanel.SelectedIndex != 1)
            return;

         if (Selection.SelectedCountry == Country.Empty)
         {
            if (!CountryMainTableLayoutPanel.Enabled)
               return;
            DataTabPanel.TabPages[1].SuspendLayout();
            ClearDecorationDataCountry();
            Globals.State = State.Loading;
            ClearCountryGui();
            Globals.State = State.Running;
            CountryMainTableLayoutPanel.Enabled = false;
            DataTabPanel.TabPages[1].ResumeLayout();
         }
         else
         {
            CountryMainTableLayoutPanel.Enabled = true;
            SetDecorationDataCountry(Selection.SelectedCountry);
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
         InitializeProvinceHistoryGui();
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
         DataClasses.Saveables.Region.ItemsModified += _regionEditingGui.OnCorrespondingDataChange;

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
         Selection.OnProvinceSelectionChange += (sender, i) => UpdateTabIfSelected(0);
         Selection.OnCountrySelectionChange += (sender, i) => UpdateTabIfSelected(1);
         Selection.OnProvinceSelectionChange += (sender, i) => UpdateTabIfSelected(3);

         DataTabPanel.SelectedIndexChanged += (sender, args) => UpdateGui();


         // CustomTextBox
         _localisationTextBox = ControlFactory.GetProvinceLocBox(typeof(Province).GetProperty(nameof(Province.TitleLocalisation)));
         LocTableLayoutPanel.Controls.Add(_localisationTextBox, 1, 1);

         _provAdjTextBox = ControlFactory.GetProvinceLocBox(typeof(Province).GetProperty(nameof(Province.AdjectiveLocalisation)));
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

         _ownerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioOwner))!, Globals.Countries);
         MisProvinceData.Controls.Add(_ownerTagBox, 1, 0);
         _controllerTagBox = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioController))!, Globals.Countries);
         MisProvinceData.Controls.Add(_controllerTagBox, 1, 1);

         _coreSelector = new(typeof(Province).GetProperty(nameof(Province.ScenarioCores)),
                             ref LoadGuiEvents.ProvLoadAction,
                             () => Selection.GetSelectedProvinces,
                             Globals.Countries.Keys.ToList(),
                             typeof(Tag).GetProperty("TagValue")!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_coreSelector, 0, 2);

         _discoveredBy = new(typeof(Province).GetProperty(nameof(Province.ScenarioDiscoveredBy)),
                             ref LoadGuiEvents.ProvLoadAction,
                             () => Selection.GetSelectedProvinces,
                             [.. Globals.Countries.Keys.Select(x => x.TagValue).ToList(), .. Globals.TechnologyGroups.Keys],
                             null!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_discoveredBy, 0, 4);


         _buildingsSelector = new(typeof(Province).GetProperty(nameof(Province.ScenarioBuildings)),
                                  ref LoadGuiEvents.ProvLoadAction,
                                  () => Selection.GetSelectedProvinces,
                                  Globals.Buildings,
                                  typeof(Building).GetProperty(nameof(Building.Name))!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_buildingsSelector, 1, 2);

         _claimSelector = new(typeof(Province).GetProperty(nameof(Province.ScenarioClaims)),
                              ref LoadGuiEvents.ProvLoadAction,
                              () => Selection.GetSelectedProvinces,
                              Globals.Countries.Keys.ToList(),
                              typeof(Tag).GetProperty("TagValue")!)
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
         };
         ProvinceEditingLayout.Controls.Add(_claimSelector, 0, 1);

         _permaClaimSelector = new(typeof(Province).GetProperty(nameof(Province.ScenarioPermanentClaims)),
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

         _tradeCenterComboBox = ControlFactory.SimpleComboBoxProvince<int>(typeof(Province).GetProperty(nameof(Province.ScenarioCenterOfTrade))!);
         _tradeCenterComboBox.Items.AddRange(["0", "1", "2", "3"]);
         _tradeCenterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         TradePanel.Controls.Add(_tradeCenterComboBox, 1, 0);

         _tradeGoodsComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioTradeGood))!, Globals.TradeGoods);
         TradePanel.Controls.Add(_tradeGoodsComboBox, 1, 1);

         _cultureComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioCulture))!, Globals.Cultures);
         MisProvinceData.Controls.Add(_cultureComboBox, 1, 3);

         _religionComboBox = ControlFactory.GetBindablePropertyComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioReligion))!, Globals.Religions);
         MisProvinceData.Controls.Add(_religionComboBox, 1, 2);

         // TODO GEDANKEN MACHEN Wie man das hier am besten macht
         _terrainComboBox = ControlFactory.GetBindableFakePropertyComboBox(typeof(Province).GetProperty(nameof(Province.Terrain))!, Globals.Terrains);
         _terrainComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
         MisProvinceData.Controls.Add(_terrainComboBox, 1, 5);


         _taxNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioBaseTax)), 0);
         _taxNumeric.Minimum = 0;
         _taxNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_taxNumeric, 3, 0);

         _prdNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioBaseProduction)), 0);
         _prdNumeric.Minimum = 0;
         _prdNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_prdNumeric, 3, 1);

         _mnpNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioBaseManpower)), 0);
         _mnpNumeric.Minimum = 0;
         _mnpNumeric.Maximum = 1000;
         MisProvinceData.Controls.Add(_mnpNumeric, 3, 2);

         _autonomyNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioLocalAutonomy)), 0.0f);
         _autonomyNumeric.Minimum = 0;
         _autonomyNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_autonomyNumeric, 1, 0);

         _devastationNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioDevastation)), 0.0f);
         _devastationNumeric.Minimum = 0;
         _devastationNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_devastationNumeric, 1, 1);

         _prosperityNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioProsperity)), 0.0f);
         _prosperityNumeric.Minimum = 0;
         _prosperityNumeric.Maximum = 100;
         FloatLayoutPanel.Controls.Add(_prosperityNumeric, 1, 2);

         _extraCostNumeric = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioExtraCost)), 0);
         _extraCostNumeric.Minimum = 0;
         _extraCostNumeric.Maximum = 1000;
         TradePanel.Controls.Add(_extraCostNumeric, 1, 2);

         _capitalNameTextBox = ControlFactory.GetPropertyTextBox(typeof(Province).GetProperty(nameof(Province.ScenarioCapital)));
         MisProvinceData.Controls.Add(_capitalNameTextBox, 1, 4);

         _isCityCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.ScenarioIsCity))!);
         _isCityCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isCityCheckBox, 3, 3);

         _isHreCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.ScenarioIsHre))!);
         _isHreCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isHreCheckBox, 3, 4);

         _isParliamentSeatCheckbox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.ScenarioIsSeatInParliament))!);
         _isParliamentSeatCheckbox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_isParliamentSeatCheckbox, 3, 5);

         _hasRevoltCheckBox = ControlFactory.GetExtendedCheckBoxProvince(typeof(Province).GetProperty(nameof(Province.ScenarioHasRevolt))!);
         _hasRevoltCheckBox.CheckedChanged += ProvinceEditingEvents.OnExtendedCheckBoxCheckedChanged;
         MisProvinceData.Controls.Add(_hasRevoltCheckBox, 3, 6);

         MWAttirbuteCombobox.Items.AddRange(typeof(Province).GetProperties()
                                                            .Where(prop => Attribute.IsDefined(prop, typeof(ToolTippable)))
                                                            .Select(x => x.Name).ToArray<object>());
         MWAttirbuteCombobox.SelectedIndexChanged += (sender, args) =>
         {
            Selection.SetMagicWandProperty(MWAttirbuteCombobox.Text);
         };

         // NATIVES TAB
         _tribalOwner = ControlFactory.GetTagComboBox(typeof(Province).GetProperty(nameof(Province.ScenarioTribalOwner))!, Globals.Countries);
         NativesLayoutPanel.Controls.Add(_tribalOwner, 1, 0);

         _nativesSizeTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioNativeSize)), 0, 100);
         _nativesSizeTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativesSizeTextBox, 1, 1);

         _nativeFerocityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioNativeFerocity)), 0f, 100);
         _nativeFerocityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeFerocityTextBox, 1, 3);

         _nativeHostilityTextBox = ControlFactory.GetPropertyNumeric(typeof(Province).GetProperty(nameof(Province.ScenarioNativeHostileness)), 0, 100);
         _nativeHostilityTextBox.Maximum = 10000;
         NativesLayoutPanel.Controls.Add(_nativeHostilityTextBox, 1, 2);

         // TRADE_COMPANIES TAB
         _tradeCompanyInvestments = ControlFactory.GetExtendedComboBox(nameof(Province.ScenarioTradeCompanyInvestments));
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

      internal void UpdateRedoDepth(object? sender, Func<(int, int)> valueTuple)
      {
         var (simple, full) = valueTuple();
         RedoDepthLabel.Text = $"Redos [{simple}/{full}]";
      }

      internal void UpdateUndoDepth(object? sender, Func<(int, int)> valueTuple)
      {
         var (simple, full) = valueTuple();
         UndoDepthLabel.Text = $"Undos [{simple}/{full}]";
      }

      #endregion

      public void UpdateErrorCounts(int errors, int infos)
      {
         if (Globals.State == State.Loading)
            return;

         if (InvokeRequired)
            Invoke(new Action(() => UpdateErrorCounts(errors, infos)));
         else
            ErrorCountLabel.Text = $"Errors: {errors} | Infos: {infos}";
      }

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
         if (Globals.State == State.Loading)
            return;
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

      #region CountyEditGUI

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

      private PropertyNumeric<Country, int> _countryDevelopmentNumeric = null!;

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
         _tagSelectionBox = new()
         {
            Margin = new(2),
            Dock = DockStyle.Fill,
            Font = new("Arial", 10, FontStyle.Bold),
         };
         CountryFlagLabel = ControlFactory.GetFlagLabel();
         _tagSelectionBox.OnTagChanged += CountryGuiEvents.TagSelectionBox_OnTagChanged;
         CountryHeaderTLP.Controls.Add(_tagSelectionBox, 2, 0);
         CountryHeaderTLP.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

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
                                                                                   Enumerable.Range(1, Define.GetValue<int>("NDefines.NCountry.MAX_GOV_RANK")).Select(i => i).ToList());
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
         _countryDevelopmentNumeric = ControlFactory.GetPropertyNumericCountry(typeof(Country).GetProperty(nameof(Country.Development)), 0);
         _countryDevelopmentNumeric.Minimum = 0;
         _countryDevelopmentNumeric.Maximum = 100000;
         _countryDevelopmentNumeric.UseTimer = false;
         _countryDevelopmentNumeric.IsSilent = true;

         DevelopmenTLP.Controls.Add(_countryDevelopmentNumeric, 1, 0);
         GeneralToolTip.SetToolTip(_countryDevelopmentNumeric, "LMB = +- 1\nSHIFT + LMB = +- 5\nCTRL + LMB = +-10\nThe development is only added to one random province in the selected country per click.");

         // Quick Assign
         _historicalUnits = ControlFactory.GetPropertyQuickAssignControlCC<List<string>, string>([.. Globals.Units.Keys], typeof(CommonCountry).GetProperty(nameof(CommonCountry.HistoricUnits))!, null, "Historic Units", -1, LandUnit.AutoSelectFuncUnits);

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
         TagAndColorTLP.Controls.Add(CountryLoc, 1, 0);

         CountryADJLoc = ControlFactory.GetPropertyTextBoxCountry(typeof(Country).GetProperty(nameof(Country.AdjectiveLocalisation)), ControlFactory.DefaultMarginType.Slim);
         TagAndColorTLP.Controls.Add(CountryADJLoc, 1, 1);

         CountryCustomToolStripLayoutPanel.Paint += TableLayoutBorder_Paint;
         OpenCountryFileButton.Enter += SetSavingToolTipCountryFileButton;

         _monarchNames = new(typeof(CommonCountry).GetProperty(nameof(CommonCountry.MonarchNames)), ref LoadGuiEvents.CommonCountryLoadAction, () => [Selection.SelectedCountry.CommonCountry])
         {
            Dock = DockStyle.Fill,
            Margin = new(0),
         };

         MonarchNamesTLP.Controls.Add(_monarchNames, 0, 1);
      }


      private void MonarchNamesImportButton_Click(object sender, EventArgs e)
      {
         var objs = new NIntInputForm.NIntInputObj[2];
         objs[0] = new("Num to monarch names import", 1, -1, Selection.SelectedCountry.HistoryCountry.PrimaryCulture.TotalNameCount / 4f, false);
         objs[1] = new("Female fraction", 0, 1, 0.1f, true);

         var numToImport = NIntInputForm.ShowGet(objs, "Monarch names from culture");

         if (Math.Abs(numToImport[0] - (-1f)) < 0.04)
            return;

         MonarchName.GenerateFromCulture(Selection.SelectedCountry, (int)Math.Round(numToImport[0]), numToImport[1]);
      }

      private void ClearDecorationDataCountry()
      {
         CountryFlagLabel.SetCountry(Country.Empty);
         _tagSelectionBox.SelectedItem = DataClasses.GameDataClasses.Tag.Empty;
         CountryNameLabel.Text = "-";
      }

      private void SetDecorationDataCountry(Country country)
      {
         CountryFlagLabel.SetCountry(country.Tag);
         _tagSelectionBox.SelectedItem = country.Tag;
         CountryNameLabel.Text = country.TitleLocalisation;
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

         // Misc
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
         if (_governmentTypeBox.Items.Count > 0)
            _governmentTypeBox.SelectedIndex = 0;
         _governmentRankBox.SetDefault();
         _governmentReforms.SetDefault();

         // Development
         _countryDevelopmentNumeric.SetDefault();

         // Quick Assign
         _historicalUnits.SetDefault();
         _historicalIdeas.SetDefault();
         _historicRivals.SetDefault();
         _historicFriends.SetDefault();
         _estatePrivileges.SetDefault();

         Globals.State = State.Running;
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
         Selection.SelectedCountry.SpreadDevInSelectedCountryIfValid((int)_countryDevelopmentNumeric.Value);
      }

      #endregion

      #region ProvinceEditGUI

      private PrvHistIntUi _prvHistTaxNumeric = null!;
      private PrvHistIntUi _prvHistPrdNumeric = null!;
      private PrvHistIntUi _prvHistMnpNumeric = null!;
      private PrvHistIntUi _prvHistExtraCostNumeric = null!;
      private PrvHistIntUi _prvHistNativesSizeNumeric = null!;
      private PrvHistIntUi _prvHistNativesHostilityNumeric = null!;

      private PrvHistFloatUi _prvHistAutonomyNumeric = null!;
      private PrvHistFloatUi _prvHistDevastationNumeric = null!;
      private PrvHistFloatUi _prvHistProsperityNumeric = null!;
      private PrvHistFloatUi _prvHistNativesFerocityNumeric = null!;

      private PrvHistSetAddUi _prvHistIsCityCheckBox = null!;
      private PrvHistSetAddUi _prvHistIsHreCheckBox = null!;
      private PrvHistSetAddUi _prvHistIsParliamentSeatCheckbox = null!;
      // private PrvHistSetAddUi _prvHistIasRevoltCheckBox = null!; to complex for the quick UI

      private BindablePrvHistDropDownUi<Country, Tag> _prvHistOwnerTagBox = null!;
      private BindablePrvHistDropDownUi<Country, Tag> _prvHistControllerTagBox = null!;
      private BindablePrvHistDropDownUi<Country, Tag> _prvHistTribalOwnerComboBox = null!;

      private BindablePrvHistDropDownUi<Religion, string> _prvHistReligionComboBox = null!;
      private BindablePrvHistDropDownUi<Culture, string> _prvHistCultureComboBox = null!;
      private BindablePrvHistDropDownUi<TradeGood, string> _prvHistTradeGoodsComboBox = null!;
      private PrvHistDropDownUi<int> _prvHistTradeCenterComboBox = null!;

      private PrvHistCollectionUi _prvHistClaimSelector = null!;
      private PrvHistCollectionUi _prvHistPermaClaimSelector = null!;
      private PrvHistCollectionUi _prvHistCores = null!;
      private PrvHistCollectionUi _prvHistBuildings = null!;
      private PrvHistCollectionUi _prvHistDiscoveredBy = null!;

      private PrvHistTextBoxUi _prvHistLocTextBox = null!;
      private PrvHistTextBoxUi _prvHistCapitalTextBox = null!;

      private PrvHistSeparator[] _prvHistSeparators = null!;


      // ------------------- PROVINCE EDITING TAB ------------------- \\
      private void InitializeProvinceHistoryGui()
      {
         ProvHistoryLayout.Paint += TableLayoutBorder_Paint;

         _prvHistSeparators = new PrvHistSeparator[7];
         for (var i = 0; i < _prvHistSeparators.Length; i++)
            _prvHistSeparators[i] = ControlFactory.GetDefaultSeparator();

         var blockOffset = 1;

         _prvHistOwnerTagBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.Owner),
                                                                           typeof(Province).GetProperty(nameof(Province.Owner))!,
                                                                           Globals.Countries,
                                                                           Scopes.Province.Effects[OwnerEffect.EffectName],
                                                                           true);
         _prvHistControllerTagBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.Controller),
                                                                                typeof(Province).GetProperty(nameof(Province.Controller))!,
                                                                                Globals.Countries,
                                                                                Scopes.Province.Effects[ControllerEffect.EffectName],
                                                                                true);

         ProvHistoryLayout.Controls.Add(_prvHistOwnerTagBox, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistControllerTagBox, 0, blockOffset + 1);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[3], 0, blockOffset + 2);
         blockOffset += 3;

         _prvHistTaxNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.BaseTax),
                                                             typeof(Province).GetProperty(nameof(Province.BaseTax))!,
                                                             Scopes.Province.Effects[AddBaseTaxEffect.EffectName],
                                                             Scopes.Province.Effects[BaseTaxEffect.EffectName]);
         _prvHistPrdNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.BaseProduction),
                                                             typeof(Province).GetProperty(nameof(Province.BaseProduction))!,
                                                             Scopes.Province.Effects[AddBaseProductionEffect.EffectName],
                                                             Scopes.Province.Effects[BaseProductionEffect.EffectName]);
         _prvHistMnpNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.BaseManpower),
                                                             typeof(Province).GetProperty(nameof(Province.BaseManpower))!,
                                                             Scopes.Province.Effects[AddBaseManpowerEffect.EffectName],
                                                             Scopes.Province.Effects[BaseManpowerEffect.EffectName]);

         ProvHistoryLayout.Controls.Add(_prvHistTaxNumeric, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistPrdNumeric, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistMnpNumeric, 0, blockOffset + 2);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[0], 0, blockOffset + 3);
         blockOffset += 4;

         _prvHistReligionComboBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.Religion),
                                                                                typeof(Province).GetProperty(nameof(Province.Religion))!,
                                                                                Globals.Religions,
                                                                                Scopes.Province.Effects[ReligionEffect.EffectName],
                                                                                false);
         _prvHistCultureComboBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.Culture),
                                                                               typeof(Province).GetProperty(nameof(Province.Culture))!,
                                                                               Globals.Cultures,
                                                                               Scopes.Province.Effects[CultureEffect.EffectName],
                                                                               false);
         _prvHistTradeGoodsComboBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.TradeGood),
                                                                                  typeof(Province).GetProperty(nameof(Province.TradeGood))!,
                                                                                  Globals.TradeGoods,
                                                                                  Scopes.Province.Effects[TradeGoodsEffect.EffectName],
                                                                                  false,
                                                                                  true);
         _prvHistTradeCenterComboBox = ControlFactory.GetPrvHistDropDownUi<int>(nameof(Province.CenterOfTrade),
                                                                                typeof(Province).GetProperty(nameof(Province.CenterOfTrade))!,
                                                                                Scopes.Province.Effects[CenterOfTradeEffect.EffectName],
                                                                                false,
                                                                                true);
         _prvHistTradeCenterComboBox.DropDown.Items.AddRange([.."0", "1", "2", "3"]);

         ProvHistoryLayout.Controls.Add(_prvHistReligionComboBox, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistCultureComboBox, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistTradeGoodsComboBox, 0, blockOffset + 2);
         ProvHistoryLayout.Controls.Add(_prvHistTradeCenterComboBox, 0, blockOffset + 3);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[4], 0, blockOffset + 4);
         blockOffset += 5;

         _prvHistIsCityCheckBox = ControlFactory.GetPrvHistBoolUi(nameof(Province.IsCity),
                                                                  typeof(Province).GetProperty(nameof(Province.IsCity))!,
                                                                  Scopes.Province.Effects[IsCityEffect.EffectName],
                                                                  false);
         _prvHistIsHreCheckBox = ControlFactory.GetPrvHistBoolUi(nameof(Province.IsHre),
                                                                 typeof(Province).GetProperty(nameof(Province.IsHre))!,
                                                                 Scopes.Province.Effects[HreEffect.EffectName],
                                                                 false);
         _prvHistIsParliamentSeatCheckbox = ControlFactory.GetPrvHistBoolUi(nameof(Province.IsSeatInParliament),
                                                                            typeof(Province).GetProperty(nameof(Province.IsSeatInParliament))!,
                                                                            Scopes.Province.Effects[SeatInParliamentEffect.EffectName],
                                                                            false);

         ProvHistoryLayout.Controls.Add(_prvHistIsCityCheckBox, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistIsHreCheckBox, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistIsParliamentSeatCheckbox, 0, blockOffset + 2);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[2], 0, blockOffset + 3);
         blockOffset += 4;

         _prvHistLocTextBox = ControlFactory.GetPrvHistTextBoxUi(nameof(Province.TitleLocalisation));
         _prvHistCapitalTextBox = ControlFactory.GetPrvHistTextBoxUi(nameof(Province.Capital));

         ProvHistoryLayout.Controls.Add(_prvHistLocTextBox, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistCapitalTextBox, 0, blockOffset + 1);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[6], 0, blockOffset + 2);
         blockOffset += 3;

         _prvHistDevastationNumeric = ControlFactory.GetPrvHistFloatUi(nameof(Province.Devastation),
                                                                       typeof(Province).GetProperty(nameof(Province.Devastation))!,
                                                                       Scopes.Province.Effects[AddDevastationEffect.EffectName],
                                                                       Scopes.Province.Effects[AddDevastationEffect.EffectName],
                                                                       hasSet: false);
         _prvHistAutonomyNumeric = ControlFactory.GetPrvHistFloatUi(nameof(Province.LocalAutonomy),
                                                                    typeof(Province).GetProperty(nameof(Province.LocalAutonomy))!,
                                                                    Scopes.Province.Effects[AddLocalAutonomyEffect.EffectName],
                                                                    Scopes.Province.Effects[SetLocalAutonomyEffect.EffectName]);
         _prvHistProsperityNumeric = ControlFactory.GetPrvHistFloatUi(nameof(Province.Prosperity),
                                                                      typeof(Province).GetProperty(nameof(Province.Prosperity))!,
                                                                      Scopes.Province.Effects[AddProsperityEffect.EffectName],
                                                                      Scopes.Province.Effects[AddProsperityEffect.EffectName],
                                                                      hasSet:false);
         _prvHistExtraCostNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.ExtraCost),
                                                                   typeof(Province).GetProperty(nameof(Province.ExtraCost))!,
                                                                   Scopes.Province.Effects[ExtraCostEffect.EffectName],
                                                                   Scopes.Province.Effects[ExtraCostEffect.EffectName],
                                                                   hasSet: false);

         ProvHistoryLayout.Controls.Add(_prvHistDevastationNumeric, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistAutonomyNumeric, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistProsperityNumeric, 0, blockOffset + 2);
         ProvHistoryLayout.Controls.Add(_prvHistExtraCostNumeric, 0, blockOffset + 3);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[1], 0, blockOffset + 4);
         blockOffset += 5;

         _prvHistCores = ControlFactory.GetPrvHistCollectionUi(nameof(Province.Cores));
         _prvHistClaimSelector = ControlFactory.GetPrvHistCollectionUi(nameof(Province.Claims));
         _prvHistPermaClaimSelector = ControlFactory.GetPrvHistCollectionUi(nameof(Province.PermanentClaims));
         _prvHistBuildings = ControlFactory.GetPrvHistCollectionUi(nameof(Province.Buildings));
         _prvHistDiscoveredBy = ControlFactory.GetPrvHistCollectionUi(nameof(Province.DiscoveredBy));


         ProvHistoryLayout.Controls.Add(_prvHistCores, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistClaimSelector, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistPermaClaimSelector, 0, blockOffset + 2);
         ProvHistoryLayout.Controls.Add(_prvHistBuildings, 0, blockOffset + 3);
         ProvHistoryLayout.Controls.Add(_prvHistDiscoveredBy, 0, blockOffset + 4);

         ProvHistoryLayout.Controls.Add(_prvHistSeparators[5], 0, blockOffset + 5);
         blockOffset += 6;

         _prvHistTribalOwnerComboBox = ControlFactory.GetBindablePrvHistDropDownUi(nameof(Province.TribalOwner),
                                                                               typeof(Province).GetProperty(nameof(Province.TribalOwner))!,
                                                                               Globals.Countries,
                                                                               Scopes.Province.Effects[TribalOwnerEffect.EffectName],
                                                                               true);
         _prvHistNativesSizeNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.NativeSize),
                                                                     typeof(Province).GetProperty(nameof(Province.NativeSize))!,
                                                                     Scopes.Province.Effects[NativeSizeEffect.EffectName],
                                                                     Scopes.Province.Effects[NativeSizeEffect.EffectName],
                                                                     hasSet: false);
         _prvHistNativesHostilityNumeric = ControlFactory.GetPrvHistIntUi(nameof(Province.NativeHostileness),
                                                                         typeof(Province).GetProperty(nameof(Province.NativeHostileness))!,
                                                                         Scopes.Province.Effects[NativeHostilnessEffect.EffectName],
                                                                         Scopes.Province.Effects[NativeHostilnessEffect.EffectName],
                                                                         hasSet: false);
         _prvHistNativesFerocityNumeric = ControlFactory.GetPrvHistFloatUi(nameof(Province.NativeFerocity),
                                                                         typeof(Province).GetProperty(nameof(Province.NativeFerocity))!,
                                                                         Scopes.Province.Effects[NativeFerocityEffect.EffectName],
                                                                         Scopes.Province.Effects[NativeFerocityEffect.EffectName],
                                                                         hasSet: false);

         ProvHistoryLayout.Controls.Add(_prvHistTribalOwnerComboBox, 0, blockOffset + 0);
         ProvHistoryLayout.Controls.Add(_prvHistNativesSizeNumeric, 0, blockOffset + 1);
         ProvHistoryLayout.Controls.Add(_prvHistNativesHostilityNumeric, 0, blockOffset + 2);
         ProvHistoryLayout.Controls.Add(_prvHistNativesFerocityNumeric, 0, blockOffset + 3);
      }

      #endregion


      private void infoToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new InformationForm().ShowDialog();
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
         var atlas = GameIconDefinition.CreateTextureAtlas();
         ImagePopUp.ShowImage(atlas);
         atlas.Dispose();
      }

      private void audioTestToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var icon = GameIconDefinition.Icons[GameIcons.TradeGoods].Icon;
         icon.Save(Path.Combine(Globals.AppDataPath, "trade_node_strip.png"), ImageFormat.Png);
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

      private void AchievementsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new AchievementsWindow().ShowDialog();
      }

      private void definesEditorToolStripMenuItem_Click(object sender, EventArgs e)
      {
         new DefinesEditor().ShowDialog();
      }

      private void namesGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<NameGeneratorForm>(Globals.Settings.Misc.NameGenConfig.NameGenConfig);
      }

      private void testTriggerToolStripMenuItem_Click(object sender, EventArgs e)
      {
      }

      private void effectsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var browser = new WikiBrowser(WikiHelper.LoadEffectsFromAssembly);
         browser.ShowDialog();
      }

      private void ErrorCountLabel_MouseEnter(object sender, EventArgs e)
      {
#if DEBUG
         ErrorCountLabel.ToolTipText = $"Critical: {LogManager.ModCriticalCount}\nErrors: {LogManager.ModErrorCount}\nWarnings: {LogManager.ModWarningCount}\nInformation: {LogManager.ModInformationCount}\n-----------------\nDebug: {LogManager.ModDebugCount}";
#else
         ErrorCountLabel.ToolTipText = $"Critical: {LogManager.CriticalCount}\nErrors: {LogManager.ErrorCount}\nWarnings: {LogManager.WarningCount}\nInformation: {LogManager.InformationCount}";
#endif
      }

      private void calenderViewToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<CalenderHistoryView>();
      }

      private void saveSelectionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var sd = new SelectionDrawerForm();
         sd.Show();
      }

      private void applyMaskToImageToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var source = GameIconDefinition.ReadImage("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\gfx\\flags\\BUR.tga");
         var mask = GameIconDefinition.ReadImage("S:\\SteamLibrary\\steamapps\\common\\Europa Universalis IV\\gfx\\interface\\small_shield_mask.tga");
         var result = BmpLoading.ApplyMask(source, mask);

         result.SaveBmpToModforgeData("maskedImage.png");
      }

      private void missionExporterToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FormsHelper.ShowIfAnyOpen<MissionTreeExporter>();
      }

      private void ShowHistoryCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         ShowHistoryEntries = ShowHistoryCheckBox.Checked;
         RenderHistoryIfNeeded();
      }

      #region HISTORY ENTRIES

      /*
       * Add context menu strip on items in the history list
       * Add context menu on HistoryEntry tree
       * LMB double-click on HistoryEntry tree to edit the value
       * ENTF on selected to delete entry / token
       */



      #endregion

      private void scriptedEffectsToolStripMenuItem_Click(object sender, EventArgs e)
      {

         var browser = new WikiBrowser(WikiHelper.LoadScriptedEffects);
         browser.SetContextMenu(WikiHelper.ScriptedEffectContextMenu());
         browser.ShowDialog();
      }

      private void scopesToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenLink("https://eu4.paradoxwikis.com/Scopes");
      }

      private void triggerToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenLink("https://eu4.paradoxwikis.com/Triggers");
      }

      private void effectsToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenLink("https://eu4.paradoxwikis.com/Effects");
      }

      private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ProcessHelper.OpenLink("https://eu4.paradoxwikis.com/Modifier_list");
      }

   }
}
