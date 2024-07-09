namespace Editor.DataClasses;

public class Country(Tag tag, string fileName)
{
   public Tag Tag { get; } = tag;
   public Tag ColonialParent { get; set; } = Tag.Empty;
   public string FileName { get; } = fileName;

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
}

public class CountryHistoryEntry (DateTime date)
{
   public DateTime Date { get; } = date;
}

public struct Person
{
   public string Name { get; set; }
   public string MonarchName { get; set; }
   public string Dynasty { get; set; }
   public DateTime BirthDate { get; set; }
   public DateTime DeathDate { get; set; }
   public int ClaimStrength { get; set; }
   public int Adm { get; set; }
   public int Dip { get; set; }
   public int Mil { get; set; }
   public bool IsFemale { get; set; }
   public Tag CountryOfOrigin { get; set; }
}