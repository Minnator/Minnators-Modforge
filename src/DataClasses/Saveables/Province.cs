using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Forms.Feature;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;
using Editor.Saving;

namespace Editor.DataClasses.Saveables
{
   internal class ProvinceData
   {
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
      private float _localAutonomy;
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
      private string _latentTradeGood = string.Empty;
      private List<Tag> _claims = [];
      private List<Tag> _permanentClaims = [];
      private List<Tag> _cores = [];
      private List<string> _discoveredBy = [];
      private List<Building> _buildings = [];
      private List<string> _tradeCompanyInvestments = [];
      private List<ApplicableModifier> _permanentProvinceModifiers = [];
      private List<ApplicableModifier> _provinceModifiers = [];
      private List<string> _provinceTriggeredModifiers = [];
      private List<IElement> _scriptedEffects = [];
      private List<TradeModifier> _tradeModifiers = [];

      public void SetData(Province province)
      {
         province.Controller = _controller;
         province.Owner = _owner;
         province.TribalOwner = _tribalOwner;
         province.BaseManpower = _baseManpower;
         province.BaseTax = _baseTax;
         province.BaseProduction = _baseProduction;
         province.CenterOfTrade = _centerOfTrade;
         province.ExtraCost = _extraCost;
         province.NativeHostileness = _nativeHostileness;
         province.NativeSize = _nativeSize;
         province.RevoltRisk = _revoltRisk;
         province.Nationalism = _nationalism;
         province.CitySize = _citySize;
         province.NativeFerocity = _nativeFerocity;
         province.LocalAutonomy = _localAutonomy;
         province.Devastation = _devastation;
         province.Prosperity = _prosperity;
         province.IsHre = _isHre;
         province.IsCity = _isCity;
         province.HasRevolt = _hasRevolt;
         province.IsSeatInParliament = _isSeatInParliament;
         province.Capital = _capital;
         province.Culture = _culture;
         province.Religion = _religion;
         province.ReformationCenter = _reformationCenter;
         province.TradeGood = _tradeGood;
         province.LatentTradeGood = _latentTradeGood;
         province.Claims = _claims;
         province.PermanentClaims = _permanentClaims;
         province.Cores = _cores;
         province.DiscoveredBy = _discoveredBy;
         province.Buildings = _buildings;
         province.TradeCompanyInvestments = _tradeCompanyInvestments;
         province.PermanentProvinceModifiers = _permanentProvinceModifiers;
         province.ProvinceModifiers = _provinceModifiers;
         province.ProvinceTriggeredModifiers = _provinceTriggeredModifiers;
         province.ScriptedEffects = _scriptedEffects;
         province.TradeModifiers = _tradeModifiers;
      }

      public void GetData(Province province)
      {
         _controller = province.Controller;
         _owner = province.Owner;
         _tribalOwner = province.TribalOwner;
         _baseManpower = province.BaseManpower;
         _baseTax = province.BaseTax;
         _baseProduction = province.BaseProduction;
         _centerOfTrade = province.CenterOfTrade;
         _extraCost = province.ExtraCost;
         _nativeHostileness = province.NativeHostileness;
         _nativeSize = province.NativeSize;
         _revoltRisk = province.RevoltRisk;
         _nationalism = province.Nationalism;
         _citySize = province.CitySize;
         _nativeFerocity = province.NativeFerocity;
         _localAutonomy = province.LocalAutonomy;
         _devastation = province.Devastation;
         _prosperity = province.Prosperity;
         _isHre = province.IsHre;
         _isCity = province.IsCity;
         _hasRevolt = province.HasRevolt;
         _isSeatInParliament = province.IsSeatInParliament;
         _capital = province.Capital;
         _culture = province.Culture;
         _religion = province.Religion;
         _reformationCenter = province.ReformationCenter;
         _tradeGood = province.TradeGood;
         _latentTradeGood = province.LatentTradeGood;
         _claims = province.Claims;
         _permanentClaims = province.PermanentClaims;
         _cores = province.Cores;
         _discoveredBy = province.DiscoveredBy;
         _buildings = province.Buildings;
         _tradeCompanyInvestments = province.TradeCompanyInvestments;
         _permanentProvinceModifiers = province.PermanentProvinceModifiers;
         _provinceModifiers = province.ProvinceModifiers;
         _provinceTriggeredModifiers = province.ProvinceTriggeredModifiers;
         _scriptedEffects = province.ScriptedEffects;
         _tradeModifiers = province.TradeModifiers;
      }
   }


