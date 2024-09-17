namespace Editor.DataClasses.GameDataClasses
{
   public struct MonarchName(string name, int ordinalNumber, int chance)
   {
      public string Name { get; } = name;
      public int OrdinalNumber { get; } = ordinalNumber;
      public int Chance { get; } = chance;

      // Chances for female names are negative
      public readonly bool IsFemale => Chance < 0;
   }

   public class CountryHistoryEntry(DateTime date)
   {
      public DateTime Date { get; } = date;
      public List<Person> Persons { get; set; } = [];
      public List<Leader> Leaders { get; set; } = [];
      public List<Effect> Effects { get; set; } = [];

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
      public List<string> Traits { get; set; } = []; //TODO replace with dynamic enum

      public bool IsAlive => DeathDate == DateTime.MinValue;

      public override string ToString()
      {
         return $"{Name} ({Type})";
      }

      public Leader()
      {

      }
   }
}