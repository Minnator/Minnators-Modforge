using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Parser;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.Saveables;

public class CommonCountry : Saveable, IGetSetProperty
{
   public CommonCountry(Country country, ObjEditingStatus status = ObjEditingStatus.Modified)
   {
      country.CommonCountry = this;
      _country = country;
      base.EditingStatus = status;
   }

   public CommonCountry(Country country, ref PathObj path) : this(country, ObjEditingStatus.Unchanged)
   {
      SetPath(ref path);
   }

   private Country _country;
   private Color _revolutionaryColor = Color.Empty;
   private string _graphicalCulture = string.Empty;
   private Color _color = Color.Empty;
   private List<string> _historicIdeas = [];
   private List<string> _historicUnits = [];
   private List<string> _leaderNames = [];
   private List<string> _armyNames = [];
   private List<string> _fleetNames = [];
   private List<string> _shipNames = [];
   private List<MonarchName> _monarchNames = [];
   private int _historicalScore = 0;
   private Tag _colonialParent = Tag.Empty;
   private int _randomNationChance = -1;
   private string _historicalCouncil = string.Empty;
   private string _preferredReligion = string.Empty;
   private string _specialUnitCulture = string.Empty;
   private List<string> _customAttributes = [];

   public string GraphicalCulture
   {
      get => _graphicalCulture;
      set => SetField(ref _graphicalCulture, value);
   }

   public Color Color   
   {
      get => _color;
      set => SetField(ref _color, value);
   }

   public Color RevolutionaryColor
   {
      get => _revolutionaryColor;
      set => SetField(ref _revolutionaryColor, value);
   }

   public List<string> HistoricIdeas  
   {
      get => _historicIdeas;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _historicIdeas, value);
   }

   public List<string> HistoricUnits
   {
      get => _historicUnits;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _historicUnits, value);
   }

   public List<string> ShipNames
   {
      // TODO: why is this being called twice if set by the interface
      get => _shipNames;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _shipNames, value);
   }

   public List<string> FleetNames
   {
      get => _fleetNames;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _fleetNames, value);
   }

   public List<string> ArmyNames
   {
      get => _armyNames;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _armyNames, value);
   }

   public List<string> LeaderNames
   {
      get => _leaderNames;
      set => SetIfModifiedEnumerable<List<string>, string>(ref _leaderNames, value);
   }

   public List<MonarchName> MonarchNames
   {
      get => _monarchNames;
      set => SetIfModifiedEnumerable<List<MonarchName>, MonarchName>(ref _monarchNames, value);
   }

   public int HistoricalScore
   {
      get => _historicalScore;
      set => SetField(ref _historicalScore, value);
   }
   public Tag ColonialParent
   {
      get => _colonialParent;
      set => SetField(ref _colonialParent, value);
   }
   public int RandomNationChance
   {
      get => _randomNationChance;
      set => SetField(ref _randomNationChance, value);
   }

   public string HistoricalCouncil
   {
      get => _historicalCouncil;
      set => SetField(ref _historicalCouncil, value);
   }

   public string PreferredReligion
   {
      get => _preferredReligion;
      set => SetField(ref _preferredReligion, value);
   }

   public string SpecialUnitCulture
   {
      get => _specialUnitCulture;
      set => SetField(ref _specialUnitCulture, value);
   }

   public List<string> CustomAttributes
   {
      get => _customAttributes;
      set => SetField(ref _customAttributes, value);
   }

   public static CommonCountry Empty { get; } = new(Country.Empty, ObjEditingStatus.Immutable);

   public void SetProperty(string propName, object value)
   {
      var prop = GetType().GetProperty(propName);
      if (prop == null)
         return;
      prop.SetValue(this, value);
   }

   public object? GetProperty(string propName)
   {
      var prop = GetType().GetProperty(propName);
      if (prop == null)
         return null;
      return prop.GetValue(this);
   }

   public event PropertyChangedEventHandler? PropertyChanged;
   public override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new (propertyName));
   }
   public override SaveableType WhatAmI() => SaveableType.Country;
   public override string[] GetDefaultFolderPath() => ["common", _country.CountryFilePath.FilePath];
   public override string GetFileEnding() => ".txt";
   public override KeyValuePair<string, bool> GetFileName() => new(_country.CountryFilePath.FileName, true);
   public override string SavingComment() => $"{_country.Tag} ({_country.TitleLocalisation})";
   public override string GetSavePromptString() => $"Save common country file for {_country.TitleLocalisation}";
   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      AddColor(0, Color, ref sb);
      AddString(0, GraphicalCulture, "graphical_culture", ref sb);
      sb.AppendLine($"revolutionary_colors = {{ {RevolutionaryColor.R,3} {RevolutionaryColor.G,3} {RevolutionaryColor.B,3} }}");
      if (ColonialParent != Tag.Empty)
         AddString(0, ColonialParent.ToString(), "colonial_parent", ref sb);
      if (RandomNationChance != -1)
         AddInt(0,  RandomNationChance, "random_nation_chance",ref sb);
      AddString(0, HistoricalCouncil, "historical_council", ref sb);
      AddString(0, PreferredReligion, "preferred_religion", ref sb);
      AddString(0, SpecialUnitCulture, "special_unit_culture", ref sb);
      AddInt(0, HistoricalScore, "historical_score", ref sb);
      sb.AppendLine();
      AddFormattedStringListOnePerRow("historical_idea_groups", HistoricIdeas, 0, ref sb);
      AddFormattedStringListOnePerRow("historical_units", HistoricUnits, 0, ref sb);
      AddFormattedStringListAutoQuote("leader_names", LeaderNames, 0, ref sb);
      AddFormattedStringListAutoQuote("army_names", ArmyNames, 0, ref sb);
      AddFormattedStringListAutoQuote("fleet_names", FleetNames, 0, ref sb);
      AddFormattedStringListAutoQuote("ship_names", ShipNames, 0, ref sb);
      AddFormattedStringListOnePerRow("monarch_names", MonarchNames.Select(x => x.ToString()).ToList(), 0, ref sb);

      return sb.ToString();
   }

   public override string ToString()
   {
      return $"{_country.Tag}: [common\\{_country.CountryFilePath.FilePath}]";
   }
}

