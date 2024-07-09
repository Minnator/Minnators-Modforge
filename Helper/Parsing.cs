using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses;
using Group = Editor.DataClasses.Group;

namespace Editor.Helper;
#nullable enable
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
   private static readonly Regex OpeningRegex = new (@"(?<name>[A-Za-z0-9_]+)\s*=\s*{", RegexOptions.Compiled);
   private static readonly Regex ClosingRegex = new (@"}", RegexOptions.Compiled);
   private static readonly Regex StringListRegex = new (@"(?:""(?:[^""\\]|\\.)*""|\S+)", RegexOptions.Compiled);
   private static readonly Regex ColorRegex = new (@"(?<r>\d+)\s+(?<g>\d+)\s+(?<b>\d+)", RegexOptions.Compiled);
   private static readonly Regex MonarchNameRegex = new (@"([\p{L} ]+) #(\d+)""\s*=\s*(-?\d+)", RegexOptions.Compiled);

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

   public static List<IElement> GetNestedElementsIterative(int index, string str)
   {
      return GetNestedElementsIterative(index, ref str);
   }
   public static unsafe List<IElement> GetNestedElementsIterative(int index, ref string str)
   {
      var openingMatches = OpeningRegex.Matches(str, index);
      var closingMatches = ClosingRegex.Matches(str, index);

      List<IElement> elements = [];
      ModifiableStack<IElement> stack = new();

      var closingCount = closingMatches.Count;
      var openingCount = openingMatches.Count;

      if (closingCount == 0 && openingCount == 0)
      {
         elements.Add(new Content(str));
         return elements;
      }

      var openedCnt = 0;
      var endCnt = 0;

      for (var i = 0; i <= openingCount; i++)
      {
         // openingMatch is null when there are no more opening brackets, but we need to close the current element.
         var openingMatch = i == openingCount ? null : openingMatches[i];
         var start = openingMatch?.Index ?? int.MaxValue;
         
         if (endCnt >= closingMatches.Count)
            throw new ParsingException("Could not find closing bracket for opening bracket at index " + start);
         var end = closingMatches[endCnt].Index;

         // If there is content before the first opening bracket add it as a content element.
         if (openedCnt == 0 && start > 0)
         {
            var content = str[..start].Trim();
            if (!string.IsNullOrWhiteSpace(content))
               elements.Add(new Content(content));
         }

         // While there are closing brackets before the next opening bracket we need to close the current element.
         while (end < start)
         {
            // Get the current element from the stack and update its end.
            endCnt++;
            var element = (Block)stack.Pop();
            element.End = end;
            var nextEnd = endCnt < closingCount ? closingMatches[endCnt].Index : 0;

            // if there is content between this closing bracket and the next opening bracket add it as a content element.
            if (start != int.MaxValue)
            {
               var content = str[(end + 1)..start].Trim();
               if (content.Contains('}'))
               {
                  content = string.Empty; //TODO why this so fucked
               }
               if (!string.IsNullOrWhiteSpace(content))
               {
                  element.Blocks.Add(new Content(content));
               }
            }
            // if there is content between this and the next closing bracket at it to the current element as a content element.
            else if (endCnt < closingCount && start > nextEnd)
            {
               var content = str[(end + 1)..nextEnd].Trim();
               if (!string.IsNullOrWhiteSpace(content))
               {
                  element.Blocks.Add(new Content(content));
               }
            }

            if (stack.Count > 0) // If the stack is not empty, the element is a sub element and thus added to the parent element.
            {
               ((Block*)stack.PeekRef())->Blocks.Add(element);
            }
            else // If the stack is empty, the element is a top level element and thus added to the elements list.
            {
               elements.Add(element);
               // The stack is empty so there could be content after the last closing bracket to the next opening bracket.
               if (start != int.MaxValue)
               {
                  var content = str[(end + 1)..start].Trim();
                  if (!string.IsNullOrWhiteSpace(content))
                  {
                     elements.Add(new Content(content));
                  }
               }
            }

            if (endCnt >= closingCount)
               break;
            end = nextEnd;
         }

         if (end > start)
         {
            //openingMatch cant be null here as it is only null once start is int.MaxValue
            var blockElement = new Block(start, end, openingMatch!.Groups["name"].Value, []);
            var contentStart = start + openingMatch.Length;
            // if there is content between this and the next opening bracket without there being a closing bracket add it has a content element.
            if (openedCnt + 1 < openingCount)
            {
               var nextIndex = openingMatches[openedCnt + 1].Index;
               if (nextIndex < end)
               {
                  var content = str[contentStart..nextIndex].Trim();
                  if (!string.IsNullOrWhiteSpace(content))
                  {
                     blockElement.Blocks.Add(new Content(content));
                  }
               }
               else
               {
                  var content = str[contentStart..end].Trim();
                  if (content.Contains("133 43 27"))
                     content = string.Empty;
                  if (!string.IsNullOrWhiteSpace(content))
                  {
                     blockElement.Blocks.Add(new Content(content));
                  }
               }
            }
            else // Get the content between the opening bracket and the closing bracket.
            {
               var content = str[contentStart..end].Trim();
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
   [Obsolete]
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
      var inQuotes = false;
      for (var i = 0; i < line.Length; i++)
      {
         switch (line[i])
         {
            case '"':
               inQuotes = !inQuotes;
               break;
            case '#' when !inQuotes:
               return line[..i];
         }
      }
      return line;
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
   public static List<string> GetStringList(ref string value)
   {
      List<string> strList = [];

      var matches = StringListRegex.Matches(value);
      foreach (Match match in matches) 
         strList.Add(match.ToString().Trim());
      return strList;
   }

   // overload in case the string is not passed by referenceable
   public static List<string> GetStringList(string value)
   {
      return GetStringList(ref value);
   }

   public static List<KeyValuePair<string, string>> GetKeyValueList(string value)
   {
      return GetKeyValueList(ref value);
   }

   public static List<KeyValuePair<string, string>> GetKeyValueList(ref string value)
   {
      List<KeyValuePair<string, string>>  keyValueList = [];
      var lines = value.Split('\n');
      foreach (var line in lines)
      {
         var elements = line.Split('=');
         if (elements.Length != 2)
            continue;
         keyValueList.Add(new(elements[0].Trim(), elements[1].Trim()));
      }
      return keyValueList;
   }

   public static void RemoveCommentFromMultilineString(string value)
   {
      RemoveCommentFromMultilineString(ref value);
   }

   public static void RemoveCommentFromMultilineString(ref string str)
   {
      var sb = new StringBuilder();
      var lines = str.Split('\n');
      foreach (var line in lines)
      {
         sb.AppendLine(RemoveCommentFromLine(line));
      }
      str = sb.ToString();
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

   public static Color ParseColor(string str)
   {
      var match = ColorRegex.Match(str);
      if (!match.Success)
         throw new ParsingException("Could not parse color: " + str);

      var r = int.Parse(match.Groups["r"].Value);
      var g = int.Parse(match.Groups["g"].Value);
      var b = int.Parse(match.Groups["b"].Value);

      return Color.FromArgb(r, g, b);
   }

   public static void ParseMonarchNames(string input, out List<MonarchName> names)
   {
      names = [];
      var matches = MonarchNameRegex.Matches(input);

      foreach (Match match in matches)
      {
         var name = match.Groups[1].Value;
         var ordinal = int.Parse(match.Groups[2].Value);
         var chance = int.Parse(match.Groups[3].Value);

         names.Add(new (name, ordinal, chance));
      }
   }
}


public class ProvinceParsingException(int index, string value) : Exception($"Could not parse province at index {index}: {value}");