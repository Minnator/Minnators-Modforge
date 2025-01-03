﻿using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static class EnhancedParser
   {
      /// <summary>
      /// Can convert a string to a type T (int, float, bool, string)
      /// Generates a LoadingError if the conversion fails
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="str"></param>
      /// <param name="po"></param>
      /// <returns></returns>
      public static T? Convert<T>(string? str, PathObj po)
      {
         if (typeof(T) == typeof(int))
         {
            if (int.TryParse(str, out var result))
               return (T)(object)result;
            _ = new LoadingError(po, $"Failed to parse \"{str}\" as int when expecting an int", type: ErrorType.TypeConversionError);
         }
         else if (typeof(T) == typeof(float))
         {
            if (float.TryParse(str, out var result))
               return (T)(object)result;
            _ = new LoadingError(po, $"Failed to parse \"{str}\" as float when expecting a float", type: ErrorType.TypeConversionError);
         }
         else if (typeof(T) == typeof(bool))
         {
            if (bool.TryParse(str, out var result))
               return (T)(object)result;
            _ = new LoadingError(po, $"Failed to parse \"{str}\" as bool when expecting a boolean", type: ErrorType.TypeConversionError);
         }
         else if (typeof(T) == typeof(string))
            return (T)(object)str!;
         return default;
      }

      public static T ParseBlock<T>(string blockName, EnhancedBlock block, PathObj po, ref int limit, Func<EnhancedBlock, PathObj, T> evaluator, Func<T> fallback)
      {
         if (!block.GetSubBlockByName(blockName, out var subBlock))
            return fallback();

         limit++;
         return evaluator(subBlock, po);
      }

      public static List<T> ParseBlockMultiple<T>(string blockName, EnhancedBlock block, PathObj po, ref int limit, Func<EnhancedBlock, PathObj, T?> evaluator)
      {
         List<T> result = [];
         if (!block.GetAllSubBlockByName(blockName, out var subBlocks))
            return [];

         limit += subBlocks.Count;
         foreach (var subBlock in subBlocks)
         {
            var value = evaluator(subBlock, po);
            if (value != null)
               result.Add(value);
         }

         return result;
      }

      public static bool CheckLimit(EnhancedBlock block, int limit, PathObj po)
      {
         if (block.SubBlockCount > limit)
         {
            _ = new LoadingError(po, $"Unexpected block element in block \"{block.Name}\"! Expected {limit} but got {block.SubBlockCount}", block.StartLine, type: ErrorType.UnexpectedBlockElement);
            return false;
         }
         return true;
      }

      public static (List<EnhancedBlock>, List<EnhancedContent>) GetElements(out PathObj pathObj, params string[] internalPath)
      {
         if (!GetModOrVanillaPath(out pathObj, internalPath))
            return ([], []);
         return GetElements(pathObj);
      }

      public static bool GetModOrVanillaPath(out PathObj pathObj, params string[] internalPath)
      {
         if (FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, internalPath))
         {
            pathObj = PathObj.FromPath(path, isModPath);
            return true;
         }
         _ = new LoadingError(new(internalPath, isModPath), "File not found", type: ErrorType.FileNotFound, level: LogType.Critical);
         pathObj = PathObj.Empty;
         return false;
      }
      
      public static (List<EnhancedBlock>, List<EnhancedContent>) GetElements(this PathObj pathObj)
      {
         if (!IO.ReadAllInANSI(pathObj, out var content))
            return ([], []);
         return GetElements(pathObj, content);
      }

      private static unsafe (List<EnhancedBlock>, List<EnhancedContent>) GetElements(PathObj pathObj, string input)
      {
         var lines = input.Split('\n');
         var contents = new List<EnhancedContent>();
         var blocks = new List<EnhancedBlock>();
         StringBuilder currentContent = new();
         ModifiableStack<EnhancedBlock> blockStack = new();

         var isInQuotes = false;
         var isInWord = false;
         var isInWhiteSpace = false;

         for (var i = 0; i < lines.Length; i++)
         {
            var length = lines[i].Length;
            var charIndex = 0;

            var wordStart = -1;
            var wordEnd = -1;
            isInWord = false;
            isInWhiteSpace = false;
            var line = lines[i].ToCharArray();
            while (charIndex < length)
            {
               var c = line[charIndex];
               switch (c)
               {
                  case '\\':
                     if (isInQuotes)
                     {
                        charIndex++;
                        if (line.Length > charIndex)
                           currentContent.Append(line[charIndex]);
                     }
                     else
                        currentContent.Append(c);
                     break;
                  case '"':
                     isInQuotes = !isInQuotes;
                     break;
                  case '{':
                     if (isInQuotes)
                     {
                        currentContent.Append(c);
                        break;
                     }


                     var nameLength = wordEnd - wordStart;
                     if (currentContent.Length < 1)
                     {
                        _ = new LoadingError(pathObj, "Block name cannot be empty", i, charIndex, level: LogType.Critical);
                        return ([], []);
                     }

                     if (wordEnd < 0 || wordStart < 0)
                     {
                        _ = new LoadingError(pathObj, "Block name cannot be empty", i, charIndex, level: LogType.Critical);
                        return ([], []);
                     }

                     Span<char> charSpan = stackalloc char[nameLength];
                     currentContent.CopyTo(wordStart, charSpan, nameLength); // We copy the name of the block from the sb
                     currentContent.Remove(wordStart, currentContent.Length - wordStart); // we remove anything after the name start
                     var newBlock = new EnhancedBlock(new(charSpan), i); // we create a new block

                     wordStart = -1;
                     wordEnd = -1;

                     var trimmed = currentContent.ToString().Trim();

                     if (trimmed.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmed, i); // We create a new content element as there is no block element on the stack
                        if (blockStack.IsEmpty)
                        {
                           contents.Add(content);
                           blocks.Add(newBlock);
                        }
                        else // We add the content to the previous block element
                        {
                           var currentBlock = blockStack.PeekRef();
                           currentBlock->ContentElements.Add(content);
                           currentBlock->SubBlocks.Add(newBlock);

                        }
                        currentContent.Clear();
                     }
                     else  // No Content to be added, only add the new Block which was started
                     {
                        if (blockStack.IsEmpty)
                           blocks.Add(newBlock);
                        else
                           blockStack.PeekRef()->SubBlocks.Add(newBlock);
                     }

                     blockStack.Push(newBlock);

                     break;
                  case '}':
                     if (isInQuotes)
                     {
                        currentContent.Append(c);
                        break;
                     }
                     if (blockStack.IsEmpty)
                     {
                        _ = new LoadingError(pathObj, "Unmatched closing brace", i + 1, charIndex, level: LogType.Critical);
                        return ([], []);
                     }

                     var trimmedClosing = currentContent.ToString().Trim();

                     if (trimmedClosing.Length > 0)  // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmedClosing, i); // We create a new content element as there is no block element on the stack
                        blockStack.PeekRef()->ContentElements.Add(content);
                        currentContent.Clear();
                     }

                     blockStack.Pop();
                     break;
                  case '#':
                     if (!isInQuotes) // # is in quotes and thus allowed
                     {
                        charIndex = length;
                        break;
                     }

                     currentContent.Append(c);
                     break;
                  default:
                     if (!isInQuotes) // We only add whitespace if we are in quotes
                     {
                        if (char.IsWhiteSpace(c))
                        {
                           if (!isInWhiteSpace)
                           {
                              isInWhiteSpace = true;
                              isInWord = false;
                              if (wordStart != -1)
                                 currentContent.Append(' ');
                           }
                           break;
                        }
                        isInWhiteSpace = false;
                        if (c != '=')
                        {
                           if (!isInWord)
                           {
                              wordEnd = currentContent.Length + 1;
                              wordStart = currentContent.Length;
                              isInWord = true;
                           }
                           else
                              wordEnd = currentContent.Length + 1;
                        }
                        else
                           isInWord = false;
                     }

                     currentContent.Append(c);
                     break;
               }
               charIndex++;
            }

            if (currentContent.Length >= 1 && char.IsWhiteSpace(currentContent[^1]))
               currentContent.Remove(currentContent.Length - 1, 1);

            currentContent.Append('\n');
         }

         if (!blockStack.IsEmpty)
         {
            _ = new LoadingError(pathObj, "Unmatched opening brace", blockStack.PeekRef()->StartLine + 1, 0, level: LogType.Critical);
            return ([], []);
         }

         return (blocks, contents);
      }

      public enum FileContentAllowed
      {
         Both,
         BlocksOnly,
         ContentOnly
      }

      public static (List<EnhancedBlock>, List<EnhancedContent>) LoadBase(FileContentAllowed fca, out PathObj po, params string[] internalPath)
      {
         var elements = GetElements(out po, internalPath);
         var (blocks, contents) = elements;

         switch (fca)
         {
            case FileContentAllowed.Both:
               break;
            case FileContentAllowed.BlocksOnly:
               if (contents.Count != 0)
                  _ = new LoadingError(po, "Detected content in a file where only blocks are allowed!", type: ErrorType.UnexpectedContentElement, level: LogType.Error);
               break;
            case FileContentAllowed.ContentOnly:
               if (blocks.Count != 0)
                  _ = new LoadingError(po, "Detected blocks in a file where only content is allowed!", type: ErrorType.UnexpectedBlockElement, level: LogType.Error);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(fca), fca, null);
         }
         return elements;
      }
   }
   public readonly struct LineKvp<T, TQ>
   {
      public LineKvp(T key, TQ value, int line)
      {
         Key = key;
         Value = value;
         Line = line;
      }

      public T Key { get; }
      public TQ Value { get; }
      public int Line { get; }
   }
}