public class HistoryCountry : Saveable, IGetSetProperty, IHistoryProvider<CountryHistoryEntry>
{

   public HistoryCountry(Country country, ObjEditingStatus status = ObjEditingStatus.Modified)
   {
      country.HistoryCountry = this;
      _country = country;
      base.EditingStatus = status;
   }

   public HistoryCountry(Country country, ref PathObj path) : this(country, ObjEditingStatus.Unchanged)
   {
      SetPath(ref path);
   }

   private Country _country;
   private bool _isElector; //
   private float _mercantilism = 0; //
   private int _fixedCapital = -1;//
   private int _governmentRank = 0; //
   private string _secondaryReligion = string.Empty; //
   private Government _government = Government.Empty; //
   private Culture _primaryCulture = Culture.Empty; //
   private string _religion = string.Empty; //
   private string _religiousSchool = string.Empty; //
   private TechnologyGroup _unitType = TechnologyGroup.Empty; //
   private List<GovernmentReform> _governmentReforms = []; //
   private List<Culture> _acceptedCultures = []; //
   private List<string> _unlockedCults = []; //
   private List<string> _estatePrivileges = []; //
   private List<string> _harmonizedReligions = []; //
   private Province _capital = Province.Empty; //
   private List<IElement> _initialEffects = []; //
   private List<ModifierAbstract> _modifiers = []; //
   private List<RulerModifier> _rulerModifiers = []; //
   private TechnologyGroup _technologyGroup = TechnologyGroup.Empty; //
   private List<CountryHistoryEntry> _history = []; //
   private List<Tag> _historicalFriends = []; //
   private List<Tag> _historicalRivals = []; //
   private Mana _nationalFocus = Mana.NONE; //

   #region Getters and Setters

   public float Mercantilism
   {
      get => _mercantilism;
      set => SetField(ref _mercantilism, value);
   }

   public List<ModifierAbstract> Modifiers
   {
      get => _modifiers;
      set => SetField(ref _modifiers, value);
   }

