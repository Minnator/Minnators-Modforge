using System.Collections;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Misc;
using static Editor.Helper.ProvinceEnumHelper;
using Region = Editor.DataClasses.GameDataClasses.Region;

namespace Editor.Helper
{
   public static class ProvColHelper
   {
      public enum AdvancedPropertiesEditables
      {
         Country,
      }

      public static ICollection<string> GetProvinceCollectionNames(SaveableType type)
      {
         return type switch
         {
            SaveableType.Area => Globals.Areas.Keys,
            SaveableType.Region => Globals.Regions.Keys,
            SaveableType.SuperRegion => Globals.SuperRegions.Keys,
            SaveableType.Continent => Globals.Continents.Keys,
            SaveableType.ProvinceGroup => Globals.ProvinceGroups.Keys,
            SaveableType.TradeNode => Globals.TradeNodes.Keys,
            SaveableType.TradeCompany => Globals.TradeCompanies.Keys,
            SaveableType.ColonialRegion => Globals.ColonialRegions.Keys,
            //SaveableType.Country => [..Globals.Countries.Keys],
            _ => []
         };
      }

      public static bool GetProvinceCollectionForTypeAndName<T>(SaveableType type, string name, out T value) where T : ProvinceComposite
      {
         value = default!;
         switch (type)
         {
            case SaveableType.Area:
               if (!Globals.Areas.TryGetValue(name, out var area) || area is not T areaT)
                  return false;
               value = areaT;
               return true;
            case SaveableType.Region:
               if (!Globals.Regions.TryGetValue(name, out var region) || region is not T regionT)
                  return false;
               value = regionT;
               return true;
            case SaveableType.SuperRegion:
               if (!Globals.SuperRegions.TryGetValue(name, out var superRegion) || superRegion is not T superRegionT)
                  return false;
               value = superRegionT;
               return true;
            case SaveableType.Continent:
               if (!Globals.Continents.TryGetValue(name, out var continent) || continent is not T continentT)
                  return false;
               value = continentT;
               return true;
            case SaveableType.ProvinceGroup:
               if (!Globals.ProvinceGroups.TryGetValue(name, out var provinceGroup) || provinceGroup is not T provinceGroupT)
                  return false;
               value = provinceGroupT;
               return true;
            case SaveableType.TradeNode:
               if (!Globals.TradeNodes.TryGetValue(name, out var tradeNode) || tradeNode is not T tradeNodeT)
                  return false;
               value = tradeNodeT;
               return true;
            case SaveableType.TradeCompany:
               if (!Globals.TradeCompanies.TryGetValue(name, out var tradeCompany) || tradeCompany is not T tradeCompanyT)
                  return false;
               value = tradeCompanyT;
               return true;
            case SaveableType.ColonialRegion:
               if (!Globals.ColonialRegions.TryGetValue(name, out var colonialRegion) || colonialRegion is not T colonialRegionT)
                  return false;
               value = colonialRegionT;
               return true;
            case SaveableType.Country:
               if (!Globals.Countries.TryGetValue(name, out var country) || country is not T countryT)
                  return false;
               value = countryT;
               return true;

            case SaveableType.Province:
               if (!int.TryParse(name, out var id) || !Globals.ProvinceIdToProvince.TryGetValue(id, out var prov) || prov is not T province)
                  return false;
               value = province;
               return true;
            default:
               return false;
         }
      }

      public static ICollection<T> GetProvinceCollectionOfTypeForProvinces<T>(ICollection<Province> provinces, SaveableType targetType) where T : ProvinceComposite
      {
         if (targetType == SaveableType.Province)
            return (provinces as ICollection<T>)!;

         HashSet<T> collection = [];
         foreach (var province in provinces)
         {
            //TODO maybe cache the inbetween parents so additional checks can be avoided. E.g. In same Region so we do not need to go to SuperRegion
            var parent = province.GetFirstParentOfType(targetType);
            if (parent != ProvinceComposite.Empty && parent is T parentT)
               collection.Add(parentT);
         }

         return collection;
      }

      public static T CreateNewObject<T>(string name, Color color, SaveableType type) where T : ProvinceComposite
      {
         switch (type)
         {
            case SaveableType.Area:
               return (new Area(name, color) as T)!;
            case SaveableType.Region:
               return (new Region(name, color) as T)!;
            case SaveableType.TradeNode:
               return (new TradeNode(name, color) as T)!;
            case SaveableType.TradeCompany:
               return (new TradeCompany([], name, color) as T)!;
            case SaveableType.ColonialRegion:
               return (new ColonialRegion(name, color) as T)!;
            case SaveableType.SuperRegion:
               return (new SuperRegion(name, color) as T)!;
            case SaveableType.Continent:
               return (new Continent(name, color) as T)!;
            case SaveableType.ProvinceGroup:
               return (new ProvinceGroup(name, color) as T)!;
            case SaveableType.Country:
               return (new Country(name, CountryFilePath.Empty, color) as T)!;
            default:
               throw new ArgumentOutOfRangeException(nameof(type), type, null);
         }
      }

      public static bool SetObjectInCollectionIfExists(object obj)
      {
         var converted = Convert.ChangeType(obj, obj.GetType());
         switch (converted)
         {
            case Country country:
               if (!Globals.Countries.TryGetValue(country.Tag, out _))
                  return false;
               Globals.Countries[country.Tag] = country;
               if (Globals.MapModeManager.CurrentMapModeType == MapModeType.Country)
                  Globals.MapModeManager.RenderCurrent();
               return true;
            default:
               return false;
         }
      }

      public static List<Province> GetProvincesWithAttribute(string attribute, object value, bool onlyLandProvinces = true)
      {
         if (onlyLandProvinces)
            return GetLandProvincesWithAttribute(attribute, value);
         var provinces = GetLandProvincesWithAttribute(attribute, value);
         provinces.AddRange(GetSeaProvincesWithAttribute(attribute, value));
         return provinces;
      }

      public static List<Province> GetProvincesWithAttribute(ProvAttrGet attribute, object value, bool onlyLandProvinces = true)
      {
         return GetProvincesWithAttribute(attribute.ToString(), value, onlyLandProvinces);
      }

      private static List<Province> GetLandProvincesWithAttribute(string attribute, object value)
      {
         List<Province> provinces = [];
         foreach (var id in Globals.LandProvinces)
            if (HasAttribute(id, attribute, value)) 
               provinces.Add(id);
         return provinces;
      }

      private static bool HasAttribute(Province province, string attribute, object value)
      {
         var attr = province.GetAttribute(attribute)!;
         var val = value.ToString();
         if (attr is IList list)
         {
            foreach (var item in list)
               if (item is not null && item.ToString()!.Equals(val))
                  return true;
         }
         else if (attr.ToString()!.Equals(val))
         {
            return true;
         }
         return false;
      }

      private static List<Province> GetSeaProvincesWithAttribute(string attribute, object value)
      {
         List<Province> provinces = [];
         foreach (var id in Globals.SeaProvinces)
            if (HasAttribute(id, attribute, value))
               provinces.Add(id);
         return provinces;
      }

   }
}