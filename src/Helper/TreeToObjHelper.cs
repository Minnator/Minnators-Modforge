using Antlr4.Runtime.Tree;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Saving;
using static Editor.Helper.TreeContextHelper;
using static PDXLanguageParser;

namespace Editor.Helper
{
   public static class TreeToObjHelper
   {

      public static Area GetAreaFromContext()
      {
         return Area.Empty;
      }

      public static Color GetColorFromContext(PDXLanguageParser.ColorContext? cc, bool generateIfNull = true)
      {
         if (cc == null)
            return Globals.ColorProvider.GetRandomColor();
         var ints = GetIntFromContext(cc.INT());
         return Color.FromArgb(ints[0], ints[1], ints[2]);
      }



      public static (List<Province>, Color) GetProvincesFromContext(IntListContext? ic)
      {
         if (ic == null)
            return ([], Color.Empty);
         var provinces = new List<Province>();
         var color = Color.Empty;

         if (ic.INT() != null)
         {
            var ints = GetIntFromContext(ic.INT());
            for (var i = 0; i < ints.Length; i += 3)
            {
               if (!Globals.ProvinceIdToProvince.TryGetValue(ints[i], out var prov))
               {
                  _ = new LoadingError(PathObj.Empty, "Can not resolve province id!", ErrorType.SyntaxError, ic);
                  continue;
               }
               provinces.Add(prov);
            }
         }
         else
         {
            var col = ic.color();
            if (col == null)
               return (provinces, color);
            if (col.Length > 1) 
               _ = new LoadingError(PathObj.Empty, "Color already set in area!", ErrorType.TODO_ERROR, ic);
            color = GetColorFromContext(col[0]);
         }
         return (provinces, color);
      }

   }
}