   public List<RulerModifier> RulerModifiers
   {
      get => _rulerModifiers;
      set => SetField(ref _rulerModifiers, value);
   }
   
   public List<IElement> InitialEffects
   {
      get => _initialEffects;
      set => SetField(ref _initialEffects, value);
   }

   [GameIcon(GameIcons.GovernmentReform)]
   public List<GovernmentReform> GovernmentReforms
   {
      get => _governmentReforms;
      set => SetIfModifiedEnumerable<List<GovernmentReform>, GovernmentReform>(ref _governmentReforms, value);
   }

   [GameIcon(GameIcons.AcceptedCultures)]
   public List<Culture> AcceptedCultures
   {
      get => _acceptedCultures;
      set => SetIfModifiedEnumerable<List<Culture>, Culture>(ref _acceptedCultures, value);
   }

   public List<string> UnlockedCults
   {
      get => _unlockedCults;
      set => SetField(ref _unlockedCults, value);
   }

   public List<string> EstatePrivileges
   {
      get => _estatePrivileges;
      set => SetField(ref _estatePrivileges, value);
   }

   public List<string> HarmonizedReligions
   {
      get => _harmonizedReligions;
      set => SetField(ref _harmonizedReligions, value);
   }

   public string SecondaryReligion
   {
      get => _secondaryReligion;
      set => SetField(ref _secondaryReligion, value);
   }

   public Government Government
   {
      get => _government;
      set => SetField(ref _government, value);
   }

   public Culture PrimaryCulture
   {
      get => _primaryCulture;
      set => SetField(ref _primaryCulture, value);
   }

   public string Religion
   {
      get => _religion;
      set => SetField(ref _religion, value);
   }

   public TechnologyGroup TechnologyGroup
   {
      get => _technologyGroup;
      set => SetField(ref _technologyGroup, value);
   }

   public string ReligiousSchool
   {
      get => _religiousSchool;
      set => SetField(ref _religiousSchool, value);
   }

   public TechnologyGroup UnitType
   {
      get => _unitType;
      set => SetField(ref _unitType, value);
   }

   public Mana NationalFocus
   {
      get => _nationalFocus;
      set => SetField(ref _nationalFocus, value);
   }

   public int GovernmentRank
   {
      get => _governmentRank;
      set => SetField(ref _governmentRank, value);
   }

   public Province Capital
   {
      get => _capital;
      set => SetField(ref _capital, value);
   }

   public string GetCapitalLoc => _capital.TitleLocalisation;

   public List<Tag> HistoricalRivals
   {
      get => _historicalRivals;
      set => SetField(ref _historicalRivals, value);
   }

   public List<Tag> HistoricalFriends
   {
      get => _historicalFriends;
      set => SetField(ref _historicalFriends, value);
   }

   public int FixedCapital
   {
      get => _fixedCapital;
      set => SetField(ref _fixedCapital, value);
   }
   
   public bool IsElector
   {
      get => _isElector;
      set => SetField(ref _isElector, value);
   }

   public List<CountryHistoryEntry> History
   {
      get => _history;
      set => SetField(ref _history, value);
   }

   #endregion

   public static HistoryCountry Empty { get; } = new(Country.Empty, ObjEditingStatus.Immutable);
   public void SetProperty(string propName, object value)
   {
      var prop = GetType().GetProperty(propName);
      if (prop == null)
         return;
      prop.SetValue(this, value);
   }

