using System.Text;
using Editor.Helper;
using Editor.Interfaces;
using static Editor.Helper.ProvinceEventHandler;

namespace Editor.DataClasses.GameDataClasses;

public class ProvinceData()
{
   // ======================================== IMPORTANT =========================================
   // IF CHANGING ANYTHING HERE ALSO UPDATE InitializeInitial AND ResetHistory
   // The "//." means that there is a GUI to modify this value
   // ======================================== Properties ========================================

   public Tag Controller = Tag.Empty;                       //.
   public Tag Owner = Tag.Empty;                            //.
   public Tag TribalOwner = Tag.Empty;
   public Tag TradeCompany = Tag.Empty;
   public int BaseManpower = 1;                             //.
   public int BaseTax = 1;                                  //.
   public int BaseProduction = 1;                           //.
   public int CenterOfTrade;
   public int ExtraCost;
   public int NativeFerocity;      //-
   public int NativeHostileness;   //- Extra sub tab
   public int NativeSize;          //-
   public int RevoltRisk;
   public int Nationalism;
   public float LocalAutonomy;
   public float Devastation;
   public float Prosperity;
   public bool IsHre;                                       //.
   public bool IsCity;                                      //.
   public bool HasRevolt;                                   //.
   public bool IsSeatInParliament;                          //.
   public string Capital = string.Empty;                    //.
   public string Culture = string.Empty;                    //.
   public string Religion = string.Empty;                   //.
   public string Area = string.Empty;
   public string TradeGood = "";
   public string Continent = string.Empty;            //! not in province editing interface
   public string LatentTradeGood = string.Empty;      //+ ProvinceHistoryEntry editing interface
   public string ReformationCenter = string.Empty;    //+ ProvinceHistoryEntry editing interface
   public List<Tag> Claims = [];                            //.
   public List<Tag> Cores = [];                             //.
   public List<string> DiscoveredBy = [];
   public List<string> Buildings = [];
   public List<string> TradeCompanyInvestments = [];
   public List<string> ProvinceModifiers = [];
   public List<string> PermanentProvinceModifiers = [];
   public List<string> ProvinceTriggeredModifiers = [];
}

public class Province : IProvinceCollection
{
   private readonly ProvinceData _data = new();
   private List<HistoryEntry> _history = [];

