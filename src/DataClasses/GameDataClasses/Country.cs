using System.ComponentModel;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Newtonsoft.Json;

namespace Editor.DataClasses.GameDataClasses;

public enum CountryAttrType
{
   Int,
   Float,
   String,
   Bool,
   List,
   Tag,
   Color
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class CountryAttrMetadata(CountryAttrType type) : Attribute
{
   public CountryAttrType Type { get; } = type;
}

public enum CountrySetter
{
   [CountryAttrMetadata(CountryAttrType.Color)] color,
}

public class Country(Tag tag, Color color, string fileName) : ProvinceCollection<Province>(tag.ToString(), color)
{
   public new static Country Empty => new(Tag.Empty, Color.Empty, string.Empty);
   [Browsable(false)]
   public Tag Tag { get; set; } = tag;
   [Browsable(false)]
   public Tag ColonialParent { get; set; } = Tag.Empty;
   [Browsable(false)]
   public string FileName { get; } = fileName;
   [Browsable(false)]
   public Color RevolutionaryColor { get; set; } = Color.Empty;
   [Browsable(false)]
   public string Gfx { get; set; } = string.Empty;
   [Browsable(false)]
   public string HistoricalCouncil { get; set; } = string.Empty;
   public string PreferredReligion { get; set; } = string.Empty;
   public string SpecialUnitCulture { get; set; } = string.Empty;
   public int HistoricalScore { get; set; } = 0;
   public bool CanBeRandomNation { get; set; } = true;
   [Browsable(false)]
   public List<ModifierAbstract> Modifiers { get; set; } = [];
   [Browsable(false)]
   public List<RulerModifier> RulerModifiers { get; set; } = [];
   public List<string> HistoricalIdeas { get; set; } = [];
   public List<string> HistoricalUnits { get; set; } = [];
   public List<string> CustomAttributes { get; set; } = [];
   [Browsable(false)]
   public List<MonarchName> MonarchNames { get; set; } = [];
   [Browsable(false)]
   public List<string> ShipNames { get; set; } = [];
   [Browsable(false)]
   public List<string> FleetNames { get; set; } = [];
   [Browsable(false)]
   public List<string> ArmyNames { get; set; } = [];
   [Browsable(false)]
   public List<string> LeaderNames { get; set; } = [];
   // Effects on initialization
   [Browsable(false)]
   public List<Effect> InitialEffects { get; set; } = [];
   // HISTORY
   [Browsable(false)]
   public List<string> GovernmentReforms { get; set; } = [];
   [Browsable(false)]
   public List<string> AcceptedCultures { get; set; } = [];
   public List<string> UnlockedCults { get; set; } = [];
   public List<string> EstatePrivileges { get; set; } = [];
   public List<string> HarmonizedReligions { get; set; } = [];
   public string SecondaryReligion { get; set; } = string.Empty;
   [Browsable(false)]
   public string Government { get; set; } = string.Empty;
   [Browsable(false)]
   public string PrimaryCulture { get; set; } = string.Empty;
   [Browsable(false)]
   public string Religion { get; set; } = string.Empty;
   [Browsable(false)]
   public TechnologyGroup TechnologyGroup { get; set; } = TechnologyGroup.Empty;
   public string ReligiousSchool { get; set; } = string.Empty;
   [Browsable(false)]
   public string UnitType { get; set; } = string.Empty;
   [Browsable(false)]
   public Mana NationalFocus { get; set; } = Mana.NONE;
   public List<Tag> HistoricalRivals { get; set; } = [];
   public List<Tag> HistoricalFriends { get; set; } = [];
   [Browsable(false)]
   public int GovernmentRank { get; set; } = 0;
   [Browsable(false)]
   [JsonIgnore]
   public Province Capital { get; set; } = Province.Empty;

   public int FixedCapital { get; set; } = -1;
   public int ArmyTradition { get; set; } = 0;
   public float Mercantilism { get; set; } = 0;
   public float ArmyProfessionalism { get; set; } = 0;
   public float Prestige { get; set; } = 0;
   public bool IsElector { get; set; } = false;

   [Browsable(false)]
   public List<CountryHistoryEntry> History { get; set; } = [];

   public CountryHistoryEntry? GetClosestAfterDate(DateTime date)
   {
      if (History.Count == 0)
         return null;
      return History.OrderBy(h => h.Date).FirstOrDefault(h => h.Date > date);
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

      var devParts = MathHelper.SplitIntoNRandomPieces(3, dev, Globals.Settings.MiscSettings.MinDevelopmentInGeneration,
         Globals.Settings.MiscSettings.MaxDevelopmentInGeneration);

      prov.BaseTax += devParts[0];
      prov.BaseProduction += devParts[1];
      prov.BaseManpower += devParts[2];

      return prov;
   }

   [Browsable(false)]
   [JsonIgnore]
   public bool Exists => SubCollection.Count > 0;

   public string GetLocalisation()
   {
      return Localisation.GetLoc(Tag);
   }

   public string GetAdjectiveLocalisation()
   {
      return Localisation.GetLoc($"{Tag}_ADJ");
   }

   public override string ToString()
   {
      return $"{Tag.ToString()} ({GetLocalisation()})";
   }

   public override bool Equals(object? obj)
   {
      if (obj is Country other)
         return Tag == other.Tag;
      return false;
   }

   public override int GetHashCode()
   {
      return Tag.GetHashCode();
   }

   public static bool operator ==(Country a, Country b)
   {
      return a.Tag == b.Tag;
   }

   public static bool operator !=(Country a, Country b)
   {
      return a.Tag != b.Tag;
   }

   public static implicit operator Country(Tag tag)
   {
      return Globals.Countries[tag];
   }

   public static implicit operator Tag(Country country)
   {
      return country.Tag;
   }

   public override SaveableType WhatAmI()
   {
      return SaveableType.Country;
   }

   public override string SavingComment()
   {
      return Localisation.GetLoc(Tag);
   }

   public override PathObj GetDefaultSavePath()
   {
      return new (["history", "countries"]);
   }

   public override string GetSaveString(int tabs)
   {
      // TODO
      return "NOT YET SUPPORTED!";
   }

   public override string GetSavePromptString()
   {
      return $"Save countries file for {SavingComment()}";
   }

   public static EventHandler<ProvinceComposite> ColorChanged = delegate { };

   public override void ColorInvoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToColorEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }
   public static EventHandler<ProvinceCollectionEventArguments<Province>> ItemsModified = delegate { };

   public override void Invoke(ProvinceCollectionEventArguments<Province> eventArgs)
   {
      ItemsModified.Invoke(this, eventArgs);
   }

   public override void AddToEvent(EventHandler<ProvinceCollectionEventArguments<Province>> eventHandler)
   {
      ItemsModified += eventHandler;
   }

   public override void RemoveGlobal()
   {
      Globals.Countries.Remove(Name);
   }

   public override void AddGlobal()
   {
      Globals.Countries.Add(Tag, this);
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
      var maxProvinceDistance = Globals.Settings.MiscSettings.MaxProvinceDistanceForCountryWithSameSize;
      var maxDevDifference = Globals.Settings.MiscSettings.MaxCountryDevDifferenceForCountryWithSameSize;

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
         if (country.HistoricalFriends.Contains(rival))
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
         if (country.HistoricalRivals.Contains(friend))
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
