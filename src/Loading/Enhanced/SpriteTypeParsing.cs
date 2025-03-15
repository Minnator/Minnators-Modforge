using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static class SpriteTypeParsing
   {
      public static void Load()
      {
         Dictionary<string, SpriteType> sprites = [];

         var files = FilesHelper.GetAllFilesInFolder(searchPattern: "*.gfx", "interface");

         foreach (var file in files)
         {
            var po = PathObj.FromPath(file);

            var (blocks, _) = po.LoadBase(EnhancedParser.FileContentAllowed.BlocksOnly);

            if (blocks.Count != 1)
            {
               _ = new LoadingError(po, "Sprite type file must contain exactly one block!", -1, -1, ErrorType.InvalidFileStructure);
               continue;
            }

            if (blocks[0].Name != "spriteTypes")
            {
               _ = new LoadingError(po, "Sprite type file must contain a block named \"spriteTypes\"!", -1, -1, ErrorType.InvalidFileStructure);
               continue;
            }

            foreach (var spriteTypeBlock in blocks[0].GetSubBlocks(true, po))
               foreach (var element in spriteTypeBlock.ContentElements)
               {
                  var name = string.Empty;
                  string[] internalPath = [];
                  foreach (var line in element.GetLineKvpEnumerator(po, trimQuotes:true))
                  {
                     switch (line.Key)
                     {
                        case "name":
                           name = line.Value;
                           break;
                        case "texturefile":
                           internalPath = line.Value.Split("//");
                           break;
                     }
                  }

                  if (string.IsNullOrEmpty(name))
                  {
                     _ = new LoadingError(po, "Sprite type name is missing!", element.StartLine, -1, ErrorType.MissingObjectDefinition);
                     continue;
                  }

                  if (internalPath.Length == 0)
                  {
                     _ = new LoadingError(po, "Sprite type internal path is missing!", element.StartLine, -1, ErrorType.MissingObjectDefinition);
                     continue;
                  }

                  if (!sprites.TryAdd(name, new (internalPath, name, po.IsModPath))) 
                     _ = new LoadingError(po, $"Sprite type \"{name}\" already exists!", element.StartLine, -1, ErrorType.DuplicateObjectDefinition);
               }
         }

         // TODO for now only using mission icons so this must be called after missions loading
         FilterAllUsedByMissions(sprites);
         sprites.Clear();
         sprites.TrimExcess();


         //var slottedLayout = MissionLayoutEngine.LayoutFile("Pomerania_Missions.txt");
         var slots = MissionLayoutEngine.LayoutSlotsOfFile("Brunei_Missions.txt");

         MissionLayoutEngine.LayoutToImage(slots, MissionView.CompletionType.Completed, MissionView.FrameType.Completed, Globals.Countries["BRA"]);
      }

      public static void FilterAllUsedByMissions(Dictionary<string, SpriteType> sprites)
      {
         foreach (var mission in Globals.Missions.Values)
         {
            if (!sprites.TryGetValue(mission.Icon, out var sprite))
               _ = new LoadingError(PathObj.Empty, $"Mission icon \"{mission.Icon}\" is not defined!", -1, -1, ErrorType.MissingObjectDefinition);
            else
               Globals.SpriteTypes.TryAdd(sprite.Name, sprite);
         }
      }
   }
}