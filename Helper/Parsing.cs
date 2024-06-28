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

   public static int GetClosingBracketIndex(ref string str)
   {
      var bracketCount = 0;

      for (var index = 0; index < str.Length; index++)
      {
         var c = str[index];

         if (c == '{')
            bracketCount++;
         else if (c == '}') 
            bracketCount--;

         if (bracketCount == 0)
            return index;
      }

      return -1;
   }


   public static int GetClosingBracketIndex(ref string str, int openingBracketIndex)
   {
      var bracketCount = 0;

      for (var index = openingBracketIndex; index < str.Length; index++)
      {
         var c = str[index];

         if (c == '{')
            bracketCount++;
         else if (c == '}')
            bracketCount--;

         if (bracketCount == 0)
            return index;
      }

      return -1;
   }


   public static (int, int) FindOpeningBracketAndClosingBracket(ref string str, int index)
   {
      var bracketCount = 0;
      for (var i = index; i < str.Length; i++)
      {
         if (str[i] == '{')
         {
            if (bracketCount == 0)
               return (index, GetClosingBracketIndex(ref str, index));
            bracketCount++;
         }
         else if (str[i] == '}')
            bracketCount--;

         index++;
      }

      return (-1, -1);
   }

   public static int GetLineEndingAfterComment(int index, ref string str)
   {
      for (var i = index; i < str.Length; i++)
      {
         if (str[i] == '\n')
            return i;
      }
      return -1;
   }

   public static string RemoveCommentFromLine(string line)
   {
      var index = line.IndexOf('#');
      return index == -1 ? line : line.Substring(0, index);
   }

   public static (string, string) RemoveAndGetCommentFromString(string str)
   {
      var index = str.IndexOf('#');
      return index == -1 ? (str, "") : (str.Substring(0, index), str.Substring(index + 1));
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