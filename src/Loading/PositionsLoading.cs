using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Parser;
using Parsing = Editor.Parser.Parsing;

namespace Editor.Loading
{
   
   public static class PositionsLoading
   {

      public static void Load()
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, "map", "positions.txt"))
         {
            Globals.ErrorLog.Write("Could not find positions.txt");
            return;
         }

         Parsing.RemoveCommentFromMultilineString(IO.ReadAllInUTF8(path), out var str);
         var elements = Parsing.GetElements(0, str);

         foreach (var element in elements)
         {
            if (element is not Block block)
            {
               Globals.ErrorLog.Write($"Invalid element in positions.txt: {element}");
               continue;
            }
            if (!int.TryParse(block.Name, out var id))
            {
               Globals.ErrorLog.Write($"Invalid id in positions.txt: {block.Name}");
               continue;
            }
            if (!Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
            {
               Globals.ErrorLog.Write($"Invalid province id in positions.txt: {id}");
               continue;
            }

            var pos = new Positions();

            foreach (var subBlock in block.GetBlockElements)
            {
               if (subBlock.Name.Equals("position"))
               {
                  var points = Parsing.GetPointList(subBlock.GetContent);
                  pos.City = new (points[0].X, Globals.MapHeight - points[0].Y);
                  pos.Text = new (points[2].X, Globals.MapHeight - points[2].Y);
                  pos.Port = new (points[3].X, Globals.MapHeight - points[3].Y);
                  pos.TradeNodeModel = new(points[4].X, Globals.MapHeight - points[4].Y);
                  break;
               }
            }

            province.Positions = pos;
         }
      }

   }
}