using System.Collections.Generic;
using System.Data;
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

   #region Globals from the game

   // Globals from the Game
   public string Area { get; set; } = string.Empty;
   public string Continent { get; set; } = string.Empty;

   #endregion

   #region Observable Province Data

   // Province data
   public List<Tag> Claims
   {
      get => _claims;
      set
      {
         RaiseProvinceClaimsChanged(Id, value, _claims, nameof(Claims));
         _claims = value;
      }
   }

   public List<Tag> Cores
   {
      get => _cores;
      set
      {
         RaiseProvinceCoresChanged(Id, value, _cores, nameof(Cores));
         _cores = value;
      }
   }

   public Tag Controller
   {
      get => _controller;
      set
      {
         RaiseProvinceControllerChanged(Id, value, _controller, nameof(Controller));
         _controller = value;
      }
   }

   public Tag Owner
   {
      get => _owner;
      set
      {
         RaiseProvinceOwnerChanged(Id, value, _owner, nameof(Owner));
         _owner = value;
      }
   }

   public Tag TribalOwner
   {
      get => _tribalOwner;
      set
      {
         RaiseProvinceTribalOwnerChanged(Id, value, _tribalOwner, nameof(TribalOwner));
         _tribalOwner = value;
      }
   }

   public int BaseManpower
   {
      get => _baseManpower;
      set
      {
         RaiseProvinceBaseManpowerChanged(Id, value, _baseManpower, nameof(BaseManpower));
         _baseManpower = value;
      }
   }

   public int BaseTax
   {
      get => _baseTax;
      set
      {
         RaiseProvinceBaseTaxChanged(Id, value, _baseTax, nameof(BaseTax));
         _baseTax = value;
      }
   }

   public int BaseProduction
   {
      get => _baseProduction;
      set
      {
         RaiseProvinceBaseProductionChanged(Id, value, _baseProduction, nameof(BaseProduction));
         _baseProduction = value;
      }
   }

   public int CenterOfTrade
   {
      get => _centerOfTrade;
      set
      {
         RaiseProvinceCenterOfTradeLevelChanged(Id, value, _centerOfTrade, nameof(CenterOfTrade));
         _centerOfTrade = value;
      }
   }

   public int ExtraCost
   {
      get => _extraCost;
      set
      {
         RaiseProvinceExtraCostChanged(Id, value, _extraCost, nameof(ExtraCost));
         _extraCost = value;
      }
   }

   public int NativeFerocity
   {
      get => _nativeFerocity;
      set
      {
         RaiseProvinceNativeFerocityChanged(Id, value, _nativeFerocity, nameof(NativeFerocity));
         _nativeFerocity = value;
      }
   }

   public int NativeHostileness
   {
      get => _nativeHostileness;
      set
      {
         RaiseProvinceNativeHostilenessChanged(Id, value, _nativeHostileness, nameof(NativeHostileness));
         _nativeHostileness = value;
      }
   }

   public int NativeSize
   {
      get => _nativeSize;
      set
      {
         RaiseProvinceNativeSizeChanged(Id, value, _nativeSize, nameof(NativeSize));
         _nativeSize = value;
      }
   }

   public int RevoltRisk
   {
      get => _revoltRisk;
      set
      {
         RaiseProvinceRevoltRiskChanged(Id, value, _revoltRisk, nameof(RevoltRisk));
         _revoltRisk = value;
      }
   }

   public int LocalAutonomy
   {
      get => _localAutonomy;
      set
      {
         RaiseProvinceLocalAutonomyChanged(Id, value, _localAutonomy, nameof(LocalAutonomy));
         _localAutonomy = value;
      }
   }

   public int Nationalism
   {
      get => _nationalism;
      set
      {
         RaiseProvinceNationalismChanged(Id, value, _nationalism, nameof(Nationalism));
         _nationalism = value;
      }
   }

   public List<string> DiscoveredBy
   {
      get => _discoveredBy;
      set
      {
         RaiseProvinceDiscoveredByChanged(Id, value, _discoveredBy, nameof(DiscoveredBy));
         _discoveredBy = value;
      }
   }

   public string Capital
   {
      get => _capital;
      set
      {
         RaiseProvinceCapitalChanged(Id, value, _capital, nameof(Capital));
         _capital = value;
      }
   }

   public string Culture
   {
      get => _culture;
      set
      {
         RaiseProvinceCultureChanged(Id, value, _culture, nameof(Culture));
         _culture = value;
      }
   }

   public string Religion
   {
      get => _religion;
      set
      {
         RaiseProvinceReligionChanged(Id, value, _religion, nameof(Religion));
         _religion = value;
      }
   }

   public bool HasFort15Th
   {
      get => _hasFort15Th;
      set
      {
         RaiseProvinceHasFort15thChanged(Id, value, _hasFort15Th, nameof(HasFort15Th));
         _hasFort15Th = value;
      }
   } // TODO parse to check other buildings

   public bool IsHre
   {
      get => _isHre;
      set
      {
         RaiseProvinceIsHreChanged(Id, value, _isHre, nameof(IsHre));
         _isHre = value;
      }
   }

   public bool IsCity
   {
      get => _isCity;
      set
      {
         RaiseProvinceIsCityChanged(Id, value, _isCity, nameof(IsCity));
         _isCity = value;
      }
   }

   public bool IsSeatInParliament
   {
      get => _isSeatInParliament;
      set
      {
         RaiseProvinceIsSeatInParliamentChanged(Id, value, _isSeatInParliament, nameof(IsSeatInParliament));
         _isSeatInParliament = value;
      }
   }

   public TradeGood TradeGood
   {
      get => _tradeGood;
      set
      {
         RaiseProvinceTradeGoodChanged(Id, value, _tradeGood, nameof(TradeGood));
         _tradeGood = value;
      }
   }
   #endregion
   // ======================================== Methods ========================================


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

   public List<IProvinceCollection>? ScopeIn()
   {
      return [this];
   }
}