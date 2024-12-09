using Editor.DataClasses.Misc;
using Editor.Forms.PopUps;
using Editor.Saving;

namespace Editor.Forms.Feature
{
   public partial class EditedObjectsExplorer : Form
   {
      private Dictionary<SaveableType, List<Saveable>> Cache = new();
      private TreeView mainTreeView = new()
      {
         Dock = DockStyle.Fill
      };
      public EditedObjectsExplorer()
      {
         InitializeComponent();
         MainTLP.Controls.Add(mainTreeView, 0, 0);
         mainTreeView.NodeMouseDoubleClick += mainTreeView_NodeMouseDoubleClick;
         RestructureCache();

         SaveMaster.Saving += (_, _) => RestructureCache();
         SaveMaster.SaveMasterModified += (_, a) =>
         {
            var add = a.Item1;
            var saveable = a.Item2;
            if (add)
               Cache[saveable.WhatAmI()].Add(saveable);
            else
               Cache[saveable.WhatAmI()].Remove(saveable);
            GenerateTreeView();
         };
      }



      private void RestructureCache()
      {
         Cache.Clear();
         var editedTypes = SaveMaster.Cache.Keys;

         foreach (var type in editedTypes)
            Cache.Add(type, SaveMaster.GetModifiedObjectsOfType(type));

         GenerateTreeView();
      }

      private void GenerateTreeView()
      {
         mainTreeView.Nodes.Clear();
         foreach (var kvp in Cache)
         {
            if (kvp.Value.Count == 0)
               continue;
            var typeNode = new TreeNode(kvp.Key.ToString());
            mainTreeView.Nodes.Add(typeNode);

            foreach (var saveable in kvp.Value)
            {
               var saveableNode = new TreeNode(saveable.ToString())
               {
                  Tag = saveable
               };
               typeNode.Nodes.Add(saveableNode);
            }

            typeNode.Expand();
         }
      }

      private void mainTreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
      {
         if (e.Node.Tag is not Saveable saveable)
            return;

         new RoughEditorForm(saveable, false).ShowDialog();
      }

      private void RefreshButton_Click(object sender, EventArgs e)
      {
         RestructureCache();
      }
   }
}
