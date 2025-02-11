
using Editor.ErrorHandling;

namespace Editor.Helper
{
   [Flags]
   public enum SaveableType
   {
      SaveProvinces = 1 << 0,
      Area = 1 << 1,
      Region = 1 << 2,
      TradeNode = 1 << 3,
      TradeCompany = 1 << 4,
      ColonialRegion = 1 << 5,
      SuperRegion = 1 << 6,
      Continent = 1 << 7,
      ProvinceGroup = 1 << 8,
      EventModifier = 1 << 9,
      Localisation = 1 << 10,
      Country = 1 << 11,
      Province = 1 << 12,
      Terrain = 1 << 13,
      Climate = 1 << 14,
      Price = 1 << 15,
      TradeGood = 1 << 16,
      All = SaveProvinces | Area | Region | TradeNode | TradeCompany | ColonialRegion | SuperRegion | Continent | ProvinceGroup | EventModifier | Localisation | Country | Province | Terrain | Climate | TradeGood | Price
   }


   public static class EnumHelper
   {
      public static T? GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
      {
         var type = enumVal.GetType();
         var memInfo = type.GetMember(enumVal.ToString());
         var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
         return (attributes.Length > 0) ? (T)attributes[0] : null;
      }

      public static List<string> GetSaveableNames()
      {
         return GetNamesExcluding(typeof(SaveableType), SaveableType.All.ToString());
      }

      public static List<string> GetNamesExcluding(this Type enumType, params string[] excludedNames)
      {
         if (!enumType.IsEnum)
            throw new ArgumentException("The type must be an enum.", nameof(enumType));
         return Enum.GetNames(enumType).Except(excludedNames, StringComparer.OrdinalIgnoreCase).ToList();
      }

      public static IErrorHandle ManaParseGeneral(string? value, out object mana)
      {
         if (Enum.TryParse<Mana>(value, out var manaMana))
         {
            mana = manaMana;
            return ErrorHandle.Success;
         }
         mana = Mana.NONE;
         return new ErrorObject(ErrorType.TypeConversionError, $"Could not parse {value} to {typeof(Mana)}");
      }
   }
}