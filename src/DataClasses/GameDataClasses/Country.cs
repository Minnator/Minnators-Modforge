using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses;

public class CommonCountry(string fileName, Country country) : NewSaveable, INotifyPropertyChanged
{
   private Color _revolutionaryColor = Color.Empty;
   private string _graphicalCulture = string.Empty;
   private Color _color;
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
      set => SetField(ref _historicIdeas, value);
   }

   public List<string> HistoricUnits
   {
      get => _historicUnits;
      set => SetField(ref _historicUnits, value);
   }

   public List<string> ShipNames
   {
      get => _shipNames;
      set => SetField(ref _shipNames, value);
   }

   public List<string> FleetNames
   {
      get => _fleetNames;
      set => SetField(ref _fleetNames, value);
   }

   public List<string> ArmyNames
   {
      get => _armyNames;
      set => SetField(ref _armyNames, value);
   }

   public List<string> LeaderNames
   {
      get => _leaderNames;
      set => SetField(ref _leaderNames, value);
   }

   public List<MonarchName> MonarchNames
   {
      get => _monarchNames;
      set => SetField(ref _monarchNames, value);
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



   public event PropertyChangedEventHandler? PropertyChanged;
   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
   protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      if (Globals.State == State.Running)
      {
         OnPropertyChanged(propertyName);
         EditingStatus = ObjEditingStatus.Modified;
      }
      return true;
   }
   public override SaveableType WhatAmI() => SaveableType.Country;
   public override string[] GetDefaultFolderPath() => ["common", "countries"];
   public override string GetFileEnding() => ".txt";
   public override KeyValuePair<string, bool> GetFileName() => new(country.GetLocalisation(), true);
   public override string SavingComment() => $"{country.Tag} ({country.GetLocalisation()})";
   public override string GetSavePromptString() => $"Save common country file for {country.GetLocalisation()}";
   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      AddColor(0, Color, ref sb);
      sb.AppendLine($"graphical_culture = {GraphicalCulture}");
      sb.AppendLine($"revolutionary_colors = {{ {RevolutionaryColor.R,3} {RevolutionaryColor.G,3} {RevolutionaryColor.B,3} }}");
      if (ColonialParent != Tag.Empty)
         AddString(0, "colonial_parent", ColonialParent.ToString(), ref sb);
      if (RandomNationChance != -1)
         AddInt(0, "random_nation_chance", RandomNationChance, ref sb);
      AddString(0, "historical_council", HistoricalCouncil, ref sb);
      AddString(0, "preferred_religion", PreferredReligion, ref sb);
      AddString(0, "special_unit_culture", SpecialUnitCulture, ref sb);
      AddInt(0, "historical_score", HistoricalScore, ref sb);
      AddFormattedStringListOnePerRow("historical_idea_groups", HistoricIdeas, 0, ref sb);
      AddFormattedStringListOnePerRow("historical_units", HistoricUnits, 0, ref sb);
      AddFormattedStringListOnePerRow("leader_names", LeaderNames, 0, ref sb);
      AddFormattedStringListOnePerRow("army_names", ArmyNames, 0, ref sb);
      AddFormattedStringListOnePerRow("fleet_names", FleetNames, 0, ref sb);
      AddFormattedStringListOnePerRow("ship_names", ShipNames, 0, ref sb);
      AddFormattedStringListOnePerRow("monarch_names", MonarchNames.Select(x => x.ToString()).ToList(), 0, ref sb);

      return sb.ToString();
   }
}

public class HistoryCountry(Country country) : NewSaveable, INotifyPropertyChanged
{
   private bool _isElector; //
   private int _mercantilism = 0; //
   private int _fixedCapital = -1;//
   private int _governmentRank = 0; //
   private string _secondaryReligion = string.Empty; //
   private string _government = string.Empty; //
   private string _primaryCulture = string.Empty; //
   private string _religion = string.Empty; //
   private string _religiousSchool = string.Empty; //
   private string _unitType = string.Empty; //
   private List<string> _governmentReforms = []; //
   private List<string> _acceptedCultures = []; //
   private List<string> _unlockedCults = []; //
   private List<string> _estatePrivileges = []; //
   private List<string> _harmonizedReligions = []; //
   private Province _capital = Province.Empty; //
   private List<Effect> _initialEffects = []; //
   private List<ModifierAbstract> _modifiers = []; //
   private List<RulerModifier> _rulerModifiers = []; //
   private TechnologyGroup _technologyGroup = TechnologyGroup.Empty; //
   private List<CountryHistoryEntry> _history = []; //
   private List<Tag> _historicalFriends = []; //
   private List<Tag> _historicalRivals = []; //
   private Mana _nationalFocus = Mana.NONE; //