   public object? GetProperty(string propName)
   {
      var prop = GetType().GetProperty(propName);
      if (prop == null)
         return null;
      return prop.GetValue(this);
   }
   public override SaveableType WhatAmI() => SaveableType.Country;
   public override string[] GetDefaultFolderPath() => ["history", "countries"];
   public override string GetFileEnding() => ".txt";
   public override KeyValuePair<string, bool> GetFileName() => new($"{_country.Tag} - {_country.TitleLocalisation}", true);
   public override string SavingComment() => $"{_country.Tag} ({_country.TitleLocalisation})";
   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      AddString(0, Government.Name, "government", ref sb);
      AddStringList(0, "add_government_reform", GovernmentReforms.Select(x => x.Name).ToList(), ref sb);
      AddInt(0, GovernmentRank, "government_rank", ref sb);
      AddString(0, PrimaryCulture.Name, "primary_culture", ref sb);
      AddStringList(0, "add_accepted_culture", AcceptedCultures.Select(x => x.Name).ToList(), ref sb);
      AddString(0, Religion, "religion", ref sb);
      AddString(0, SecondaryReligion, "secondary_religion", ref sb);
      AddString(0, TechnologyGroup.ToString(), "technology_group", ref sb);
      AddString(0, UnitType.Name, "unit_type", ref sb);
      if (Capital != Province.Empty)
         AddInt(0, Capital.Id, "capital", ref sb);
      if (FixedCapital != -1)
         AddInt(0, FixedCapital, "fixed_capital", ref sb);
      if (IsElector)
         AddBool(0, IsElector, "elector", ref sb);
      sb.AppendLine("# Specific Effects #");
      AddStringList(0, "historical_friend", HistoricalFriends.Select(tag => tag.ToString()).ToList(), ref sb);
      AddStringList(0, "historical_rival", HistoricalRivals.Select(tag => tag.ToString()).ToList(), ref sb);
      AddEffects(0, InitialEffects, ref sb);
      if (NationalFocus != Mana.NONE)
         AddString(0, NationalFocus.ToString(), "national_focus", ref sb);
      AddStringList(0, "set_estate_privilege", EstatePrivileges, ref sb);
      AddStringList(0, "add_harmonized_religion", HarmonizedReligions, ref sb);
      AddString(0, ReligiousSchool, "religious_school", ref sb);
      AddStringList(0, "unlock_cult", UnlockedCults, ref sb);

      AddModifiers(0, Modifiers.Cast<ISaveModifier>().ToList(), ref sb);
      AddModifiers(0, RulerModifiers.Cast<ISaveModifier>().ToList(), ref sb);

      foreach (var entry in History) 
         entry.GetSavingString(tabs, ref sb);

      return sb.ToString();
   }
   public override string GetSavePromptString() => $"Saving the history/country part";

   public event PropertyChangedEventHandler? PropertyChanged;

   public override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new (propertyName));
   }

   public override string ToString()
   {
      return $"{_country.Tag}: [history\\countries\\]";
   }
}

public class CountryFilePath : Saveable, IStringify
{
   public event PropertyChangedEventHandler? PropertyChanged;

   private string[] _filePathArr;
   private Country _country;

   public CountryFilePath(string filePath, ObjEditingStatus status = ObjEditingStatus.Modified)
   {
      InitPath(filePath);
      base.EditingStatus = status;
   }

   public CountryFilePath(string filePath, ref PathObj path) : this(filePath, ObjEditingStatus.Unchanged)
   {
      SetPath(ref path);
   }
   
   public void SetCountry(Country country)
   {
      _country = country;
   }

   private void InitPath(string path)
   {
      _filePathArr = path.Split('/');
   }

   public string FilePath => System.IO.Path.Combine(_filePathArr[..^1]);
   public string FileName => _filePathArr[^1][.._filePathArr[^1].LastIndexOf('.')];

   public string[] FilePathArr
   {
      get => _filePathArr;
      set => SetField(ref _filePathArr, value);
   }

   public static CountryFilePath Empty { get; } = new(string.Empty, ObjEditingStatus.Immutable);

   public override void OnPropertyChanged(string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new(propertyName));
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Country;
   }

   public override string[] GetDefaultFolderPath()
   {
      return ["common", "country_tags"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("modforge_country_tags", false);
   }

   public override string SavingComment()
   {
      return string.Empty;
   }

   public override string GetSaveString(int tabs)
   {
      return $"{_country?.Tag} = \"{System.IO.Path.Combine(_filePathArr).Replace('\\', '/')}\"";
   }

   public override string GetSavePromptString() => $"Saving the filenames/country part";

   public override string ToString()
   {
      return $"{_country.Tag}: [{System.IO.Path.Combine(_filePathArr)}]";
   }

   public string Stringify() => _country.Tag;
}

