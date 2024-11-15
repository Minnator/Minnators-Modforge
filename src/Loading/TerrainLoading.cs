using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;

namespace Editor.Loading;

[Loading]
public static class TerrainLoading
{
   public static void Load()
   {
      var sw = Stopwatch.StartNew();
      if (!FilesHelper.GetFilePathUniquely(out var path, "map", "terrain.txt"))
      {
         Globals.ErrorLog.Write("Failed to find terrain.txt");
         return;
      }

      Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
      var elements = Parsing.GetElements(0, content);
      List<Terrain> terrains = [];
      TerrainDefinitions terrainDefinitions = new();
      TreeDefinitions treeDefinitions = new();

      foreach (var element in elements)
      {
         if (element is not Block block)
         {
            Globals.ErrorLog.Write($"TerrainLoading: Element is not a block: {element}");
            continue;
         }

         switch (block.Name)
         {
            case "categories":
               ParseCategoriesBlock(block, ref terrains);
               break;
            case "terrain":
               ParseTerrainBlock(block, ref terrainDefinitions);
               break;
            case "tree":
               ParseTreeBlocks(block, ref treeDefinitions);
               break;
            default:
               Globals.ErrorLog.Write($"TerrainLoading: Unknown block: {block.Name}");
               break;
         }
      }

      sw.Stop();
      Globals.LoadingLog.WriteTimeStamp("Terrain.txt", sw.ElapsedMilliseconds);
   }

   private static void ParseCategoriesBlock(Block block, ref List<Terrain> terrains)
   {
      foreach (var blk in block.GetBlockElements)
      {
         Terrain terrain = new(blk.Name);
         ParseTerrainContent(blk.GetContent, ref terrain);

         foreach (var innerBlock in blk.GetBlockElements)
         {
            switch (innerBlock.Name)
            {
               case "color":
                  if (!Parsing.TryParseColor(innerBlock.GetContent, out var color))
                  {
                     Globals.ErrorLog.Write($"Failed to parse color: {innerBlock.GetContent} in {terrain.Name}");
                     continue;
                  }
                  terrain.Color = color;
                  break;
               case "terrain_override":
                  foreach (var id in Parsing.GetIntListFromString(innerBlock.GetContent))
                  {
                     if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
                     {
                        Globals.ErrorLog.Write($"Failed to find province: {id} in {terrain.Name} in terrain_override");
                        continue;
                     }
                     terrain.TerrainOverrides.Add(province);
                  }
                  break;
            }
         }
      }

   }

   private static void ParseTerrainContent(string content, ref Terrain terrain)
   {
      var kvps = Parsing.GetKeyValueList(ref content);
      if (kvps.Count == 0)
         return;

      foreach (var kvp in kvps)
      {
         switch (kvp.Key)
         {
            case "type":
               terrain.Type = kvp.Value;
               break;
            case "sound_type":
               terrain.SoundType = kvp.Value;
               break;
            case "defence":
               if (!int.TryParse(kvp.Value, out var defence))
               {
                  Globals.ErrorLog.Write($"Failed to parse defence: {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.DefenceBonus = defence;
               break;
            case "movement_cost":
               if (!float.TryParse(kvp.Value, out var movementCost))
               {
                  Globals.ErrorLog.Write($"Failed to parse movement_cost: {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.MovementCostMultiplier = movementCost;
               break;
            case "nation_designer_cost_multiplier":
               if (!float.TryParse(kvp.Value, out var nationDesignerCostMultiplier))
               {
                  Globals.ErrorLog.Write($"Failed to parse nation_designer_cost_multiplier: {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.NationDesignerCostMultiplier = nationDesignerCostMultiplier;
               break;
            case "inland_sea":
               if (!Parsing.YesNo(kvp.Value))
               {
                  Globals.ErrorLog.Write($"Failed to parse inland_sea: {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.IsInlandSea = true;
               break;
            case "is_water":
               if (!Parsing.YesNo(kvp.Value))
               {
                  Globals.ErrorLog.Write($"Failed to parse is_water: {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.IsWater = true;
               break;
            default:
               if (!ModifierParser.ParseModifierFromName(kvp.Key, kvp.Value, out var modifier))
               {
                  Globals.ErrorLog.Write($"Failed to parse modifier: {kvp.Key} = {kvp.Value} in {terrain.Name}");
                  continue;
               }
               terrain.Modifiers.Add(modifier);
               break;
         }
      }
   }

   private static void ParseTreeBlocks(Block block, ref TreeDefinitions treeDefinitions)
   {
      
   }

   private static void ParseTerrainBlock(Block block, ref TerrainDefinitions terrainDefinitions)
   {

   }
}