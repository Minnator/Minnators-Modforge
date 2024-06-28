using System.Collections.Generic;
using System.Text.RegularExpressions;
using Editor.DataClasses;

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

   public static string RemoveCommentFromLine(string line)
   {
      var index = line.IndexOf('#');
      return index == -1 ? line : line.Substring(0, index);
   }

   public static List<string> GetStringList(string value)
   {
      List<string> strList = [];

      var matches = Regex.Matches(value, @"\s*(\w+)");
      foreach (var match in matches) 
         strList.Add(match.ToString().Trim());
      return strList;
   }
}