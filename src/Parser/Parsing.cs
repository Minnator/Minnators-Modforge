using System.Diagnostics;
using System.Globalization;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Editor.DataClasses.DataStructures;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;

namespace Editor.Parser;

public class ParsingException(string message) : Exception(message);
// antlr4 -Dlanguage=CSharp Grammar.g4
public static partial class Parsing
{
   #region Regexes

   private static readonly Regex SameLineValuesRegex = SameLineValuesRegexGenerate();
   private static readonly Regex OpeningRegex = OpeningRegexGenerate();
   private static readonly Regex ClosingRegex = ClosingRegexGenerate();
   private static readonly Regex StringListRegex = StringListRegexGenerate();
   private static readonly Regex ColorRegex = ColorRegexGenerate();
   private static readonly Regex MonarchNameRegex = MonarchNameRegexGenerate();
   private static readonly Regex KeyValueRegex = KeyValueRegexGenerate();
   private static readonly Regex FullDateParseRegex = FullDateParseRegexGenerate();
   private static readonly Regex PointFRegex = PointFRegexGenerate();
   private static readonly Regex PointRegex = PointRegexGenerate();
   private static readonly Regex QuoteStringRegex = QuoteStringRegexGenerate();
   public static readonly Regex RegnalRegex = RegnalRegexGenerate();

