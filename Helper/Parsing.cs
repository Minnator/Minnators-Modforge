using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Group = Editor.DataClasses.Group;

namespace Editor.Helper;

public struct Block(int start, int end, string name, string value, List<Block> subBlocks)
{
   public int Start { get; set; } = start;
   public int End { get; set; } = end;
   public string Name { get; set; } = name;
   public string Value { get; set; } = value;
   public List<Block> Blocks { get; set; } = subBlocks;

   public int GetOutermostEnd()
   {
      return Blocks.Count == 0 ? End : Blocks[Blocks.Count - 1].GetOutermostEnd();
   }
}

public class ParsingException(string message) : Exception(message);

public static class Parsing
{
   private static readonly Regex OpeningRegex = new (@"(?<name>[\w]+)\s*=\s*{");
   private static readonly Regex ClosingRegex = new (@"}");
   private static int _blocksCount = 0;

   /// <summary>
   /// Returns a list of <c>int</c> from a string which are seperated by <c>n</c> whitespace chars.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static List<int> GetIntListFromString(string str)
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

   public static List<Block> GetNestedBLocks(int index, ref string str)
   {
      List<Block> blocks = [];
      
      while (true)
      {
         var match = OpeningRegex.Match(str, index);
         var closingMatch = ClosingRegex.Match(str, index);

         if (!match.Success)
            return blocks;
         _blocksCount ++;
         if (_blocksCount % 10 == 0)
            Console.WriteLine(_blocksCount);
         var name = match.Groups["name"].Value;
         var start = match.Index;
         List<Block> subBlocks = [];
         if (closingMatch.Index > start)
            subBlocks = GetNestedBLocks(start + match.Length, ref str);
         var end = closingMatch.Index;
         var content = str.Substring(start, end - start + 1);
         blocks.Add(new (start, end, name, content, subBlocks));
      }
   }

   public static int GetFirstClosingBracket(int index, ref string str)
   {
      return str.IndexOf('}', index);
   }

   /// <summary>
   /// Returns the index of the closing bracket of the first opening bracket in the string.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
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

   /// <summary>
   /// Returns the index of the closing bracket of the first opening bracket in the string after position <c>openingBracketIndex</c>.
   /// </summary>
   /// <param name="str"></param>
   /// <param name="openingBracketIndex"></param>
   /// <returns></returns>
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

   /// <summary>
   /// Returns the <c>index</c> of the first opening bracket and the <c>index</c> of the closing bracket of the first opening bracket in the string after index <c>index</c>.
   /// </summary>
   /// <param name="str"></param>
   /// <param name="index"></param>
   /// <returns></returns>
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

   /// <summary>
   /// Returns the <c>comment</c> of the line after the <c>index</c> and the <c>index</c> of the line ending.
   /// </summary>
   /// <param name="index"></param>
   /// <param name="str"></param>
   /// <param name="comment"></param>
   /// <returns></returns>
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

   /// <summary>
   /// Returns a list of <c>string</c> from a string which are separated by whitespace.
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   public static List<string> GetStringList(string value)
   {
      List<string> strList = [];

      var matches = Regex.Matches(value, @"\s*(\w+)");
      foreach (var match in matches) 
         strList.Add(match.ToString().Trim());
      return strList;
   }

   /// <summary>
   /// Returns a list of <c>Group</c> from a string.
   /// </summary>
   /// <param name="value"></param>
   /// <param name="index"></param>
   /// <param name="last"></param>
   /// <returns></returns>
   /// <exception cref="ProvinceParsingException"></exception>
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

   /// <summary>
   /// Parses a string to a <c>bool</c> value if it is <c>"yes"</c> or <c>"no"</c>.
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   public static bool YesNo (string value)
   {
      return value.ToLower().Equals("yes");
   }
}


public class ProvinceParsingException(int index, string value) : Exception($"Could not parse province at index {index}: {value}");