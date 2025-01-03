using Editor.ErrorHandling;

namespace Editor.Loading.Enhanced
{
   public static class DefaultMapLoading
   {
      public static void Load()
      {
         var (blocks, elements) = EnhancedParser.LoadBase(EnhancedParser.FileContentAllowed.Both, out var po, "map", "default.map");

         var attributes = EnhancedParsing.GetAttributesFromContentElements(elements, po, "max_provinces", "width", "height");

         var maxProvinces = EnhancedParser.Convert<int>(attributes[0], po);
         Globals.MapWidth = EnhancedParser.Convert<int>(attributes[1], po);
         Globals.MapHeight = EnhancedParser.Convert<int>(attributes[2], po);

         foreach (var province in Globals.Provinces)
         {
            if (province.Id >= maxProvinces) 
               _ = new LoadingError(po, $"Province {province.Id} is higher than the max_provinces value of {maxProvinces} in default.map!", 0, 0, ErrorType.InvalidProvinceId);
         }

         Globals.SeaProvinces.Clear();
         Globals.RNWProvinces.Clear();
         Globals.LakeProvinces.Clear();
         Globals.LandProvinces.Clear();
         Globals.CoastalProvinces.Clear();

         foreach (var block in blocks)
         {
            switch (block.Name)
            {
               case "sea_starts":
                  Globals.SeaProvinces = EnhancedParsing.GetProvincesFromContent(block.ContentElements, po);
                  break;
               case "only_used_for_random":
                  Globals.RNWProvinces = EnhancedParsing.GetProvincesFromContent(block.ContentElements, po);
                  break;
               case "lakes":
                  Globals.LakeProvinces = EnhancedParsing.GetProvincesFromContent(block.ContentElements, po);
                  break;
               case "force_coastal":
                  Globals.CoastalProvinces = EnhancedParsing.GetProvincesFromContent(block.ContentElements, po);
                  break;
               case "canal_definition":
               case "tree":
                  break;
               default:
                  _ = new LoadingError(po, $"Unexpected block \"{block.Name}\" in default.map!", block.StartLine, 0, ErrorType.UnexpectedBlockElement);
                  break;
            }
         }

         foreach (var p in Globals.Provinces)
         {
            if (Globals.SeaProvinces.Contains(p) || Globals.LakeProvinces.Contains(p))
            {
               Globals.NonLandProvinces.Add(p);
               continue;
            }
            if (Globals.SeaProvinces.Contains(p) || Globals.LakeProvinces.Contains(p))
               continue;
            Globals.LandProvinces.Add(p);
         }

         foreach (var p in Globals.RNWProvinces)
         {
            Globals.SeaProvinces.Remove(p);
            Globals.LakeProvinces.Remove(p);
            Globals.CoastalProvinces.Remove(p);
            Globals.LandProvinces.Remove(p);
         }
      }
   }
}