public class Country : ProvinceCollection<Province>, ITitleAdjProvider, ITarget
{
   public Country(Tag tag, CountryFilePath fileName, Color color, ObjEditingStatus status = ObjEditingStatus.Immutable) : base(tag.ToString(), color, status)
   {
      Tag = tag;
      CountryFilePath = fileName;
      fileName.SetCountry(this);

      HistoryCountry = new(this, ObjEditingStatus.Unchanged);
      CommonCountry = new(this, ObjEditingStatus.Unchanged);
   }


   public new static Country Empty { get; } = new(Tag.Empty, CountryFilePath.Empty, Color.Empty, ObjEditingStatus.Immutable);

   [TypeConverter(typeof(ExpandableObjectConverter))]
   public HistoryCountry HistoryCountry { get; set; }
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public CommonCountry CommonCountry { get; set; }
   [TypeConverter(typeof(CountryFilePath))]
   public CountryFilePath CountryFilePath { get; set; }

   
   public Tag Tag { get; set; }
   public override Color Color
   {
      get => CommonCountry.Color;
      set
      {
         CommonCountry.Color = value;
         if (Globals.State == State.Running)
            ColorInvoke(this);
      }
   }

   public int GetCountOfPropertyList(string propName)
   {
      var prop = GetType().GetProperty(propName);
      if (prop == null)
         return 0;
      var value = prop.GetValue(this);
      if (value is ICollection collection)
         return collection.Count;
      return 0;
   }

   public override void OnPropertyChanged(string? propertyName = null) { }

   public override CAddProvinceCollectionGeneral<Province> GetAddCommand(ProvinceCollection<Province> collection, bool addToGlobal)
   {
      return new CAddToCountryProvinceCollection(collection, addToGlobal);
   }

   public override CRemoveProvinceCollectionGeneral<Province> GetRemoveCommand(ProvinceCollection<Province> collection, bool addToGlobal)
   {
      return new CRemoveCountryProvinceCollection(collection, addToGlobal);
   }

   public override void InternalAdd(Province composite)
   {
      base.InternalAdd(composite);
      composite.Owner = Tag;
      composite.Controller = Tag;
   }

   public override void InternalRemove(Province composite)
   {
      base.InternalRemove(composite);

      composite.Owner = Tag.Empty;
      composite.Controller = Tag.Empty;
   }

   public CountryHistoryEntry? GetClosestAfterDate(Date date)
   {
      if (HistoryCountry.History.Count == 0)
         return null;
      return HistoryCountry.History.OrderBy(h => h.Date).FirstOrDefault(h => h.Date > date);
   }

   public int Development
   {
      get
      {
         var sum = 0;
         foreach (var province in GetProvinces())
            sum += province.TotalDevelopment;
         return sum;
      }
      set
      {
         AddDevToRandomProvince(value - Development);
      }
   }


   internal void SpreadDevInSelectedCountryIfValid(int value)
   {
      if (Selection.SelectedCountry == Country.Empty)
         return;
      var provinces = Selection.SelectedCountry.GetProvinces();
      var pieces = MathHelper.SplitIntoNRandomPieces(provinces.Count, value, Globals.Settings.Generator.DevGeneratingSettings.MinDevelopmentInGeneration, Globals.Settings.Generator.DevGeneratingSettings.MaxDevelopmentInGeneration);
      if (pieces.Count != provinces.Count)
         return;

      var devPartsOut = new List<int>[] { [], [], [] };

      for (var i = 0; i < provinces.Count; i++)
      {
         var devParts = MathHelper.SplitIntoNRandomPieces(3, pieces[i], 1, Globals.Settings.Generator.DevGeneratingSettings.MaxDevelopmentInGeneration);
         devPartsOut[0].Add(devParts[0]);
         devPartsOut[1].Add(devParts[1]);
         devPartsOut[2].Add(devParts[2]);
      }

      SetFieldMultiple(provinces, devPartsOut[0], typeof(Province).GetProperty(nameof(Province.ScenarioBaseTax))!);
      SetFieldMultiple(provinces, devPartsOut[1], typeof(Province).GetProperty(nameof(Province.ScenarioBaseProduction))!);
      SetFieldMultiple(provinces, devPartsOut[2], typeof(Province).GetProperty(nameof(Province.ScenarioBaseManpower))!);

   }

