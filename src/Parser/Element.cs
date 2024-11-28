using System.Text;
using Editor.Saving;
using static Editor.Saving.SavingUtil;

namespace Editor.Parser
{

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

      public void FormatElement(int tabs, ref StringBuilder sb)
      {
         OpenBlock(ref tabs, Name, ref sb);
         foreach (var element in Blocks) 
            element.FormatElement(tabs, ref sb);
         CloseBlock(ref tabs, ref sb);
      }

      public bool HasOnlyContent => Blocks.TrueForAll(b => !b.IsBlock);
      public string GetContent => string.Join("\n", GetContentElements.Select(c => c.Value));

      public string GetAllContent
      {
         get
         {
            var sb = new StringBuilder();
            sb.AppendLine($"{Name} = {{");
            foreach (var element in Blocks)
            {
               if (element is Content content)
                  sb.AppendLine(content.Value);
               else if (element is Block block)
               {
                  sb.AppendLine(block.GetAllContent);
               }
            }
            sb.AppendLine("}");
            return sb.ToString();
         }
      }
      
      public Block? GetBlockWithName(string name)
      {
         return GetBlockElements.FirstOrDefault(b => b.Name == name);
      }

      public override string ToString()
      {
         return Name;
      }
   }
   public class Content(string value) : IElement
   {
      public string Value { get; set; } = value;
      public bool IsBlock => false;

      public void FormatElement(int tabs, ref StringBuilder sb)
      {
         foreach (var element in Parsing.GetKeyValueList(Value)) 
            AddString(tabs, element.Value.Trim(), element.Key.Trim(), ref sb);
      }

      public override string ToString()
      {
         return Value;
      }
   }
   public interface IElement
   {
      public bool IsBlock { get; }
      public void FormatElement(int tabs, ref StringBuilder sb);
   }
}