using System.Collections;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Formatters
{
   public class ProvinceAttributeFormatter : IFormatter
   {
      private Dictionary<ProvAttrGet, string> Template { get; set; } = new()
      {
         { ProvAttrGet.base_manpower, "base_manpower" },
         { ProvAttrGet.base_tax, "base_tax" },
         { ProvAttrGet.base_production, "base_production" },
         { ProvAttrGet.capital, "capital" },
         { ProvAttrGet.culture, "culture" },
         { ProvAttrGet.owner, "owner" },
         { ProvAttrGet.religion, "religion" },
         { ProvAttrGet.trade_good, "trade_goods" },
         { ProvAttrGet.is_city, "is_city" },
         { ProvAttrGet.capital, "capital"},
         { ProvAttrGet.owner, "owner" },
         { ProvAttrGet.cores, "add_core"},
         { ProvAttrGet.controller, "controller"},
         { ProvAttrGet.hre, "hre"},
         { ProvAttrGet.discovered_by, "discovered_by"},
         { ProvAttrGet.extra_cost, "extra_cost"},
         { ProvAttrGet.center_of_trade, "center_of_trade"},
         { ProvAttrGet.claims, "add_claim"},
         { ProvAttrGet.permanent_claims, "add_permanent_claim"},
         { ProvAttrGet.native_ferocity, "native_ferocity"},
         { ProvAttrGet.native_hostileness, "native_hostileness"},
         { ProvAttrGet.native_size, "native_size"},
         { ProvAttrGet.local_autonomy, "add_local_autonomy"},
         { ProvAttrGet.devastation, "add_devastation"},
      };
      
      public bool DoesFormatValue(string attr)
      {
         return Template.ContainsKey((ProvAttrGet)Enum.Parse(typeof(ProvAttrGet), attr));
      }

      // This method is used to start the formatting process of a province
      public void Format(StringBuilder sb, Province province)
      {
         GetAllNonDefaultValues(province, out var toFormat, out var toContinue);
         Format(sb, toFormat);

         FormatterController.Format(sb, toContinue);
      }

      public void Format(StringBuilder sb, object obj) { } // we don't need to format this as it's NOT a subformatter

      /*   Following Format:         
            key = value
            key = value
            key = value
            key = value
            key = value
      */
      public void Format(StringBuilder sb, List<KeyValuePair<string, object>> toFormat)
      {
         foreach (var kvp in toFormat) 
            sb.Append($"{kvp.Key} = {kvp.Value} \n");
      }

      private void GetAllNonDefaultValues(Province province, out List<KeyValuePair<string, object>> toFormat, out List<KeyValuePair<string, object>> toContinue)
      {
         var nonDefaultValues = province.GetNonDefaultValues();

         toFormat = [];
         toContinue = [];

         foreach (var value in nonDefaultValues)
         {
            if (FormatAttribute(value.Key, out var name))
               AddCorrectIfCollection(ref toFormat, name, value.Value);
            else
               AddCorrectIfCollection(ref toContinue, value.Key, value.Value);
         }
      }

      private void AddCorrectIfCollection(ref List<KeyValuePair<string, object>> outList, string name, object value)
      {
         if (value is IList list)
         {
            foreach (var item in list)
               outList.Add(new(name, item));
         }
         else
         {
            outList.Add(new(name, value));
         }
      }

      public bool FormatAttribute(string attr, out string name)
      {
         var provAttr = (ProvAttrGet)Enum.Parse(typeof(ProvAttrGet), attr);
         if (Template.TryGetValue(provAttr, out name!))
            return true;
         name = string.Empty;
         return false;
      }
   }
}