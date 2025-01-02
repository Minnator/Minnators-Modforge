namespace Editor.Parser
{
   
   public static class ScopeParser
   {
      public enum EffectScopeType
      {
         CountryAndProvince,
         Country,
         Province,
         TradeNode,
         Unit,
         Mission,
         Dynamic,
         Logic,
         Runtime,
         None
      }
      public enum TriggerScopeType
      {
         Country,
         Province,
         TradeNode,
         Mission,
         Removed,
         None
      }
      

      // ------------------------------------ All Runtime Scopes ------------------------------------ //
      private static HashSet<string> RuntimeScopes = [];
      public static void GenerateRuntimeScopes()
      {
         RuntimeScopes = new(Globals.LandProvinces.Count + Globals.Countries.Count); //Prevent too many resizes of the HashSet
         foreach (var id in Globals.Provinces)
            RuntimeScopes.Add(id.Id.ToString());
         
         foreach (var area in Globals.Areas.Keys)
            RuntimeScopes.Add(area);

         foreach (var region in Globals.Regions.Keys)
            RuntimeScopes.Add(region);

         foreach (var superRegion in Globals.SuperRegions.Keys)
            RuntimeScopes.Add(superRegion);

         foreach (var provinceGroup in Globals.ProvinceGroups.Keys)
            RuntimeScopes.Add(provinceGroup);

         foreach (var continent in Globals.Continents.Keys)
            RuntimeScopes.Add(continent);

         /* TODO: Add these
         foreach (var tradeCompany in Globals.TradeCompanies.Keys)
            RuntimeScopes.Add(tradeCompany);

         foreach (var colonialRegion in Globals.ColonialRegions.Keys)
            RuntimeScopes.Add(colonialRegion);
         */

         foreach (var tradeNode in Globals.TradeNodes.Keys)
            RuntimeScopes.Add(tradeNode);

         GenerateCountryScope();
      }

      public static void GenerateCountryScope()
      {
         foreach (var tag in Globals.Countries.Keys)
            RuntimeScopes.Add(tag.ToString());
      }

      public static bool IsRuntimeScope(string scope)
      {
         return RuntimeScopes.Contains(scope);
      }

      public static bool IsAnyScope(string scope)
      {
         return RuntimeScopes.Contains(scope) || IsEffectScopeType(scope, out _) || IsTriggerScopeType(scope, out _);
      }

      // ------------------------------------ All hardcoded scopes ------------------------------------ //

      public static bool IsEffectScopeType(string scope, out EffectScopeType type)
      {
         foreach (var (scopeSet, scopeType) in EffectScopeTypes)
         {
            if (!scopeSet.Contains(scope))
               continue;
            type = scopeType;
            return true;
         }
         type = EffectScopeType.None;
         return false;
      }

      public static bool IsTriggerScopeType(string scope, out TriggerScopeType type)
      {
         foreach (var (scopeSet, scopeType) in TriggerScopeTypes)
         {
            if (!scopeSet.Contains(scope))
               continue;
            type = scopeType;
            return true;
         }
         type = TriggerScopeType.None;
         return false;
      }

      public static bool IsLogicScope(string scope)
      {
         return LogicScopes.Contains(scope);
      }

      public static bool IsAnyTriggerScope(string scope)
      {
         return TriggerScopes.Contains(scope) || ProvinceTriggerScopes.Contains(scope) || TradeNodeTriggerScopes.Contains(scope) || MissionTriggerScopes.Contains(scope) || RemovedTriggerScopes.Contains(scope);
      }



      // Dynamic scopes
      // ROOT, FROM, PREV, THIS

      // Logic scopes
      // AND, OR, NOT
      // if = { limit = { } }
      // if = { limit = { } } else = { } 
      // if = { limit = { } } else_if = { limit = { } }
      
      // Dynamic Scopes
      // Used within the scopes--e.g., ROOT = { }
      private static readonly HashSet<string> DynamicScopes = 
      [
         "ROOT", "FROM", "PREV", "THIS"
      ];

      // Logic Scopes
      // Used within the scopes--e.g., AND = { }
      private static readonly HashSet<string> LogicScopes = 
      [
         "AND", "OR", "NOT", "if", "else", "else_if", "limit"
      ];
      
      // all fixed Country and Province scopes 
      private static readonly HashSet<string> CountryAndProvinceScopes = 
         [
            "emperor", "revolution_target", "crusade_target", "papal_controller", "colonial_parent", "overlord", "capital_scope", "owner", "controller", "sea_zone", "tribal_owner", "most_province_trade_power", "strongest_trade_power"
         ];

      // Effect Scopes Country
      // Used within the country scopes--e.g., FRA = { }
      private static readonly HashSet<string> CountryEffectScopes = 
      [
         "every_ally", "every_coalition_member", "every_country", "every_country_including_inactive", "every_elector", "every_enemy_country", "every_known_country", "every_local_enemy", "every_neighbor_country", "every_rival_country", "every_subject_country", "every_war_enemy_country", "random_ally", "random_coalition_member", "random_country", "random_elector", "random_enemy_country", "random_hired_mercenary_company", "random_known_country", "random_local_enemy", "random_neighbor_country", "random_rival_country", "random_subject_country", "random_war_enemy_country", "every_core_province", "every_heretic_province", "every_owned_province", "every_province", "random_core_province", "random_heretic_province", "random_owned_area", "random_owned_province", "random_active_trade_node", "random_trade_node", "home_trade_node_effect_scope", "every_capital_province", "random_capital_province", "every_claimed_province", "random_claimed_province"
      ];

      // Effect Scopes Province
      // Used within the province scopes--e.g., 123 = { }
      private static readonly HashSet<string> ProvinceEffectScopes =
      [
         "every_empty_neighbor_province", "every_neighbor_province", "every_province_in_state",
         "random_empty_neighbor_province", "random_neighbor_province", "random_province_in_state",
         "random_area_province", "random_province", "every_core_country", "random_core_country", "area", "region"
      ];

      // Effect Scopes TradeNode
      // Used within the trade node scopes--e.g., random_active_trade_node = { }
      private static readonly HashSet<string> TradeNodeEffectScopes = 
      [
         "every_privateering_country", "random_privateering_country", "every_trade_node_member_country", "random_trade_node_member_country", "every_trade_node_member_province", "random_trade_node_member_province"
      ];

      // Effect Scopes Unit
      // Used within unit scopes
      private static readonly HashSet<string> UnitEffectScopes = 
      [
         "unit_owner", "enemy_unit", "location"
      ];

      // Effect Scopes Missions
      // Accessible within mission 
      private static readonly HashSet<string> MissionEffectScopes = 
      [
         "every_target_province", "random_target_province"
      ];

      // Trigger Scopes Country
      // Used within the country scopes--e.g., FRA = { }
      private static readonly HashSet<string> TriggerScopes = 
      [
         "any_army", "all_ally", "all_coalition_member", "all_country", "all_countries_including_self", "all_elector", "all_enemy_country", "all_known_country", "all_local_enemy", "all_neighbor_country", "all_rival_country", "all_subject_country", "all_war_enemy_countries", "all_core_province", "all_heretic_province", "all_owned_province", "all_province", "all_state_province", "all_states", "all_active_trade_node", "all_trade_node", "any_ally", "any_coalition_member", "any_country", "any_elector", "any_enemy_country", "any_known_country", "any_local_enemy", "any_neighbor_country", "any_rival_country", "any_subject_country", "any_war_enemy_country", "any_core_province", "any_heretic_province", "any_hired_mercenary_company", "any_owned_province", "any_province", "any_state", "any_active_trade_node", "any_trade_node", "home_trade_node", "any_great_power", "any_other_great_power", "all_capital_provinces", "any_capital_province", "all_claimed_provinces", "any_claimed_province", "any_tribal_land"
      ];

      // Trigger Scopes Province
      // Used within the province scopes--e.g., 123 = { }
      private static readonly HashSet<string> ProvinceTriggerScopes = 
      [
         "all_empty_neighbor_province", "all_province_in_state", "all_neighbor_province", "any_empty_neighbor_province", "any_province", "any_province_in_state", "any_friendly_coast_border_province", "any_neighbor_province", "all_core_country", "any_core_country", "area_for_scope_province", "region_for_scope_province", "colonial_region_for_scope_province"
      ];

      // Trigger Scopes TradeNode
      // Used within the trade node scopes--e.g., random_active_trade_node = { }
      private static readonly HashSet<string> TradeNodeTriggerScopes = 
      [
         "all_privateering_country", "all_trade_node_member_country", "any_privateering_country", "any_trade_node_member_country", "all_trade_node_member_province", "any_trade_node_member_province"
      ];

      // Trigger Scopes Missions
      // Accessible within mission
      private static readonly HashSet<string> MissionTriggerScopes = 
      [
         "all_target_province", "any_target_province"
      ];

      // Removed Trigger Scopes
      private static readonly HashSet<string> RemovedTriggerScopes =
      [
         "every_trade_node"
      ];

      // Dictionary to map the scopes to their respective types
      private static readonly Dictionary<HashSet<string>, EffectScopeType> EffectScopeTypes = new()
      {
         { CountryAndProvinceScopes, EffectScopeType.CountryAndProvince },
         { CountryEffectScopes, EffectScopeType.Country },
         { ProvinceEffectScopes, EffectScopeType.Province },
         { TradeNodeEffectScopes, EffectScopeType.TradeNode },
         { UnitEffectScopes, EffectScopeType.Unit },
         { MissionEffectScopes, EffectScopeType.Mission },
         { DynamicScopes, EffectScopeType.Dynamic },
         { LogicScopes, EffectScopeType.Logic }
      };

      // Dictionary to map the scopes to their respective types
      private static readonly Dictionary<HashSet<string>, TriggerScopeType> TriggerScopeTypes = new()
      {
         { TriggerScopes, TriggerScopeType.Country },
         { ProvinceTriggerScopes, TriggerScopeType.Province },
         { TradeNodeTriggerScopes, TriggerScopeType.TradeNode },
         { MissionTriggerScopes, TriggerScopeType.Mission },
         { RemovedTriggerScopes, TriggerScopeType.Removed }
      };
   }

   public class IllegalScopeException : Exception
   {
      public IllegalScopeException(string scope) : base($"Illegal scope: {scope}")
      {
      }

      public IllegalScopeException(string scope, string message) : base($"Illegal scope: {scope} - {message}")
      {
      }
   }
}