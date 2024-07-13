using System.Collections.Generic;
using System.Drawing;
using Editor.Helper;
using Editor.Interfaces;
using static Editor.Helper.ProvinceEventHandler;

namespace Editor.DataClasses;

#nullable enable

public class Province : IProvinceCollection
{
   private Tag _controller = Tag.Empty;
   private Tag _owner = Tag.Empty;
   private Tag _tribalOwner = Tag.Empty;
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
   private bool _hasFort15Th;
   private bool _isHre;
   private bool _isCity;
   private bool _isSeatInParliament;
   private string _capital = string.Empty;
   private string _culture = string.Empty;
   private string _religion = string.Empty;
   private string _area = string.Empty;
   private string _continent = string.Empty;
   private string _latentTradeGood = string.Empty;
   private List<Tag> _claims = [];
   private List<Tag> _cores = [];
   private List<string> _discoveredBy = [];
   private TradeGood _tradeGood;
   private List<HistoryEntry> _history = [];
   private List<MultilineAttribute> _multilineAttributes = [];

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

   // Events for the ProvinceValues will only be raised if the State is Running otherwise they will be supressed to improve performance when loading

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
         if (Globals.State == State.Running)
            RaiseProvinceOwnerChanged(Id, value, _owner, nameof(Owner));
         _owner = value;
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

   public bool HasFort15Th
   {
      get => _hasFort15Th;
      set
      {
         var old = _hasFort15Th;
         _hasFort15Th = value;
         if (Globals.State == State.Running)
            RaiseProvinceHasFort15thChanged(Id, value, old, nameof(HasFort15Th));
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

   public TradeGood TradeGood
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

   public List<MultilineAttribute> MultilineAttributes
   {
      get => _multilineAttributes;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceMultilineAttributesChanged(Id, value, _multilineAttributes, nameof(MultilineAttributes));
         _multilineAttributes = value;
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


   #endregion
   // ======================================== Methods ========================================

   public void LoadHistoryForDate(DateTime date)
   {
      // History Entries are sorted by default. Se w can load entries as long as the date is less than the current date
      foreach (var he in _history)
      {
         if (he.Date <= date)
            he.Apply(this);
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

   public object GetAttribute(string key)
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
         "has_fort_15th" => HasFort15Th,
         "is_hre" => IsHre,
         "is_city" => IsCity,
         "is_seat_in_parliament" => IsSeatInParliament,
         "trade_good" => TradeGood,
         "history" => History,
         "multiline_attributes" => MultilineAttributes,
         "id" => Id,
         "name" => GetLocalisation(),
         _ => default!
      };
   }
   public void SetAttribute(string name, string value)
   {
      switch (name)
      {
         case "add_claim":
            Claims.Add(Tag.FromString(value));
            break;
         case "add_core":
            Cores.Add(Tag.FromString(value));
            break;
         case "base_manpower":
            if (int.TryParse(value, out var manpower))
               BaseManpower = manpower;
            else
            {
               Globals.ErrorLog.Write($"Could not parse base_manpower: {value} for province id {Id}");
               BaseManpower = 1;
            }
            break;
         case "base_production":
            if (int.TryParse(value, out var production))
               BaseProduction = production;
            else
            {
               Globals.ErrorLog.Write($"Could not parse base_production: {value} for province id {Id}");
               BaseProduction = 1;
            }
            break;
         case "base_tax":
            if (int.TryParse(value, out var tax))
               BaseTax = tax;
            else
            {
               Globals.ErrorLog.Write($"Could not parse base_tax: {value} for province id {Id}");
               BaseTax = 1;
            }
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
            {
               Globals.ErrorLog.Write($"Could not parse extra_cost: {value} for province id {Id}");
               ExtraCost = 0;
            }
            break;
         case "fort_15th":
            HasFort15Th = Parsing.YesNo(value);
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
            {
               Globals.ErrorLog.Write($"Could not parse native_ferocity: {value} for province id {Id}");
               NativeFerocity = 0;
            }
            break;
         case "native_hostileness":
            if (int.TryParse(value, out var hostileness))
               NativeHostileness = hostileness;
            else
            {
               Globals.ErrorLog.Write($"Could not parse native_hostileness: {value} for province id {Id}");
               NativeHostileness = 0;
            }
            break;
         case "native_size":
            if (int.TryParse(value, out var size))
               NativeSize = size;
            else
            {
               Globals.ErrorLog.Write($"Could not parse native_size: {value} for province id {Id}");
               NativeSize = 0;
            }
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
            TradeGood = TradeGoodHelper.FromString(value);
            break;
         case "tribal_owner":
            TribalOwner = Tag.FromString(value);
            break;
         case "unrest":
            if (int.TryParse(value, out var unrest))
               RevoltRisk = unrest;
            else
            {
               Globals.ErrorLog.Write($"Could not parse unrest: {value} for province id {Id}");
               RevoltRisk = 0;
            }
            break;
         case "shipyard":
            // TODO parse shipyard
            break;
         case "revolt_risk":
            if (int.TryParse(value, out var risk))
               RevoltRisk = risk;
            else
            {
               Globals.ErrorLog.Write($"Could not parse revolt_risk: {value} for province id {Id}");
               RevoltRisk = 0;
            }
            break;
         case "add_local_autonomy":
            if (int.TryParse(value, out var autonomy))
               LocalAutonomy = autonomy;
            else
            {
               Globals.ErrorLog.Write($"Could not parse add_local_autonomy: {value} for province id {Id}");
               LocalAutonomy = 0;
            }
            break;
         case "add_nationalism":
            if (int.TryParse(value, out var nationalism))
               Nationalism = nationalism;
            else
            {
               Globals.ErrorLog.Write($"Could not parse add_nationalism: {value} for province id {Id}");
               Nationalism = 0;
            }
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