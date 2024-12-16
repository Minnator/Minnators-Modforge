using System.Diagnostics.CodeAnalysis;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.Helper;
using Editor.Parser;
using static Editor.Saving.SavingUtil;

namespace Editor.DataClasses.GameDataClasses
{
   public readonly struct MonarchName : IEquatable<MonarchName>
   {
      public MonarchName(string name, int ordinalNumber, int chance)
      {
         Name = name;
         OrdinalNumber = ordinalNumber;
         Chance = chance;
      }

      public MonarchName(string name, int chance)
      {
         Name = name;
         OrdinalNumber = Parsing.GetRegnalFromString(name);
         Chance = chance;
      }

      public string Name { get; }
      public int OrdinalNumber { get; }
      public int Chance { get; }

      // Chances for female names are negative
      public readonly bool IsFemale => Chance < 0;

      public static MonarchName Empty { get; } = new("", 0, 0);

      public override bool Equals([NotNullWhen(true)] object? obj)
      {
         if (obj is MonarchName other)
            return obj.GetHashCode() == other.GetHashCode();
         return false;
      }

      public override int GetHashCode()
      {
         return HashCode.Combine(Name, OrdinalNumber, Chance);
      }

      public static bool operator ==(MonarchName left, MonarchName right)
      {
         return left.Equals(right);
      }

      public static bool operator !=(MonarchName left, MonarchName right)
      {
         return !(left == right);
      }

      public bool Equals(MonarchName other)
      {
         return Name == other.Name && OrdinalNumber == other.OrdinalNumber && Chance == other.Chance;
      }

      public override string ToString()
      {
         return $"\"{Name}\" = {Chance}";
      }
      public static void DeleteFromGlobals(string name)
      {
         List<MonarchName> countryMonarchNames = [.. Selection.SelectedCountry.CommonCountry.MonarchNames];
         InternalDelete(name, ref countryMonarchNames);
         Selection.SelectedCountry.CommonCountry.MonarchNames = countryMonarchNames;
      }

      private static void InternalDelete(string name, ref List<MonarchName> names) => names.RemoveAt(names.FindIndex(x => x.Name.Equals(name)));

      public static void AddToGlobals(MonarchName name)
      {
         List<MonarchName> countryMonarchNames = [..Selection.SelectedCountry.CommonCountry.MonarchNames];
         InternalAdd(name, ref countryMonarchNames);
         Selection.SelectedCountry.CommonCountry.MonarchNames = countryMonarchNames;
      }

      private static void InternalAdd(MonarchName name, ref List<MonarchName> names) => names.Add(name);

      public static void UpdateGlobals(string oldName, MonarchName newName)
      {
         List<MonarchName> countryMonarchNames = [.. Selection.SelectedCountry.CommonCountry.MonarchNames];
         InternalDelete(oldName, ref countryMonarchNames);
         InternalAdd(newName, ref countryMonarchNames);
         Selection.SelectedCountry.CommonCountry.MonarchNames = countryMonarchNames;
      }
   }

   

   public enum PersonType
   {
      Monarch,
      Heir,
      Queen,
      Monarch_Consort
   }

   public struct Person
   {
      public Person()
      {
         BirthDate = Date.MinValue;
         DeathDate = Date.MinValue;
      }
      public PersonType Type { get; set; }
      public string Name { get; set; }
      public string MonarchName { get; set; }
      public string Dynasty { get; set; }
      public string Culture { get; set; }
      public string Religion { get; set; }
      public Date BirthDate { get; set; }
      public Date DeathDate { get; set; }
      public int ClaimStrength { get; set; }
      public int Adm { get; set; }
      public int Dip { get; set; }
      public int Mil { get; set; }
      public bool IsFemale { get; set; }
      public bool IsRegent { get; set; }
      public bool BlockDisinherit { get; set; }
      public Tag CountryOfOrigin { get; set; }

      public void GetSavingString(int tabs, ref StringBuilder sb)
      {
         OpenBlock(ref tabs, Type.ToString().ToLower(), ref sb);
         AddQuotedString(tabs, Name, "name", ref sb);
         AddQuotedString(tabs, MonarchName, "monarch_name", ref sb);
         AddQuotedString(tabs, Dynasty, "dynasty", ref sb);
         AddString(tabs, Culture, "culture", ref sb);
         AddString(tabs, Religion, "religion", ref sb);
         AddDate(tabs, BirthDate, "birth_date", ref sb);
         AddDate(tabs, DeathDate, "death_date", ref sb);
         AddInt(tabs, ClaimStrength, "claim", ref sb);
         AddInt(tabs, Adm, "ADM", ref sb);
         AddInt(tabs, Dip, "DIP", ref sb);
         AddInt(tabs, Mil, "MIL", ref sb);
         AddBoolIfYes(tabs, IsFemale, "female", ref sb);
         AddBoolIfYes(tabs, IsRegent, "regent", ref sb);
         AddBoolIfYes(tabs, BlockDisinherit, "block_disinherit", ref sb);
         AddString(tabs, CountryOfOrigin, "country_of_origin", ref sb);
         CloseBlock(ref tabs, ref sb);
      }
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
      public Date DeathDate { get; set; } = Date.MinValue;
      public List<string> Personalities { get; set; } = []; 
      public List<string> Traits { get; set; } = []; 

      public bool IsAlive => DeathDate == Date.MinValue;

      public override string ToString()
      {
         return $"{Name} ({Type})";
      }

      public Leader()
      {

      }

      public void GetSavingString(int tabs, ref StringBuilder sb)
      {
         OpenBlock(ref tabs, "leader", ref sb);
         AddQuotedString(tabs, Name, "name", ref sb);
         AddString(tabs, Type.ToString().ToLower(), "type", ref sb);
         AddInt(tabs, Fire, "fire", ref sb);
         AddInt(tabs, Shock, "shock", ref sb);
         AddInt(tabs, Maneuver, "manuever", ref sb);
         AddInt(tabs, Siege, "siege", ref sb);
         AddDate(tabs, DeathDate, "death_date", ref sb);
         CloseBlock(ref tabs, ref sb);
      }
   }
}