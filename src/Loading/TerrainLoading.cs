using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Parser;
using Editor.Saving;
namespace Editor.Loading;


public static class TerrainLoading
{
   public static void Load()
   {
      if (!FilesHelper.GetFilePathUniquely(out var path, "map", "terrain.txt"))
      {
         Globals.ErrorLog.Write("Failed to find terrain.txt");
         return;
      }

      Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var content);
      var elements = Parsing.GetElements(0, content);

      var pathObj = PathObj.FromPath(path);

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
               ParseCategoriesBlock(block, ref pathObj);
               break;
            case "terrain":
               ParseTerrainBlock(block, ref Globals.TerrainDefinitions);
               break;
            case "tree":
               ParseTreeBlocks(block, ref Globals.TreeDefinitions);
               break;
            default:
               Globals.ErrorLog.Write($"TerrainLoading: Unknown block: {block.Name}");
               break;
         }
      }
   }

   private static void ParseCategoriesBlock(Block block, ref PathObj path)
   {
      foreach (var blk in block.GetBlockElements)
      {
         Terrain terrain = new(blk.Name, Color.Empty, ref path);
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
                     if (province.Terrain != Terrain.Empty)
                     {
                        _ = new LoadingError(path, "Province is assigned to more than one terrain_override!");
                        continue;
                     }
                     terrain.Add(province);
                     province.Terrain = terrain;
                  }
                  break;
            }
         }

         Globals.Terrains.Add(terrain.Name, terrain);
      }
      SaveMaster.AddRangeToDictionary(path, Globals.Terrains.Values);
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
      if (block.GetContent != string.Empty)
      {
         Globals.ErrorLog.Write($"Tree block has content forbidden: {block.GetContent}");
         return;
      }

      foreach (var blk in block.GetBlockElements)
      {
         string name;
         string terrain;
         byte[] colorIndex;

         name = blk.Name;

         var kvps = Parsing.GetKeyValueList(blk.GetContent);
         if (kvps.Count != 1)
         {
            Globals.ErrorLog.Write($"Failed to parse tree block: {blk.GetContent}");
            continue;
         }
         terrain = kvps[0].Value;

         if (blk.GetBlockElements.Count != 1)
         {
            Globals.ErrorLog.Write($"Failed to parse tree block, unexpected: {blk.GetContent}");
            continue;
         }

         colorIndex = Parsing.GetByteListFromString(blk.GetBlockElements[0].GetContent).ToArray();
         // find the terrain with equal name
         if (!Globals.Terrains.TryGetValue(terrain, out var ter))
            ter = Terrain.Empty;
         
         treeDefinitions.AddDefinition(name, ter, colorIndex);


      }
   }

   private static void ParseTerrainBlock(Block block, ref TerrainDefinitions terrainDefinitions)
   {
      if (block.GetContent != string.Empty)
      {
         Globals.ErrorLog.Write($"Terrain block has content forbidden: {block.GetContent}");
         return;
      }

      foreach (var blk in block.GetBlockElements)
      {
         string name;
         string type;
         byte colorIndex;

         name = blk.Name;

         var kvps = Parsing.GetKeyValueList(blk.GetContent);
         if (kvps.Count != 1)
         {
            Globals.ErrorLog.Write($"Failed to parse terrain block: {blk.GetContent}");
            continue;
         }
         type = kvps[0].Value;

         if (blk.GetBlockElements.Count != 1)
         {
            Globals.ErrorLog.Write($"Failed to parse terrain block, unexpected: {blk.GetContent}");
            continue;
         }

         if (!byte.TryParse(blk.GetBlockElements[0].GetContent, out colorIndex))
         {
            Globals.ErrorLog.Write($"Failed to parse color index: {blk.GetBlockElements[0].GetContent}");
            continue;
         }

         if (!Globals.Terrains.TryGetValue(type, out var ter))
            ter = Terrain.Empty;

         terrainDefinitions.AddDefinition(name, ter, colorIndex);
      }
   }
}