   public class Province : ProvinceComposite, ITitleAdjProvider, IHistoryProvider<ProvinceHistoryEntry>, ITarget, IComparable
   {
      public enum modifiyingOperation
      {
         Base,
         HistoryEntry,
      }

      private ProvinceData _initialData = new();

      #region Data
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
      private float _localAutonomy;                                                       
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
      private string _latentTradeGood = string.Empty;           
      private List<Tag> _claims = [];                            
      private List<Tag> _permanentClaims = [];                  
      private List<Tag> _cores = [];                            
      private List<string> _discoveredBy = [];                  
      private List<Building> _buildings = [];                      
      private List<string> _tradeCompanyInvestments = [];        
      private List<ApplicableModifier> _permanentProvinceModifiers = []; 
      private List<ApplicableModifier> _provinceModifiers = [];           
      private List<string> _provinceTriggeredModifiers = [];     
      private List<IElement> _scriptedEffects = [];
      private List<TradeModifier> _tradeModifiers = [];

      public void SetInit()
      {
         _initialData.GetData(this);
      }

      public void ResetToInit()
      {
         _initialData.SetData(this);
      }

      // ##################### Complex setter #####################

      [ToolTippable]
      public Country Owner
      {
         get => _owner;
         set
         {
            if (_owner == value)
               return;

            if (Globals.State == State.Running)
            {
               if (Globals.Countries.TryGetValue(_owner, out var valueOldOwner))
                  valueOldOwner.Remove(this);
               if (!Globals.Countries.TryGetValue(value, out var owner))
                  return;
               _owner = value;
               owner.Add(this);
               SetField(ref _owner, value);
            }
            else
            {
               _owner = value;
            }
         }
      }


      #endregion

      // ##################### Simple setter #####################
      # region SimpleSetter

      [GameIcon(GameIcons.Claim)]
      public List<Tag> Claims
      {
         get => _claims;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _claims, value);
      }

