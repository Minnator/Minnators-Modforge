using System.Collections.Generic;
using System.Drawing;
using Editor.Interfaces;
using static Editor.Helper.ProvinceEventHandler;

namespace Editor.DataClasses;

#nullable enable

public class Province : IProvinceCollection
{
   private List<Tag> _claims = [];
   private List<Tag> _cores = [];
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
   private List<string> _discoveredBy = [];
   private string _capital = string.Empty;
   private string _culture = string.Empty;
   private string _religion = string.Empty;
   private bool _hasFort15Th;
   private bool _isHre;
   private bool _isCity;
   private bool _isSeatInParliament;
   private TradeGood _tradeGood;
   private string _area = string.Empty;
   private string _continent = string.Empty;
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
      set
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

   #endregion
   // ======================================== Methods ========================================

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
}