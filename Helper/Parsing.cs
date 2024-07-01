using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Group = Editor.DataClasses.Group;

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

   public static int GetLineEndingAfterComment(int index, ref string str, out string comment)
   {
      comment = string.Empty;
      for (var i = index; i < str.Length; i++)
      {
         if (str[i] == '\n')
         {
            comment = str.Substring(index, i - index);
            return i;
         }
      }
      return -1;
   }

   public static string RemoveCommentFromLine(string line)
   {
      var index = line.IndexOf('#');
      return index == -1 ? line : line.Substring(0, index);
   }

   public static string GetCommentFromLine(string line)
   {
      var index = line.IndexOf('#');
      return index == -1 ? "" : line.Substring(index + 1);
   }

   public static string RemoveAndGetCommentFromString(string str)
   {
      var index = str.IndexOf('#');
      return index == -1 ? str : str.Substring(0, index);
   }

   public static List<string> GetStringList(string value)
   {
      List<string> strList = [];

      var matches = Regex.Matches(value, @"\s*(\w+)");
      foreach (var match in matches) 
         strList.Add(match.ToString().Trim());
      return strList;
   }

   public static List<Group> GetGroups(ref string value, int index, out int last)
   {
      List<Group> groups = [];
      Stack<int> stack = [];
      last = index;
      var len = value.Length;

      for (var i = index; i < len; i++)
      {
         var c = value[i];
         if (c == '{')
            stack.Push(i);
         else if (c == '}')
         {
            if (stack.Count == 0)
               throw new ProvinceParsingException(i, value);

            var start = stack.Pop();
            groups.Add(new (start, i, value.Substring (start, i - start + 1)));
            if (stack.Count == 0)
            {
               last = i;
               break;
            }
         }
      }
      if (stack.Count > 0)
         throw new ProvinceParsingException(len, value);
      return groups;
   }

   public static bool YesNo (string value)
   {
      return value.ToLower().Equals("yes");
   }
}


public class ProvinceParsingException(int index, string value) : Exception($"Could not parse province at index {index}: {value}");