   public ICollection<Province> GetCoreProvinces()
   {
      ICollection<Province> provinces = [];
      foreach (var id in Globals.LandProvinces)
      {
         if (id.Cores.Contains(Tag))
            provinces.Add(id);
      }
      return provinces;
   }

   public ICollection<Province> GetGetClaimedProvinces()
   {
      ICollection<Province> provinces = [];
      foreach (var id in Globals.LandProvinces)
      {
         if (id.Claims.Contains(Tag))
            provinces.Add(id);
      }
      return provinces;
   }

   public Province AddDevToRandomProvince(int dev)
   {
      var provinces = GetProvinces().ToList();
      var prov = provinces[Globals.Random.Next(provinces.Count)];

      if (dev < 3)
      {
         var devToImprov = Globals.Random.Next(3);
         switch (devToImprov)
         {
            case 0:
                  prov.ScenarioBaseTax += dev;
               break;
            case 1:
                  prov.ScenarioBaseProduction += dev;
               break;
            case 2:
                  prov.ScenarioBaseManpower += dev;
               break;
            default:
               prov.ScenarioBaseTax += dev;
               break;
         }
         return prov; 
      }

      var devParts = MathHelper.SplitIntoNRandomPieces(3, dev, Globals.Settings.Generator.DevGeneratingSettings.MinDevelopmentInGeneration,
         Globals.Settings.Generator.DevGeneratingSettings.MaxDevelopmentInGeneration);

      prov.ScenarioBaseTax += devParts[0];
      prov.ScenarioBaseProduction += devParts[1];
      prov.ScenarioBaseManpower += devParts[2];

      return prov;
   }

   public bool Exists => SubCollection.Count > 0;

   public string TitleLocalisation
   {
      get => Localisation.GetLoc(Tag);
      set => Localisation.AddOrModifyLocObject(TitleKey, value);
   }

   public string AdjectiveLocalisation
   {
      get => Localisation.GetLoc(AdjectiveKey);
      set => Localisation.AddOrModifyLocObject(AdjectiveKey, value);
   }

   public override string ToString() => Tag.ToString();
   public override bool Equals(object? obj)
   {
      if (obj is Country other)
         return Tag == other.Tag;
      return false;
   }
   public override int GetHashCode() => Tag.GetHashCode();
   public static bool operator ==(Country a, Country b) => a.Tag == b.Tag;
   public static bool operator !=(Country a, Country b) => a.Tag != b.Tag;
   public static implicit operator Country(Tag tag) => Globals.Countries[tag];
   public static implicit operator Tag(Country country) => country.Tag;
   public override SaveableType WhatAmI() => SaveableType.Country;
   public override string[] GetDefaultFolderPath()
   {
      return ["common", "countries"];
   }

   public override string GetFileEnding()
   {
      return ".txt";
   }

   public override KeyValuePair<string, bool> GetFileName()
   {
      return new("modforge_country_tags", false);
   }

   public override string SavingComment() => Localisation.GetLoc(TitleKey);
   public override string GetSaveString(int tabs)
   {
      throw new EvilActions("You should not do this! We need to fix the country mess!");
   }

   public override string GetSavePromptString() => $"Save countries file for {SavingComment()}";
   public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

