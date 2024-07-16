using System.Security.Policy;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;

namespace Editor.DataClasses.ConsoleCommands
{
   public class Dump : ConsoleCommand
   {
      public override string Command { get; } = "dump";
      public override string Description { get; } = "Dumps the current state of the given objects to a file.";
      public override string Help { get; } = "dump <object> <[optional]folderPath>";
      public override string[] Aliases { get; } = [];
      private readonly HashSet<string> _validObjects = [
         "provinces", "countries", "regions", "tradeGoods", "religions", "cultures", "tags", "history", "tradegoods", "tradenodes"
      ];
      public override void Execute(string[] args)
      {
         var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
         if (args.Length == 2)
         {
            if (!_validObjects.Contains(args[1]))
            {
               Output.WriteError($"Invalid object [{args[1]}]; can not dump!");
               return;
            }
            // Get the download folder of the user
            DumpObject(args[1], defaultPath);
            Output.WriteGoodFeedback($"Dumped to {defaultPath}");
         }
         else if (args.Length == 3)
         {
            if (!_validObjects.Contains(args[1]))
            {
               Output.WriteError($"Invalid object [{args[1]}]; can not dump!");
               return;
            }

            if (Directory.Exists(args[2]))
            {
               DumpObject(args[1], args[2]);
               Output.WriteGoodFeedback($"Dumped to {args[2]}");
            }
            else
            {
               Output.WriteError($"Invalid path [{args[2]}]; Dumping to default: {defaultPath}");
               DumpObject(args[1], defaultPath);
               Output.WriteGoodFeedback($"Dumped to {defaultPath}");
            }
         }
         else if (args.Length == 4)
         {
            if (!_validObjects.Contains(args[1]))
            {
               Output.WriteError($"Invalid object [{args[1]}]; can not dump!");
               return;
            }

            if (!int.TryParse(args[2], out var id))
            {
               Output.WriteError($"Invalid province id [{args[1]}]; can not dump!");
               return;
            }
            if (Directory.Exists(args[3]))
            {
               if (Globals.Provinces.TryGetValue(id, out var province))
               {
                  province.DumpHistory(args[3]);
               }
               Output.WriteGoodFeedback($"Dumped to {args[2]}");
            }
            else
            {
               Output.WriteError($"Invalid path [{args[3]}]; Dumping to default: {defaultPath}\n");
               if (Globals.Provinces.TryGetValue(id, out var province))
               {
                  province.DumpHistory(defaultPath);
               }
               Output.WriteGoodFeedback($"Dumped to {defaultPath}");
            }
         }
         else
         {
            Output.WriteError("Invalid number of arguments; can not dump!");
         }

      }

      private void DumpObject(string obj, string path)
      {
         switch (obj)
         {
            case "provinces":
               //DumpProvinces(path);
               break;
            case "countries":
               //DumpCountries(path);
               break;
            case "regions":
               //DumpRegions(path);
               break;
            case "tradenodes":
               TradeNodeHelper.DumpTradeNodes(path);
               break;
            case "religions":
               //DumpReligions(path);
               break;
            case "cultures":
               //DumpCultures(path);
               break;
            case "tags":
              // DumpTags(path);
               break;
            case "history":
               //DumpHistory(path);
               break;
            case "tradegoods":
               TradeGoodHelper.DumpTradeGoods(path);
               break;
         }
      }
   }
}