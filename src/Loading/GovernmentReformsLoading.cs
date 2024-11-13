using System.Diagnostics;
using System.Text;
using Editor.DataClasses;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   public static class GovernmentReformsLoading
   {

      public static void Load()
      {
         var sw = Stopwatch.StartNew();

         var files = FilesHelper.GetFilesFromModAndVanillaUniquely("*.txt", "common", "government_reforms");

         //TODO parallelize
         foreach (var file in files)
         {
            ParseGovernmentReformsFile(file);
         }

         sw.Stop();
         Globals.LoadingLog.WriteTimeStamp("Government Reforms", sw.ElapsedMilliseconds);

         var sb = new StringBuilder();
         sb.AppendLine("Government Reforms:");
         foreach (var reform in Globals.GovernmentReforms.Values)
         {
            sb.AppendLine(reform.ToString());
         }
         // users downloads folder
         var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
         File.WriteAllText(Path.Combine(downloads, "government_reforms.txt"), sb.ToString());
         
      }

      private static void ParseGovernmentReformsFile(string path)
      {

         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
         var elements = Parsing.GetElements(0, content);

         foreach (var element in elements)
         {
            if (element is not Block reformBlock)
            {
               Globals.ErrorLog.Write($"Error parsing government reform file {path}: {element} is not a block");
               continue;
            }
            var reform = new GovernmentReform(reformBlock.Name);
            ParseReformBlocks(reformBlock.GetBlockElements, ref reform);
            ParseReformContent(reformBlock.GetContent, ref reform);

            if (!Globals.GovernmentReforms.TryAdd(reform.Name, reform))
            {
               Globals.ErrorLog.Write($"Dublicate government reform in file {path}: {reform.Name} already exists");
               continue;
            }
         }
      }

      private static void ParseReformBlocks(List<Block> blocks, ref GovernmentReform reform)
      {
         foreach (var block in blocks)
         {
            switch (block.Name)
            {
               case "modifiers":
                  ModifierParser.ParseModifiers(block.GetContent, out var modifiers);
                  reform.Modifiers.AddRange(modifiers);
                  break;
               case "potential":
                  // TODO once triggers are implemented
                  break;
               case "trigger":
                  // TODO once triggers are implemented
                  break;
               case "effect":
                  if (EffectParser.ParseEffects(block.GetContent, out var effects))
                     reform.Effects.AddRange(effects);
                  else
                     Globals.ErrorLog.Write($"Error parsing government reform {reform.Name}: {block.GetContent} is not a valid effect");
                  break;
               case "remove_effect":
                  if (EffectParser.ParseEffects(block.GetContent, out var effects2))
                     reform.RemoveEffects.AddRange(effects2);
                  else
                     Globals.ErrorLog.Write($"Error parsing government reform {reform.Name}: {block.GetContent} is not a valid effect");
                  break;
               case "custom_attributes":
                  reform.CustomAttributes.AddRange(Parsing.GetKeyValueList(block.GetContent));
                  break;
               case "conditional":
                  var conditional = new Conditional();
                  foreach (var conditionalBlock in block.Blocks)
                  {
                     if (conditionalBlock is Block condBlock)
                     {
                        switch (condBlock.Name)
                        {
                           case "allow":
                              // TODO once triggers are implemented
                              break;
                           case "custom_attributes":
                              conditional.Attributes.AddRange(Parsing.GetKeyValueList(condBlock.GetContent));
                              break;
                           case "government_abilities":
                              var lines = Parsing.GetLinesOfString(condBlock.GetContent);
                              conditional.Abilities.AddRange(lines);
                              break;
                           case "states_general_mechanic":
                           case "factions":
                              // TODO once triggers are implemented
                              break;
                           default:
                              Globals.ErrorLog.Write($"Error parsing government reform {reform.Name}: {condBlock.Name} is not a valid block in a conditional");
                              break;
                        }
                     }

                     var kvps = Parsing.GetKeyValueList(block.GetContent);
                     foreach (var kvp in kvps)
                     {
                        if (ReformAttributes.Contains(kvp.Key))
                           conditional.Attributes.Add(kvp);
                     }
                  }
                  reform.Conditionals.Add(conditional);
                  break;

            }
         }
      }

      private static void ParseReformContent(string content, ref GovernmentReform reform)
      {
         var kvps = Parsing.GetKeyValueList(content);
         foreach (var kvp in kvps)
         {
            if (ReformAttributes.Contains(kvp.Key))
               reform.Attributes.Add(kvp);

            if (kvp.Key == "icon")
               reform.Icon = kvp.Value;
         }
      }


      public static readonly HashSet<string> ReformAttributes = 
         [
            "valid_for_new_country ", "allow_convert ", "rulers_can_be_generals ", "heirs_can_be_generals ", "fixed_rank ", "republican_name ", "claim_states ", "religion ", "republic ", "dictatorship ", "is_elective ", "queen ", "heir ", "has_parliament ", "has_devotion ", "has_meritocracy ", "allow_force_tributary ", "duration ", "election_on_death ", "monarchy ", "tribal ", "different_religion_acceptance ", "different_religion_group_acceptance ", "boost_income ", "monastic ", "can_use_trade_post ", "native_mechanic ", "can_form_trade_league ", "free_city ", "is_trading_city ", "trade_city_reform ", "maintain_dynasty ", "allow_migration ", "nation_designer_trigger ", "nation_designer_cost ", "papacy ", "has_harem ", "has_pashas ", "has_janissaries ", "allow_vassal_war ", "allow_vassal_alliance ", "min_autonomy ", "factions_frame ", "factions ", "foreign_slave_rulers ", "royal_marriage ", "nomad ", "raze_province ", "assimilation_cultures ", "states_general_mechanic ", "valid_for_nation_designer ", "allow_normal_conversion ", "force_conversion_gives_global_holy_war_released_modifier ", "start_territory_to_estates ", "has_term_election ", "force_admiral_leader ", "force_general_leader ", "admirals_become_rulers ", "generals_become_rulers ", "allow_banners ", "uses_revolutionary_zeal ", "revolutionary ", "revolutionary_client_state ", "allow_draft_transport_ships ", "free_concentrate_development ", "disallowed_trade_goods ", "can_customise_heir ", "allow_cawa ", "can_change_primary_culture ", "allow_carolean ", "can_inherit_personal_unions ", "block_cultural_union ", "has_cultural_union ", "can_remove_idea_group ", "mercs_do_not_cost_army_professionalism ", "is_eligible_for_hre_emperor "
         ];
   }
}