   public override void ColorInvoke(ProvinceComposite composite) => ColorChanged.Invoke(this, composite);
   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler) => ColorChanged += handler;
   public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };
   public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs) => ItemsModified.Invoke(this, eventArgs);
   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler) => ItemsModified += eventHandler;
   public override void RemoveGlobal() => Globals.Countries.Remove(Name);
   public override void AddGlobal() => Globals.Countries.Add(Tag, this);
   public bool GetFlagPath(out string path) => PathManager.GetModOrVanillaPath(out path, out _, "gfx", "flags", $"{Tag}.tga");
   public Bitmap GetFlagBitmap()
   {
      if (!GetFlagPath(out var path))
         return PathManager.GetDefaultFlagPath();
      return ImageReader.ReadImage(path);
   }
   public List<Country> GetNeighbours()
   {
      List<Country> neighbours = [];
      foreach (var province in GetProvinces())
         foreach (var neighbour in Globals.AdjacentProvinces[province])
               if (neighbour.Owner != Empty && !neighbours.Contains(neighbour.Owner) && neighbour.Owner.Tag != Tag)
                  neighbours.Add(neighbour.Owner);
      return neighbours;
   }
   private void GetNeighboursInDistanceRecursive(int provinceDistance, List<Country> countries)
   {
      if (provinceDistance <= 0)
         return;

      var neighbours = GetNeighbours();
      foreach (var neighbour in neighbours)
      {
         if (!countries.Contains(neighbour))
         {
            countries.Add(neighbour);
            neighbour.GetNeighboursInDistanceRecursive(provinceDistance - 1, countries);
         }
      }
   }
   public List<Country> GetNeighboringCountriesWithSameSize()
   {
      var maxProvinceDistance = Globals.Settings.Generator.DistanceSettings.MaxProvinceDistanceForCountryWithSameSize;
      var maxDevDifference = Globals.Settings.Generator.DistanceSettings.MaxCountryDevDifferenceForCountryWithSameSize;

      List<Country> countries = [];
      List<Country> neighbours = [];

      GetNeighboursInDistanceRecursive(maxProvinceDistance, neighbours);

      foreach (var neighbour in GetNeighbours())
         if (Math.Abs(neighbour.Development - Development) <= maxDevDifference)
            countries.Add(neighbour);

      return countries;
   }
   public static List<Tag> GetHistoricRivals(int num)
   {
      var country = Selection.SelectedCountry;
      if (country == Empty)
         return [];

      var countries = country.GetNeighboringCountriesWithSameSize();
      if (countries.Count <= num)
         return countries.Select(c => c.Tag).ToList();

      List<Tag> rivals = [];
      for (var i = 0; i < num; i++)
      {
         var rival = countries[Globals.Random.Next(countries.Count)];
         if (country.HistoryCountry.HistoricalFriends.Contains(rival))
         {
            i--;
            goto removeRival;
         }
         rivals.Add(rival.Tag);
         removeRival:
         countries.Remove(rival);
      }
      return rivals;
   }

   public static List<Tag> GetHistoricFriends(int num)
   {
      var country = Selection.SelectedCountry;
      if (country == Empty)
         return [];

      var countries = country.GetNeighboringCountriesWithSameSize();
      if (countries.Count <= num)
         return countries.Select(c => c.Tag).ToList();

      List<Tag> friends = [];
      for (var i = 0; i < num; i++)
      {
         var friend = countries[Globals.Random.Next(countries.Count)];
         if (country.HistoryCountry.HistoricalRivals.Contains(friend))
         {
            i--;
            goto removeFriend;
         }
         friends.Add(friend.Tag);
         removeFriend:
         countries.Remove(friend);
      }
      return friends;
   }

   public static EventHandler<Country> CountryCreated = delegate { };

   public static void Create(string tag, Color color)
   {
      var fileName = Localisation.GetLoc(tag);
      if (string.IsNullOrWhiteSpace(fileName)) 
         fileName = tag;
      var country = new Country(tag, new ($"countries/{fileName}.txt"), color, ObjEditingStatus.Unchanged);
      var temp = new CommonCountry(country);
      var temp2 = new HistoryCountry(country);
      Globals.Countries.Add(tag, country);
      CountryCreated.Invoke(country, country);
   }

   public string TitleKey => Tag;

   public string AdjectiveKey => $"{Tag}_ADJ";


   public static IErrorHandle TryParse(string value, out Country country)
   {
      if (Tag.TryParse(value, out var tag))
         if (Globals.Countries.TryGetValue(tag, out country!))
            return ErrorHandle.Success;
      country = Empty;
      return new ErrorObject(ErrorType.TypeConversionError, "Could not parse Tag!", addToManager: false);
   }

   public static IErrorHandle GeneralParse(string value, out object result)
   {
      var handle = TryParse(value, out var country);
      result = country;
      return handle;
   }
}
