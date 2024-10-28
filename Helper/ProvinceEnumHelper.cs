using System.Reflection;

namespace Editor.Helper
{
   public static class ProvinceEnumHelper
   {

      public enum ProvAttrType
      {
         Int,
         Float,
         String,
         Bool,
         List,
         Tag
      }

      [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
      public sealed class ProvAttrMetadata(ProvAttrType type) : Attribute
      {
         public ProvAttrType Type { get; } = type;
      }
      public enum ProvAttrGet
      {
         [ProvAttrMetadata(ProvAttrType.Int)] base_manpower,
         [ProvAttrMetadata(ProvAttrType.Int)] base_tax,
         [ProvAttrMetadata(ProvAttrType.Int)] base_production,
         [ProvAttrMetadata(ProvAttrType.Float)] native_ferocity,
         [ProvAttrMetadata(ProvAttrType.String)] area,
         [ProvAttrMetadata(ProvAttrType.Bool)] hre,
         [ProvAttrMetadata(ProvAttrType.String)] culture,
         [ProvAttrMetadata(ProvAttrType.String)] religion,
         [ProvAttrMetadata(ProvAttrType.List)] claims,
         [ProvAttrMetadata(ProvAttrType.List)] permanent_claims,
         [ProvAttrMetadata(ProvAttrType.List)] cores,
         [ProvAttrMetadata(ProvAttrType.Tag)] controller,
         [ProvAttrMetadata(ProvAttrType.Tag)] owner,
         [ProvAttrMetadata(ProvAttrType.Tag)] tribal_owner,
         [ProvAttrMetadata(ProvAttrType.Int)] center_of_trade,
         [ProvAttrMetadata(ProvAttrType.Int)] extra_cost,
         [ProvAttrMetadata(ProvAttrType.Int)] native_hostileness,
         [ProvAttrMetadata(ProvAttrType.Int)] native_size,
         [ProvAttrMetadata(ProvAttrType.Int)] revolt_risk,
         [ProvAttrMetadata(ProvAttrType.Float)] local_autonomy,
         [ProvAttrMetadata(ProvAttrType.Float)] devastation,
         [ProvAttrMetadata(ProvAttrType.Float)] prosperity,
         [ProvAttrMetadata(ProvAttrType.Int)] nationalism,
         [ProvAttrMetadata(ProvAttrType.List)] discovered_by,
         [ProvAttrMetadata(ProvAttrType.String)] capital,
         [ProvAttrMetadata(ProvAttrType.Bool)] is_city,
         [ProvAttrMetadata(ProvAttrType.Bool)] seat_in_parliament,
         [ProvAttrMetadata(ProvAttrType.String)] trade_good,
         [ProvAttrMetadata(ProvAttrType.List)] history,
         [ProvAttrMetadata(ProvAttrType.Int)] id,
         [ProvAttrMetadata(ProvAttrType.String)] name,
         [ProvAttrMetadata(ProvAttrType.Bool)] revolt,
         [ProvAttrMetadata(ProvAttrType.Bool)] is_occupied,
         [ProvAttrMetadata(ProvAttrType.String)] latent_trade_good,
         [ProvAttrMetadata(ProvAttrType.Int)] citysize,
         [ProvAttrMetadata(ProvAttrType.List)] trade_company_investment,
         [ProvAttrMetadata(ProvAttrType.Int)] total_development,
         [ProvAttrMetadata(ProvAttrType.String)] continent,
         [ProvAttrMetadata(ProvAttrType.List)] buildings,
      }
      public enum ProvAttrSet
      {
         add_claim,
         remove_claim,
         add_core,
         remove_core,
         base_manpower,
         add_base_manpower,
         base_production,
         add_base_production,
         base_tax,
         add_base_tax,
         capital,
         center_of_trade,
         controller,
         culture,
         discovered_by,
         remove_discovered_by,
         extra_cost,
         hre,
         is_city,
         native_ferocity,
         native_hostileness,
         native_size,
         owner,
         religion,
         seat_in_parliament,
         trade_goods,
         tribal_owner,
         unrest,
         shipyard,
         revolt,
         revolt_risk,
         add_local_autonomy,
         add_nationalism,
         add_trade_company_investment,
         add_to_trade_company,
         reformation_center,
         add_province_modifier,
         remove_province_modifier,
         add_permanent_province_modifier,
         remove_permanent_province_modifier,
         add_province_triggered_modifier,
         remove_province_triggered_modifier,
         set_global_flag,
         devastation,
         prosperity,
         remove_permanent_claim,
         add_permanent_claim,
         citysize,
      }

      public static List<ProvAttrGet> GetNumericalAttributes(params ProvAttrType[] types)
      {
         // Create a list to hold the numerical attributes
         List<ProvAttrGet> numericalAttributes = [];

         // Get all the fields of the ProvAttr enum
         var fields = typeof(ProvAttrGet).GetFields(BindingFlags.Public | BindingFlags.Static);

         // Iterate through each field
         foreach (var field in fields)
         {
            // Get the ProvAttrMetadata attribute of the field
            var attrMeta = (ProvAttrMetadata)Attribute.GetCustomAttribute(field, typeof(ProvAttrMetadata))!;

            // If the attribute is of the specified type, add it to the list
            if (types.Contains(attrMeta.Type))
            {
               numericalAttributes.Add((ProvAttrGet)field.GetValue(null)!);
            }
         }

         // Return the list of numerical attributes
         return numericalAttributes;
      }

      public static ProvAttrType GetAttributeType(this ProvAttrGet attribute)
      {
         // Get the field of the ProvAttr enum
         var field = typeof(ProvAttrGet).GetField(attribute.ToString())!;
         // Get the ProvAttrMetadata attribute of the field
         return ((ProvAttrMetadata)Attribute.GetCustomAttribute(field, typeof(ProvAttrMetadata))!).Type;
      }

   }
}