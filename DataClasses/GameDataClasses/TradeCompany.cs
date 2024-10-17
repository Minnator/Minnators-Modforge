using Editor.Helper;
using Editor.Interfaces;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : IProvinceCollection
   {
      public TradeCompany(string codename)
      {
         Name = codename;
         GenericName = string.Empty;
         SpecificName = string.Empty;
         Provinces = [];
         Color = Color.Empty;
      }

      public TradeCompany(string codeName, string genericName, string specificName, List<Province> provinces, Color color)
      {
         Name = codeName;
         GenericName = genericName;
         SpecificName = specificName;
         Provinces = [..provinces];
         Color = color;
      }

      public static TradeCompany Empty => new ("Empty");
      public string Name { get; set; }
      public string GenericName { get; set; }
      public string SpecificName { get; set; }
      public HashSet<Province> Provinces { get; set; }
      public Color Color { get; set; }

      public int[] GetProvinceIds()
      {
         List<int> provinces = [];
         foreach (var province in Provinces)
            provinces.Add(province.Id);
         return [..provinces];
      }

      public ICollection<Province> GetProvinces()
      {
         throw new NotImplementedException();
      }

      public IProvinceCollection? ScopeOut()
      {
         throw new IllegalScopeException(nameof(TradeCompany), "Can not scope out from tradeCompany");
      }

      public List<IProvinceCollection>? ScopeIn()
      {
         throw new IllegalScopeException(nameof(TradeCompany), "Can not scope in from tradeCompany");
      }

      public string GetLocalisation()
      {
         return Localisation.GetLoc(Name);
      }

      public override string ToString()
      {
         return Name;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TradeCompany company)
            return Name.Equals(company.Name);
         return false;
      }

      public override int GetHashCode()
      {
         return Name.GetHashCode();
      }
   }
}