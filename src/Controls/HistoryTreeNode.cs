namespace Editor.Controls;

public class HistoryTreeNode(CommandHistoryType commandHistoryType) : TreeNode
{
   public CommandHistoryType CommandHistoryType { get; } = commandHistoryType;

   public HistoryTreeNode(string text, CommandHistoryType commandHistoryType): this(commandHistoryType)
   {
      Text = text;
   }
}