using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using Group = Editor.DataClasses.Group;

namespace Editor.Helper;

public class Block(int start, int end, string name, List<IElement> subBlocks) : IElement
{
   public int Start { get; set; } = start;
   public int End { get; set; } = end;
   public string Name { get; set; } = name;
   public List<IElement> Blocks { get; set; } = subBlocks;
   public bool IsBlock => true;
   public List<Content> GetContentElements
   {
      get
      {
         List<Content> contents = [];
         foreach (var block in Blocks)
         {
            if (block is Content c)
               contents.Add(c);
         }
         return contents;
      }
   }

   public List<Block> GetBlockElements
   {
      get
      {
         List<Block> blocks = [];
         foreach (var block in Blocks)
         {
            if (block is Block b)
               blocks.Add(b);

         }
         return blocks;
      }
   }
}

public class Content(string value) : IElement
{
   public string Value { get; set; } = value;
   public bool IsBlock => false;
}
public interface IElement
{
   public bool IsBlock { get; }
}

public class ParsingException(string message) : Exception(message);

public static class Parsing
{
   private static readonly Regex OpeningRegex = new (@"(?<name>[\w]+)\s*=\s*{");
   private static readonly Regex ClosingRegex = new (@"}");

   /// <summary>
   /// Returns a list of <c>int</c> from a string which are separated by <c>n</c> whitespace chars.
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

   public static List<IElement> GetNestedElementsIterative(int index, ref string str)
   {
      List<IElement> elements = [];

      var openingMatches = OpeningRegex.Matches(str, index);
      var closingMatches = ClosingRegex.Matches(str, index);
      Stack<IElement> stack = [];

      var openedCnt = 0;
      var endCnt = 0;
      foreach (Match openingMatch in openingMatches)
      {
         var start = openingMatch.Index;
         if (endCnt >= closingMatches.Count)
            throw new ParsingException("Could not find closing bracket for opening bracket at index " + start);
         var end = closingMatches[endCnt].Index;

         // While there are closing brackets before the next opening bracket we need to close the current element.
         while (end < start)
         {
            // Get the current element from the stack and update its end.
            endCnt++;
            var element = (Block)stack.Pop();
            element.End = end;

            // if there is content between this closing bracket and the next opening bracket add it as a content element.
            var contentLength = openingMatch.Index - (end + 1);
            var content = str.Substring(end + 1, contentLength).Trim();
            if (!string.IsNullOrWhiteSpace(content))
            {
               element.Blocks.Add(new Content(content));
            }

            if (stack.Count > 0) // If the stack is not empty, the element is a sub element and thus added to the parent element.
            {
               var element2 = (Block)stack.Pop();
               element2.Blocks.Add(element);
               stack.Push(element2);
            }
            else // If the stack is empty, the element is a top level element and thus added to the elements list.
               elements.Add(element);

            // Get the next closing bracket.
            if (endCnt >= closingMatches.Count)
               throw new ParsingException("Could not find closing bracket for opening bracket at index " + start);
            end = closingMatches[endCnt].Index;
         }
         if (end > start)
         {
            var blockElement = new Block(start, end, openingMatch.Groups["name"].Value, []);
            if (openedCnt + 1 < openingMatches.Count && openingMatches[openedCnt + 1].Index < end) 
               // if there is content between this and the next opening bracket without there being a closing bracket add it as a content element.
            {
               var contentStart = start + openingMatch.Length;
               var content = str.Substring(contentStart, openingMatches[openedCnt + 1].Index - contentStart).Trim();
               if (!string.IsNullOrWhiteSpace(content))
               {
                  blockElement.Blocks.Add(new Content(content));
               }
            }
            else // Get the content between the opening bracket and the closing bracket.
            {
               var contentStart = start + openingMatch.Length;
               var content = str.Substring(contentStart, end - contentStart).Trim();
               if (!string.IsNullOrWhiteSpace(content))
               {
                  blockElement.Blocks.Add(new Content(content));
               }
            }
            stack.Push(blockElement);
            openedCnt++;
         }

      }
      return elements;
   }

   public static List<IElement> GetNestedBLocksRecursive(int index, ref string str, out int newEnd)
   {
      List<IElement> elements = [];
      newEnd = index;

      while (true)
      {
         var opening = OpeningRegex.Match(str, index);
         var closingMatch = ClosingRegex.Match(str, index);

         if (!opening.Success)
         {
            if (closingMatch.Success) 
               newEnd = closingMatch.Index;
            return elements;
         }

         var nextOpening = OpeningRegex.Match(str, opening.Index + opening.Length);
         List<IElement> subElements = [];
         var start = opening.Index; 
         var closingIndex = closingMatch.Index;
         int end;


         if (closingIndex < start) 
         {
            newEnd = closingIndex;
            return elements;
         }

         var subString = str.Substring(index, start - index).Trim();
         if (!string.IsNullOrEmpty(subString))
            elements.Add(new Content(subString));

         if (nextOpening.Success && closingIndex > nextOpening.Index) // 
         {
            subElements = GetNestedBLocksRecursive(start + opening.Length, ref str, out newEnd);
            index = end = newEnd + 1;
         }
         else 
         {
            index = closingIndex + 1;
            end = closingIndex;
            var subStr = str.Substring(start + opening.Length, end - start - opening.Length).Trim();
            if (!string.IsNullOrEmpty(subStr))
               subElements.Add(new Content(subStr));
         }
         
         elements.Add(new Block(start, end, opening.Groups["name"].Value, subElements));
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