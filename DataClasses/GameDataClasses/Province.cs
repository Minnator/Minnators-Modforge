using Editor.Helper;
using Editor.Interfaces;
using static Editor.Helper.ProvinceEventHandler;

namespace Editor.DataClasses.GameDataClasses;

public class ProvinceInitial()
{
   public Tag Controller = Tag.Empty;
   public Tag Owner = Tag.Empty;
   public Tag TribalOwner = Tag.Empty;
   public Tag TradeCompany = Tag.Empty;
   public int BaseManpower = 1;
   public int BaseTax = 1;
   public int BaseProduction = 1;
   public int CenterOfTrade = 0;
   public int ExtraCost = 0;
   public int NativeFerocity = 0;
   public int NativeHostileness = 0;
   public int NativeSize = 0;
   public int RevoltRisk = 0;
   public int LocalAutonomy = 0;
   public int Nationalism = 0;
   public bool IsHre = false;
   public bool IsCity = false;
   public bool HasRevolt = false;
   public bool IsSeatInParliament = false;
   public string Capital = string.Empty;
   public string Culture = string.Empty;
   public string Religion = string.Empty;
   public string Area = string.Empty;
   public string Continent = string.Empty;
   public string LatentTradeGood = string.Empty;
   public string ReformationCenter = string.Empty;
   public List<Tag> Claims = [];
   public List<Tag> Cores = [];
   public List<string> DiscoveredBy = [];
   public List<string> Buildings = [];
   public List<string> TradeCompanyInvestments = [];
   public List<string> ProvinceModifiers = [];
   public List<string> PermanentProvinceModifiers = [];
   public List<string> ProvinceTriggeredModifiers = [];
   public string TradeGood = "";
   public List<HistoryEntry> History = [];
}

public class Province : IProvinceCollection
{
   private Tag _controller = Tag.Empty;
   private Tag _owner = Tag.Empty;
   private Tag _tribalOwner = Tag.Empty;
   private Tag _tradeCompany = Tag.Empty;
   private int _baseManpower;
   private int _baseTax;
   private int _baseProduction;
   private int _centerOfTrade;
   private int _extraCost;
   private int _nativeFerocity;
   private int _nativeHostileness;
   private int _nativeSize;
   private int _revoltRisk;
   private int _localAutonomy;
   private int _nationalism;
   private bool _isHre;
   private bool _isCity;
   private bool _hasRevolt;
   private bool _isSeatInParliament;
   private string _capital = string.Empty;
   private string _culture = string.Empty;
   private string _religion = string.Empty;
   private string _area = string.Empty;
   private string _continent = string.Empty;
   private string _latentTradeGood = string.Empty;
   private string _reformationCenter = string.Empty;
   private List<Tag> _claims = [];
   private List<Tag> _cores = [];
   private List<string> _discoveredBy = [];
   private List<string> _buildings = [];
   private List<string> _tradeCompanyInvestments = [];
   private List<string> _provinceModifiers = [];
   private List<string> _permanentProvinceModifiers = [];
   private List<string> _provinceTriggeredModifiers = [];
   private string _tradeGood = string.Empty;
   private List<HistoryEntry> _history = [];

   public ProvinceInitial ProvinceInitial { get; set; } = new();

   #region ManagementData

   // Management data
   public int Id { get; set; }
   public Color Color { get; set; }
   public int BorderPtr { get; set; }
   public int BorderCnt { get; set; }
   public int PixelPtr { get; set; }
   public int PixelCnt { get; set; }
   public Rectangle Bounds { get; set; }
   public Point Center { get; set; }


   #endregion

   // Events for the ProvinceValues will only be raised if the State is Running otherwise they will be suppressed to improve performance when loading

   #region Globals from the game

