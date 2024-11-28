using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
using Newtonsoft.Json;
using static Editor.Events.ProvinceEventHandler;
using static Editor.Helper.ProvinceEnumHelper;
using Parsing = Editor.Parser.Parsing;

namespace Editor.DataClasses.GameDataClasses;

public class ProvinceData()
{
   // ======================================== IMPORTANT =========================================
   // IF CHANGING ANYTHING HERE ALSO UPDATE InitializeInitial AND ResetHistory
   // The "//." means that there is a GUI to modify this value
   // ======================================== Properties ========================================

   public Tag Controller = Tag.Empty;                       //.
   public Tag Owner = Tag.Empty;                            //.
   public Tag TribalOwner = Tag.Empty;                      //.NAT
   public Tag TradeCompany = Tag.Empty;                     //.TC
   public int BaseManpower = 1;                             //.
   public int BaseTax = 1;                                  //.
   public int BaseProduction = 1;                           //.
   public int CenterOfTrade;                                //.
   public int ExtraCost;                                    //.
   public int NativeHostileness;                            //.NAT
   public int NativeSize;                                   //.NAT
   public int RevoltRisk;
   public int Nationalism;
   public int CitySize;
   public float NativeFerocity;                             //.NAT
   public float LocalAutonomy;                              //.                           
   public float Devastation;                                //.
   public float Prosperity;                                 //.
   public bool IsHre;                                       //.
   public bool IsCity;                                      //.
   public bool HasRevolt;                                   //.
   public bool IsSeatInParliament;                          //.
   public string Capital = string.Empty;                    //.
   public string Culture = string.Empty;                    //.
   public string Religion = string.Empty;                   //.
   public Area Area = Area.Empty;
   public string TradeGood = "";                            //.
   public string Continent = string.Empty;                  //! not in province editing interface
   public string LatentTradeGood = string.Empty;            //+ ProvinceHistoryEntry editing interface
   public string ReformationCenter = string.Empty;          //+ ProvinceHistoryEntry editing interface
   public List<Tag> Claims = [];                            //.
   public List<Tag> PermanentClaims = [];                   //.
   public List<Tag> Cores = [];                             //.
   public List<string> DiscoveredBy = [];                   //.
   public List<string> Buildings = [];                      //.
   public List<string> TradeCompanyInvestments = [];        // TC
   public List<ApplicableModifier> PermanentProvinceModifiers = [];   // MOD
   public List<ApplicableModifier> ProvinceModifiers = [];            // MOD
   public List<string> ProvinceTriggeredModifiers = [];     // MOD
   public List<Effect> ScriptedEffects = [];           
   public List<TradeModifier> TradeModifiers = [];  
}

public class Province : ProvinceComposite
{
   public Province(int id, Color color, ObjEditingStatus status = ObjEditingStatus.Modified) : base(id.ToString(), color)
   {
      base.EditingStatus = status;
      Id = id;
   }

   public Province(int id, Color color, ref PathObj path) : this(id, color, ObjEditingStatus.Unchanged)
   {
      SetPath(ref path);
   }

   private readonly ProvinceData _data = new();
   private List<HistoryEntry> _history = [];
   public int FileIndex { get; set; } = 0;
   public override SaveableType WhatAmI()
   {
      return SaveableType.Province;
   }

   public override void OnPropertyChanged(string? propertyName = null) { }