   #region Getters and Setters

   public int Mercantilism
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
   
   public List<Effect> InitialEffects
   {
      get => _initialEffects;
      set => SetField(ref _initialEffects, value);
   }

   public List<string> GovernmentReforms
   {
      get => _governmentReforms;
      set => SetField(ref _governmentReforms, value);
   }

   public List<string> AcceptedCultures
   {
      get => _acceptedCultures;
      set => SetField(ref _acceptedCultures, value);
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

   public string Government
   {
      get => _government;
      set => SetField(ref _government, value);
   }

   public string PrimaryCulture
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

   public string UnitType
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

   public override SaveableType WhatAmI() => SaveableType.Country;
   public override string[] GetDefaultFolderPath() => ["history", "countries"];
   public override string GetFileEnding() => ".txt";
   public override KeyValuePair<string, bool> GetFileName() => new($"{country.Tag} - {country.GetLocalisation()}", true);
   public override string SavingComment() => $"{country.Tag} ({country.GetLocalisation()})";
   public override string GetSaveString(int tabs)
   {
      var sb = new StringBuilder();
      AddString(0, "government", Government, ref sb);
      AddStringList(0, "add_government_reform", GovernmentReforms, ref sb);
      AddInt(0, "government_rank", GovernmentRank, ref sb);
      AddString(0, "primary_culture", PrimaryCulture, ref sb);
      AddStringList(0, "add_accepted_culture", AcceptedCultures, ref sb);
      AddString(0, "religion", Religion, ref sb);
      AddString(0, "secondary_religion", SecondaryReligion, ref sb);
      AddString(0, "technology_group", TechnologyGroup.ToString(), ref sb);
      AddString(0, "unit_type", UnitType, ref sb);
      if (Capital != Province.Empty)
         AddInt(0, "capital", Capital.Id, ref sb);
      if (FixedCapital != -1)
         AddInt(0, "fixed_capital", FixedCapital, ref sb);
      if (IsElector)
         AddBool(0, IsElector, "elector", ref sb);
      sb.AppendLine("# Specific Effects #");
      AddStringList(0, "historical_friend", HistoricalFriends.Select(tag => tag.ToString()).ToList(), ref sb);
      AddStringList(0, "historical_rival", HistoricalRivals.Select(tag => tag.ToString()).ToList(), ref sb);
      AddEffects(0, InitialEffects, ref sb);
      if (NationalFocus != Mana.NONE)
         AddString(0, "national_focus", NationalFocus.ToString(), ref sb);
      AddStringList(0, "set_estate_privilege", EstatePrivileges, ref sb);
      AddStringList(0, "add_harmonized_religion", HarmonizedReligions, ref sb);
      AddString(0, "religious_school", ReligiousSchool, ref sb);
      AddStringList(0, "unlock_cult", UnlockedCults, ref sb);

      AddModifiers(0, Modifiers.Cast<ISaveModifier>().ToList(), ref sb);
      AddModifiers(0, RulerModifiers.Cast<ISaveModifier>().ToList(), ref sb);

      foreach (var entry in History)
      {
         sb.AppendLine($"{entry.Date} = {{");
         sb.AppendLine(entry.Content);
         sb.AppendLine("}");
      }

      return sb.ToString();
   }
   public override string GetSavePromptString() => $"Saving the history/country part";

   public event PropertyChangedEventHandler? PropertyChanged;

   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      if (Globals.State == State.Running)
      {
         OnPropertyChanged(propertyName);
         EditingStatus = ObjEditingStatus.Modified;
      }
      return true;
   }

}


public class Country : ProvinceCollection<Province>
{
   public new static Country Empty => new(Tag.Empty, Color.Empty, string.Empty);

