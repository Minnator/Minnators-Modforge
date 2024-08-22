using System.Collections;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Formatters
{
   public class ProvinceAttributeFormatter : IFormatter
   {
      private Dictionary<ProvAttr, string> Template { get; set; } = new()
      {
         { ProvAttr.base_manpower, "base_manpower" },
         { ProvAttr.base_tax, "base_tax" },
         { ProvAttr.base_production, "base_production" },
         { ProvAttr.capital, "capital" },
         { ProvAttr.culture, "culture" },
         { ProvAttr.owner, "owner" },
         { ProvAttr.religion, "religion" },
         { ProvAttr.trade_good, "trade_goods" },
         { ProvAttr.is_city, "is_city" },
         { ProvAttr.capital, "capital"},
         { ProvAttr.owner, "owner" },
         { ProvAttr.cores, "add_core"},
         { ProvAttr.controller, "controller"},
         { ProvAttr.hre, "hre"},
         { ProvAttr.discovered_by, "discovered_by"},
         { ProvAttr.extra_cost, "extra_cost"},
         { ProvAttr.center_of_trade, "center_of_trade"},
         { ProvAttr.claims, "add_claim"},
         { ProvAttr.permanent_claims, "add_permanent_claim"},
         { ProvAttr.native_ferocity, "native_ferocity"},
         { ProvAttr.native_hostileness, "native_hostileness"},
         { ProvAttr.native_size, "native_size"},
         { ProvAttr.local_autonomy, "add_local_autonomy"},
         { ProvAttr.devastation, "add_devastation"},
      };
      
      public bool DoesFormatValue(string attr)
      {
         return Template.ContainsKey((ProvAttr)Enum.Parse(typeof(ProvAttr), attr));
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
         var provAttr = (ProvAttr)Enum.Parse(typeof(ProvAttr), attr);
         if (Template.TryGetValue(provAttr, out name!))
            return true;
         name = string.Empty;
         return false;
      }
   }
}