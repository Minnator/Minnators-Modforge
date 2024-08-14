using Editor.Helper;
using System.Diagnostics.Metrics;
using Editor.Interfaces;

namespace Editor.DataClasses.GameDataClasses;

public class Country(Tag tag, string fileName) : IProvinceCollection
{
   public Tag Tag { get; } = tag;
   public Tag ColonialParent { get; set; } = Tag.Empty;
   public string FileName { get; } = fileName;
   public static Country Empty => new(Tag.Empty, string.Empty);

   public int[] GetProvinceIds()
   {
      List<int> provinces = [];
      foreach (var prv in Globals.Provinces.Values)
      {
         if (prv.Owner == Tag)
            provinces.Add(prv.Id);
      }
      return [.. provinces];
   }

   public Province[] GetProvinces()
   {
      var ids = GetProvinceIds();
      var provinces = new Province[ids.Length];
      for (var i = 0; i < ids.Length; i++) 
         provinces[i] = Globals.Provinces[ids[i]];
      return provinces;
   }
   

   public IProvinceCollection? ScopeOut()
   {
      return null; //TODO if there is an overlord return it
   }

   public List<IProvinceCollection>? ScopeIn()
   {
      return null; //TODO if there are vassals return them
   }

   public Color Color { get; set; } = Color.Empty;
   public Color RevolutionaryColor { get; set; } = Color.Empty;
   public string Gfx { get; set; } = string.Empty;
   public string HistoricalCouncil { get; set; } = string.Empty;
   public string PreferredReligion { get; set; } = string.Empty;
   public string SpecialUnitCulture { get; set; } = string.Empty;
   public int HistoricalScore { get; set; } = 0;
   public bool CanBeRandomNation { get; set; } = true;

   public List<string> HistoricalIdeas { get; set; } = [];
   public List<string> HistoricalUnits { get; set; } = [];
   public List<MonarchName> MonarchNames { get; set; } = [];
   public List<string> ShipNames { get; set; } = [];
   public List<string> FleeTNames { get; set; } = [];
   public List<string> ArmyNames { get; set; } = [];
   public List<string> LeaderNames { get; set; } = [];


   // HISTORY
   public List<string> GovernmentReforms { get; set; } = [];
   public List<string> AcceptedCultures { get; set; } = [];
   public List<string> UnlockedCults { get; set; } = [];
   public List<string> EstatePrivileges { get; set; } = [];
   public List<string> HarmonizedReligions { get; set; } = [];
   public string SecondaryReligion { get; set; } = string.Empty;
   public string Government { get; set; } = string.Empty;
   public string PrimaryCulture { get; set; } = string.Empty;
   public string Religion { get; set; } = string.Empty;
   public string TechnologyGroup { get; set; } = string.Empty;
   public string ReligiousSchool { get; set; } = string.Empty;
   public string UnitType { get; set; } = string.Empty;
   public Mana NationalFocus { get; set; } = Mana.NONE;
   public List<Tag> HistoricalRivals { get; set; } = [];
   public List<Tag> HistoricalFriends { get; set; } = [];
   public int GovernmentRank { get; set; } = 0;
   private int _capital = -1;
   public int Capital
   {
      get => _capital;
      set
      {
         if (value == -1)
            return;
         // Keeping the capitals list up to date to have a list of all capitals of nations which are currently on the map
         if (Exists)
         {
            Globals.Capitals.Add(value);
            Globals.Capitals.Remove(_capital);
         }
         else
            Globals.Capitals.Remove(_capital);
         _capital = value;
      }
   }

   public int FixedCapital { get; set; } = -1;
   public int Mercantilism { get; set; } = 0;
   public int ArmyTradition { get; set; } = 0;
   public float ArmyProfessionalism { get; set; } = 0;
   public float Prestige { get; set; } = 0;
   public bool IsElector { get; set; } = false;

   public List<CountryHistoryEntry> History { get; set; } = [];

   public CountryHistoryEntry? GetClosestAfterDate(DateTime date)
   {
      if (!History.Any())
         return null;
      return History.OrderBy(h => h.Date).FirstOrDefault(h => h.Date > date);
   }

   public List<int> GetOwnedProvinces
   {
      get
      {
         List<int> provinces = [];
         foreach (var id in Globals.LandProvinces)
         {
            if (Globals.Provinces[id].Owner == Tag)
               provinces.Add(id);
         }
         return provinces;
      }
   }

   public List<int> GetCoreProvinces
   {
      get
      {
         List<int> provinces = [];
         foreach (var id in Globals.LandProvinces)
         {
            if (Globals.Provinces[id].Cores.Contains(Tag))
               provinces.Add(id);
         }
         return provinces;
      }
   }

   public List<int> GetClaimedProvinces
   {
      get
      {
         List<int> provinces = [];
         foreach (var id in Globals.LandProvinces)
         {
            if (Globals.Provinces[id].Claims.Contains(Tag))
               provinces.Add(id);
         }
         return provinces;
      }
   }

   public bool Exists
   {
      get
      {
         foreach (var id in Globals.LandProvinces)
         {
            if (Globals.Provinces[id].Owner == Tag)
               return true;
         }
         return false;
      }
   }

   public string GetLocalisation()
   {
      return Localisation.GetLoc(Tag);
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
}

public class CountryHistoryEntry(DateTime date)
{
   public DateTime Date { get; } = date;
   public List<Person> Persons { get; set; } = [];
   public List<Leader> Leaders { get; set; } = [];
   public List<KeyValuePair<string, string>> Effects { get; set; } = [];

   public bool HasPerson => Persons.Any();
   public bool HasEffect => Effects.Any();

   public bool HasMonarch => Persons.Any(p => p.Type == PersonType.Monarch);
   public bool HasHeir => Persons.Any(p => p.Type == PersonType.Heir);
   public bool HasQueen => Persons.Any(p => p.Type == PersonType.Queen);
   public int MonarchCount => Persons.Count(p => p.Type == PersonType.Monarch);
   public int HeirCount => Persons.Count(p => p.Type == PersonType.Heir);
   public int QueenCount => Persons.Count(p => p.Type == PersonType.Queen);
}

public enum PersonType
{
   Monarch,
   Heir,
   Queen
}

public struct Person
{
   public PersonType Type { get; set; }
   public string Name { get; set; }
   public string MonarchName { get; set; }
   public string Dynasty { get; set; }
   public string Culture { get; set; }
   public string Religion { get; set; }
   public DateTime BirthDate { get; set; }
   public DateTime DeathDate { get; set; }
   public int ClaimStrength { get; set; }
   public int Adm { get; set; }
   public int Dip { get; set; }
   public int Mil { get; set; }
   public bool IsFemale { get; set; }
   public bool IsRegent { get; set; }
   public bool BlockDisinherit { get; set; }
   public Tag CountryOfOrigin { get; set; }
}

public enum LeaderType
{
   General,
   Admiral,
   Explorer,
   Conquistador
}

public struct Leader
{
   public string Name { get; set; } = string.Empty;
   public int Fire { get; set; } = 0;
   public int Shock { get; set; } = 0;
   public int Maneuver { get; set; } = 0;
   public int Siege { get; set; } = 0;
   public int Age { get; set; } = 0;
   public bool IsFemale { get; set; } = false;
   public LeaderType Type { get; set; } = LeaderType.General;
   public DateTime DeathDate { get; set; } = DateTime.MinValue;
   public List<string> Personalities { get; set; } = []; //TODO replace with dynamic enum

   public bool IsAlive => DeathDate == DateTime.MinValue;

   public override string ToString()
   {
      return $"{Name} ({Type})";
   }

   public Leader()
   {

   }
}