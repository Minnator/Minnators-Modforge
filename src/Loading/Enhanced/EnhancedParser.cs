using System.Diagnostics;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.ErrorHandling;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public static class EnhancedParser
   {
      public static List<IEnhancedElement> GetElements(out PathObj pathObj, params string[] internalPath)
      {
         if (!FilesHelper.GetModOrVanillaPath(out var path, out var isModPath, internalPath))
         {
            _ = new LoadingError(new(internalPath, isModPath), "File not found", type:ErrorType.FileNotFound, level:LogType.Critical);
            pathObj = PathObj.Empty;
            return [];
         }
         pathObj = PathObj.FromPath(path, isModPath);
         return GetElements(pathObj);
      }


      public static List<IEnhancedElement> GetElements(this PathObj pathObj)
      {
         if (!IO.ReadAllInANSI(pathObj.GetPath(), out var content)) 
            return [];
         return GetElements(pathObj, content);
      }

      private static unsafe List<IEnhancedElement> GetElements(PathObj pathObj, string input)
      {

         var lines = input.Split('\n');
         var result = new List<IEnhancedElement>();
         StringBuilder currentContent = new();
         ModifiableStack<EnhancedBlock> blocks = new();

         var isExcaping = false;
         var isInQuotes = false;
         var isInWord = false;
         var isInWhiteSpace = false;

         for (var i = 0; i < lines.Length; i++)
         {
            var length = lines[i].Length;
            var charIndex = 0;
            char c;

            var wordStart = -1;
            var wordEnd = -1;
            isInWord = false;
            isInWhiteSpace = false;

            while (charIndex < length)
            {
               c = lines[i][charIndex];
               switch (c)
               {
                  case '\\': //TODO: Implement escaping
                     break;
                  case '"':
                     isInQuotes = !isInQuotes;
                     break;
                  case '{':
                     if (isInQuotes)
                        goto default;

                     var nameLength = wordEnd - wordStart;
                     if (currentContent.Length < 1)
                     {
                        _ = new LoadingError(pathObj, "Block name cannot be empty", i, charIndex, level: LogType.Critical);
                        return [];
                     }

                     if (wordEnd < 0 || wordStart < 0)
                     {
                        _ = new LoadingError(pathObj, "Block name cannot be empty", i, charIndex, level: LogType.Critical);
                        return [];
                     }

                     Span<char> charSpan = stackalloc char[nameLength];
                     currentContent.CopyTo(wordStart, charSpan, nameLength); // We copy the name of the block from the sb
                     currentContent.Remove(wordStart, currentContent.Length - wordStart); // we remove anything after the name start
                     var newBlock = new EnhancedBlock(new (charSpan), i); // we create a new block

                     wordStart = -1;
                     wordEnd = -1;

                     var trimmed = currentContent.ToString().Trim();
 
                     if (trimmed.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmed, i); // We create a new content element as there is no block element on the stack
                        if (blocks.IsEmpty)
                        {
                           result.Add(content);
                           result.Add(newBlock);
                        }
                        else // We add the content to the previous block element
                        {
                           var currentBlock = blocks.PeekRef();
                           currentBlock->Elements.Add(content);
                           currentBlock->Elements.Add(newBlock);
                           
                        }
                        currentContent.Clear();
                     }
                     else  // No Content to be added, only add the new Block which was started
                     {
                        if (blocks.IsEmpty)
                           result.Add(newBlock);
                        else
                           blocks.PeekRef()->Elements.Add(newBlock);
                     }

                     blocks.Push(newBlock);

                     break;
                  case '}':
                     if (isInQuotes)
                        goto default;

                     if (blocks.IsEmpty)
                     {
                        _ = new LoadingError(pathObj, "Unmatched closing brace", i + 1, charIndex, level: LogType.Critical);
                        return [];
                     }
                     
                     var trimmedClosing = currentContent.ToString().Trim();

                     if (trimmedClosing.Length > 0)  // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(trimmedClosing, i); // We create a new content element as there is no block element on the stack
                        blocks.PeekRef()->Elements.Add(content);
                        currentContent.Clear();
                     }

                     blocks.Pop();
                     break;
                  case '#':
                     if (!isInQuotes) // # is in quotes and thus allowed
                     {
                        charIndex = length;
                        break;
                     }

                     goto default;
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

         if (!blocks.IsEmpty)
         {
            _ = new LoadingError(pathObj, "Unmatched opening brace",blocks.PeekRef()->StartLine + 1, 0, level: LogType.Critical);
            return [];
         }
         return result;
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