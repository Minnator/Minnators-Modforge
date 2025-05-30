﻿using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;
using Editor.Saving;

namespace Editor.DataClasses.Saveables
{
   public class Province : ProvinceComposite, ITitleAdjProvider, IHistoryProvider<ProvinceHistoryEntry>, ITarget, IComparable
   {
      #region Init Scenario Data

      private int _scenarioBaseTax;
      private int _scenarioBaseProduction;
      private int _scenarioBaseManpower;
      private int _scenarioCenterOfTrade;
      private int _scenarioExtraCost;
      private int _scenarioNativeHostileness;
      private int _scenarioNativeSize;
      private int _scenarioRevoltRisk;
      private int _scenarioNationalism;
      private int _scenarioCitySize;

      private float _scenarioNativeFerocity;
      private float _scenarioLocalAutonomy;
      private float _scenarioDevastation;
      private float _scenarioProsperity;

      private bool _scenarioIsHre;
      private bool _scenarioIsCity;
      private bool _scenarioHasRevolt;
      private bool _scenarioIsSeatInParliament;

      private string _scenarioCapital = string.Empty;

      private Country _scenarioController = Country.Empty;
      private Country _scenarioOwner = Country.Empty;
      private Country _scenarioTribalOwner = Country.Empty;
      
      private Culture _scenarioCulture = Culture.Empty;
      private Religion _scenarioReligion = Religion.Empty;
      private Religion _scenarioReformationCenter = Religion.Empty;
      private TradeGood _scenarioTradeGood = TradeGood.Empty;
      private TradeGood _scenarioLatentTradeGood = TradeGood.Empty;
      private List<string> _scenarioDiscoveredBy = [];
      private List<string> _scenarioTradeCompanyInvestments = [];
      private List<string> _scenarioProvinceTriggeredModifiers = [];
      private List<Country> _scenarioClaims = [];
      private List<Country> _scenarioPermanentClaims = [];
      private List<Country> _scenarioCores = [];
      private List<Building> _scenarioBuildings = [];
      private List<ApplicableModifier> _scenarioPermanentProvinceModifiers = [];
      private List<ApplicableModifier> _scenarioProvinceModifiers = [];
      private List<IElement> _scenarioScriptedEffects = [];
      private List<TradeModifier> _scenarioTradeModifiers = [];

      #endregion

      #region State Data
      private Country _controller = Country.Empty;
      private Country _owner = Country.Empty;
      private Country _tribalOwner = Country.Empty;
      private int _baseManpower = 1;
      private int _baseTax = 1;
      private int _baseProduction = 1;
      private int _centerOfTrade;
      private int _extraCost;
      private int _nativeHostileness;
      private int _nativeSize;
      private int _revoltRisk;
      private int _nationalism;  
      private int _citySize;
      private float _nativeFerocity;
      private float _autonomy;
      private float _devastation;
      private float _prosperity;
      private bool _isHre;
      private bool _isCity;
      private bool _hasRevolt;
      private bool _isSeatInParliament;
      private string _capital = string.Empty;
      private Culture _culture = Culture.Empty;
      private Religion _religion = Religion.Empty;
      private Religion _reformationCenter = Religion.Empty;
      private TradeGood _tradeGood = TradeGood.Empty;
      private TradeGood _latentTradeGood = TradeGood.Empty;
      private List<Country> _claims = [];
      private List<Country> _permaClaims = [];
      private List<Country> _cores = [];
      private List<string> _discoveredBy = [];
      private List<Building> _buildings = [];
      private List<string> _tradeCompanyInvestments = [];
      private List<ApplicableModifier> _permanentProvinceModifiers = [];
      private List<ApplicableModifier> _provinceModifiers = [];
      private List<string> _provinceTriggeredModifiers = [];
      private List<IElement> _scriptedEffects = [];
      private List<TradeModifier> _tradeModifiers = [];


      // ##################### Complex setter #####################

      [ToolTippable]
      public Country Owner
      {
         get => _owner;
         set
         {
            if (_owner == value)
               return;

            if (_owner != Country.Empty)
               _owner.Remove(this);
            if (value == Country.Empty)
               return;
            value.Add(this);
            _owner = value;
         }
      }
      public Country ScenarioOwner
      {
         get => _scenarioOwner;
         set => SetField(ref _scenarioOwner, ref _owner, value);
      }


