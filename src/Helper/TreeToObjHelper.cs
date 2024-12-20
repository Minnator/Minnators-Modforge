using Antlr4.Runtime.Tree;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Saving;
using static Editor.Helper.TreeContextHelper;

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



      public static List<Province> GetProvincesFromContext(params PDXLanguageParser.IntListContext[] ic)
      {
         var provinces = new List<Province>();
         foreach (var intListContext in ic)
         {
            var ints = GetIntFromContext(intListContext.INT());
            for (var i = 0; i < ints.Length; i += 3)
            {
               if (!Globals.ProvinceIdToProvince.TryGetValue(ints[i], out var prov))
               {
                  _ = new LoadingError(PathObj.Empty, "Can not resolve province id!", ErrorType.SyntaxError, intListContext);
                  continue;
               }
               provinces.Add(prov);
            }
         }
         return provinces;
      }

   }
}