   [GeneratedRegex(@"#(?<num>\d+)", RegexOptions.Compiled)]
   private static partial Regex RegnalRegexGenerate();
   [GeneratedRegex(@"\""(.*)\""", RegexOptions.Compiled)]
   private static partial Regex QuoteStringRegexGenerate();
   // Generate Regexes during compile time
   [GeneratedRegex(@"[\""""].+?[\""""]|\S+", RegexOptions.Compiled)]
   private static partial Regex SameLineValuesRegexGenerate();
   [GeneratedRegex(@"(?<x>-?\d+\.\d+)\s+(?<y>-?\d+\.\d+)", RegexOptions.Compiled)]
   private static partial Regex PointFRegexGenerate();
   [GeneratedRegex(@"(?<x>\d+).\d+\s+(?<y>\d+).\d+", RegexOptions.Compiled)]
   private static partial Regex PointRegexGenerate();
   [GeneratedRegex(@"(?<year>\d{1,4})-(?<month>\d{1,2})-(?<day>\d{1,2})", RegexOptions.Compiled)]
   private static partial Regex FullDateParseRegexGenerate();
   [GeneratedRegex(@"(?<name>[A-Za-z0-9_.$]+)\s*=\s*{", RegexOptions.Compiled)]
   private static partial Regex OpeningRegexGenerate();
   [GeneratedRegex(@"}", RegexOptions.Compiled)]
   private static partial Regex ClosingRegexGenerate();
   [GeneratedRegex(@"(?:""(?:[^""\\]|\\.)*""|\S+)", RegexOptions.Compiled)]
   private static partial Regex StringListRegexGenerate();
   [GeneratedRegex(@"(?<r>\d+)\s+(?<g>\d+)\s+(?<b>\d+)", RegexOptions.Compiled)]
   private static partial Regex ColorRegexGenerate();
   [GeneratedRegex(@"""(?<name>.+)""\s*=\s*(?<chance>[\d-]+)", RegexOptions.Compiled)]
   private static partial Regex MonarchNameRegexGenerate();
   [GeneratedRegex(@"(?<key>[A-Za-z_0-9-.]+)\s*=\s*""?(?<value>[A-Za-z_0-9-.]+)""?", RegexOptions.Compiled)]
   private static partial Regex KeyValueRegexGenerate();


   #endregion

   /// <summary>
   /// Parses a date in any given format that can occur in the game files, which is valid
   /// </summary>
   /// <param name="dateString"></param>
   /// <param name="dateValue"></param>
   /// <returns></returns>
   public static bool TryParseDate(string dateString, out Date dateValue)
   {
      return Date.TryParse(dateString, out dateValue).Ignore();
   }

   /// <summary>
   /// Returns a list of <c>int</c> from a string which are separated by <c>n</c> whitespace chars.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static List<int> GetIntListFromString(string str)
   {
      List<int> idList = [];

      var matches = IntListRegex().Matches(str);
      foreach (var match in matches)
      {
         if (int.TryParse(match.ToString(), out var value))
            idList.Add(value);
      }
      return idList;
   }

   /// <summary>
   /// Returns a list of <c>byte</c> from a string which are separated by <c>n</c> whitespace chars.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static List<byte> GetByteListFromString(string str)
   {
      List<byte> idList = [];

      var matches = ByteListRegex().Matches(str);
      foreach (var match in matches)
         if (byte.TryParse(match.ToString(), out var value))
            idList.Add(value);
      return idList;
   }

   public static List<string> GetLinesOfString(ref string str)
   {
      var lines = str.Split('\n');
      return RemoveAllEmptyLines(lines);
   }

   public static List<string> GetLinesOfString(string str)
   {
      return GetLinesOfString(ref str);
   }

   public static List<Province> GetProvincesFromString(string str)
   {
      var ids = GetIntListFromString(str);
      List<Province> provinces = [];
      foreach (var id in ids)
      {
         if (Globals.ProvinceIdToProvince.TryGetValue(id, out var province))
            provinces.Add(province);
      }
      return provinces;
   }

   /// <summary>
   /// Returns a list of <c>string</c> from a string which are separated by whitespace or newline and in quotes.
   /// </summary>
   /// <param name="input"></param>
   /// <returns></returns>
   public static List<string> GetQuotedStringList(string input)
   {
      List<string> strList = [];
      var matches = QuoteStringRegex.Matches(input);
      foreach (Match match in matches)
         strList.Add(match.ToString().Trim());
      return strList;
   }

   /// <summary>
   /// Returns a list of <c>IElement</c> from a string which contains nested blocks can content elements with the format <c>"Block.name = { Block.Content; Block.Blocks }"</c>.
   /// </summary>
   /// <param name="index"></param>
   /// <param name="str"></param>
   /// <returns></returns>
   public static List<IElement> GetElements(int index, string str)
   {
      return GetElements(index, ref str);
   }

   /// <summary>
   /// Returns a list of <c>IElement</c> from a string which contains nested blocks can content elements with the format <c>"Block.name = { Block.Content; Block.Blocks }"</c>.
   /// </summary>
   /// <param name="index"></param>
   /// <param name="str"></param>
   /// <returns></returns>
   /// TODO: improve performance by rewrite and rethink the logic
   public static unsafe List<IElement> GetElements(int index, ref string str)
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
            if(start == int.MaxValue)
               throw new ParsingException($"Missing closing bracket at end of file!");
            else
               throw new ParsingException($"Could not find closing bracket for opening bracket {start}");
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
            if (stack.Count == 0)
               break;
            // Get the current element from the stack and update its end.
            endCnt++;
            var element = (Block)stack.Pop();
            element.End = end;
            var nextEnd = endCnt < closingCount ? closingMatches[endCnt].Index : 0;
            Content? contentToNextOpening = null!;

            // if there is content between this closing bracket and the next opening bracket add it as a content element.
            if (start != int.MaxValue && stack.Count > 0)
            {
               string content;
               if (end < start && nextEnd < start && end < nextEnd)
                  content = str[(end + 1)..nextEnd].Trim();
               else
                  content = str[(end + 1)..start].Trim();

               if (content.Contains('}'))
               {
                  content = content.Replace('}', ' ').Trim();
               }
               if (!string.IsNullOrWhiteSpace(content))
               {
                  contentToNextOpening = new (content);
                  //element.Blocks.Add(new Content(content));
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
               ((Block*)stack.PeekRef()) -> Blocks.Add(element);
               if (contentToNextOpening != null!)
                  ((Block*)stack.PeekRef()) -> Blocks.Add(contentToNextOpening);
            }
            else // If the stack is empty, the element is a top level element and thus added to the elements list.
            {
               elements.Add(element);
               // The stack is empty so there could be content after the last closing bracket to the next opening bracket.
               if (start != int.MaxValue && stack.Count != 0)
               {
                  var content = str[(end + 1)..start].Trim();
                  if (!string.IsNullOrWhiteSpace(content))
                  {
                     elements.Add(new Content(content));
                  }
               }
               else if (start != int.MaxValue && stack.Count == 0)
               {
                  var content = str[(end + 1)..start].Trim();
                  if (!string.IsNullOrWhiteSpace(content))
                  {
                     elements.Add(new Content(content));
                  }
               }
            }

            // Get any content after the last closing bracket and the EOF.
            if (endCnt >= closingCount)
            {
               var content = str[(end + 1)..].Trim();
               if (!string.IsNullOrWhiteSpace(content))
               {
                  elements.Add(new Content(content));
               }
               break;
            }

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
   
   /// <summary>
   /// Removes a comment indicated by a <c>#</c> from a line.
   /// </summary>
   /// <param name="line"></param>
   /// <returns></returns>
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

   public static List<string> RemoveAllEmptyLines(ICollection<string> lines)
   {
      List<string> nonEmptyLines = [];
      foreach (var line in lines)
      {
         if (!string.IsNullOrWhiteSpace(line))
            nonEmptyLines.Add(line);
      }
      return nonEmptyLines;
   }

   /// <summary>
   /// Returns a list of <c>string</c> from a string which are separated by whitespace.
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   public static List<string> GetStringList(string value)
   {
      return GetStringList(ref value);
   }

   public static string[] GetStringListWithoutQuotes(string value)
   {
      var matches = StringListRegex.Matches(value);
      var strList = new string[matches.Count];
      for (var i = 0; i < matches.Count; i++) strList[i] = matches[i].ToString().Trim().TrimQuotes();
      return strList;
   }

   /// <summary>
   /// Removes all comments from the given string.
   /// Parses the string to a list of <c>KeyValuePair</c> with the key and value of each line.
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   public static List<KeyValuePair<string, string>> GetKeyValueList(string value)
   {
      return GetKeyValueList(ref value);
   }

   public static List<PointF> GetPointFList(string value)
   {
      List<PointF> points = [];
      var matches = PointFRegex.Matches(value);
      foreach (Match match in matches)
      {
         if (float.TryParse(match.Groups["x"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) && float.TryParse(match.Groups["y"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
            points.Add(new(x, y));
      }
      return points;
   }

   public static List<KeyValuePair<string, string>> GetKeyValueListWithoutQuotes(string value)
   {
      return GetKeyValueList(ref value).Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.TrimQuotes())).ToList();
   }

   public static List<Point> GetPointList(string value)
   {
      List<Point> points = [];
      var matches = PointRegex.Matches(value);
      foreach (Match match in matches)
      {
         if (int.TryParse(match.Groups["x"].Value, out var x) && int.TryParse(match.Groups["y"].Value, out var y))
            points.Add(new(x, y));
      }
      return points;
   }

   /// <summary>
   /// Removes all comments from a multiline string.
   /// Parses the string to a list of <c>KeyValuePair</c> with the key and value of each line.
   /// </summary>
   /// <param name="value"></param>
   /// <returns></returns>
   public static List<KeyValuePair<string, string>> GetKeyValueList(ref string value)
   {
      List<KeyValuePair<string, string>>  keyValueList = [];
      RemoveCommentFromMultilineString(value, out var commentFreeStr);
      var lines = commentFreeStr.Split('\n');
      foreach (var line in lines)
      {
         // split at all = and remove leading and trailing whitespaces
         var keyValue = line.Split('=');

         List<string> allKvps = [];
         // Trim all values and get same line values
         for (var i = 0; i < keyValue.Length; i++)
         {
            keyValue[i] = keyValue[i].Trim();
            var sameLineValues = SameLineValuesRegex.Matches(keyValue[i]);
            for (var j = 0; j < sameLineValues.Count; j++) 
               allKvps.Add(sameLineValues[j].Value);
         }

         for (var i = 0; i < allKvps.Count; i += 2)
         {
            if (i + 1 >= allKvps.Count)
               break;
            keyValueList.Add(new(allKvps[i], allKvps[i + 1]));
         }
      }

      return keyValueList;
   }

   public static bool GetChancesListFromKeyValuePairs(List<KeyValuePair<string, string>> kvps, out List<KeyValuePair<string, int>> chances)
   {
      chances = [];
      foreach (var kvp in kvps)
      {
         if (int.TryParse(kvp.Value, out var chance))
            chances.Add(new(kvp.Key, chance));
         else
         {
            Globals.ErrorLog.Write($"Could not parse chance: {kvp.Value} in {kvp.Key} = {kvp.Value}");
            return false;
         }
      }
      return true;
   }

   /// <summary>
   /// Iterates of all lines in a multiline string and removes comments from each line.
   /// </summary>
   /// <param name="value"></param>
   /// <param name="commentFreeStr"></param>
   public static void RemoveCommentFromMultilineString(string value, out string commentFreeStr)
   {
      RemoveCommentFromMultilineString(ref value, out commentFreeStr);
   }

   public static void RemoveCommentFromMultilineString(ref string str, out string commentFreeStr)
   {
      var sb = new StringBuilder();
      var lines = str.Split('\n');
      foreach (var line in lines) 
         sb.AppendLine(RemoveCommentFromLine(line));
      commentFreeStr = sb.ToString();
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

   /// <summary>
   /// Parses a string to a <c>color</c> if it is in the format <c>"r g b"</c>.
   /// </summary>
   /// <param name="str"></param>
   /// <param name="color"></param>
   /// <returns></returns>
   /// <exception cref="ParsingException"></exception>
   public static bool TryParseColor(string str, out Color color)
   {
      color = Color.Empty;
      var match = ColorRegex.Match(str);
      if (!match.Success)
         return false;

      var r = int.Parse(match.Groups["r"].Value);
      var g = int.Parse(match.Groups["g"].Value);
      var b = int.Parse(match.Groups["b"].Value);

      color = Color.FromArgb(r, g, b);
      return true;
   }

   /// <summary>
   /// Parses a string to a <c>color</c> if it is in the format <c>"r g b"</c> where each value is a percentage.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static Color ParseColorPercentile(string str)
   {

      // Split the string by spaces
      var components = str.Split(' ');

      if (components.Length != 3)
      {
         Globals.ErrorLog.Write("Could not parse color: " + str);
         return Color.Empty;
      }

      var red = float.Parse(components[0], CultureInfo.InvariantCulture) * 255;
      var green = float.Parse(components[1], CultureInfo.InvariantCulture) * 255;
      var blue = float.Parse(components[2], CultureInfo.InvariantCulture) * 255;

      return Color.FromArgb((int)red, (int)green, (int)blue);
   }

   /// <summary>
   /// Returns a list of <c>MonarchName</c> from a string which contains names with ordinal and chance.
   /// </summary>
   /// <param name="input"></param>
   /// <param name="names"></param>
   public static void ParseMonarchNames(string input, out List<MonarchName> names)
   {
      names = [];
      var matches = MonarchNameRegex.Matches(input);

      foreach (Match match in matches)
      {
         var name = match.Groups["name"].Value;

         try
         {
            var chance = int.Parse(match.Groups["chance"].Value);
            names.Add(new(name, chance));
         }
         catch (Exception e)
         {
            Globals.ErrorLog.Write($"Could not parse chance: {match.Groups["chance"].Value} in {name} = {match.Groups["chance"].Value}");
            return;
         }

      }
   }

   /// <summary>
   /// Takes in a string and returns a <C>Person</C> with all attributes from the string set.
   /// </summary>
   /// <param name="content"></param>
   /// <param name="person"></param>
   public static void ParsePersonFromString(string content, out Person person)    
   {
      person = new();
      var kvps = GetKeyValueList(content);
      foreach (var kvp in kvps)
      {
         var val = RemoveCommentFromLine(kvp.Value);
         switch (kvp.Key.ToLower())
         {
            case "country_of_origin":
               person.CountryOfOrigin = val;
               break;
            case "name":
               person.Name = val.TrimQuotes();
               break;
            case "monarch_name":
               person.MonarchName = val.TrimQuotes();
               break;
            case "dynasty":
               person.Dynasty = val.TrimQuotes();
               break;
            case "culture":
               person.Culture = val.TrimQuotes();
               break;
            case "religion":
               person.Religion = val;
               break;
            case "birth_date":
               if (!TryParseDate(val, out var birthDate))
                  Globals.ErrorLog.Write($"Could not parse birth date: \"{val}\" in person: \"{person.Name}\"");
               person.BirthDate = birthDate;
               break;
            case "death_date":
               if (!TryParseDate(val, out var deathDate))
                  Globals.ErrorLog.Write($"Could not parse death date: \"{val}\" in person: \"{person.Name}\"");
               person.DeathDate = deathDate;
               break;
            case "claim":
               if (int.TryParse(val, out var claimStrength))
                  person.ClaimStrength = claimStrength;
               else
                  Globals.ErrorLog.Write("Could not parse claim strength: " + val);
               break;
            case "adm":
               if (int.TryParse(val, out var adm))
                  person.Adm = adm;
               else
                  Globals.ErrorLog.Write("Could not parse adm: " + val);
               break;
            case "dip":
               if (int.TryParse(val, out var dip))
                  person.Dip = dip;
               else
                  Globals.ErrorLog.Write("Could not parse dip: " + val);
               break;
            case "mil":
               if (int.TryParse(val, out var mil))
                  person.Mil = mil;
               else
                  Globals.ErrorLog.Write("Could not parse mil: " + val);
               break;
            case "female":
               person.IsFemale = YesNo(val);
               break;
            case "regent":
               person.IsRegent = YesNo(val);
               break;
            case "block_disinherit":
               person.BlockDisinherit = YesNo(val);
               break;
            default:
               Globals.ErrorLog.Write("Unknown key in Person: " + kvp.Key);
               break;
         }
      }
   }

   /// <summary>
   /// Tries to parse a string to a <c>Leader</c> object and sets all attributes from the string.
   /// </summary>
   /// <param name="str"></param>
   /// <param name="leader"></param>
   /// <returns></returns>
   public static bool ParseLeaderFromString(string str, out Leader leader)
   {
      var kvps = GetKeyValueList(str);
      leader = new();

      foreach (var kv in kvps)
      {
         switch (kv.Key.ToLower())
         {
            case "name":
               leader.Name = kv.Value.TrimQuotes();
               break;
            case "fire":
               if (int.TryParse(kv.Value, out var fire))
                  leader.Fire = fire;
               else
               {
                  Globals.ErrorLog.Write($"Could not parse fire: " + kv.Value + $"in leader {leader.Name}");
                  return false;
               }
               break;
            case "shock":
               if (int.TryParse(kv.Value, out var shock))
                  leader.Shock = shock;
               else
               {
                  Globals.ErrorLog.Write("Could not parse shock: " + kv.Value + $"in leader {leader.Name}");
                  return false;
               }
               break;
            case "manuever":
               if (int.TryParse(kv.Value, out var maneuver))
                  leader.Maneuver = maneuver;
               else
               {
                  Globals.ErrorLog.Write("Could not parse maneuver: " + kv.Value + $"in leader {leader.Name}");
                  return false;
               }
               break;
            case "siege":
               if (int.TryParse(kv.Value, out var siege))
                  leader.Siege = siege;
               else
               {
                  Globals.ErrorLog.Write("Could not parse siege: " + kv.Value + $"in leader {leader.Name}");
                  return false;
               }
               break;
            case "type":
               if (Enum.TryParse<LeaderType>(kv.Value, true, out var leaderType))
                  leader.Type = leaderType;
               else
               {
                  Globals.ErrorLog.Write("Could not parse type: " + kv.Value + $" in leader {leader.Name}");
                  return false;
               }
               break;
            case "female":
               leader.IsFemale = YesNo(kv.Value);
               break;
            case "death_date":
               if (!TryParseDate(kv.Value, out var deathDate))
               {
                  Globals.ErrorLog.Write("Could not parse death date: " + kv.Value + $" in leader {leader.Name}");
                  return false;
               }
               leader.DeathDate = deathDate;
               break;
            case "personality":
               leader.Personalities.Add(kv.Value);
               break;
            case "trait":
               leader.Traits.Add(kv.Value);
               break;
            default:
               Globals.ErrorLog.Write("Unknown key in Leader: " + kv.Key);
               return false;
         }
      }
      return true;
   }

   /// <summary>
   /// Parses a string to the enum <c>Mana</c>.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static Mana ManaFromString(string str)
   {
      return str.ToLower() switch
      {
         "adm" => Mana.ADM,
         "dip" => Mana.DIP,
         "mil" => Mana.MIL,
         _ => Mana.NONE
      };
   }

   /// <summary>
   /// TODO Parses a Blocks content to either a <c>Scope</c>, <c>Effect</c>, <c>Trigger</c> or <c>Condition</c>. and returns a bool indicating if the parsing was successful.
   /// TODO implement this and the the output of it
   /// </summary>
   /// <param name="block"></param>
   /// <param name="output"></param>
   /// <returns></returns>
   internal static bool ParseDynamicContent(Block block, out string output)
   {
      output = default!;
      if (ScopeParser.IsAnyScope(block.Name))
      {
         //TODO parse Scope
         return true;
      }
      if (EffectParser.IsAnyEffect(block.Name))
      {
         //TODO parse Effect
         return true;
      }
      if (ScopeParser.IsAnyTriggerScope(block.Name))
      {
         //TODO parse Trigger
         return true;
      }
      if (ScopeParser.IsLogicScope(block.Name))
      {
         //TODO parse Condition
         return true;
      }
      return false;
   }
   
   /// <summary>
   /// Returns true if the given string is a valid <c>TradeGood</c>.
   /// </summary>
   /// <param name="str"></param>
   /// <returns></returns>
   public static bool IsValidTradeGood(string str)
   {
      return Globals.TradeGoods.ContainsKey(str);
   }

   /// <summary>
   /// Parses a string to a date if all elements of a date are present: <c>year</c> - <c>month</c> - <c>day</c>.
   /// </summary>
   /// <param name="text"></param>
   /// <param name="date"></param>
   /// <returns></returns>
   public static bool TryParseFullDate(string text, out DateTime date)
   {
      date = DateTime.MinValue;
      var match = FullDateParseRegex.Match(text);
      if (!match.Success)
         return false;

      var year = int.Parse(match.Groups["year"].Value);
      var month = int.Parse(match.Groups["month"].Value);
      var day = int.Parse(match.Groups["day"].Value);

      if (year < 1 || year > 9999 || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
         return false;

      date = new (year, month, day);
      return true;
   }

   public static bool GetSimpleKvpArray(string input, out string[] kvps)
   {
      kvps = input.Split('\n', '=');
      return kvps.Length % 2 == 0;
   }

   public static bool ParseTriggeredName(Block block, out TriggeredName tn)
   {
      tn = TriggeredName.Empty;
      var name = string.Empty;
      IElement? trigger = null;

      foreach (var element in block.Blocks)
      {
         if (element is Block { Name: "trigger" } triggerBlock)
         {
            trigger = triggerBlock;
            continue;
         }
         if (element is not Content c)
         {
            Globals.ErrorLog.Write($"Error: Illegal Element found in {block.Name}");
            return false;
         }
         var elementContent = c.Value.Split('=');
         if (elementContent.Length != 2)
         {
            Globals.ErrorLog.Write($"Error: Illegal Content found in {c.Value}");
            return false;
         }
         elementContent[1] = elementContent[1].TrimQuotes();
      }
      tn = new(name, trigger);
      return true;
   }

   /// <summary>
   /// Trims quotes (single or double) from the start and end of the string.
   /// </summary>
   /// <param name="input">The input string.</param>
   /// <param name="trim">If ture the string is also trimmed before removing quotes</param>
   /// <returns>The trimmed string.</returns>
   public static string TrimQuotes(this string input, bool trim = true)
   {
      if (string.IsNullOrEmpty(input))
         return input;

      if (trim)
         input = input.Trim();

      if (input.Length >= 2 &&
          ((input[0] == '"' && input[^1] == '"') ||
           (input[0] == '\'' && input[^1] == '\'')))
      {
         return input[1..^1];
      }
      return input;
   }


   public static int GetRegnalFromString(string str)
   {
      if (RegnalRegex.IsMatch(str))
      {
         var match = RegnalRegex.Match(str);
         return int.Parse(match.Groups["num"].Value);
      }
      return int.MinValue;
   }

   public static IErrorHandle ParseMonarchNameAndRegnal(string input, out string name, out int regnal)
   {
      var match = RegnalRegex.Match(input);
      if (!match.Success)
      {
         name = string.Empty;
         regnal = 0;
         return new ErrorObject(ErrorType.MisformedMonarchName, "No valid regnal number found!");
      }

      regnal = int.Parse(match.Groups["num"].Value);
      name = input[..match.Index].Trim();

      return ErrorHandle.Success;
   }

   public static IErrorHandle GetMonarchNameFromTextBoxes(TextBox nameTextBox, TextBox chanceTextBox, out MonarchName mName)
   {
      var handle = ParseMonarchNameAndRegnal(nameTextBox.Text, out _, out var regnal);
      if (!handle.Log())
      {
         mName = MonarchName.Empty;
         return handle;
      }

      mName = new(nameTextBox.Text, regnal, int.Parse(chanceTextBox.Text));
      return ErrorHandle.Success;
   }

   [GeneratedRegex(@"\s*(\d+)")]
   private static partial Regex ByteListRegex();
   [GeneratedRegex(@"\s*(\d+)")]
   private static partial Regex IntListRegex();
}

