using System;
using Editor.Commands;
using System.Windows.Forms;

namespace Editor
{
   public partial class HistoryTree : Form
   {
      private readonly Action<int> _callback;
      public HistoryTree(Action<int> callback)
      {
         InitializeComponent();
         _callback = callback;
      }

      public void Visualize(HistoryNode rootNode)
      {
         var root = AddToNode(rootNode);

         HistoryTreeView.Nodes.Clear();
         HistoryTreeView.Nodes.Add(root);
         HistoryTreeView.ExpandAll();
      }


      private static TreeNode AddToNode(HistoryNode history)
      {
         var node = new TreeNode(history.Command.GetDescription());
         node.Tag = history.Id;

         foreach (var child in history.Children) 
            node.Nodes.Add(AddToNode(child));

         return node;
      }

      private void RestoreButton_Click(object sender, System.EventArgs e)
      {
         if (HistoryTreeView.SelectedNode is null)
            return;

         _callback.Invoke((int)HistoryTreeView.SelectedNode.Tag);
         Close();
      }
   }
}