   public override string[] GetDefaultFolderPath()
   {
      return ["history", "provinces"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new ($"{Id.ToString()} - {GetLocalisation()}", true);
   }

   public override string SavingComment()
   {
      return GetLocalisation();
   }

   public override string GetSaveString(int tabs)
   {
      ProvinceSaver.GetProvinceFileString(this, out var saveString);
      return saveString;
   }

   public override string GetSavePromptString()
   {
      return $"Save province file {GetLocalisation()}";
   }

   public DateTime LastModified { get; set; } = DateTime.MinValue;
   public ProvinceData ProvinceData { get; set; } = new();
   
   #region ManagementData

   // Management data
   public int Id { get; init; } 
   public Positions Positions { get; set; } = new();
   public int BorderPtr { get; set; }
   public int BorderCnt { get; set; }
   public int PixelPtr { get; set; }
   public int PixelCnt { get; set; }
   public Point Center { get; set; }

   public Point[] Pixels { get; set; } = [];
   public Point[] Borders { get; set; } = [];

   #endregion
   #region Globals from the game

   #endregion

   // Events for the ProvinceValues will only be raised if the State is Running otherwise they will be suppressed to improve performance when loading

   #region Globals from the game

   // Globals from the Game
   public Area GetArea() => GetFirstParentOfType(SaveableType.Area) as Area ?? Area.Empty;

   public Continent GetContinent() => GetFirstParentOfType(SaveableType.Continent) as Continent ?? Continent.Empty;

   #endregion

   #region Observable Province Data

   // Province data
   public Terrain AutoTerrain = Terrain.Empty;
   public Terrain Terrain
   {
      get
      {
         foreach (var terrain in Globals.Terrains)
         {
            if (terrain.TerrainOverrides.Contains(this))
               return terrain;
         }

         return Terrain.Empty;
      }
   }

   public List<Tag> Claims
   {
      get => _data.Claims;
      set
      {
         _data.Claims = value;
         if (Globals.State == State.Running)
            RaiseProvinceClaimsChanged(this, value, nameof(Claims));
      }
   }

   public List<Tag> PermanentClaims
   {
      get => _data.PermanentClaims;
      set
      {
         _data.PermanentClaims = value;
         if (Globals.State == State.Running)
            RaiseProvincePermanentClaimsChanged(this, value, nameof(PermanentClaims));
      }
   }


   public List<Tag> Cores
   {
      get => _data.Cores;
      set
      {
         _data.Cores = value;
         if (Globals.State == State.Running)
            RaiseProvinceCoresChanged(this, value, nameof(Cores));
      }
   }

   public Tag Controller
   {
      get => _data.Controller;
      set
      {
         _data.Controller = value;
         if (Globals.State == State.Running)
            RaiseProvinceControllerChanged(this, value, nameof(Controller));
      }
   }

   public Tag Owner
   {
      get => _data.Owner;
      set
      {
         if (_data.Owner == value)
            return;

         if (Globals.State == State.Running)
         {
            if (Globals.Countries.TryGetValue(_data.Owner, out var valueOldOwner))
               valueOldOwner.Remove(this);
            if (!Globals.Countries.TryGetValue(value, out var owner))
               return;
            _data.Owner = value;
            owner.Add(this);
            RaiseProvinceOwnerChanged(this, value, nameof(Owner));
         }
         else
         {
            _data.Owner = value;
         }
      }
   }

   public Tag TribalOwner
   {
      get => _data.TribalOwner;
      set
      {
         _data.TribalOwner = value;
         if (Globals.State == State.Running)
            RaiseProvinceTribalOwnerChanged(this, value, nameof(TribalOwner));
      }
   }

   public int BaseManpower
   {
      get => _data.BaseManpower;
      set
      {
         _data.BaseManpower = Math.Max(0, value);
         if (Globals.State == State.Running)
            RaiseProvinceBaseManpowerChanged(this, value, nameof(BaseManpower));
      }
   }

   public int BaseTax
   {
      get => _data.BaseTax;
      set
      {
         _data.BaseTax = Math.Max(0, value);
         if (Globals.State == State.Running)
            RaiseProvinceBaseTaxChanged(this, value, nameof(BaseTax));
      }
   }

   public int BaseProduction
   {
      get => _data.BaseProduction;
      set
      {
         _data.BaseProduction = Math.Max(0, value);
         if (Globals.State == State.Running)
            RaiseProvinceBaseProductionChanged(this, value, nameof(BaseProduction));
      }
   }

   public int CenterOfTrade
   {
      get => _data.CenterOfTrade;
      set
      {
         _data.CenterOfTrade = Math.Max(0, value);
         if (Globals.State == State.Running)
            RaiseProvinceCenterOfTradeLevelChanged(this, value, nameof(CenterOfTrade));
      }
   }

   public int ExtraCost
   {
      get => _data.ExtraCost;
      set
      {
         _data.ExtraCost = Math.Max(0, value);
         if (Globals.State == State.Running)
            RaiseProvinceExtraCostChanged(this, value, nameof(ExtraCost));
      }
   }

   public float NativeFerocity
   {
      get => _data.NativeFerocity;
      set
      {
         _data.NativeFerocity = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeFerocityChanged(this, value, nameof(NativeFerocity));
      }
   }

   public int NativeHostileness
   {
      get => _data.NativeHostileness;
      set
      {
         _data.NativeHostileness = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeHostilenessChanged(this, value, nameof(NativeHostileness));
      }
   }

   public int NativeSize
   {
      get => _data.NativeSize;
      set
      {
         _data.NativeSize = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeSizeChanged(this, value, nameof(NativeSize));
      }
   }

   public int RevoltRisk
   {
      get => _data.RevoltRisk;
      set
      {
         _data.RevoltRisk = value;
         if (Globals.State == State.Running)
            RaiseProvinceRevoltRiskChanged(this, value, nameof(RevoltRisk));
      }
   }

   public int CitySize
   {
      get => _data.CitySize;
      set
      {
         _data.CitySize = value;
         if (Globals.State == State.Running)
            RaiseProvinceCitySizeChanged(this, value, nameof(CitySize));
      }
   }

   public int Nationalism
   {
      get => _data.Nationalism;
      set
      {
         _data.Nationalism = value;
         if (Globals.State == State.Running)
            RaiseProvinceNationalismChanged(this, value, nameof(Nationalism));
      }
   }
   public float LocalAutonomy
   {
      get => _data.LocalAutonomy;
      set
      {
         _data.LocalAutonomy = value;
         if (Globals.State == State.Running)
            RaiseProvinceLocalAutonomyChanged(this, value, nameof(LocalAutonomy));
      }
   }

   public float Devastation
   {
      get => _data.Devastation;
      set
      {
         _data.Devastation = value;
         if (Globals.State == State.Running)
            RaiseProvinceDevastationChanged(this, value, nameof(Devastation));
      }
   }

   public float Prosperity
   {
      get => _data.Prosperity;
      set
      {
         _data.Prosperity = value;
         if (Globals.State == State.Running)
            RaiseProvinceProsperityChanged(this, value, nameof(Prosperity));
      }
   }

   public List<string> DiscoveredBy
   {
      get => _data.DiscoveredBy;
      set
      {
         _data.DiscoveredBy = value;
         if (Globals.State == State.Running)
            RaiseProvinceDiscoveredByChanged(this, value, nameof(DiscoveredBy));
      }
   }

   public string Capital
   {
      get => _data.Capital;
      set
      {
         _data.Capital = value;
         if (Globals.State == State.Running)
            RaiseProvinceCapitalChanged(this, value, nameof(Capital));
      }
   }

   public string Culture
   {
      get => _data.Culture;
      set
      {
         _data.Culture = value;
         if (Globals.State == State.Running)
            RaiseProvinceCultureChanged(this, value, nameof(Culture));
      }
   }

   public string Religion
   {
      get => _data.Religion;
      set
      {
         _data.Religion = value;
         if (Globals.State == State.Running)
            RaiseProvinceReligionChanged(this, value, nameof(Religion));
      }
   }

   public List<string> Buildings
   {
      get => _data.Buildings;
      set
      {
         _data.Buildings = value;
         if (Globals.State == State.Running)
            RaiseProvinceBuildingsChanged(this, value, nameof(Buildings));
      }
   } 

   public bool IsHre
   {
      get => _data.IsHre;
      set
      {
         _data.IsHre = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsHreChanged(this, value, nameof(IsHre));
      }
   }

   public bool IsCity
   {
      get => _data.IsCity;
      set
      {
         _data.IsCity = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsCityChanged(this, value, nameof(IsCity));
      }
   }

   public bool IsSeatInParliament
   {
      get => _data.IsSeatInParliament;
      set
      {
         _data.IsSeatInParliament = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsSeatInParliamentChanged(this, value, nameof(IsSeatInParliament));
      }
   }

   public string TradeGood
   {
      get => _data.TradeGood;
      set
      {
         _data.TradeGood = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeGoodChanged(this, value, nameof(TradeGood));
      }
   }

   public List<HistoryEntry> History
   {
      get => _history;
      private set
      {
         _history = value;
         if (Globals.State == State.Running)
            RaiseProvinceHistoryChanged(this, value, nameof(History));
      }
   }

   public string LatentTradeGood
   {
      get => _data.LatentTradeGood;
      set
      {
         _data.LatentTradeGood = value;
         if (Globals.State == State.Running)
            RaiseProvinceLatentTradeGoodChanged(this, value, nameof(LatentTradeGood));
      }
   }

   public bool HasRevolt
   {
      get => _data.HasRevolt;
      set
      {
         _data.HasRevolt = value;
         if (Globals.State == State.Running)
            RaiseProvinceHasRevoltChanged(this, value, nameof(HasRevolt));
      }
   }

   public string ReformationCenter
   {
      get => _data.ReformationCenter;
      set
      {
         _data.ReformationCenter = value;
         if (Globals.State == State.Running)
            RaiseProvinceReformationCenterChanged(this, value, nameof(ReformationCenter));
      }
   }

   public Tag TradeCompany
   {
      get => _data.TradeCompany;
      set
      {
         _data.TradeCompany = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyChanged(this, value, nameof(TradeCompany));
      }
   }

   public List<string> TradeCompanyInvestments
   {
      get => _data.TradeCompanyInvestments;
      set
      {
         _data.TradeCompanyInvestments = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyInvestmentChanged(this, value, nameof(TradeCompanyInvestments));
      }
   }

   public List<ApplicableModifier> ProvinceModifiers
   {
      get => _data.ProvinceModifiers;
      set
      {
         _data.ProvinceModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvinceProvinceModifiersChanged(this, value, nameof(ProvinceModifiers));
      }
   }

   public List<ApplicableModifier> PermanentProvinceModifiers
   {
      get => _data.PermanentProvinceModifiers;
      set
      {
         _data.PermanentProvinceModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvincePermanentProvinceModifiersChanged(this, value, nameof(PermanentProvinceModifiers));
      }
   }

   public List<string> ProvinceTriggeredModifiers
   {
      get => _data.ProvinceTriggeredModifiers;
      set
      {
         _data.ProvinceTriggeredModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvinceProvinceTriggeredModifiersChanged(this, value, nameof(ProvinceTriggeredModifiers));
      }
   }

   public List<Effect> Effects
   {
      get => _data.ScriptedEffects;
      set
      {
         _data.ScriptedEffects = value;
         if (Globals.State == State.Running)
            RaiseProvinceScriptedEffectsChanged(this, value, nameof(Effects));
      }
   }

   public List<TradeModifier> TradeModifiers
   {
      get => _data.TradeModifiers;
      set
      {
         _data.TradeModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeModifiersChanged(this, value, nameof(TradeModifiers));
      }
   }

   #endregion
   [JsonIgnore]
   public bool IsNonRebelOccupied => Owner != Controller && Controller != "REB";
   [JsonIgnore]
   public int GetOccupantColor
   {
      get
      {
         if (HasRevolt)
            return Color.Black.ToArgb();
         if (IsNonRebelOccupied)
         {
            if (Globals.Countries.TryGetValue(Controller, out var controller))
               return controller.Color.ToArgb();
         }
         return Color.Transparent.ToArgb();
      }
   }

   [JsonIgnore]
   public string GetTradeCompany
   {
      get
      {
         foreach (var tradeCompany in Globals.TradeCompanies.Values)
            if (tradeCompany.GetProvinces().Contains(this))
               return tradeCompany.Name;
         return string.Empty;
      }
   }

   // ================================== ToAndFromGuiMethods ==================================
   public void LoadToGui()
   {
      Globals.MapWindow.LoadProvinceToGui(this);
   }


   // ======================================== Methods ========================================

   /// <summary>
   /// Copies the Province values to the ProvinceData values to be able to revert to the original standard when going back in time
   /// </summary>
   public void InitializeInitial()
   {
      ProvinceData.Area = GetArea();
      ProvinceData.Claims = Claims;
      ProvinceData.PermanentClaims = PermanentClaims;
      ProvinceData.Cores = Cores;
      ProvinceData.Controller = Controller;
      ProvinceData.Owner = Owner;
      ProvinceData.TribalOwner = TribalOwner;
      ProvinceData.BaseManpower = BaseManpower;
      ProvinceData.BaseTax = BaseTax;
      ProvinceData.BaseProduction = BaseProduction;
      ProvinceData.CenterOfTrade = CenterOfTrade;
      ProvinceData.ExtraCost = ExtraCost;
      ProvinceData.NativeFerocity = NativeFerocity;
      ProvinceData.NativeHostileness = NativeHostileness;
      ProvinceData.NativeSize = NativeSize;
      ProvinceData.RevoltRisk = RevoltRisk;
      ProvinceData.LocalAutonomy = LocalAutonomy;
      ProvinceData.Devastation = Devastation;
      ProvinceData.Prosperity = Prosperity;
      ProvinceData.Nationalism = Nationalism;
      ProvinceData.IsHre = IsHre;
      ProvinceData.IsCity = IsCity;
      ProvinceData.HasRevolt = HasRevolt;
      ProvinceData.IsSeatInParliament = IsSeatInParliament;
      ProvinceData.Capital = Capital;
      ProvinceData.Culture = Culture;
      ProvinceData.Religion = Religion;
      ProvinceData.Buildings = Buildings;
      ProvinceData.TradeGood = TradeGood;
      ProvinceData.LatentTradeGood = LatentTradeGood;
      ProvinceData.ReformationCenter = ReformationCenter;
      ProvinceData.TradeCompany = TradeCompany;
      ProvinceData.TradeCompanyInvestments = TradeCompanyInvestments;
      ProvinceData.ProvinceModifiers = ProvinceModifiers;
      ProvinceData.PermanentProvinceModifiers = PermanentProvinceModifiers;
      ProvinceData.ProvinceTriggeredModifiers = ProvinceTriggeredModifiers;
      ProvinceData.ScriptedEffects = Effects;
      ProvinceData.TradeModifiers = TradeModifiers;
      ProvinceData.CitySize = CitySize;
   }

   /// <summary>
   /// Loads the Province values from the ProvinceData values to revert to the original standard when going back in time
   /// </summary>
   public void ResetHistory()
   {
      GetArea().Add(this);
      Claims = ProvinceData.Claims;
      PermanentClaims = ProvinceData.PermanentClaims;
      Cores = ProvinceData.Cores;
      Controller = ProvinceData.Controller;
      Owner = ProvinceData.Owner;
      TribalOwner = ProvinceData.TribalOwner;
      BaseManpower = ProvinceData.BaseManpower;
      BaseTax = ProvinceData.BaseTax;
      BaseProduction = ProvinceData.BaseProduction;
      CenterOfTrade = ProvinceData.CenterOfTrade;
      ExtraCost = ProvinceData.ExtraCost;
      NativeFerocity = ProvinceData.NativeFerocity;
      NativeHostileness = ProvinceData.NativeHostileness;
      NativeSize = ProvinceData.NativeSize;
      RevoltRisk = ProvinceData.RevoltRisk;
      LocalAutonomy = ProvinceData.LocalAutonomy;
      Devastation = ProvinceData.Devastation;
      Prosperity = ProvinceData.Prosperity;
      Nationalism = ProvinceData.Nationalism;
      IsHre = ProvinceData.IsHre;
      IsCity = ProvinceData.IsCity;
      HasRevolt = ProvinceData.HasRevolt;
      IsSeatInParliament = ProvinceData.IsSeatInParliament;
      Capital = ProvinceData.Capital;
      Culture = ProvinceData.Culture;
      Religion = ProvinceData.Religion;
      Buildings = ProvinceData.Buildings;
      TradeGood = ProvinceData.TradeGood;
      LatentTradeGood = ProvinceData.LatentTradeGood;
      ReformationCenter = ProvinceData.ReformationCenter;
      TradeCompany = ProvinceData.TradeCompany;
      TradeCompanyInvestments = ProvinceData.TradeCompanyInvestments;
      ProvinceModifiers = ProvinceData.ProvinceModifiers;
      PermanentProvinceModifiers = ProvinceData.PermanentProvinceModifiers;
      ProvinceTriggeredModifiers = ProvinceData.ProvinceTriggeredModifiers;
      Effects = ProvinceData.ScriptedEffects;
      TradeModifiers = ProvinceData.TradeModifiers;
      CitySize = ProvinceData.CitySize;
   }

   /// <summary>
   /// Loads the history for the given date
   /// </summary>
   /// <param name="date"></param>
   public void LoadHistoryForDate(Date date)
   {
      // History Entries are sorted by default. Se w can load entries as long as the date is less than the current date
      foreach (var historyEntry in _history)
      {
         if (historyEntry.Date <= date)
            historyEntry.Apply(this);
         else
            break;
      }
   }

   public void AddHistoryEntry(HistoryEntry entryOld)
   {
      _history.Add(entryOld);
      SortHistoryEntriesByDate();
   }

   private void SortHistoryEntriesByDate()
   {
      _history.Sort((x, y) => x.Date.CompareTo(y.Date));
   }

   public void ProvinceDataChanged(object? obj, ProvinceDataChangedEventArgs e)
   {
      /*
      if (Globals.State == State.Running)
      {
         EditingStatus = ObjEditingStatus.Modified;
         LastModified = DateTime.Now;
      }
      */
   }

   public object GetAttribute(ProvAttrGet key)
   {
      return GetAttribute(key.ToString());
   }

   /// <summary>
   /// Returns the value of the given key for the province attribute
   /// </summary>
   /// <param name="key"></param>
   /// <returns></returns>
   public object GetAttribute(string key)
   {
      if (Globals.BuildingKeys.Contains(key))
         return Buildings.Contains(key) ? "yes" : "no";

      if (!Enum.TryParse<ProvAttrGet>(key, true, out var getter))
      {
         Globals.ErrorLog.Write($"Could not parse {key} attribute to get for province id {Id}");
         return "";
      }

      return getter switch
      {
         ProvAttrGet.base_manpower => BaseManpower,
         ProvAttrGet.base_tax => BaseTax,
         ProvAttrGet.base_production => BaseProduction,
         ProvAttrGet.total_development => GetTotalDevelopment(),
         ProvAttrGet.area => GetArea(), 
         ProvAttrGet.continent => GetContinent(),
         ProvAttrGet.claims => Claims,
         ProvAttrGet.permanent_claims => PermanentClaims,
         ProvAttrGet.cores => Cores,
         ProvAttrGet.controller => Controller,
         ProvAttrGet.owner => Owner,
         ProvAttrGet.tribal_owner => TribalOwner,
         ProvAttrGet.center_of_trade => CenterOfTrade,
         ProvAttrGet.extra_cost => ExtraCost,
         ProvAttrGet.native_ferocity => NativeFerocity,
         ProvAttrGet.native_hostileness => NativeHostileness,
         ProvAttrGet.native_size => NativeSize,
         ProvAttrGet.revolt_risk => RevoltRisk,
         ProvAttrGet.local_autonomy => LocalAutonomy,
         ProvAttrGet.nationalism => Nationalism,
         ProvAttrGet.discovered_by => DiscoveredBy,
         ProvAttrGet.capital => Capital,
         ProvAttrGet.culture => Culture,
         ProvAttrGet.religion => Religion,
         ProvAttrGet.buildings => Buildings,
         ProvAttrGet.hre => IsHre,
         ProvAttrGet.is_city => IsCity,
         ProvAttrGet.seat_in_parliament => IsSeatInParliament,
         ProvAttrGet.trade_good => TradeGood,
         ProvAttrGet.history => History,
         ProvAttrGet.id => Id,
         ProvAttrGet.name => GetLocalisation(),
         ProvAttrGet.revolt => HasRevolt, // Was changed from has_revolt to revolt no idea if this breaks stuff
         ProvAttrGet.is_occupied => IsNonRebelOccupied,      
         ProvAttrGet.devastation => Devastation,
         ProvAttrGet.prosperity => Prosperity,
         ProvAttrGet.latent_trade_good => LatentTradeGood,
         ProvAttrGet.citysize => CitySize,
         ProvAttrGet.terrain => Terrain,
         _ => ""
      };
   }

   public void SetAttribute(ProvAttrSet key, string value)
   {
      SetAttribute(key.ToString(), value);
   }

   /// <summary>
   /// Sets the attribute for the province if it exists
   /// </summary>
   /// <param name="name"></param>
   /// <param name="value"></param>
   public void SetAttribute(string name, string value)
   {
      if (Globals.Buildings.Contains(new (name)))
      {
         if (Parsing.YesNo(value))
         {
            Buildings = [..Buildings, name];
         }
         else
         {
            var oldBuildings = new List<string>(Buildings);
            oldBuildings.Remove(name);
            Buildings = oldBuildings;
         }
         return;
      }

      if (!Enum.TryParse<ProvAttrSet>(name, true, out var setter))
      {
         if (EffectParser.ParseScriptedEffect(name, value, out var effect))
         {
            Effects.Add(effect);
            return;
         }

         if (EffectParser.ParseSimpleEffect(name, value, out var eff))
         {
            Effects.Add(eff);
            return;
         }
         Globals.ErrorLog.Write($"Could not parse {name} to set attribute for province id {Id}");
         return;
      }

      switch (setter)
      {
         case ProvAttrSet.add_claim:
            Claims.Add(Tag.FromString(value));
            break;
         case ProvAttrSet.remove_claim:
            Claims.Remove(Tag.FromString(value));
            break;
         case ProvAttrSet.add_core:
            Cores.Add(Tag.FromString(value));
            break;
         case ProvAttrSet.remove_core:
            Cores.Remove(Tag.FromString(value));
            break;
         case ProvAttrSet.base_manpower:
            if (int.TryParse(value, out var manpower))
               BaseManpower = manpower;
            else
               Globals.ErrorLog.Write($"Could not parse base_manpower: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_base_manpower:
            if (int.TryParse(value, out var manpow))
               BaseManpower += manpow;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_manpower: {value} for province id {Id}");
            break;
         case ProvAttrSet.base_production:
            if (int.TryParse(value, out var production))
               BaseProduction = production;
            else
               Globals.ErrorLog.Write($"Could not parse base_production: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_base_production:
            if (int.TryParse(value, out var prod))
               BaseProduction += prod;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_production: {value} for province id {Id}");
            break;
         case ProvAttrSet.base_tax:
            if (int.TryParse(value, out var tax))
               BaseTax = tax;
            else
               Globals.ErrorLog.Write($"Could not parse base_tax: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_base_tax:
            if (int.TryParse(value, out var btax))
               BaseTax += btax;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_tax: {value} for province id {Id}");
            break;
         case ProvAttrSet.capital:
            Capital = value;
            break;
         case ProvAttrSet.center_of_trade:
            if (int.TryParse(value, out var cot))
               CenterOfTrade = cot;
            else
            {
               Globals.ErrorLog.Write($"Could not parse center_of_trade: {value} for province id {Id}");
               CenterOfTrade = 0;
            }
            break;
         case ProvAttrSet.controller:
            Controller = Tag.FromString(value);
            break;
         case ProvAttrSet.culture:
            Culture = value;
            break;
         case ProvAttrSet.discovered_by:
            DiscoveredBy.Add(value);
            break;
         case ProvAttrSet.remove_discovered_by:
            DiscoveredBy.Remove(value);
            break;
         case ProvAttrSet.extra_cost:
            if (int.TryParse(value, out var cost))
               ExtraCost = cost;
            else
               Globals.ErrorLog.Write($"Could not parse extra_cost: {value} for province id {Id}");
            break;
         case ProvAttrSet.hre:
            IsHre = Parsing.YesNo(value);
            break;
         case ProvAttrSet.is_city:
            IsCity = Parsing.YesNo(value);
            break;
         case ProvAttrSet.native_ferocity:
            if (float.TryParse(value, out var ferocity))
               NativeFerocity = ferocity;
            else
               Globals.ErrorLog.Write($"Could not parse native_ferocity: {value} for province id {Id}");
            break;
         case ProvAttrSet.native_hostileness:
            if (int.TryParse(value, out var hostileness))
               NativeHostileness = hostileness;
            else
               Globals.ErrorLog.Write($"Could not parse native_hostileness: {value} for province id {Id}");
            break;
         case ProvAttrSet.native_size:
            if (int.TryParse(value, out var size))
               NativeSize = size;
            else
               Globals.ErrorLog.Write($"Could not parse native_size: {value} for province id {Id}");
            break;
         case ProvAttrSet.owner:
            Owner = Tag.FromString(value);
            break;
         case ProvAttrSet.religion:
            Religion = value;
            break;
         case ProvAttrSet.seat_in_parliament:
            IsSeatInParliament = Parsing.YesNo(value);
            break;
         case ProvAttrSet.trade_goods:
            if (TradeGoodHelper.IsTradeGood(value))
               TradeGood = value;
            break;
         case ProvAttrSet.tribal_owner:
            TribalOwner = Tag.FromString(value);
            break;
         case ProvAttrSet.unrest:
            if (int.TryParse(value, out var unrest))
               RevoltRisk = unrest;
            else
               Globals.ErrorLog.Write($"Could not parse unrest: {value} for province id {Id}");
            break;
         case ProvAttrSet.revolt:
            HasRevolt = !string.IsNullOrWhiteSpace(value);
            break;
         case ProvAttrSet.revolt_risk:
            if (int.TryParse(value, out var risk))
               RevoltRisk = risk;
            else
               Globals.ErrorLog.Write($"Could not parse revolt_risk: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_local_autonomy:
            if (int.TryParse(value, out var autonomy))
               LocalAutonomy = autonomy;
            else
               Globals.ErrorLog.Write($"Could not parse add_local_autonomy: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_nationalism:
            if (int.TryParse(value, out var nationalism))
               Nationalism = nationalism;
            else
               Globals.ErrorLog.Write($"Could not parse add_nationalism: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_trade_company_investment:
            TradeCompanyInvestments.Add(value);
            break;
         case ProvAttrSet.add_to_trade_company:
            TradeCompany = Tag.FromString(value);
            break;
         case ProvAttrSet.reformation_center:
            ReformationCenter = value;
            break;
         case ProvAttrSet.add_province_triggered_modifier:
            ProvinceTriggeredModifiers.Add(value);
            break;
         case ProvAttrSet.remove_province_triggered_modifier:
            ProvinceTriggeredModifiers.Remove(value);
            break;
         case ProvAttrSet.set_global_flag:
            // Case to ignore stuff
            break;
         case ProvAttrSet.devastation:
            if (float.TryParse(value, out var dev))
               Devastation = dev;
            else
               Globals.ErrorLog.Write($"Could not parse devastation: {value} for province id {Id}");
            break;
         case ProvAttrSet.prosperity:
            if (float.TryParse(value, out var prosp))
               Prosperity = prosp;
            else
               Globals.ErrorLog.Write($"Could not parse prosperity: {value} for province id {Id}");
            break;
         case ProvAttrSet.add_province_modifier:
            if (ModifierParser.ParseApplicableModifier(value, out var mod))
               AddModifier(ModifierType.ProvinceModifier, mod);
            else 
               Globals.ErrorLog.Write($"Could not parse add_province_modifier: {value} for province id {Id}");
            break;
         case ProvAttrSet.remove_province_modifier:
            ProvinceModifiers.RemoveAll(x => x.Name == value);
            break;
         case ProvAttrSet.add_permanent_province_modifier:
            if (ModifierParser.ParseApplicableModifier(value, out var permaMod))
               AddModifier(ModifierType.PermanentProvinceModifier, permaMod);
            else
               Globals.ErrorLog.Write($"Could not parse add_permanent_province_modifier: {value} for province id {Id}");
            break;
         case ProvAttrSet.remove_permanent_province_modifier:
            PermanentProvinceModifiers.RemoveAll(x => x.Name == value);
            break;
         case ProvAttrSet.add_permanent_claim:
            PermanentClaims.Add(Tag.FromString(value));
            break;
         case ProvAttrSet.remove_permanent_claim:
            PermanentClaims.Remove(Tag.FromString(value));
            break;
         case ProvAttrSet.citysize:
            if (int.TryParse(value, out var size2))
               CitySize = size2;
            else
               Globals.ErrorLog.Write($"Could not parse citisize: {value} for province id {Id}");
            break;
         default:
            Globals.ErrorLog.Write($"Could not parse {name} attribute for province id {Id} to set value {value}");
            break;
      }
   }

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
            PermanentProvinceModifiers.Add((ApplicableModifier)mod);;
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

   public int GetTotalDevelopment() => BaseManpower + BaseTax + BaseProduction;
   public ICollection<Province> Neighbors => Globals.AdjacentProvinces[this];

   public string GetLocalisation()
   {
      return Localisation.GetLoc(GetLocalisationString());
   }

   [JsonIgnore]
   public string GetLocalisationAdj => $"PROV_ADJ{Id}";

   public string GetLocalisationString()
   {
      return $"PROV{Id}";
   }
   public override int[] GetProvinceIds()
   {
      return [Id];
   }

   public override Rectangle GetBounds()
   {
      return Bounds;
   }

   public override void SetBounds()
   {
      Bounds = Geometry.GetBounds(Borders);
   }

   public override ICollection<Province> GetProvinces()
   {
      return [this];
   }

   public List<Province> GetProvincesWithSameCulture()
   {
      List<Province> provinces = [];
      foreach (var province in Globals.Provinces)
      {
         if (province.Culture == Culture)
            provinces.Add(province);
      }
      return provinces;
   }
   public List<Province> GetProvincesWithSameCultureGroup()
   {
      List<Province> provinces = [];
      if (!Globals.Cultures.TryGetValue(Culture, out var cult) ||
          !Globals.CultureGroups.TryGetValue(cult.CultureGroup, out var groupsName))
         return provinces;

      foreach (var prov in Globals.LandProvinces)
      {
         if (Globals.Cultures.TryGetValue(prov.Culture, out var culture))
            if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var cultureGroup))
               if (cultureGroup.Name == groupsName.Name)
                  provinces.Add(prov);
      }

      return provinces;
   }



   public void DumpHistory(string path)
   {
      var sb = new StringBuilder();
      foreach (var historyEntry in History)
         sb.AppendLine(historyEntry.ToString());

      File.WriteAllText(System.IO.Path.Combine(path, $"{Id}_dump.txt"), sb.ToString());
   }


   public string GetHistoryFilePath()
   {
      var fileName = $"{Id}-{GetLocalisation()}.txt";
      return System.IO.Path.Combine(Globals.ModPath, "history", "provinces", fileName);
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
      return $"{Id} ({GetLocalisation()})";
   }


   public static EventHandler<ProvinceComposite> ColorChanged = delegate { };
   [JsonIgnore]
   public EventHandler<ProvinceComposite> ItemAddedToArea = delegate { };
   [JsonIgnore]
   public EventHandler<ProvinceComposite> ItemRemovedFromArea = delegate { };
   [JsonIgnore]
   public EventHandler<ProvinceComposite> ItemModified = delegate { };

   public override void ColorInvoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }

   public new static Province Empty => new (-1, Color.Empty, ObjEditingStatus.Immutable);
}