      [GameIcon(GameIcons.Claim)]
      public List<Tag> PermanentClaims
      {
         get => _permanentClaims;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _permanentClaims, value);
      }
      [GameIcon(GameIcons.Core, false)]
      public List<Tag> Cores
      {
         get => _cores;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _cores, value);
      }
      [ToolTippable]
      public Country Controller
      {
         get => _controller;
         set => SetField(ref _controller, value);
      }

      [ToolTippable]
      public Country TribalOwner
      {
         get => _tribalOwner;
         set => SetField(ref _tribalOwner, value);
      }

      [ToolTippable]
      public int BaseManpower
      {
         get => _baseManpower;
         set => SetField(ref _baseManpower, Math.Max(1, value));
      }

      [ToolTippable]
      public int BaseTax
      {
         get => _baseTax;
         set => SetField(ref _baseTax, Math.Max(0, value));
      }

      [ToolTippable]
      public int BaseProduction
      {
         get => _baseProduction;
         set => SetField(ref _baseProduction, Math.Max(0, value));
      }

      [ToolTippable]
      public int CenterOfTrade
      {
         get => _centerOfTrade;
         set => SetField(ref _centerOfTrade, Math.Max(0, value));
      }

      [ToolTippable]
      public int ExtraCost
      {
         get => _extraCost;
         set => SetField(ref _extraCost, Math.Max(0, value));
      }

      [ToolTippable]
      public float NativeFerocity
      {
         get => _nativeFerocity;
         set => SetField(ref _nativeFerocity, value);
      }

      [ToolTippable]
      public int NativeHostileness
      {
         get => _nativeHostileness;
         set => SetField(ref _nativeHostileness, value);
      }

      [ToolTippable]
      public int NativeSize
      {
         get => _nativeSize;
         set => SetField(ref _nativeSize, value);
      }

      [ToolTippable]
      public int RevoltRisk
      {
         get => _revoltRisk;
         set => SetField(ref _revoltRisk, value);
      }

      [ToolTippable]
      public int CitySize
      {
         get => _citySize;
         set => SetField(ref _citySize, value);
      }

      [ToolTippable]
      public int Nationalism
      {
         get => _nationalism;
         set => SetField(ref _nationalism, value);
      }

      [ToolTippable]
      public float LocalAutonomy
      {
         get => _localAutonomy;
         set => SetField(ref _localAutonomy, value);
      }

      [ToolTippable]
      public float Devastation
      {
         get => _devastation;
         set => SetField(ref _devastation, value);
      }

      [ToolTippable]
      public float Prosperity
      {
         get => _prosperity;
         set => SetField(ref _prosperity, value);
      }

      [GameIcon(GameIcons.DiscoverAchievement)]
      public List<string> DiscoveredBy
      {
         get => _discoveredBy;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _discoveredBy, value);
      }

      [ToolTippable]
      public string Capital
      {
         get => _capital;
         set => SetField(ref _capital, value);
      }

      [ToolTippable]
      public Culture Culture
      {
         get => _culture;
         set => SetField(ref _culture, value);
      }

      [ToolTippable]
      public Religion Religion
      {
         get => _religion;
         set => SetField(ref _religion, value);
      }

      [GameIcon(GameIcons.Building, false)]
      public List<Building> Buildings
      {
         get => _buildings;
         set => SetIfModifiedEnumerable<List<Building>, Building>(ref _buildings, value);
      }

      [ToolTippable]
      public bool IsHre
      {
         get => _isHre;
         set => SetField(ref _isHre, value);
      }

      [ToolTippable]
      public bool IsCity
      {
         get => _isCity;
         set => SetField(ref _isCity, value);
      }

      [ToolTippable]
      public bool IsSeatInParliament
      {
         get => _isSeatInParliament;
         set => SetField(ref _isSeatInParliament, value);
      }

      [ToolTippable]
      public TradeGood TradeGood
      {
         get => _tradeGood;
         set => SetField(ref _tradeGood, value);
      }

      [ToolTippable]
      public string LatentTradeGood
      {
         get => _latentTradeGood;
         set => SetField(ref _latentTradeGood, value);
      }

      [ToolTippable]
      public bool HasRevolt
      {
         get => _hasRevolt;
         set => SetField(ref _hasRevolt, value);
      }

      [ToolTippable]
      public Religion ReformationCenter
      {
         get => _reformationCenter;
         set => SetField(ref _reformationCenter, value);
      }

      public List<string> TradeCompanyInvestments
      {
         get => _tradeCompanyInvestments;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _tradeCompanyInvestments, value);
      }

      public List<ApplicableModifier> ProvinceModifiers
      {
         get => _provinceModifiers;
         set => SetIfModifiedEnumerable<List<ApplicableModifier>, ApplicableModifier>(ref _provinceModifiers, value);
      }

      public List<ApplicableModifier> PermanentProvinceModifiers
      {
         get => _permanentProvinceModifiers;
         set => SetIfModifiedEnumerable<List<ApplicableModifier>, ApplicableModifier>(ref _permanentProvinceModifiers, value);
      }

      public List<string> ProvinceTriggeredModifiers
      {
         get => _provinceTriggeredModifiers;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _provinceTriggeredModifiers, value);
      }

      public List<IElement> ScriptedEffects
      {
         get => _scriptedEffects;
         set => SetIfModifiedEnumerable<List<IElement>, IElement>(ref _scriptedEffects, value);
      }

      public List<TradeModifier> TradeModifiers
      {
         get => _tradeModifiers;
         set => SetIfModifiedEnumerable<List<TradeModifier>, TradeModifier>(ref _tradeModifiers, value);
      }


      #endregion

      public List<ProvinceHistoryEntry> History { get; set; } = [];
      

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
         var entries = History.Where(x => x.Date <= date);
         foreach (var entry in entries)
            foreach (var eff in entry.Effects)
               eff.Activate(this);
      }

      public void ResetHistory()
      {
         _initialData.SetData(this);
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
      protected override bool InternalFieldSet<T>(ref T field, T value, PropertyInfo property)
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