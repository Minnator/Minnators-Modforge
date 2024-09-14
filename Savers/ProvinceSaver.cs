using System.Collections;
using System.Globalization;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms;
using Editor.Helper;

namespace Editor.Savers
{
   public static class ProvinceSaver
   {
      //---------------------------------------- Accessible Methods ----------------------------------------

      
      public static bool SaveAllLandProvinces()
      {
         var worked = true;
         foreach (var province in Globals.LandProvinces)
         {
            if (!SaveToHistoryFile(Globals.Provinces[province]))
               worked = false;
         }

         return worked;
      }

      /// <summary>
      /// Writes a file for the given province with all its values which are not default.
      /// </summary>
      /// <param name="province"></param>
      /// <returns></returns>
      public static bool SaveToHistoryFile(this Province province)
      {
         GetProvinceFileString(province, out var content);
         //return IO.WriteToFile(province.GetHistoryFilePath(), content, false);

         // Debug stuff
         var fileName = $"{province.Id}-{province.GetLocalisation()}.txt";
         var downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
         return IO.WriteAllInANSI(Path.Combine(downloadsFolder, "FileSavingTest", fileName), content, false);
      }

      /// <summary>
      /// Writes a file for all provinces with all its values which are not default and have the status "Modified".
      /// </summary>
      /// <returns></returns>
      public static bool SaveAllModifiedProvinces()
      {
         var worked = true;
         foreach (var province in EditingManager.GetModifiedProvinces())
         {
            if (!SaveToHistoryFile(province))
               worked = false;
         }

         return worked;
      }

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
      private static void GetProvinceFileString(Province province, out string contentString)
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
         AddCollection("add_core", p.GetAttribute(ProvAttr.cores), ref sb);
         AddItem("owner", p.GetAttribute(ProvAttr.owner), ref sb);
         AddItem("controller", p.GetAttribute(ProvAttr.controller), ref sb);
         sb.AppendLine();
         AddItem("base_tax", p.GetAttribute(ProvAttr.base_tax), ref sb);
         AddItem("base_production", p.GetAttribute(ProvAttr.base_production), ref sb);
         AddItem("base_manpower", p.GetAttribute(ProvAttr.base_manpower), ref sb);
         sb.AppendLine();
         AddCollection("add_claim", p.GetAttribute(ProvAttr.claims), ref sb);
         AddCollection("add_permanent_claim", p.GetAttribute(ProvAttr.permanent_claims), ref sb);
         sb.AppendLine();
         AddItem("center_of_trade", p.GetAttribute(ProvAttr.center_of_trade), ref sb);
         AddItem("trade_goods", p.GetAttribute(ProvAttr.trade_good), ref sb);
         AddItem("extra_cost", p.GetAttribute(ProvAttr.extra_cost), ref sb);
         sb.AppendLine();
         AddItem("culture", p.GetAttribute(ProvAttr.culture), ref sb);
         AddItem("religion", p.GetAttribute(ProvAttr.religion), ref sb);
         sb.AppendLine($"capital = \"{p.GetAttribute(ProvAttr.capital)}\"");
         sb.AppendLine();
         AddItem("hre", p.GetAttribute(ProvAttr.hre), ref sb);
         AddItem("is_city", p.GetAttribute(ProvAttr.is_city), ref sb);
         AddItem("seat_in_parliament", p.GetAttribute(ProvAttr.seat_in_parliament), ref sb);
         sb.AppendLine();
         AddItem("add_local_autonomy", p.GetAttribute(ProvAttr.local_autonomy), ref sb);
         AddItem("add_devastation", p.GetAttribute(ProvAttr.devastation), ref sb);
         AddItem("add_prosperity", p.GetAttribute(ProvAttr.prosperity), ref sb);
         AddItem("add_nationalism", p.GetAttribute(ProvAttr.nationalism), ref sb);
         sb.AppendLine();
         AddItem("native_size", p.GetAttribute(ProvAttr.native_size), ref sb);
         AddItem("native_ferocity", p.GetAttribute(ProvAttr.native_ferocity), ref sb);
         AddItem("native_hostileness", p.GetAttribute(ProvAttr.native_hostileness), ref sb);
         AddItem("tribal_owner", p.GetAttribute(ProvAttr.tribal_owner), ref sb);
         sb.AppendLine();
         AddCollection("add_building", p.Buildings, ref sb);
         sb.AppendLine();
         AddCollection("discovered_by", p.GetAttribute(ProvAttr.discovered_by), ref sb);
         sb.AppendLine();
      }

      /// <summary>
      /// Adds all values to the string builder which are no history entries or definitions
      /// </summary>
      /// <param name="sb"></param>
      /// <param name="province"></param>
      private static void SaveAnythingElse(ref StringBuilder sb, Province province)
      {
         // latent_trade_goods
         var latentTradeGoods = province.GetAttribute(ProvAttr.latent_trade_good) as string;
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
         sb.AppendLine($"{entry.Date.ToString("yyyy.MM.dd")} = {{");
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
               sb.AppendLine($"{name} = {GetYesNo(b)}");
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

      private static string GetYesNo(bool value)
      {
         return value ? "yes" : "no";
      }

   }
}