      #endregion

      // ##################### Simple setter #####################
      # region SimpleSetter

      public List<Country> Claims { get => _claims; set => _claims = value; }
      [GameIcon(GameIcons.Claim)]
      public List<Country> ScenarioClaims
      {
         get => _scenarioClaims;
         set => SetIfModifiedEnumerable<List<Country>, Country>(ref _scenarioClaims, ref _claims, value);
      }

      public List<Country> PermaClaims { get => _permaClaims; set => _permaClaims = value; }
      [GameIcon(GameIcons.Claim)]
      public List<Country> ScenarioPermanentClaims
      {
         get => _scenarioPermanentClaims;
         set => SetIfModifiedEnumerable<List<Country>, Country>(ref _scenarioPermanentClaims, ref _permaClaims, value);
      }

      public List<Country> Cores { get => _cores; set => _cores = value; }
      [GameIcon(GameIcons.Core, false)]
      public List<Country> ScenarioCores
      {
         get => _scenarioCores;
         set => SetIfModifiedEnumerable<List<Country>, Country>(ref _scenarioCores, ref _cores, value);
      }

      [ToolTippable]
      public Country Controller
      {
         get => _controller; 
         set => _controller = value;
      }
      public Country ScenarioController {
         get => _scenarioController;
         set => SetField(ref _scenarioController, ref _controller, value);
      }

      [ToolTippable]
      public Country TribalOwner { get => _tribalOwner; set => _tribalOwner = value; }
      public Country ScenarioTribalOwner
      {
         get => _scenarioTribalOwner;
         set => SetField(ref _scenarioTribalOwner, ref _tribalOwner, value);
      }

      [ToolTippable]
      public int BaseManpower { get => _baseManpower; set => _baseManpower = Math.Max(0, value); }
      public int ScenarioBaseManpower
      {
         get => _scenarioBaseManpower;
         set => SetField(ref _scenarioBaseManpower, ref _baseManpower, Math.Max(0, value));
      }

      [ToolTippable]
      public int BaseTax { get => _baseTax; set => _baseTax = value; }
      public int ScenarioBaseTax
      {
         get => _scenarioBaseTax;
         set => SetField(ref _scenarioBaseTax, ref _baseTax, Math.Max(0, value));
      }

      [ToolTippable]
      public int BaseProduction { get => _baseProduction; set => _baseProduction = Math.Max(0, value); }
      public int ScenarioBaseProduction
      {
         get => _scenarioBaseProduction;
         set => SetField(ref _scenarioBaseProduction, ref _baseProduction, Math.Max(0, value));
      }

      [ToolTippable]
      public int CenterOfTrade { get => _centerOfTrade; set => _centerOfTrade = Math.Max(0, value); }
      public int ScenarioCenterOfTrade
      {
         get => _scenarioCenterOfTrade;
         set => SetField(ref _scenarioCenterOfTrade, ref _centerOfTrade, Math.Max(0, value));
      }

      [ToolTippable]
      public int ExtraCost { get => _extraCost; set => _extraCost = Math.Max(0, value); }
      public int ScenarioExtraCost
      {
         get => _scenarioExtraCost;
         set => SetField(ref _scenarioExtraCost, ref _extraCost, Math.Max(0, value));
      }

      [ToolTippable]
      public float NativeFerocity { get => _nativeFerocity; set => _nativeFerocity = Math.Max(0, value); }
      public float ScenarioNativeFerocity
      {
         get => _scenarioNativeFerocity;
         set => SetField(ref _scenarioNativeFerocity, ref _nativeFerocity, Math.Max(0, value));
      }

      [ToolTippable]
      public int NativeHostileness { get => _nativeHostileness; set => _nativeHostileness = Math.Max(0, value); }
      public int ScenarioNativeHostileness
      {
         get => _scenarioNativeHostileness;
         set => SetField(ref _scenarioNativeHostileness, ref _nativeHostileness, Math.Max(0, value));
      }