   [TypeConverter(typeof(ExpandableObjectConverter))]
   public HistoryCountry HistoryCountry { get; set; }
   [TypeConverter(typeof(ExpandableObjectConverter))]
   public CommonCountry CommonCountry { get; set; }

   public Country(Tag tag, Color color, string fileName) : base(tag.ToString(), color)
   {
      CommonCountry = new (fileName, this);
      HistoryCountry = new (this);
      FileName = fileName;
      Tag = tag;
   }
   public Tag Tag { get; set; }
   public string FileName { get; }

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

   
   public CountryHistoryEntry? GetClosestAfterDate(DateTime date)
   {
      if (HistoryCountry.History.Count == 0)
         return null;
      return HistoryCountry.History.OrderBy(h => h.Date).FirstOrDefault(h => h.Date > date);
   }

   public int GetDevelopment()
   {
      var sum = 0;
      foreach (var province in GetProvinces())
         sum += province.GetTotalDevelopment();
      return sum;
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
                  prov.BaseTax += dev;
               break;
            case 1:
                  prov.BaseProduction += dev;
               break;
            case 2:
                  prov.BaseManpower += dev;
               break;
            default:
               prov.BaseTax += dev;
               break;
         }
         return prov;
      }

      var devParts = MathHelper.SplitIntoNRandomPieces(3, dev, Globals.Settings.Misc.MinDevelopmentInGeneration,
         Globals.Settings.Misc.MaxDevelopmentInGeneration);

      prov.BaseTax += devParts[0];
      prov.BaseProduction += devParts[1];
      prov.BaseManpower += devParts[2];

      return prov;
   }

   public bool Exists => SubCollection.Count > 0;

   public string GetLocalisation() => Localisation.GetLoc(Tag);
   public string GetAdjectiveLocalisation() => Localisation.GetLoc($"{Tag}_ADJ");
   public override string ToString() => $"{Tag.ToString()} ({GetLocalisation()})";
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

   public override string SavingComment() => Localisation.GetLoc(Tag);
   public override string GetSaveString(int tabs)
   {
      // TODO
      return "NOT YET SUPPORTED!";
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
   public bool GetFlagPath(out string path) => FilesHelper.GetModOrVanillaPath(out path, out _, "gfx", "flags", $"{Tag}.tga");
   public Bitmap GetFlagBitmap()
   {
      if (!GetFlagPath(out var path))
         return FilesHelper.GetDefaultFlagPath();
      return ImageReader.ReadTGAImage(path);
   }
   public List<Country> GetNeighbours()
   {
      List<Country> neighbours = [];
      foreach (var province in GetProvinces())
         foreach (var neighbour in Globals.AdjacentProvinces[province])
               if (neighbour.Owner != Tag.Empty && !neighbours.Contains(neighbour.Owner) && neighbour.Owner != Tag)
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
      var maxProvinceDistance = Globals.Settings.Misc.MaxProvinceDistanceForCountryWithSameSize;
      var maxDevDifference = Globals.Settings.Misc.MaxCountryDevDifferenceForCountryWithSameSize;

      List<Country> countries = [];
      List<Country> neighbours = [];

      GetNeighboursInDistanceRecursive(maxProvinceDistance, neighbours);

      foreach (var neighbour in GetNeighbours())
         if (Math.Abs(neighbour.GetDevelopment() - GetDevelopment()) <= maxDevDifference)
            countries.Add(neighbour);

      return countries;
   }
   public static List<string> GetHistoricRivals(int num)
   {
      var country = Selection.SelectedCountry;
      if (country == Empty)
         return [];

      var countries = country.GetNeighboringCountriesWithSameSize();
      if (countries.Count <= num)
         return countries.Select(c => c.Tag.ToString()).ToList();

      List<string> rivals = [];
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

   public static List<string> GetHistoricFriends(int num)
   {
      var country = Selection.SelectedCountry;
      if (country == Empty)
         return [];

      var countries = country.GetNeighboringCountriesWithSameSize();
      if (countries.Count <= num)
         return countries.Select(c => c.Tag.ToString()).ToList();

      List<string> friends = [];
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
}
