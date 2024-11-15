using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class TradeCompanyInvestmentsLoading
   {
      public static void Load()
      {
         FilesHelper.GetFilesUniquelyAndCombineToOne(out var rawContent, "common", "tradecompany_investments");
         Parsing.RemoveCommentFromMultilineString(ref rawContent, out var content);

         var elements = Parsing.GetElements(0, content);

         foreach (var element in elements)
         {
            if (element is not Block tradeCompInvest)
            {
               Globals.ErrorLog.Write($"Illegal content in <tradecompany_investments>: {((Content)element).Value}");
               continue;
            }

            var tci = new TradeCompanyInvestment(tradeCompInvest.Name);

            foreach (var element2 in tradeCompInvest.Blocks)
            {
               if (element2 is not Block block)
               {
                  ParseTCIAttributes(((Content)element2), tci);
                  continue;
               }

               switch (block.Name)
               {
                  case "allow":
                     tci.Triggers = block.GetContent;
                     break;
                  case "company_province_area_modifier":
                     ModifierParser.ParseModifiers(block.GetContent, out var modifiers);
                     tci.ProvinceModifiers.AddRange(modifiers);
                     break;
                  case "area_modifier":
                     ModifierParser.ParseModifiers(block.GetContent, out modifiers);
                     tci.AreaModifiers.AddRange(modifiers);
                     break;
                  case "owner_company_region_modifier":
                     ModifierParser.ParseModifiers(block.GetContent, out modifiers);
                     tci.OwnerRegionModifiers.AddRange(modifiers);
                     break;
                  case "owner_modifier":
                     ModifierParser.ParseModifiers(block.GetContent, out modifiers);
                     tci.OwnerModifiers.AddRange(modifiers);
                     break;
                  case "company_region_modifier":
                     ModifierParser.ParseModifiers(block.GetContent, out modifiers);
                     tci.RegionModifiers.AddRange(modifiers);
                     break;
                  case "ai_global_worth": // I don't care about that as I am not saving that file yet.
                  case "ai_area_worth":
                  case "ai_region_worth":
                     break;
                  default:
                     Globals.ErrorLog.Write($"Unknown Block in <{tradeCompInvest.Name}>: {block.Name}");
                     break;
               }
            }

            Globals.TradeCompanyInvestments.Add(tci.Name, tci);
         }
      }

      private static void ParseTCIAttributes(Content content, TradeCompanyInvestment tci)
      {
         var kvps = Parsing.GetKeyValueList(content.Value);
         foreach (var kvp in kvps)
         {
            switch (kvp.Key)
            {
               case "upgrades_to":
                  tci.UpgradesTo = kvp.Value;
                  break;
               case "category":
                  tci.Category = kvp.Value switch
                  {
                     "company_garrison" => TCICategory.company_garrison,
                     "harbor" => TCICategory.harbor,
                     "local_venture" => TCICategory.local_venture,
                     "foreign_influence" => TCICategory.foreign_influence,
                     "governance" => TCICategory.governance,
                     _ => TCICategory.company_garrison
                  };
                  break;
               case "cost":
                  if (!float.TryParse(kvp.Value, out var cost))
                     Globals.ErrorLog.Write($"Illegal value for <cost> in <{content}>: {kvp.Value}");
                  tci.Cost = (int)cost;
                  break;
               case "sprite":
                  kvp.Value.TrimQuotes(out var sprite);
                  tci.Sprite = sprite;
                  break;
               default:
                  Globals.ErrorLog.Write($"Unknown Attribute in <{content}>: {kvp.Key}");
                  break;

            }
         }
      }
   }
}