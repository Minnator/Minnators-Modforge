using System.ComponentModel;
using System.Runtime.CompilerServices;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;

namespace Editor.DataClasses.GameDataClasses
{
   public class Province : ProvinceComposite, ITitleAdjProvider, IHistoryProvider<ProvinceHistoryEntry>
   {
      #region Data
      private Tag _controller = Tag.Empty;                       
      private Tag _owner = Tag.Empty;                            
      private Tag _tribalOwner = Tag.Empty;                        
      private int _baseManpower;                                
      private int _baseTax;                                      
      private int _baseProduction;                              
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
      private string _culture = string.Empty;                   
      private string _religion = string.Empty;                   
      private string _tradeGood = string.Empty;                   
      private string _latentTradeGood = string.Empty;           
      private string _reformationCenter = string.Empty;         
      private List<Tag> _claims = [];                            
      private List<Tag> _permanentClaims = [];                  
      private List<Tag> _cores = [];                            
      private List<string> _discoveredBy = [];                  
      private List<string> _buildings = [];                      
      private List<string> _tradeCompanyInvestments = [];        
      private List<ApplicableModifier> _permanentProvinceModifiers = []; 
      private List<ApplicableModifier> _provinceModifiers = [];           
      private List<string> _provinceTriggeredModifiers = [];     
      private List<IElement> _scriptedEffects = [];
      private List<TradeModifier> _tradeModifiers = [];


      // ##################### Complex setter #####################
      
      public Tag Owner
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
      public List<Tag> Claims
      {
         get => _claims;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _claims, value);
      }

      public List<Tag> PermanentClaims
      {
         get => _permanentClaims;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _permanentClaims, value);
      }
      public List<Tag> Cores
      {
         get => _cores;
         set => SetIfModifiedEnumerable<List<Tag>, Tag>(ref _cores, value);
      }

      public Tag Controller
      {
         get => _controller;
         set => SetField(ref _controller, value);
      }

      public Tag TribalOwner
      {
         get => _tribalOwner;
         set => SetField(ref _tribalOwner, value);
      }

      public int BaseManpower
      {
         get => _baseManpower;
         set => SetField(ref _baseManpower, Math.Max(0, value));
      }

      public int BaseTax
      {
         get => _baseTax;
         set => SetField(ref _baseTax, Math.Max(0, value));
      }

      public int BaseProduction
      {
         get => _baseProduction;
         set => SetField(ref _baseProduction, Math.Max(0, value));
      }

      public int CenterOfTrade
      {
         get => _centerOfTrade;
         set => SetField(ref _centerOfTrade, Math.Max(0, value));
      }

      public int ExtraCost
      {
         get => _extraCost;
         set => SetField(ref _extraCost, Math.Max(0, value));
      }

      public float NativeFerocity
      {
         get => _nativeFerocity;
         set => SetField(ref _nativeFerocity, value);
      }

      public int NativeHostileness
      {
         get => _nativeHostileness;
         set => SetField(ref _nativeHostileness, value);
      }

      public int NativeSize
      {
         get => _nativeSize;
         set => SetField(ref _nativeSize, value);
      }

      public int RevoltRisk
      {
         get => _revoltRisk;
         set => SetField(ref _revoltRisk, value);
      }

      public int CitySize
      {
         get => _citySize;
         set => SetField(ref _citySize, value);
      }

      public int Nationalism
      {
         get => _nationalism;
         set => SetField(ref _nationalism, value);
      }

      public float LocalAutonomy
      {
         get => _localAutonomy;
         set => SetField(ref _localAutonomy, value);
      }

      public float Devastation
      {
         get => _devastation;
         set => SetField(ref _devastation, value);
      }

      public float Prosperity
      {
         get => _prosperity;
         set => SetField(ref _prosperity, value);
      }

      public List<string> DiscoveredBy
      {
         get => _discoveredBy;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _discoveredBy, value);
      }

      public string Capital
      {
         get => _capital;
         set => SetField(ref _capital, value);
      }

      public string Culture
      {
         get => _culture;
         set => SetField(ref _culture, value);
      }

      public string Religion
      {
         get => _religion;
         set => SetField(ref _religion, value);
      }

      public List<string> Buildings
      {
         get => _buildings;
         set => SetIfModifiedEnumerable<List<string>, string>(ref _buildings, value);
      }

      public bool IsHre
      {
         get => _isHre;
         set => SetField(ref _isHre, value);
      }

      public bool IsCity
      {
         get => _isCity;
         set => SetField(ref _isCity, value);
      }

      public bool IsSeatInParliament
      {
         get => _isSeatInParliament;
         set => SetField(ref _isSeatInParliament, value);
      }

      public string TradeGood
      {
         get => _tradeGood;
         set => SetField(ref _tradeGood, value);
      }

      public string LatentTradeGood
      {
         get => _latentTradeGood;
         set => SetField(ref _latentTradeGood, value);
      }

      public bool HasRevolt
      {
         get => _hasRevolt;
         set => SetField(ref _hasRevolt, value);
      }

      public string ReformationCenter
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

      public string TitleKey => $"PROV{Id}";
      public string AdjectiveKey => $"PROV_ADJ{Id}";
      public string TitleLocalisation
      {
         get
         {
            if (Globals.Settings.Misc.UseDynamicProvinceNames)
               return Localisation.GetDynamicProvinceLoc(this);
            return Localisation.GetLoc(TitleKey);
         }
      }
      public string AdjectiveLocalisation => Localisation.GetLoc(AdjectiveKey);

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
      public Area Area => GetFirstParentOfType(SaveableType.Area) as Area ?? Area.Empty;
      public Continent Continent => GetFirstParentOfType(SaveableType.Continent) as Continent ?? Continent.Empty;
      public TradeCompany TradeCompany => GetFirstParentOfType(SaveableType.TradeCompany) as TradeCompany ?? TradeCompany.Empty;

      // Map Concerns
      public bool IsNonRebelOccupied => Owner != Controller && Controller != "REB";
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

      public Terrain AutoTerrain = Terrain.Empty;
      public Terrain Terrain => GetFirstParentOfType(SaveableType.Terrain) as Terrain ?? Terrain.Empty;


      #region History

      // TODO

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


      #region Operators / Equals

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

      #endregion
   }
}