using System.Diagnostics.Metrics;
using Editor.DataClasses.Commands;
using Editor.Events;
using Editor.Helper;
using Editor.Interfaces;
using static Editor.Helper.ProvinceEnumHelper;

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
   public Tag Tag { get; } = tag;
   public Tag ColonialParent { get; set; } = Tag.Empty;
   public string FileName { get; } = fileName;
   public static Country Empty => new(Tag.Empty, Color.Empty, string.Empty);
   public Color RevolutionaryColor { get; set; } = Color.Empty;
   public string Gfx { get; set; } = string.Empty;
   public string HistoricalCouncil { get; set; } = string.Empty;
   public string PreferredReligion { get; set; } = string.Empty;
   public string SpecialUnitCulture { get; set; } = string.Empty;
   public int HistoricalScore { get; set; } = 0;
   public bool CanBeRandomNation { get; set; } = true;
   public List<ModifierAbstract> Modifiers { get; set; } = [];
   public List<RulerModifier> RulerModifiers { get; set; } = [];
   public List<string> HistoricalIdeas { get; set; } = [];
   public List<string> HistoricalUnits { get; set; } = [];
   public List<string> CustomAttributes { get; set; } = [];
   public List<MonarchName> MonarchNames { get; set; } = [];
   public List<string> ShipNames { get; set; } = [];
   public List<string> FleeTNames { get; set; } = [];
   public List<string> ArmyNames { get; set; } = [];
   public List<string> LeaderNames { get; set; } = [];
   // Effects on initialization
   public List<Effect> InitialEffects { get; set; } = [];
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
   private Province _capital = Province.Empty;
   private Color _color = Color.Empty;

   public Province Capital
   {
      get => _capital;
      set
      {
         if (value == Province.Empty)
            return;
         // Keeping the capitals list up to date to have a list of all capitals of nations which are currently on the map
         lock (Globals.Capitals)
         {
            if (Exists)
            {
               Globals.Capitals.Add(value);
            }
            Globals.Capitals.Remove(_capital);
         }
         _capital = value;
      }
   }

   public int FixedCapital { get; set; } = -1;
   public int ArmyTradition { get; set; } = 0;
   public float Mercantilism { get; set; } = 0;
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

   public List<Province> GetOwnedProvinces
   {
      get
      {
         List<Province> provinces = [];
         foreach (var id in Globals.LandProvinces)
         {
            if (id.Owner == Tag)
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
            if (id.Cores.Contains(Tag))
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
            if (id.Claims.Contains(Tag))
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
            if (id.Owner == Tag)
               return true;
         }
         return false;
      }
   }

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

   public override ModifiedData WhatAmI()
   {
      return ModifiedData.Country;
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

   public EventHandler<ProvinceComposite> ColorChanged = delegate { };
   public EventHandler<ProvinceComposite> ItemAddedToArea = delegate { };
   public EventHandler<ProvinceComposite> ItemRemovedFromArea = delegate { };
   public EventHandler<ProvinceComposite> ItemModified = delegate { };

   public override void Invoke(ProvinceComposite composite)
   {
      ColorChanged.Invoke(this, composite);
   }

   public override void AddToEvent(EventHandler<ProvinceComposite> handler)
   {
      ColorChanged += handler;
   }

   public override void Invoke(CProvinceCollectionType type, ProvinceComposite composite)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ItemAddedToArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Remove:
            ItemRemovedFromArea.Invoke(this, composite);
            break;
         case CProvinceCollectionType.Modify:
            ItemModified.Invoke(this, composite);
            break;
      }
   }

   public override void AddToEvent(CProvinceCollectionType type, EventHandler<ProvinceComposite> eventHandler)
   {
      switch (type)
      {
         case CProvinceCollectionType.Add:
            ItemAddedToArea += eventHandler;
            break;
         case CProvinceCollectionType.Remove:
            ItemRemovedFromArea += eventHandler;
            break;
         case CProvinceCollectionType.Modify:
            ItemModified += eventHandler;
            break;
      }
   }
}
