using System;
using System.Text;
using Editor.DataClasses.Misc;
using Editor.Parser;
using Editor.Saving;

namespace Editor.Loading.Enhanced
{
   public class EnhancedParsingException(string message, int line, int index) : Exception($"Error at line {line} | {index}: {message}");

   public static class EnhancedParser
   {

      public static unsafe List<IEnhancedElement> GetElements(ref PathObj pathObj, string input)
      {

         var lines = input.Split('\n');
         var result = new List<IEnhancedElement>();
         StringBuilder currentContent = new();
         ModifiableStack<EnhancedBlock> blocks = new();

         var isInQuotes = false;
         var isInWord = false;

         for (var i = 0; i < lines.Length; i++)
         {
            var length = lines[i].Length;
            var charIndex = 0;
            char c;

            var wordStart = 0;
            var wordEnd = 0;
            isInWord = false;

            while (charIndex < length)
            {
               c = lines[i][charIndex];
               switch (c)
               {
                  case '"':
                     isInQuotes = !isInQuotes;
                     break;
                  case '{':
                     if (isInQuotes)
                        goto default;

                     var nameLength = wordEnd - wordStart;
                     if (currentContent.Length < 1)
                        throw new EnhancedParsingException("Block name cannot be empty", i, charIndex); // we cannot recover from this

                     Span<char> charSpan = stackalloc char[nameLength];
                     currentContent.CopyTo(wordStart, charSpan, nameLength); // We copy the name of the block from the sb
                     currentContent.Remove(wordStart, currentContent.Length - wordStart); // we remove anything after the name start
                     var newBlock = new EnhancedBlock(new (charSpan), i); // we create a new block
                     wordEnd = -1;
                     wordStart = -1; // We reset the word start and end


                     if (currentContent.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(currentContent.ToString(), i); // We create a new content element as there is no block element on the stack
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
                        throw new EnhancedParsingException("Unmatched closing brace", i, charIndex); // We cannot recover from this

                     if (currentContent.Length > 0) // We have remaining content in the currentContent which we need to add to the previous block element
                     {
                        var content = new EnhancedContent(currentContent.ToString(), i); // We create a new content element as there is no block element on the stack
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
                           isInWord = false;
                           currentContent.Append(' ');
                           break;
                        }
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
            currentContent.Append('\n');
         }

         if (!blocks.IsEmpty)
         {
            throw new EnhancedParsingException("Unmatched opening brace", blocks.PeekRef()->StartLine, 0);
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