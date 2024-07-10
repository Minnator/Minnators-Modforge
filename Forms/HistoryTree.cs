using System;
using System.Windows.Forms;
using Editor.Commands;
using Editor.Controls;

namespace Editor.Forms
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
         HistoryTreeView.Nodes.Clear();
         var root = new HistoryTreeNode("Root", CommandHistoryType.Action);
         HistoryTreeView.Nodes.Add(root);
         AddToNode(rootNode, root);
         HistoryTreeView.Nodes.RemoveAt(0);
         HistoryTreeView.ExpandAll();
      }
      
      public void VisualizeFull(HistoryNode rootNode)
      {
         HistoryTreeView.Nodes.Clear();
         HistoryTreeView.Nodes.Add(AddToNodeFull(rootNode));
         HistoryTreeView.ExpandAll();
      }
      
      private static TreeNode AddToNodeFull(HistoryNode history)
      {
         var node = new HistoryTreeNode(history.Command.GetDescription(), history.Type)
         {
            Tag = history.Id
         };

         foreach (var child in history.Children)
            node.Nodes.Add(AddToNodeFull(child));

         return node;
      }

      private static void AddToNode(HistoryNode history, HistoryTreeNode parent)
      {
         // only add nodes if they are a ComplexSelection or an Action
         if (history.Type is CommandHistoryType.ComplexSelection or CommandHistoryType.Action)
         {
            var node = new HistoryTreeNode(history.Command.GetDescription(), history.Type)
            {
               Tag = history.Id
            };
            parent.Nodes.Add(node);
            parent = node;
         }

         foreach (var child in history.Children)
            AddToNode(child, parent);
      }

      private void RestoreButton_Click(object sender, EventArgs e)
      {
         if (HistoryTreeView.SelectedNode is null)
            return;

         _callback.Invoke((int)HistoryTreeView.SelectedNode.Tag);
         Close();
      }

      private void ShowAllSelections_CheckedChanged(object sender, EventArgs e)
      {
         if (ShowAllSelections.Checked)
         {
            VisualizeFull(Globals.HistoryManager.GetRoot());
         }
         else
         {
            Visualize(Globals.HistoryManager.GetRoot());
         }
      }
   }
}