      [ToolTippable]
      public int NativeSize { get => _nativeSize; set => _nativeSize = Math.Max(0, value); }
      public int ScenarioNativeSize
      {
         get => _scenarioNativeSize;
         set => SetField(ref _scenarioNativeSize, ref _nativeSize, Math.Max(0, value));
      }

      [ToolTippable]
      public int RevoltRisk { get => _revoltRisk; set => _revoltRisk = Math.Max(0, value); }
      public int ScenarioRevoltRisk
      {
         get => _scenarioRevoltRisk;
         set => SetField(ref _scenarioRevoltRisk, ref _revoltRisk, Math.Max(0, value));
      }

      [ToolTippable]
      public int CitySize { get => _citySize; set => _citySize = Math.Max(0, value); }
      public int ScenarioCitySize
      {
         get => _scenarioCitySize;
         set => SetField(ref _scenarioCitySize, ref _citySize, Math.Max(0, value));
      }

      [ToolTippable]
      public int Nationalism { get => _nationalism; set => _nationalism = Math.Max(0, value); }
      public int ScenarioNationalism
      {
         get => _scenarioNationalism;
         set => SetField(ref _scenarioNationalism, ref _nationalism, Math.Max(0, value));
      }

      [ToolTippable]
      public float Autonomy { get => _autonomy; set => _autonomy = Math.Max(0, value); }
      public float ScenarioLocalAutonomy
      {
         get => _scenarioLocalAutonomy;
         set => SetField(ref _scenarioLocalAutonomy, ref _autonomy, Math.Max(0, value));
      }

      [ToolTippable]
      public float Devastation { get => _devastation; set => _devastation = Math.Max(0, value); }
      public float ScenarioDevastation
      {
         get => _scenarioDevastation;
         set => SetField(ref _scenarioDevastation, ref _devastation, Math.Max(0, value));
      }

      [ToolTippable]
      public float Prosperity { get => _prosperity; set => _prosperity = Math.Max(0, value); }
      public float ScenarioProsperity
      {
         get => _scenarioProsperity;
         set => SetField(ref _scenarioProsperity, ref _prosperity, Math.Max(0, value));
      }

      public List<string> DiscoveredBy { get => _discoveredBy; set => _discoveredBy = value; }
      [GameIcon(GameIcons.DiscoverAchievement)]
      public List<string> ScenarioDiscoveredBy
      {
         get => _scenarioDiscoveredBy;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _scenarioDiscoveredBy, ref _discoveredBy, value);
      }

      [ToolTippable]
      public string Capital { get => _capital; set => _capital = value; }
      public string ScenarioCapital
      {
         get => _scenarioCapital;
         set => SetField(ref _scenarioCapital, ref _capital, value);
      }

      [ToolTippable]
      public Culture Culture { get => _culture; set => _culture = value; }
      public Culture ScenarioCulture
      {
         get => _scenarioCulture;
         set => SetField(ref _scenarioCulture, ref _culture, value);
      }

      [ToolTippable]
      public Religion Religion { get => _religion; set => _religion = value; }
      public Religion ScenarioReligion
      {
         get => _scenarioReligion;
         set => SetField(ref _scenarioReligion, ref _religion, value);
      }

      public List<Building> Buildings { get => _buildings; set => _buildings = value; }
      [GameIcon(GameIcons.Building, false)]
      public List<Building> ScenarioBuildings
      {
         get => _scenarioBuildings;
         set => SetIfModifiedEnumerable<List<Building>, Building>(ref _scenarioBuildings, ref _buildings, value);
      }

      [ToolTippable]
      public bool IsHre { get => _isHre; set => _isHre = value; }
      public bool ScenarioIsHre
      {
         get => _scenarioIsHre;
         set => SetField(ref _scenarioIsHre, ref _isHre, value);
      }

      [ToolTippable]
      public bool IsCity { get => _isCity; set => _isCity = value; }
      public bool ScenarioIsCity
      {
         get => _scenarioIsCity;
         set => SetField(ref _scenarioIsCity, ref _isCity, value);
      }

      [ToolTippable]
      public bool IsSeatInParliament { get => _isSeatInParliament; set => _isSeatInParliament = value; }
      public bool ScenarioIsSeatInParliament
      {
         get => _scenarioIsSeatInParliament;
         set => SetField(ref _scenarioIsSeatInParliament, ref _isSeatInParliament, value);
      }

