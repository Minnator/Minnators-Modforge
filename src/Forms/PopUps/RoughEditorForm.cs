using System.ComponentModel;
using System.Windows.Forms;
using Editor.DataClasses.Commands;
using Editor.Helper;

namespace Editor.Forms.PopUps
{
   public partial class RoughEditorForm : Form
   {
      private const int PROPERTY_GRID_ROW_HEIGHT = 19;
      private readonly object? _obj;
      private readonly bool _revertable;

      public RoughEditorForm(object obj, bool revertable = true)
      {
         if (revertable)
            if (!EditingHelper.DeepCopy(obj, out _obj))
               return;
         _revertable = revertable;

         SuspendLayout();
         InitializeComponent();
         PropGrid.SelectedObject = obj;
         string revertableString;
         if (revertable)
            revertableString = "(Revertable)";
         else
            revertableString = "(NOT Revertable)";
         base.Text = $"APE: {Convert.ChangeType(obj, obj.GetType())} {revertableString}";
         AdjustFormHeight();
         ResumeLayout();
      }

      // This method is takes in the name of Property and expands all the objects of that type
      public void ExpandObjectsOfType(params string[] propName)
      {
         var root = PropGrid.SelectedGridItem;
         //Get the parent
         while (root.Parent != null)
            root = root.Parent;

         if (root != null)
         {
            foreach (GridItem g in root.GridItems)
            {
               foreach (GridItem child in g.GridItems)
               {
                  if (child.GridItemType == GridItemType.Property && propName.Contains(child.Label))
                  {
                     child.Expanded = true;
                  }
               }
            }
         }

         AdjustFormHeight();
      }

      private GridItem? FindGridItem(PropertyGrid propertyGrid, string propName)
      {
         var rootGridItem = propertyGrid.SelectedGridItem; // Start at the root
         return FindGridItemRecursive(rootGridItem, propName);
      }

      private GridItem? FindGridItemRecursive(GridItem? gridItem, string propName)
      {
         if (gridItem == null) 
            return null;

         if (gridItem.Label!.Equals(propName, StringComparison.OrdinalIgnoreCase))
            return gridItem;

         // Recurse into child grid items
         foreach (GridItem childItem in gridItem.GridItems)
         {
            var found = FindGridItemRecursive(childItem, propName);
            if (found != null)
               return found;
         }

         return null;
      }

      private void AdjustFormHeight()
      {
         var desiredHeight = GetPropertyGridContentHeight() + SystemInformation.CaptionHeight;
         var screenHeight = Screen.FromControl(this).WorkingArea.Height;
         Height = Math.Min(desiredHeight, screenHeight);
         CenterToScreen();
      }

      private static int CountVisibleGridItems(GridItem parent)
      {
         if (parent == null!)
            return 0;

         var count = 0;

         foreach (GridItem item in parent.GridItems)
         {
            count++;
            if (item.Expanded) 
               count += CountVisibleGridItems(item);
         }

         return count;
      }

      private int GetPropertyGridContentHeight()
      {
         var root = PropGrid.SelectedGridItem;
         while (root.Parent != null)
            root = root.Parent;

         if (root == null!)
            return 0;

         return (CountVisibleGridItems(root) + 1) * PROPERTY_GRID_ROW_HEIGHT + 90; // 90 to account for description box and header
      }

      private void RoughEditorForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (_revertable)
            Globals.HistoryManager.AddCommand(new CAdvancedPropertiesEditing(_obj!, PropGrid.SelectedObject));
      }
   }
}
