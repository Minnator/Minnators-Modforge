using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Editor.Helper;

public static class Parsing
{

   public static List<int> GetProvincesList(string str)
   {
      List<int> idList = [];

      var matches = Regex.Matches(str, @"\s*(\d+)");
      foreach (var match in matches)
      {
         if (int.TryParse(match.ToString(), out var value))
            idList.Add(value);
      }
      return idList;
   }

}