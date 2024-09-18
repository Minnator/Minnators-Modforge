using System.Security.Policy;
using Editor.Helper;
using Editor.Interfaces;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public class ColonialRegion(string name) : IProvinceCollection
   {
      public string Name { get; init; } = name;
      public Color Color { get; set; }
      public int TaxIncome { get; set; }
      public int NativeSize { get; set; }
      public int NativeFerocity { get; set; }
      public int NativeHostileness { get; set; }
      public List<KeyValuePair<string, int>> Cultures { get; set; } = [];
      public List<KeyValuePair<string, int>> Religions { get; set; } = [];
      public List<KeyValuePair<string, int>> TradeGoods { get; set; } = [];
      public HashSet<int> Provinces { get; set; } = [];
      public List<TriggeredName> Names { get; set; } = [];

      public int[] GetProvinceIds()
      {
         return Provinces.ToArray();
      }

      public IProvinceCollection? ScopeOut()
      {
         throw new IllegalScopeException("Can not scope out from Colonial Region");
      }

      public List<IProvinceCollection>? ScopeIn()
      {
         throw new IllegalScopeException("Can not scope in from Colonial Region");
      }

      public string GetLocalisation()
      {
         return Localisation.GetLoc(Name);
      }

      public override bool Equals(object? obj)
      {
         if (obj is ColonialRegion other)
            return Name == other.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }

      public override string ToString()
      {
         return Name;
      }
   }

   public class TriggeredName(string name, string trigger)
   {
      public string Name { get; init; } = name;
      public string Trigger { get; set; } = trigger;

      public TriggeredName(string name) : this(name, string.Empty)
      {
      }

      public static TriggeredName Empty => new (string.Empty);

      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TriggeredName other)
            return Name == other.Name;
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }
}