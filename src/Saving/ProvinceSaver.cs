using System.Collections;
using System.Globalization;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.Helper;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.Saving
{
   public static class ProvinceSaver
   {
      //---------------------------------------- Accessible Methods ----------------------------------------

      

      //---------------------------------------- Helper Methods ----------------------------------------

      // A province will be saved by passing along a reference to a string builder and the province itself.
      // The string builder will be used to write the file.
      // The different portions of the province will be added to the string builder in the following order:
      // - definition Values
      // - anything else
      // - history Entries

      /// <summary>
      /// Generates the string for the province file.
      /// </summary>
      /// <param name="province"></param>
      /// <param name="contentString"></param>
      public static void GetProvinceFileString(Province province, out string contentString)
      {
         contentString = string.Empty;
         var sb = new StringBuilder();

         sb.Append(
            $"# {province.Id} {province.GetLocalisation()} - {DateTime.Now} {Environment.NewLine} {Environment.NewLine}");
         SaveDefinitionValues(ref sb, province);
         sb.AppendLine();
         SaveAnythingElse(ref sb, province);
         sb.AppendLine();
         SaveHistoryEntries(ref sb, province);
         sb.AppendLine();

         contentString = sb.ToString();
      }

      /// <summary>
      /// Adds all definition values of the p to the string builder.
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="p"></param>
      private static void SaveDefinitionValues(ref StringBuilder sb, Province p)
      {
         AddCollection("add_core", p.GetAttribute(ProvAttrGet.cores), ref sb);
         AddItem("owner", p.GetAttribute(ProvAttrGet.owner), ref sb);
         AddItem("controller", p.GetAttribute(ProvAttrGet.controller), ref sb);
         AddItem("base_tax", p.GetAttribute(ProvAttrGet.base_tax), ref sb);
         AddItem("base_production", p.GetAttribute(ProvAttrGet.base_production), ref sb);
         AddItem("base_manpower", p.GetAttribute(ProvAttrGet.base_manpower), ref sb);
         sb.AppendLine();
         AddItem("center_of_trade", p.GetAttribute(ProvAttrGet.center_of_trade), ref sb);
         AddItem("trade_goods", p.GetAttribute(ProvAttrGet.trade_good), ref sb);
         AddItem("extra_cost", p.GetAttribute(ProvAttrGet.extra_cost), ref sb);
         sb.AppendLine();
         AddItem("culture", p.GetAttribute(ProvAttrGet.culture), ref sb);
         AddItem("religion", p.GetAttribute(ProvAttrGet.religion), ref sb);
         var capital = p.GetAttribute(ProvAttrGet.capital) as string;
         if (capital!.StartsWith('\"'))
            sb.AppendLine($"capital = {capital}");
         else
            sb.AppendLine($"capital = \"{capital}\"");
         sb.AppendLine();
         AddItem("hre", p.GetAttribute(ProvAttrGet.hre), ref sb);
         AddItem("is_city", p.GetAttribute(ProvAttrGet.is_city), ref sb);
         AddItem("citysize", p.GetAttribute(ProvAttrGet.citysize), ref sb);
         AddItem("seat_in_parliament", p.GetAttribute(ProvAttrGet.seat_in_parliament), ref sb);
         AddItem("add_local_autonomy", p.GetAttribute(ProvAttrGet.local_autonomy), ref sb);
         AddItem("add_devastation", p.GetAttribute(ProvAttrGet.devastation), ref sb);
         AddItem("add_prosperity", p.GetAttribute(ProvAttrGet.prosperity), ref sb);
         AddItem("add_nationalism", p.GetAttribute(ProvAttrGet.nationalism), ref sb);
         AddItem("native_size", p.GetAttribute(ProvAttrGet.native_size), ref sb);
         AddItem("native_ferocity", p.GetAttribute(ProvAttrGet.native_ferocity), ref sb);
         AddItem("native_hostileness", p.GetAttribute(ProvAttrGet.native_hostileness), ref sb);
         AddItem("tribal_owner", p.GetAttribute(ProvAttrGet.tribal_owner), ref sb);
         AddCollection("add_building", p.Buildings, ref sb);
         AddCollection("add_claim", p.GetAttribute(ProvAttrGet.claims), ref sb);
         AddCollection("add_permanent_claim", p.GetAttribute(ProvAttrGet.permanent_claims), ref sb);
         sb.AppendLine();
         AddCollection("discovered_by", p.GetAttribute(ProvAttrGet.discovered_by), ref sb);
         sb.AppendLine();
         AddCollection("add_province_triggered_modifier", p.ProvinceTriggeredModifiers, ref sb);
         sb.AppendLine();
         AddEffects(ref sb);
         // TODO complete complexer saving
         // TradeCompanyInvestments
      }

      /// <summary>
      /// Writes all effects to the string builder.
      /// </summary>
      private static void AddEffects(ref StringBuilder sb)
      {
         // TODO EFFECT SAVING
      }

      /// <summary>
      /// Adds all values to the string builder which are no history entries or definitions
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="province"></param>
      private static void SaveAnythingElse(ref StringBuilder sb, Province province)
      {
         // latent_trade_goods
         var latentTradeGoods = province.GetAttribute(ProvAttrGet.latent_trade_good) as string;
         if (!string.IsNullOrEmpty(latentTradeGoods))
         {
            sb.AppendLine("latent_trade_goods = {")
               .AppendLine($"   {latentTradeGoods}")
               .AppendLine("}");
         }

         // province modifiers
         foreach (var mod in province.PermanentProvinceModifiers)
         {
            sb.AppendLine("add_permanent_province_modifier = {")
               .AppendLine($"\tname = {mod.Name}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

         // province modifiers
         foreach (var mod in province.ProvinceModifiers)
         {
            sb.AppendLine("add_province_modifier = {")
               .AppendLine($"\tname = {mod.Name}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

         // trade modifiers
         foreach (var mod in province.TradeModifiers)
         {
            sb.AppendLine("add_trade_modifier = {")
               .AppendLine($"\twho = {mod.Who}\n\tpower = {mod.Power}\n\tkey = {mod.Key}\n\tduration = {mod.Duration}")
               .AppendLine("}");
         }

         // Effects
         foreach (var effect in province.Effects)
            sb.AppendLine(effect.GetEffectString(0));
      }

      /// <summary>
      /// Adds all history entries of the province to the string builder.
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="province"></param>
      private static void SaveHistoryEntries(ref StringBuilder sb, Province province)
      {
         foreach (var entry in province.History)
            SaveHistoryEntry(entry, ref sb);
      }

      private static void SaveHistoryEntry(HistoryEntry entry, ref StringBuilder sb)
      {
         sb.AppendLine($"{entry.Date} = {{");
         foreach (var effect in entry.Effects)
            sb.AppendLine($"{effect.GetEffectString(1)}");
         sb.AppendLine("}");
      }

      private static void AddCollection(string name, object list, ref StringBuilder sb)
      {
         if (list is not IEnumerable collection)
            return;

         foreach (var item in collection)
            sb.AppendLine($"{name} = {item.ToString()}");
      }

      private static void AddItem(string name, object value, ref StringBuilder sb)
      {
         switch (value)
         {
            case bool b:
               if (!b)
                  break;
               sb.AppendLine($"{name} = {SavingUtil.GetYesNo(b)}");
               return;
            case float f:
               if (f == 0)
                  break;
               sb.AppendLine($"{name} = {f.ToString(CultureInfo.InvariantCulture),2}");
               break;
            case int i:
               if (i == 0)
                  break;
               sb.AppendLine($"{name} = {i.ToString()}");
               break;
            case Tag t:
               if (t.IsEmpty())
                  break;
               sb.AppendLine($"{name} = {t.ToString()}");
               break;
            default:
               sb.AppendLine($"{name} = {value.ToString()}");
               break;
         }
      }

   }
}