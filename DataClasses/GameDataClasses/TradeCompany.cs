using Editor.Helper;
using Editor.Interfaces;
using Editor.Parser;

namespace Editor.DataClasses.GameDataClasses
{
   public class TradeCompany : IProvinceCollection
   {
      public TradeCompany(string codename)
      {
         CodeName = codename;
         GenericName = string.Empty;
         SpecificName = string.Empty;
         Provinces = [];
         Color = Color.Empty;
      }

      public TradeCompany(string codeName, string genericName, string specificName, int[] provinces, Color color)
      {
         CodeName = codeName;
         GenericName = genericName;
         SpecificName = specificName;
         Provinces = [..provinces];
         Color = color;
      }

      public static TradeCompany Empty => new ("Empty");
      
      public string CodeName { get; init; }
      public string GenericName { get; set; }
      public string SpecificName { get; set; }
      public HashSet<int> Provinces { get; set; }
      public Color Color { get; set; }

      public int[] GetProvinceIds()
      {
         return Provinces.ToArray();
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
         return Localisation.GetLoc(CodeName);
      }

      public override string ToString()
      {
         return CodeName;
      }

      public override bool Equals(object? obj)
      {
         if (obj is TradeCompany company)
            return CodeName.Equals(company.CodeName);
         return false;
      }

      public override int GetHashCode()
      {
         return CodeName.GetHashCode();
      }
   }
}