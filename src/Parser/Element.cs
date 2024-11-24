using System.Text;
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

      public string GetFormattedElement(int tabs)
      {
         var sb = new StringBuilder();
         AddTabs(tabs, ref sb);
         sb.AppendLine($"{Name} = {{");
         foreach (var element in Blocks) 
            sb.AppendLine(element.GetFormattedElement(tabs + 1));
         AddTabs(tabs, ref sb);
         sb.Append('}');
         return sb.ToString();
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

      public string GetFormattedElement(int tabs)
      {
         var sb = new StringBuilder();
         var split = Value.Split(Environment.NewLine);
         for (var i = 0; i < split.Length; i++)
         {
            var lineContent = split[i].Trim();
            if (string.IsNullOrWhiteSpace(lineContent))
               continue;
            AddTabs(tabs, ref sb);
            sb.Append(lineContent);
            if (i < split.Length - 1)
               sb.Append(Environment.NewLine);
         }

         return sb.ToString();
      }

      public override string ToString()
      {
         return Value;
      }
   }
   public interface IElement
   {
      public bool IsBlock { get; }
      public string GetFormattedElement(int tabs);
   }
}