   // Globals from the Game
   public string Area
   {
      get => _area;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceAreaChanged(Id, value, _area, nameof(Area));
         _area = value;
      }
   }

   public string Continent
   {
      get => _continent;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceContinentChanged(Id, value, _continent, nameof(Continent));
         _continent = value;
      }
   }

   #endregion

   #region Observable Province Data

   // Province data
   public List<Tag> Claims
   {
      get => _claims;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceClaimsChanged(Id, value, _claims, nameof(Claims));
         _claims = value;
      }
   }

   public List<Tag> Cores
   {
      get => _cores;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceCoresChanged(Id, value, _cores, nameof(Cores));
         _cores = value;
      }
   }

   public Tag Controller
   {
      get => _controller;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceControllerChanged(Id, value, _controller, nameof(Controller));
         _controller = value;
      }
   }

   public Tag Owner
   {
      get => _owner;
      set
      {
         var old = _owner;
         _owner = value;
         if (Globals.State == State.Running)
            RaiseProvinceOwnerChanged(Id, value, old, nameof(Owner));
      }
   }

   public Tag TribalOwner
   {
      get => _tribalOwner;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceTribalOwnerChanged(Id, value, _tribalOwner, nameof(TribalOwner));
         _tribalOwner = value;
      }
   }

   public int BaseManpower
   {
      get => _baseManpower;
      set
      {
         var old = _baseManpower;
         _baseManpower = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseManpowerChanged(Id, value, old, nameof(BaseManpower));
      }
   }

   public int BaseTax
   {
      get => _baseTax;
      set
      {
         var old = _baseTax;
         _baseTax = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseTaxChanged(Id, value, old, nameof(BaseTax));
      }
   }

   public int BaseProduction
   {
      get => _baseProduction;
      set
      {
         var old = _baseProduction;
         _baseProduction = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseProductionChanged(Id, value, old, nameof(BaseProduction));
      }
   }

   public int CenterOfTrade
   {
      get => _centerOfTrade;
      set
      {
         var old = _centerOfTrade;
         _centerOfTrade = value;
         if (Globals.State == State.Running)
            RaiseProvinceCenterOfTradeLevelChanged(Id, value, old, nameof(CenterOfTrade));
      }
   }

   public int ExtraCost
   {
      get => _extraCost;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceExtraCostChanged(Id, value, _extraCost, nameof(ExtraCost));
         _extraCost = value;
      }
   }

   public int NativeFerocity
   {
      get => _nativeFerocity;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceNativeFerocityChanged(Id, value, _nativeFerocity, nameof(NativeFerocity));
         _nativeFerocity = value;
      }
   }

   public int NativeHostileness
   {
      get => _nativeHostileness;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceNativeHostilenessChanged(Id, value, _nativeHostileness, nameof(NativeHostileness));
         _nativeHostileness = value;
      }
   }

   public int NativeSize
   {
      get => _nativeSize;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceNativeSizeChanged(Id, value, _nativeSize, nameof(NativeSize));
         _nativeSize = value;
      }
   }

   public int RevoltRisk
   {
      get => _revoltRisk;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceRevoltRiskChanged(Id, value, _revoltRisk, nameof(RevoltRisk));
         _revoltRisk = value;
      }
   }

   public int LocalAutonomy
   {
      get => _localAutonomy;
      set
      {
         var old = _localAutonomy;
         _localAutonomy = value;
         if (Globals.State == State.Running)
            RaiseProvinceLocalAutonomyChanged(Id, value, old, nameof(LocalAutonomy));
      }
   }

   public int Nationalism
   {
      get => _nationalism;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceNationalismChanged(Id, value, _nationalism, nameof(Nationalism));
         _nationalism = value;
      }
   }

   public List<string> DiscoveredBy
   {
      get => _discoveredBy;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceDiscoveredByChanged(Id, value, _discoveredBy, nameof(DiscoveredBy));
         _discoveredBy = value;
      }
   }

   public string Capital
   {
      get => _capital;
      set
      {
         var old = _capital;
         _capital = value;
         if (Globals.State == State.Running)
            RaiseProvinceCapitalChanged(Id, value, old, nameof(Capital));
      }
   }

   public string Culture
   {
      get => _culture;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceCultureChanged(Id, value, _culture, nameof(Culture));
         _culture = value;
      }
   }

   public string Religion
   {
      get => _religion;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceReligionChanged(Id, value, _religion, nameof(Religion));
         _religion = value;
      }
   }

   public List<string> Buildings
   {
      get => _buildings;
      set
      {
         var old = _buildings;
         _buildings = value;
         if (Globals.State == State.Running)
            RaiseProvinceBuildingsChanged(Id, value, old, nameof(Buildings));
      }
   } // TODO parse to check other buildings

   public bool IsHre
   {
      get => _isHre;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceIsHreChanged(Id, value, _isHre, nameof(IsHre));
         _isHre = value;
      }
   }

   public bool IsCity
   {
      get => _isCity;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceIsCityChanged(Id, value, _isCity, nameof(IsCity));
         _isCity = value;
      }
   }

   public bool IsSeatInParliament
   {
      get => _isSeatInParliament;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceIsSeatInParliamentChanged(Id, value, _isSeatInParliament, nameof(IsSeatInParliament));
         _isSeatInParliament = value;
      }
   }

   public string TradeGood
   {
      get => _tradeGood;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceTradeGoodChanged(Id, value, _tradeGood, nameof(TradeGood));
         _tradeGood = value;
      }
   }

   public List<HistoryEntry> History
   {
      get => _history;
      private set
      {
         if (Globals.State == State.Running)
            RaiseProvinceHistoryChanged(Id, value, _history, nameof(History));
         _history = value;
      }
   }

   public string LatentTradeGood
   {
      get => _latentTradeGood;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceLatentTradeGoodChanged(Id, value, _latentTradeGood, nameof(LatentTradeGood));
         _latentTradeGood = value;
      }
   }

   public bool HasRevolt
   {
      get => _hasRevolt;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceHasRevoltChanged(Id, value, _hasRevolt, nameof(HasRevolt));
         _hasRevolt = value;
      }
   }

   public string ReformationCenter
   {
      get => _reformationCenter;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceReformationCenterChanged(Id, value, _reformationCenter, nameof(ReformationCenter));
         _reformationCenter = value;
      }
   }

   public Tag TradeCompany
   {
      get => _tradeCompany;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyChanged(Id, value, _tradeCompany, nameof(TradeCompany));
         _tradeCompany = value;
      }
   }

   public List<string> TradeCompanyInvestments
   {
      get => _tradeCompanyInvestments;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyInvestmentChanged(Id, value, _tradeCompanyInvestments, nameof(TradeCompanyInvestments));
         _tradeCompanyInvestments = value;
      }
   }

   public List<string> ProvinceModifiers
   {
      get => _provinceModifiers;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceProvinceModifiersChanged(Id, value, _provinceModifiers, nameof(ProvinceModifiers));
         _provinceModifiers = value;
      }
   }

   public List<string> PermanentProvinceModifiers
   {
      get => _permanentProvinceModifiers;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvincePermanentProvinceModifiersChanged(Id, value, _permanentProvinceModifiers, nameof(PermanentProvinceModifiers));
         _permanentProvinceModifiers = value;
      }
   }

   public List<string> ProvinceTriggeredModifiers
   {
      get => _provinceTriggeredModifiers;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceProvinceTriggeredModifiersChanged(Id, value, _provinceTriggeredModifiers, nameof(ProvinceTriggeredModifiers));
         _provinceTriggeredModifiers = value;
      }
   }


   #endregion
   // ======================================== Methods ========================================

   /// <summary>
   /// Copies the Province values to the ProvinceInitial values to be able to revert to the original standard when going back in time
   /// </summary>
   public void InitializeInitial()
   {
      ProvinceInitial.Area = Area;
      ProvinceInitial.Continent = Continent;
      ProvinceInitial.Claims = Claims;
      ProvinceInitial.Cores = Cores;
      ProvinceInitial.Controller = Controller;
      ProvinceInitial.Owner = Owner;
      ProvinceInitial.TribalOwner = TribalOwner;
      ProvinceInitial.BaseManpower = BaseManpower;
      ProvinceInitial.BaseTax = BaseTax;
      ProvinceInitial.BaseProduction = BaseProduction;
      ProvinceInitial.CenterOfTrade = CenterOfTrade;
      ProvinceInitial.ExtraCost = ExtraCost;
      ProvinceInitial.NativeFerocity = NativeFerocity;
      ProvinceInitial.NativeHostileness = NativeHostileness;
      ProvinceInitial.NativeSize = NativeSize;
      ProvinceInitial.RevoltRisk = RevoltRisk;
      ProvinceInitial.LocalAutonomy = LocalAutonomy;
      ProvinceInitial.Nationalism = Nationalism;
      ProvinceInitial.IsHre = IsHre;
      ProvinceInitial.IsCity = IsCity;
      ProvinceInitial.HasRevolt = HasRevolt;
      ProvinceInitial.IsSeatInParliament = IsSeatInParliament;
      ProvinceInitial.Capital = Capital;
      ProvinceInitial.Culture = Culture;
      ProvinceInitial.Religion = Religion;
      ProvinceInitial.Buildings = Buildings;
      ProvinceInitial.TradeGood = TradeGood;
      ProvinceInitial.History = History;
      ProvinceInitial.LatentTradeGood = LatentTradeGood;
      ProvinceInitial.ReformationCenter = ReformationCenter;
      ProvinceInitial.TradeCompany = TradeCompany;
      ProvinceInitial.TradeCompanyInvestments = TradeCompanyInvestments;
      ProvinceInitial.ProvinceModifiers = ProvinceModifiers;
      ProvinceInitial.PermanentProvinceModifiers = PermanentProvinceModifiers;
      ProvinceInitial.ProvinceTriggeredModifiers = ProvinceTriggeredModifiers;
   }

   /// <summary>
   /// Loads the Province values from the ProvinceInitial values to revert to the original standard when going back in time
   /// </summary>
   public void ResetHistory()
   {
      Area = ProvinceInitial.Area;
      Continent = ProvinceInitial.Continent;
      Claims = ProvinceInitial.Claims;
      Cores = ProvinceInitial.Cores;
      Controller = ProvinceInitial.Controller;
      Owner = ProvinceInitial.Owner;
      TribalOwner = ProvinceInitial.TribalOwner;
      BaseManpower = ProvinceInitial.BaseManpower;
      BaseTax = ProvinceInitial.BaseTax;
      BaseProduction = ProvinceInitial.BaseProduction;
      CenterOfTrade = ProvinceInitial.CenterOfTrade;
      ExtraCost = ProvinceInitial.ExtraCost;
      NativeFerocity = ProvinceInitial.NativeFerocity;
      NativeHostileness = ProvinceInitial.NativeHostileness;
      NativeSize = ProvinceInitial.NativeSize;
      RevoltRisk = ProvinceInitial.RevoltRisk;
      LocalAutonomy = ProvinceInitial.LocalAutonomy;
      Nationalism = ProvinceInitial.Nationalism;
      IsHre = ProvinceInitial.IsHre;
      IsCity = ProvinceInitial.IsCity;
      HasRevolt = ProvinceInitial.HasRevolt;
      IsSeatInParliament = ProvinceInitial.IsSeatInParliament;
      Capital = ProvinceInitial.Capital;
      Culture = ProvinceInitial.Culture;
      Religion = ProvinceInitial.Religion;
      Buildings = ProvinceInitial.Buildings;
      TradeGood = ProvinceInitial.TradeGood;
      History = ProvinceInitial.History;
      LatentTradeGood = ProvinceInitial.LatentTradeGood;
      ReformationCenter = ProvinceInitial.ReformationCenter;
      TradeCompany = ProvinceInitial.TradeCompany;
      TradeCompanyInvestments = ProvinceInitial.TradeCompanyInvestments;
      ProvinceModifiers = ProvinceInitial.ProvinceModifiers;
      PermanentProvinceModifiers = ProvinceInitial.PermanentProvinceModifiers;
      ProvinceTriggeredModifiers = ProvinceInitial.ProvinceTriggeredModifiers;
   }

   public void LoadHistoryForDate(DateTime date)
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

   public void SortHistoryEntriesByDate()
   {
      _history.Sort((x, y) => x.Date.CompareTo(y.Date));
   }

   public object? GetAttribute(string key)
   {
      return key.ToLower() switch
      {
         "base_manpower" => BaseManpower,
         "base_tax" => BaseTax,
         "base_production" => BaseProduction,
         "total_development" => GetTotalDevelopment(),
         "area" => Area,
         "continent" => Continent,
         "claims" => Claims,
         "cores" => Cores,
         "controller" => Controller,
         "owner" => Owner,
         "tribal_owner" => TribalOwner,
         "center_of_trade" => CenterOfTrade,
         "extra_cost" => ExtraCost,
         "native_ferocity" => NativeFerocity,
         "native_hostileness" => NativeHostileness,
         "native_size" => NativeSize,
         "revolt_risk" => RevoltRisk,
         "local_autonomy" => LocalAutonomy,
         "nationalism" => Nationalism,
         "discovered_by" => DiscoveredBy,
         "capital" => Capital,
         "culture" => Culture,
         "religion" => Religion,
         "buildings" => Buildings,
         "is_hre" => IsHre,
         "is_city" => IsCity,
         "is_seat_in_parliament" => IsSeatInParliament,
         "trade_good" => TradeGood,
         "history" => History,
         "id" => Id,
         "name" => GetLocalisation(),
         _ => null
      };
   }
   public void SetAttribute(string name, string value)
   {
      if (Globals.Buildings.Contains(name))
      {
         if (Parsing.YesNo(value))
            Buildings.Add(name);
         else
            Buildings.Remove(name);
         return;
      }


      switch (name)
      {
         case "add_claim":
            Claims.Add(Tag.FromString(value));
            break;
         case "remove_claim":
            Claims.Remove(Tag.FromString(value));
            break;
         case "add_core":
            Cores.Add(Tag.FromString(value));
            break;
         case "remove_core":
            Cores.Remove(Tag.FromString(value));
            break;
         case "base_manpower":
            if (int.TryParse(value, out var manpower))
               BaseManpower = manpower;
            else
               Globals.ErrorLog.Write($"Could not parse base_manpower: {value} for province id {Id}");
            break;
         case "add_base_manpower":
            if (int.TryParse(value, out var manpow))
               BaseManpower += manpow;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_manpower: {value} for province id {Id}");
            break;
         case "base_production":
            if (int.TryParse(value, out var production))
               BaseProduction = production;
            else
               Globals.ErrorLog.Write($"Could not parse base_production: {value} for province id {Id}");
            break;
         case "add_base_production":
            if (int.TryParse(value, out var prod))
               BaseProduction += prod;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_production: {value} for province id {Id}");
            break;
         case "base_tax":
            if (int.TryParse(value, out var tax))
               BaseTax = tax;
            else
               Globals.ErrorLog.Write($"Could not parse base_tax: {value} for province id {Id}");
            break;
         case "add_base_tax":
            if (int.TryParse(value, out var btax))
               BaseTax += btax;
            else
               Globals.ErrorLog.Write($"Could not parse add_base_tax: {value} for province id {Id}");
            break;
         case "capital":
            Capital = value;
            break;
         case "center_of_trade":
            if (int.TryParse(value, out var cot))
               CenterOfTrade = cot;
            else
            {
               Globals.ErrorLog.Write($"Could not parse center_of_trade: {value} for province id {Id}");
               CenterOfTrade = 0;
            }
            break;
         case "controller":
            Controller = Tag.FromString(value);
            break;
         case "culture":
            Culture = value;
            break;
         case "discovered_by":
            DiscoveredBy.Add(value);
            break;
         case "extra_cost":
            if (int.TryParse(value, out var cost))
               ExtraCost = cost;
            else
               Globals.ErrorLog.Write($"Could not parse extra_cost: {value} for province id {Id}");
            break;
         case "hre":
            IsHre = Parsing.YesNo(value);
            break;
         case "is_city":
            IsCity = Parsing.YesNo(value);
            break;
         case "native_ferocity":
            if (int.TryParse(value, out var ferocity))
               NativeFerocity = ferocity;
            else
               Globals.ErrorLog.Write($"Could not parse native_ferocity: {value} for province id {Id}");
            break;
         case "native_hostileness":
            if (int.TryParse(value, out var hostileness))
               NativeHostileness = hostileness;
            else
               Globals.ErrorLog.Write($"Could not parse native_hostileness: {value} for province id {Id}");
            break;
         case "native_size":
            if (int.TryParse(value, out var size))
               NativeSize = size;
            else
               Globals.ErrorLog.Write($"Could not parse native_size: {value} for province id {Id}");
            break;
         case "owner":
            Owner = Tag.FromString(value);
            break;
         case "religion":
            Religion = value;
            break;
         case "seat_in_parliament":
            IsSeatInParliament = Parsing.YesNo(value);
            break;
         case "trade_goods":
            if (TradeGoodHelper.IsTradeGood(value))
               TradeGood = value;
            break;
         case "tribal_owner":
            TribalOwner = Tag.FromString(value);
            break;
         case "unrest":
            if (int.TryParse(value, out var unrest))
               RevoltRisk = unrest;
            else
               Globals.ErrorLog.Write($"Could not parse unrest: {value} for province id {Id}");
            break;
         case "shipyard":
            // TODO parse shipyard
            break;
         case "revolt":
            if (string.IsNullOrWhiteSpace(value))
               HasRevolt = true;
            break;
         case "revolt_risk":
            if (int.TryParse(value, out var risk))
               RevoltRisk = risk;
            else
               Globals.ErrorLog.Write($"Could not parse revolt_risk: {value} for province id {Id}");
            break;
         case "add_local_autonomy":
            if (int.TryParse(value, out var autonomy))
               LocalAutonomy = autonomy;
            else
               Globals.ErrorLog.Write($"Could not parse add_local_autonomy: {value} for province id {Id}");
            break;
         case "add_nationalism":
            if (int.TryParse(value, out var nationalism))
               Nationalism = nationalism;
            else
               Globals.ErrorLog.Write($"Could not parse add_nationalism: {value} for province id {Id}");
            break;
         case "add_trade_company_investment":
            TradeCompanyInvestments.Add(value);
            break;
         case "add_to_trade_company":
            TradeCompany = Tag.FromString(value);
            break;
         case "reformation_center":
            ReformationCenter = value;
            break;
         case "add_province_modifier":
            ProvinceModifiers.Add(value);
            break;
         case "remove_province_modifier":
            ProvinceModifiers.Remove(value);
            break;
         case "add_permanent_province_modifier":
            PermanentProvinceModifiers.Add(value);
            break;
         case "remove_permanent_province_modifier":
            PermanentProvinceModifiers.Remove(value);
            break;
         case "add_province_triggered_modifier":
            ProvinceTriggeredModifiers.Add(value);
            break;
         case "remove_province_triggered_modifier":
            ProvinceTriggeredModifiers.Remove(value);
            break;
         // Case to ignore stuff
         case "set_global_flag":
            break;
         default:
            Globals.ErrorLog.Write($"Unknown attribute {name} for province id {Id}");
            break;
      }
   }

   public int GetTotalDevelopment()
   {
      return BaseManpower + BaseTax + BaseProduction;
   }
   public string GetLocalisation()
   {
      return Globals.Localisation.TryGetValue($"PROV{Id}", out var loc) ? loc : Id.ToString();
   }

   public override bool Equals(object? obj)
   {
      if (obj is Province other)
         return Id == other.Id;
      return false;
   }

   public override int GetHashCode()
   {
      return Id.GetHashCode();
   }

   public int[] GetProvinceIds()
   {
      return [Id];
   }

   public IProvinceCollection ScopeOut()
   {
      return Globals.Areas[Area];
   }

   public List<IProvinceCollection> ScopeIn()
   {
      return [this];
   }

   public List<int> GetProvincesWithSameCulture()
   {
      List<int> provinces = [];
      foreach (var province in Globals.Provinces.Values)
      {
         if (province.Culture == Culture)
            provinces.Add(province.Id);
      }
      return provinces;
   }

   public List<int> GetProvincesWithSameCultureGroup()
   {
      List<int> provinces = [];
      foreach (var province in Globals.Provinces.Values)
      {
         if (Globals.Cultures.TryGetValue(province.Culture, out var culture))
            if (Globals.CultureGroups.TryGetValue(culture.CultureGroup, out var cultureGroup))
               if (cultureGroup.Name == Globals.Cultures[Culture].CultureGroup)
                  provinces.Add(province.Id);
      }
      return provinces;
   }

}