   public ProvinceData ProvinceData { get; set; } = new();

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
      get => _data.Area;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceAreaChanged(Id, value, nameof(Area));
         _data.Area = value;
      }
   }

   public string Continent
   {
      get => _data.Continent;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceContinentChanged(Id, value, nameof(Continent));
         _data.Continent = value;
      }
   }

   #endregion

   #region Observable Province Data

   // Province data
   public List<Tag> Claims
   {
      get => _data.Claims;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceClaimsChanged(Id, value, nameof(Claims));
         _data.Claims = value;
      }
   }

   public List<Tag> Cores
   {
      get => _data.Cores;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceCoresChanged(Id, value, nameof(Cores));
         _data.Cores = value;
      }
   }

   public Tag Controller
   {
      get => _data.Controller;
      set
      {
         _data.Controller = value;
         if (Globals.State == State.Running)
            RaiseProvinceControllerChanged(Id, value, nameof(Controller));
      }
   }

   public Tag Owner
   {
      get => _data.Owner;
      set
      {
         _data.Owner = value;
         if (Globals.State == State.Running)
            RaiseProvinceOwnerChanged(Id, value, nameof(Owner));
      }
   }

   public Tag TribalOwner
   {
      get => _data.TribalOwner;
      set
      {
         if (Globals.State == State.Running)
            RaiseProvinceTribalOwnerChanged(Id, value, nameof(TribalOwner));
         _data.TribalOwner = value;
      }
   }

   public int BaseManpower
   {
      get => _data.BaseManpower;
      set
      {
         _data.BaseManpower = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseManpowerChanged(Id, value, nameof(BaseManpower));
      }
   }

   public int BaseTax
   {
      get => _data.BaseTax;
      set
      {
         _data.BaseTax = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseTaxChanged(Id, value, nameof(BaseTax));
      }
   }

   public int BaseProduction
   {
      get => _data.BaseProduction;
      set
      {
         _data.BaseProduction = value;
         if (Globals.State == State.Running)
            RaiseProvinceBaseProductionChanged(Id, value, nameof(BaseProduction));
      }
   }

   public int CenterOfTrade
   {
      get => _data.CenterOfTrade;
      set
      {
         _data.CenterOfTrade = value;
         if (Globals.State == State.Running)
            RaiseProvinceCenterOfTradeLevelChanged(Id, value, nameof(CenterOfTrade));
      }
   }

   public int ExtraCost
   {
      get => _data.ExtraCost;
      set
      {
         _data.ExtraCost = value;
         if (Globals.State == State.Running)
            RaiseProvinceExtraCostChanged(Id, value, nameof(ExtraCost));
      }
   }

   public int NativeFerocity
   {
      get => _data.NativeFerocity;
      set
      {
         _data.NativeFerocity = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeFerocityChanged(Id, value, nameof(NativeFerocity));
      }
   }

   public int NativeHostileness
   {
      get => _data.NativeHostileness;
      set
      {
         _data.NativeHostileness = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeHostilenessChanged(Id, value, nameof(NativeHostileness));
      }
   }

   public int NativeSize
   {
      get => _data.NativeSize;
      set
      {
         _data.NativeSize = value;
         if (Globals.State == State.Running)
            RaiseProvinceNativeSizeChanged(Id, value, nameof(NativeSize));
      }
   }

   public int RevoltRisk
   {
      get => _data.RevoltRisk;
      set
      {
         _data.RevoltRisk = value;
         if (Globals.State == State.Running)
            RaiseProvinceRevoltRiskChanged(Id, value, nameof(RevoltRisk));
      }
   }


   public int Nationalism
   {
      get => _data.Nationalism;
      set
      {
         _data.Nationalism = value;
         if (Globals.State == State.Running)
            RaiseProvinceNationalismChanged(Id, value, nameof(Nationalism));
      }
   }
   public float LocalAutonomy
   {
      get => _data.LocalAutonomy;
      set
      {
         _data.LocalAutonomy = value;
         if (Globals.State == State.Running)
            RaiseProvinceLocalAutonomyChanged(Id, value, nameof(LocalAutonomy));
      }
   }

   public float Devastation
   {
      get => _data.Devastation;
      set
      {
         _data.Devastation = value;
         if (Globals.State == State.Running)
            RaiseProvinceDevastationChanged(Id, value, nameof(Devastation));
      }
   }

   public float Prosperity
   {
      get => _data.Prosperity;
      set
      {
         _data.Prosperity = value;
         if (Globals.State == State.Running)
            RaiseProvinceProsperityChanged(Id, value, nameof(Prosperity));
      }
   }

   public List<string> DiscoveredBy
   {
      get => _data.DiscoveredBy;
      set
      {
         _data.DiscoveredBy = value;
         if (Globals.State == State.Running)
            RaiseProvinceDiscoveredByChanged(Id, value, nameof(DiscoveredBy));
      }
   }

   public string Capital
   {
      get => _data.Capital;
      set
      {
         _data.Capital = value;
         if (Globals.State == State.Running)
            RaiseProvinceCapitalChanged(Id, value, nameof(Capital));
      }
   }

   public string Culture
   {
      get => _data.Culture;
      set
      {
         _data.Culture = value;
         if (Globals.State == State.Running)
            RaiseProvinceCultureChanged(Id, value, nameof(Culture));
      }
   }

   public string Religion
   {
      get => _data.Religion;
      set
      {
         _data.Religion = value;
         if (Globals.State == State.Running)
            RaiseProvinceReligionChanged(Id, value, nameof(Religion));
      }
   }

   public List<string> Buildings
   {
      get => _data.Buildings;
      set
      {
         _data.Buildings = value;
         if (Globals.State == State.Running)
            RaiseProvinceBuildingsChanged(Id, value, nameof(Buildings));
      }
   } // TODO parse to check other buildings

   public bool IsHre
   {
      get => _data.IsHre;
      set
      {
         _data.IsHre = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsHreChanged(Id, value, nameof(IsHre));
      }
   }

   public bool IsCity
   {
      get => _data.IsCity;
      set
      {
         _data.IsCity = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsCityChanged(Id, value, nameof(IsCity));
      }
   }

   public bool IsSeatInParliament
   {
      get => _data.IsSeatInParliament;
      set
      {
         _data.IsSeatInParliament = value;
         if (Globals.State == State.Running)
            RaiseProvinceIsSeatInParliamentChanged(Id, value, nameof(IsSeatInParliament));
      }
   }

   public string TradeGood
   {
      get => _data.TradeGood;
      set
      {
         _data.TradeGood = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeGoodChanged(Id, value, nameof(TradeGood));
      }
   }

   public List<HistoryEntry> History
   {
      get => _history;
      private set
      {
         _history = value;
         if (Globals.State == State.Running)
            RaiseProvinceHistoryChanged(Id, value, nameof(History));
      }
   }

   public string LatentTradeGood
   {
      get => _data.LatentTradeGood;
      set
      {
         _data.LatentTradeGood = value;
         if (Globals.State == State.Running)
            RaiseProvinceLatentTradeGoodChanged(Id, value, nameof(LatentTradeGood));
      }
   }

   public bool HasRevolt
   {
      get => _data.HasRevolt;
      set
      {
         _data.HasRevolt = value;
         if (Globals.State == State.Running)
            RaiseProvinceHasRevoltChanged(Id, value, nameof(HasRevolt));
      }
   }

   public string ReformationCenter
   {
      get => _data.ReformationCenter;
      set
      {
         _data.ReformationCenter = value;
         if (Globals.State == State.Running)
            RaiseProvinceReformationCenterChanged(Id, value, nameof(ReformationCenter));
      }
   }

   public Tag TradeCompany
   {
      get => _data.TradeCompany;
      set
      {
         _data.TradeCompany = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyChanged(Id, value, nameof(TradeCompany));
      }
   }

   public List<string> TradeCompanyInvestments
   {
      get => _data.TradeCompanyInvestments;
      set
      {
         _data.TradeCompanyInvestments = value;
         if (Globals.State == State.Running)
            RaiseProvinceTradeCompanyInvestmentChanged(Id, value, nameof(TradeCompanyInvestments));
      }
   }

   public List<string> ProvinceModifiers
   {
      get => _data.ProvinceModifiers;
      set
      {
         _data.ProvinceModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvinceProvinceModifiersChanged(Id, value, nameof(ProvinceModifiers));
      }
   }

   public List<string> PermanentProvinceModifiers
   {
      get => _data.PermanentProvinceModifiers;
      set
      {
         _data.PermanentProvinceModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvincePermanentProvinceModifiersChanged(Id, value, nameof(PermanentProvinceModifiers));
      }
   }

   public List<string> ProvinceTriggeredModifiers
   {
      get => _data.ProvinceTriggeredModifiers;
      set
      {
         _data.ProvinceTriggeredModifiers = value;
         if (Globals.State == State.Running)
            RaiseProvinceProvinceTriggeredModifiersChanged(Id, value, nameof(ProvinceTriggeredModifiers));
      }
   }


   #endregion

   public bool IsNonRebelOccupied => Owner != Controller && Controller != "REB";

   public Color GetOccupantColor
   {
      get
      {
         if (HasRevolt)
            return Color.Black;
         if (IsNonRebelOccupied)
         {
            if (Globals.Countries.TryGetValue(Owner, out var owner))
               return owner.Color;
         }
         return Color.Empty;
      }
   }
   // ======================================== Methods ========================================

   /// <summary>
   /// Copies the Province values to the ProvinceData values to be able to revert to the original standard when going back in time
   /// </summary>
   public void InitializeInitial()
   {
      ProvinceData.Area = Area;
      ProvinceData.Continent = Continent;
      ProvinceData.Claims = Claims;
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
   }

   /// <summary>
   /// Loads the Province values from the ProvinceData values to revert to the original standard when going back in time
   /// </summary>
   public void ResetHistory()
   {
      Area = ProvinceData.Area;
      Continent = ProvinceData.Continent;
      Claims = ProvinceData.Claims;
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
   }

   /// <summary>
   /// Loads the history for the given date
   /// </summary>
   /// <param name="date"></param>
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

   private void SortHistoryEntriesByDate()
   {
      _history.Sort((x, y) => x.Date.CompareTo(y.Date));
   }

   /// <summary>
   /// Returns the value of the given key for the province attribute
   /// </summary>
   /// <param name="key"></param>
   /// <returns></returns>
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
         "has_revolt" => HasRevolt,
         "is_occupied" => IsNonRebelOccupied,      
         _ => null
      };
   }

   /// <summary>
   /// Sets the attribute for the province if it exists
   /// </summary>
   /// <param name="name"></param>
   /// <param name="value"></param>
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
               HasRevolt = false;
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
               if (Globals.Cultures.TryGetValue(Culture, out var cultureToCompare))
                  if (cultureGroup.Name == cultureToCompare.Name)
                     provinces.Add(province.Id);
      }
      return provinces;
   }

   public void DumpHistory(string path)
   {
      var sb = new StringBuilder();
      foreach (var historyEntry in History)
         sb.AppendLine(historyEntry.ToString());

      File.WriteAllText(Path.Combine(path, $"{Id.ToString()}_dump.txt"), sb.ToString());
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

   public override string ToString()
   {
      return $"{Id} ({GetLocalisation()}";
   }
}