using System.Diagnostics;
using System.Text;
using Editor.DataClasses.DataStructures;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static class EnhancedParser
   {
      public static T ParseBlock<T>(string blockName, EnhancedBlock block, PathObj po, ref int limit, Func<EnhancedBlock, PathObj, T> evaluator,
                                    Func<T> fallback)
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
            _ = new LoadingError(po, $"Unexpected block element in block \"{block.Name}\"! Expected {limit} but got {block.SubBlockCount}", block.StartLine,
                                 type: ErrorType.UnexpectedBlockElement);
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
         var contentStart = 0;
         var elementIndex = 0;
         byte wasEquals = 0;

         for (var i = 0; i < lines.Length; i++)
         {
            var length = lines[i].Length;
            var charIndex = 0;

            var wordStart = -1;
            var wordEnd = -1;
            isInWord = false;
            isInWhiteSpace = false;

            if (lines[i].Length == 0)
            {
               currentContent.Append('\n');
               continue;
            }

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
                     if (isInQuotes)
                        if (wasEquals == 2)
                           wasEquals = 3;
                     isInQuotes = !isInQuotes;
                     isInWhiteSpace = false;
                     currentContent.Append(c);
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
                     var newBlock = new EnhancedBlock(new(charSpan), i, elementIndex++); // we create a new block

                     wordStart = -1;
                     wordEnd = -1;

                     var trimmed = currentContent.ToString().Trim();

                     if (trimmed.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmed, contentStart,
                                                          elementIndex++); // We create a new content element as there is no block element on the stack
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
                     }
                     else // No Content to be added, only add the new Block which was started
                     {

                        if (blockStack.IsEmpty)
                           blocks.Add(newBlock);
                        else
                           blockStack.PeekRef()->SubBlocks.Add(newBlock);
                     }

                     currentContent.Clear();
                     blockStack.Push(newBlock);
                     contentStart = i;
                     wasEquals = 0;

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

                     var currentStr = currentContent.ToString();
                     var trimmedClosing = currentStr.Trim();

                     if (trimmedClosing.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmedClosing, currentStr[0] == '\n' ? contentStart + 1 : contentStart,
                                                          elementIndex++); // We create a new content element as there is no block element on the stack
                        blockStack.PeekRef()->ContentElements.Add(content);
                        currentContent.Clear();
                     }

                     blockStack.Pop();
                     contentStart = i;
                     wasEquals = 0;
                     break;
                  case '#':
                     if (!isInQuotes) // # is in quotes and thus allowed
                     {
                        charIndex = length;
                        break;
                     }

                     currentContent.Append(c);
                     break;
                  case '\r':

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
                                 if (wasEquals > 2)
                                 {
                                    wasEquals = 1;
                                    currentContent.Append('\t');
                                 }
                                 else
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
                              
                              if (wasEquals == 2)
                                 wasEquals = 3;
                              else
                                 wasEquals = 0;
                           }
                           else
                              wordEnd = currentContent.Length + 1;
                        }
                        else
                        {
                           isInWord = false;
                           wasEquals = 2;
                        }
                     }

                     currentContent.Append(c);
                     break;
               }

               charIndex++;
            }

            if (currentContent.Length >= 1 && char.IsWhiteSpace(currentContent[^1]) && currentContent[^1] != '\n')
               currentContent.Remove(currentContent.Length - 1, 1);
            
            currentContent.Append('\n');
            wasEquals = 0;
         }

         if (!blockStack.IsEmpty)
         {
            _ = new LoadingError(pathObj, "Unmatched opening brace", blockStack.PeekRef()->StartLine + 1, 0, level: LogType.Critical);
            return ([], []);
         }

         if (currentContent.Length > 0)
         {
            var contentStr = currentContent.ToString();
            if (!string.IsNullOrWhiteSpace(contentStr))
               contents.Add(new(contentStr, contentStart, elementIndex++));
         }

         return (blocks, contents);
      }

      public enum FileContentAllowed
      {
         Both,
         BlocksOnly,
         ContentOnly
      }

      public static List<IEnhancedElement> LoadBaseOrder(this PathObj po)
      {

         List<EnhancedBlock> blocks = [];
         List<EnhancedContent> contents = [];
         (blocks, contents) = GetElements(po);

         return MergeBlocksAndContent(blocks, contents).ToList();
      }

      public static List<IEnhancedElement> LoadBaseOrder(out PathObj po, params string[] internalPath)
      {
         List<EnhancedBlock> blocks = [];
         List<EnhancedContent> contents = [];
         (blocks, contents) = GetElements(out po, internalPath);
         return MergeBlocksAndContent(blocks, contents).ToList();
      }

      public static List<IEnhancedElement> LoadBaseOrder(string content, PathObj po)
      {
         var (blocks, contents) = GetElements(po, content);
         return MergeBlocksAndContent(blocks, contents).ToList();
      }


      public static IEnumerable<IEnhancedElement> MergeBlocksAndContent(List<EnhancedBlock> blocks, List<EnhancedContent> contents)
      {
         var indexBlocks = 0;
         var indexContent = 0;
         

         // Traverse both lists
         while (indexBlocks < blocks.Count && indexContent < contents.Count)
         {
            if (blocks[indexBlocks].Index < contents[indexContent].Index)
            {
               yield return blocks[indexBlocks];
               indexBlocks++;
            }
            else
            {
               yield return contents[indexContent];
               indexContent++;
            }
         }

         // Copy remaining elements from list1
         while (indexBlocks < blocks.Count)
         {
            yield return blocks[indexBlocks];
            indexBlocks++;
         }

         // Copy remaining elements from list2
         while (indexContent < contents.Count)
         {
            yield return contents[indexContent];
            indexContent++;
         }
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

      public static (List<EnhancedBlock>, List<EnhancedContent>) LoadBase(this PathObj po, FileContentAllowed fca)
      {
         var (blocks, contents) = GetElements(po);

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
         return (blocks, contents);
      
      }

      public static (List<EnhancedBlock>, List<EnhancedContent>) LoadBase(this PathObj po, FileContentAllowed fca, string input)
      {
         var (blocks, contents) = GetElements(po, input);

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
         return (blocks, contents);

      }

      public static IErrorHandle IsValidString(string value, out string result)
      {
         result = value;

         var startsWith = value.StartsWith('\"');
         var endsWith = value.EndsWith('\"');

         if (startsWith && endsWith)
         {
            result = value[1..^1];
            return ErrorHandle.Success;
         }

         if ((startsWith && !endsWith) || (!startsWith && endsWith))
            return new ErrorObject(ErrorType.TempParsingError, "Invalid string value. Must start and end with \" or have non at all", addToManager: false);
         return ErrorHandle.Success;
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