      [ToolTippable]
      public TradeGood TradeGood { get => _tradeGood; set => _tradeGood = value; }
      public TradeGood ScenarioTradeGood
      {
         get => _scenarioTradeGood;
         set => SetField(ref _scenarioTradeGood, ref _tradeGood, value);
      }

      [ToolTippable]
      public TradeGood LatentTradeGood { get => _latentTradeGood; set => _latentTradeGood = value; }
      public TradeGood ScenarioLatentTradeGood
      {
         get => _scenarioLatentTradeGood;
         set => SetField(ref _scenarioLatentTradeGood, ref _latentTradeGood, value);
      }

      [ToolTippable]
      public bool HasRevolt { get => _hasRevolt; set => _hasRevolt = value; }
      public bool ScenarioHasRevolt
      {
         get => _scenarioHasRevolt;
         set => SetField(ref _scenarioHasRevolt, ref _hasRevolt, value);
      }

      [ToolTippable]
      public Religion ReformationCenter { get => _reformationCenter; set => _reformationCenter = value; }
      public Religion ScenarioReformationCenter
      {
         get => _scenarioReformationCenter;
         set => SetField(ref _scenarioReformationCenter, ref _reformationCenter, value);
      }

      public List<string> TradeCompanyInvestments { get => _tradeCompanyInvestments; set => _tradeCompanyInvestments = value; }
      public List<string> ScenarioTradeCompanyInvestments
      {
         get => _scenarioTradeCompanyInvestments;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _scenarioTradeCompanyInvestments, ref _tradeCompanyInvestments, value);
      }

      public List<ApplicableModifier> ProvinceModifiers { get => _provinceModifiers; set => _provinceModifiers = value; }
      public List<ApplicableModifier> ScenarioProvinceModifiers
      {
         get => _scenarioProvinceModifiers;
         set => SetIfModifiedEnumerable<List<ApplicableModifier>, ApplicableModifier>(ref _scenarioProvinceModifiers, ref _provinceModifiers, value);
      }

      public List<ApplicableModifier> PermanentProvinceModifiers { get => _permanentProvinceModifiers; set => _permanentProvinceModifiers = value; }
      public List<ApplicableModifier> ScenarioPermanentProvinceModifiers
      {
         get => _scenarioPermanentProvinceModifiers;
         set => SetIfModifiedEnumerable<List<ApplicableModifier>, ApplicableModifier>(ref _scenarioPermanentProvinceModifiers, ref _permanentProvinceModifiers, value);
      }

      public List<string> ProvinceTriggeredModifiers { get => _provinceTriggeredModifiers; set => _provinceTriggeredModifiers = value; }
      public List<string> ScenarioProvinceTriggeredModifiers
      {
         get => _scenarioProvinceTriggeredModifiers;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _scenarioProvinceTriggeredModifiers, ref _provinceTriggeredModifiers, value);
      }
      
      public List<IElement> ScriptedEffects { get => _scriptedEffects; set => _scriptedEffects = value; }
      public List<IElement> ScenarioScriptedEffects
      {
         get => _scenarioScriptedEffects;
         set => SetIfModifiedEnumerable<List<IElement>, IElement>(ref _scenarioScriptedEffects, ref _scriptedEffects, value);
      }

      public List<TradeModifier> TradeModifiers { get => _tradeModifiers; set => _tradeModifiers = value; }
      public List<TradeModifier> ScenarioTradeModifiers
      {
         get => _scenarioTradeModifiers;
         set => SetIfModifiedEnumerable<List<TradeModifier>, TradeModifier>(ref _scenarioTradeModifiers, ref _tradeModifiers, value);
      }


      #endregion

      public List<ProvinceHistoryEntry> History
      {
         get => _history;
         set => SetIfModifiedEnumerable<List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(ref _history, value);
      }


         // Command
         // Set scenario
         // Render scenario
         // Recalculate state
         // Render state

      public bool SetField<T>(ref T scenario, ref T state, T value, [CallerMemberName] string? propertyName = null)
      {
         // Everytime it is set
         if (EqualityComparer<T>.Default.Equals(scenario, value))
            return false;
         var property = GetPropertyInfo(propertyName);
         Debug.Assert(property != null, nameof(property) + " != null");

         // If Suppressed we don't create a command
         if (Globals.State == State.Running && !Suppressed)
         {
            HistoryManager.AddCommand(new CModifyProperty<T>(property, this, value, scenario));
         }
         else
         {
            if (!Globals.IsInHistory)
               state = scenario = value;
            else
            {
               scenario = value;
               RecalculateState();
            }
         }
         return true;
      }

      public bool SetIfModifiedEnumerable<T, Q>(ref T scenario, ref T state, T value, [CallerMemberName] string? propertyName = null) where T : IEnumerable<Q>
      {
         Debug.Assert(scenario is not null && value is not null, "field is not null && value is not null in SetIfModifiedEnumerable");
         if (scenario.SequenceEqual(value))
            return false;

         var property = GetPropertyInfo(propertyName);
         Debug.Assert(property != null, nameof(property) + " != null");

         if (Globals.State == State.Running && !Suppressed)
         {
            HistoryManager.AddCommand(new CModifyProperty<T>(property, this, value, scenario));
         }
         else
         {
            if (!Globals.IsInHistory)
            {
               // We are in scenario editing so we also have to set the state as we are currently displaying the scenario
               state = scenario = value;
            }
            else
            {
               scenario = value;
               RecalculateState();
            }
         }
         return true;
      }

      public void RecalculateState()
      {
         if (Globals.IsInHistory || Globals.State != State.Running) // Is the state correct
            ProvinceHistoryManager.ReloadDate(this);
      }

      #region MMF Data
      public int TotalDevelopment => _baseManpower + _baseTax + _baseProduction;
      [ToolTippable]
      public int Id { get; init; }
      public Positions Positions { get; set; } = new();
      public Point Center { get; set; }

      // ######################### Map Data #########################
      private Memory<Point> _pixels;
      private Memory<Point> _borders;
      private Dictionary<Province, Memory<Point>> _provinceBorders = new();
      private List<ProvinceHistoryEntry> _history = [];
      private int _baseTax1 = 0;

      public Memory<Point> Pixels
      {
         get => _pixels;
         set => _pixels = value;
      }

      public Memory<Point> Borders
      {
         get => _borders;
         set => _borders = value;
      }

      public Dictionary<Province, Memory<Point>> ProvinceBorders
      {
         get => _provinceBorders;
         set => _provinceBorders = value;
      }

      public ICollection<Province> Neighbors => Globals.AdjacentProvinces[this];

      public void TempBorderFix()
      {
         var totalLength = _provinceBorders.Values.Sum(list => list.Length);
         var borderArray = new Point[totalLength];
         var offset = 0;
         foreach (var memory in _provinceBorders.Values)
         {
            memory.Span.CopyTo(borderArray.AsSpan(offset));
            offset += memory.Length;
         }
         Borders = new(borderArray);
      }
      #endregion

      public Province(int id, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(id.ToString(), color)
      {
         base.EditingStatus = status;
         Id = id;
      }

      public Province(int id, Color color, ref PathObj path) : this(id, color, ObjEditingStatus.Unchanged)
      {
         SetPath(ref path);
      }


      public new static Province Empty { get; } = new(-1, Color.Empty, ObjEditingStatus.Immutable);

      [ToolTippable]
      public string TitleKey => $"PROV{Id}";
      [ToolTippable]
      public string AdjectiveKey => $"PROV_ADJ{Id}";
      [ToolTippable]
      public string TitleLocalisation
      {
         get
         {
            if (Globals.Settings.Misc.CustomizationOptions.UseDynamicProvinceNames)
               return Localisation.GetDynamicProvinceLoc(this);
            return Localisation.GetLoc(TitleKey);
         }
         set
         {
            Localisation.AddOrModifyLocObject(TitleKey, value);
         }
      }
      [ToolTippable]
      public string AdjectiveLocalisation
      {
         get
         {
            return Localisation.GetLoc(AdjectiveKey);
         }
         set
         {
            Localisation.AddOrModifyLocObject(AdjectiveKey, value);
         }
      }

      // Events 
      public static event EventHandler<ProvinceComposite>? ColorChanged;
      public event PropertyChangedEventHandler? PropertyChanged;
      public override void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new(propertyName));
      
      // Saveable
      public override SaveableType WhatAmI() => SaveableType.Province;
      public override string[] GetDefaultFolderPath() => ["history", "provinces"];
      public override string GetFileEnding() => ".txt";
      public override KeyValuePair<string, bool> GetFileName() => new($"{Id.ToString()} - {TitleLocalisation}", true);
      public override string SavingComment() => TitleLocalisation;
      public override string GetSavePromptString() => $"Save province file {TitleLocalisation}";
      public override string GetSaveString(int tabs)
      {
         ProvinceSaver.GetProvinceFileString(this, out var saveString);
         return saveString;
      }

      // ProvinceComposite
      public override void ColorInvoke(ProvinceComposite composite) => ColorChanged?.Invoke(this, composite);
      public override void AddToColorEvent(EventHandler<ProvinceComposite> handler) => ColorChanged += handler;
      public override ICollection<Province> GetProvinces() => [this];
      public override ICollection<int> GetProvinceIds() => [Id];
      public override Rectangle GetBounds() => Bounds;
      public override void SetBounds() => Bounds = Geometry.GetBounds(Borders);

      // ######################### Utility #########################
      // Collections
      [ToolTippable]
      public Area Area => GetFirstParentOfType(SaveableType.Area) as Area ?? Area.Empty;
      [ToolTippable]
      public Continent Continent => GetFirstParentOfType(SaveableType.Continent) as Continent ?? Continent.Empty;
      [ToolTippable]
      public TradeCompany TradeCompany => GetFirstParentOfType(SaveableType.TradeCompany) as TradeCompany ?? TradeCompany.Empty;

      [ToolTippable]
      public Terrain Terrain
      {
         get => GetFirstParentOfType(SaveableType.Terrain) as Terrain ?? Terrain.Empty;
         set {
            value.SubCollection.Add(this);
         }
      }



      // Map Concerns
      [ToolTippable]
      public bool IsNonRebelOccupied => Owner != Controller && Controller.Tag != "REB";
      public int OccupantColor
      {
         get
         {
            if (_hasRevolt)
               return Color.Black.ToArgb();
            if (IsNonRebelOccupied)
            {
               if (Globals.Countries.TryGetValue(_controller, out var controller))
                  return controller.Color.ToArgb();
            }
            return Color.Transparent.ToArgb();
         }
      }

      [ToolTippable]
      public Terrain AutoTerrain { get; set; } = Terrain.Empty;



      #region History

      public void LoadHistoryForDate(Date date)
      {
         Debug.Assert(date > ProvinceHistoryManager.CurrentLoadedDate || ProvinceHistoryManager.CurrentLoadedDate == Date.MinValue, "date > ProvinceHistoryManager.CurrentLoadedDate");
         foreach (var entry in ProvinceHistoryManager.EnumerateFromToDate(History, ProvinceHistoryManager.CurrentLoadedDate, date))
            foreach (var eff in entry.Effects)
               eff.Activate(this);
      }

      public void ResetHistory()
      {
         Owner = ScenarioOwner;
         Controller = ScenarioController;
         TribalOwner = ScenarioTribalOwner;
         BaseTax = ScenarioBaseTax;
         BaseProduction = ScenarioBaseProduction;
         BaseManpower = ScenarioBaseManpower;
         CenterOfTrade = ScenarioCenterOfTrade;
         ExtraCost = ScenarioExtraCost;
         NativeHostileness = ScenarioNativeHostileness;
         NativeSize = ScenarioNativeSize;
         NativeFerocity = ScenarioNativeFerocity;
         Devastation = ScenarioDevastation;
         Prosperity = ScenarioProsperity;
         Autonomy = ScenarioLocalAutonomy;
         Religion = ScenarioReligion;
         Culture = ScenarioCulture;
         ReformationCenter = ScenarioReformationCenter;
         TradeGood = ScenarioTradeGood;
         LatentTradeGood = ScenarioLatentTradeGood;
         IsCity = ScenarioIsCity;
         IsHre = ScenarioIsHre;
         IsSeatInParliament = ScenarioIsSeatInParliament;
         HasRevolt = ScenarioHasRevolt;
         Buildings = new(ScenarioBuildings);
         TradeCompanyInvestments = new(ScenarioTradeCompanyInvestments);
         DiscoveredBy = new(ScenarioDiscoveredBy);
         Claims = new(ScenarioClaims);
         PermaClaims = new(ScenarioPermanentClaims);
         Cores = new(ScenarioCores);
         ProvinceModifiers = new(ScenarioProvinceModifiers);
         PermanentProvinceModifiers = new(ScenarioPermanentProvinceModifiers);
         ProvinceTriggeredModifiers = new(ScenarioProvinceTriggeredModifiers);
         TradeModifiers = new(ScenarioTradeModifiers);
         ScriptedEffects = new(ScenarioScriptedEffects);
         RevoltRisk = ScenarioRevoltRisk;
         CitySize = ScenarioCitySize;
         Nationalism = ScenarioNationalism;
         Capital = ScenarioCapital;
      }

      public void SetAllValues(Province other)
      {
         if (Globals.Settings.Misc.CopyScenario)
         {
            ScenarioOwner = other.ScenarioOwner;
            ScenarioController = other.ScenarioController;
            ScenarioTribalOwner = other.ScenarioTribalOwner;
            ScenarioBaseTax = other.ScenarioBaseTax;
            ScenarioBaseProduction = other.ScenarioBaseProduction;
            ScenarioBaseManpower = other.ScenarioBaseManpower;
            ScenarioCenterOfTrade = other.ScenarioCenterOfTrade;
            ScenarioExtraCost = other.ScenarioExtraCost;
            ScenarioNativeHostileness = other.ScenarioNativeHostileness;
            ScenarioNativeSize = other.ScenarioNativeSize;
            ScenarioNativeFerocity = other.ScenarioNativeFerocity;
            ScenarioDevastation = other.ScenarioDevastation;
            ScenarioProsperity = other.ScenarioProsperity;
            ScenarioLocalAutonomy = other.ScenarioLocalAutonomy;
            ScenarioReligion = other.ScenarioReligion;
            ScenarioCulture = other.ScenarioCulture;
            ScenarioReformationCenter = other.ScenarioReformationCenter;
            ScenarioTradeGood = other.ScenarioTradeGood;
            ScenarioLatentTradeGood = other.ScenarioLatentTradeGood;
            ScenarioIsCity = other.ScenarioIsCity;
            ScenarioIsHre = other.ScenarioIsHre;
            ScenarioIsSeatInParliament = other.ScenarioIsSeatInParliament;
            ScenarioHasRevolt = other.ScenarioHasRevolt;
            ScenarioRevoltRisk = other.ScenarioRevoltRisk;
            ScenarioCitySize = other.ScenarioCitySize;
            ScenarioNationalism = other.ScenarioNationalism;
            ScenarioCapital = other.ScenarioCapital;
            ScenarioBuildings = new(other.ScenarioBuildings);
            ScenarioTradeCompanyInvestments = new(other.ScenarioTradeCompanyInvestments);
            ScenarioDiscoveredBy = new(other.ScenarioDiscoveredBy);
            ScenarioClaims = new(other.ScenarioClaims);
            ScenarioPermanentClaims = new(other.ScenarioPermanentClaims);
            ScenarioCores = new(other.ScenarioCores);
            ScenarioProvinceModifiers = new(other.ScenarioProvinceModifiers);
            ScenarioPermanentProvinceModifiers = new(other.ScenarioPermanentProvinceModifiers);
            ScenarioProvinceTriggeredModifiers = new(other.ScenarioProvinceTriggeredModifiers);
            ScenarioTradeModifiers = new(other.ScenarioTradeModifiers);
            ScenarioScriptedEffects = new(other.ScenarioScriptedEffects);
         }

         if (Globals.Settings.Misc.CopyHistory)
         {
            History = new(other.History);
         }

         MapModeManager.RenderCurrent();
      }

      public int GetNumOfHistoryEntriesForRange(Date lower, Date higher)
      {
         return GetHistoryEntriesForRange(lower, higher).Count;
      }

      public List<ProvinceHistoryEntry> GetHistoryEntriesForRange(Date lower, Date higher)
      {
         return History.Where(x => x.Date >= lower && x.Date <= higher).ToList();
      }

      public List<ProvinceHistoryEntry> GetHistoryEntriesForDate(Date date)
      {
         return History.Where(x => x.Date == date).ToList();
      }


      #endregion

      #region Modifiers

      // TODO

      public void AddModifier(ModifierType type, ModifierAbstract mod)
      {
         switch (type)
         {
            case ModifierType.ProvinceModifier:
               ProvinceModifiers.Add((ApplicableModifier)mod);
               break;
            case ModifierType.ProvinceTriggeredModifier:
               ProvinceTriggeredModifiers.Add(mod.Name);
               break;
            case ModifierType.PermanentProvinceModifier:
               PermanentProvinceModifiers.Add((ApplicableModifier)mod); ;
               break;
            case ModifierType.CountryModifier:
               Globals.ErrorLog.Write($"Country modifier {mod.Name} cannot be added to province {Id}");
               break;
            case ModifierType.TriggeredModifier:
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(type), type, null);
         }
      }

      public void RemoveModifier(string name, ModifierType type)
      {
         switch (type)
         {
            case ModifierType.ProvinceModifier:
               foreach (var mod in ProvinceModifiers)
                  if (mod.Name.Equals(name))
                  {
                     ProvinceModifiers.Remove(mod);
                     break;
                  }
               break;
            case ModifierType.ProvinceTriggeredModifier:
               ProvinceTriggeredModifiers.Remove(name);
               break;
            case ModifierType.PermanentProvinceModifier:
               PermanentProvinceModifiers.RemoveAll(x => x.Name == name);
               break;
            case ModifierType.CountryModifier:
               break;
            case ModifierType.TriggeredModifier:
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(type), type, null);
         }
      }
      #endregion

      public static IErrorHandle TryParse(string value, out Province province)
      {
         if (int.TryParse(value, out var result))
         {
            if (Globals.ProvinceIdToProvince.TryGetValue(result, out province))
               return ErrorHandle.Success;

            province = Empty;
            return new ErrorObject(ErrorType.TypeConversionError, "Could not parse \"" + value + "\" to a province ID!", addToManager: false);
         }
         province = Empty;
         return new ErrorObject(ErrorType.TypeConversionError, $"Province ID \"{result}\" not found!", addToManager: false);
      }

      public static IErrorHandle GeneralParse(string value, out object province)
      {
         IErrorHandle errorHandle = TryParse(value, out var prov);
         province = prov;
         return errorHandle;
      }


      /// <summary>
      /// Is always called when a value in a saveable is changed (If the property calls SetField)
      /// Will call OnPropertyChange if it is not suppresed
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="field"></param>
      /// <param name="value"></param>
      /// <param name="property"></param>
      /// <returns></returns>
      protected override bool SetFieldInternal<T>(ref T field, T value, PropertyInfo property)
      {
         if (Globals.State == State.Running && !Suppressed)
         {
            // check if base or HE
            HistoryManager.AddCommand(new CModifyProperty<T>(property, this, value, field));
            OnPropertyChanged(property.Name);
         }
         field = value;
         return true;
      }

      #region Operators / Equals

      public int CompareTo(Province? other)
      {
         if (other is null)
            return 1;
         return Id.CompareTo(other.Id);
      }

      public override bool Equals(object? obj)
      {
         if (obj is Province other)
            return Id == other.Id;
         return false;
      }
      public override int GetHashCode()
      {
         return Id;
      }

      // == and != operators
      public static bool operator ==(Province? left, Province? right)
      {
         if (left is null && right is null)
            return true;
         if (left is null || right is null)
            return false;
         return left.Equals(right);
      }

      public static bool operator !=(Province? left, Province? right)
      {
         if (left is null && right is null)
            return false;
         if (left is null || right is null)
            return true;
         return !left.Equals(right);
      }

      public static implicit operator int(Province province)
      {
         return province.Id;
      }

      public override string ToString()
      {
         return $"{Id} ({TitleLocalisation})";
      }

      public int CompareTo(object? obj)
      {
         if (obj is Province other)
            return Id.CompareTo(other.Id);
         return 1;
      }

      #endregion
   }
}