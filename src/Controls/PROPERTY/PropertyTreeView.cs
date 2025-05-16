using Editor.Saving;
using System.Numerics;
using System.Reflection;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using System.Diagnostics;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Newtonsoft.Json.Linq;
using System.Drawing.Imaging;

namespace Editor.Controls.PROPERTY
{
   public sealed class ProvinceHistoryEntryTreeView : PropertyTreeView<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>
   {
      private ContextMenuStrip _itemMenuStrip;
      private ProvinceHistoryEntry? _clickedEntry;

      public ProvinceHistoryEntryTreeView(
         PropertyInfo? propertyInfo,
         ref LoadGuiEvents.LoadAction<Province> loadHandle,
         Func<List<Province>> getSaveables) :
         base(propertyInfo, ref loadHandle, getSaveables)
      {
         BorderStyle = BorderStyle.FixedSingle;
         Margin = new(3, 0, 0, 0);

         var deleteButton = new ToolStripButton
         {
            Text = "Delete Entry",
            Name = "DeleteEntry",
            DisplayStyle = ToolStripItemDisplayStyle.Text
         };
         deleteButton.Click += DeleteEntry;

         _itemMenuStrip = new ();
         var modifyButton = new ToolStripButton
         {
            Text = "Modify Value",
            Name = "ModifyValue",
            DisplayStyle = ToolStripItemDisplayStyle.Text
         };
         modifyButton.Click += ModifyValue;

         var addNewHistoryEntry = new ToolStripButton
         {
            Text = "Add New Entry",
            Name = "AddNewEntry", 
            DisplayStyle = ToolStripItemDisplayStyle.Text
         };
         addNewHistoryEntry.Click += AddNewEntry;

         _itemMenuStrip.Items.Add(modifyButton);
         _itemMenuStrip.Items.Add(deleteButton);
         _itemMenuStrip.Items.Add(addNewHistoryEntry);

         _itemMenuStrip.Opening += (sender, args) =>
         {
            _clickedEntry = GetEntryBelowCursor();
            if (_clickedEntry is null)
            {
               _itemMenuStrip.Items[0].Enabled = getSaveables().Count == 1;
               _itemMenuStrip.Items[1].Enabled = false;
            }
            else
            {
               _itemMenuStrip.Items[0].Enabled = true;
               _itemMenuStrip.Items[1].Enabled = true;
            }

         };

         ContextMenuStrip = _itemMenuStrip;

         MouseDown += OnLeftMouseButtonDown;
      }

      private void ModifyValue(object? sender, EventArgs e)
      {
         
      }

      private void AddNewEntry(object? sender, EventArgs e)
      {
         var saveables = getSaveables();
         if (saveables.Count != 1)
            return;

         ProvinceHistoryEntry entry;
         int index;
         if (_clickedEntry == null)
         {
            entry = new(new(Globals.MapWindow.DateControl.Date.TimeStamp), HistoryEntryManager.PHEIndex++);
            index = saveables[0].History.BinarySearch(entry);
         }
         else
         {
            entry = new(new(_clickedEntry.Date.TimeStamp + 1), HistoryEntryManager.PHEIndex++);
            index = saveables[0].History.BinarySearch(_clickedEntry);
         }

         if (index < 0)
            index = ~index;
         else
            index++;

         if (index == saveables[0].History.Count)
            Saveable.SetFieldEditCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(getSaveables.Invoke(), [entry], [], PropertyInfo);
         else
            Saveable.InsertInFieldCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(getSaveables.Invoke()[0], entry, index, PropertyInfo);
      }

      private void DeleteEntry(object? sender, EventArgs e)
      {
         if (_clickedEntry is null)
            return;

         var saveables = getSaveables();
         if (saveables.Count != 1)
            return;
         var index = saveables[0].History.BinarySearch(_clickedEntry);
         
         Saveable.RemoveInFieldCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(saveables[0], index, PropertyInfo);
      }
      
      public override void SetValue(List<ProvinceHistoryEntry> value)
      {
         Nodes.Clear();
         var sb = new StringBuilder();
         foreach (var entry in value)
         {
            var entryNode = new TreeNode(entry.Date);
            foreach (var effect in entry.Effects)
            {
               effect.GetTokenString(0, ref sb);
               var effectNode = new TreeNode(sb.ToString());
               entryNode.Nodes.Add(effectNode);
               sb.Clear();
            }

            Nodes.Add(entryNode);
         }

         ExpandAll();

         // how to edit:
         /*
          * Enable the GUI to edit the history
          * RMB to delete an effect / entry
          * LMB to select and mark as this will be edited --> all changes are applied here
          * When history is enabled no history selected --> Create new entries on changes
          */

         Invalidate();
      }

      public ProvinceHistoryEntry? GetEntryBelowCursor()
      {
         var saveables = getSaveables();
         if (saveables.Count != 1)
            return null;

         var entry = GetNodeAt(PointToClient(Cursor.Position));

         if (entry is null)
            return null;

         var index = Nodes.IndexOf(entry);
         if (index == -1) 
            return null;
         return saveables[0].History[index];
      }

      public void OnLeftMouseButtonDown(object? sender, MouseEventArgs e)
      {
         if (e.Button == MouseButtons.Left)
         {
            var entry = GetEntryBelowCursor();
            if (entry is not null)
            {
               ProvinceHistoryManager.LoadDate(entry.Date);
            }
         }
      }
   }

   public class PropertyTreeView<TSaveable, TProperty, TItem> : TreeView, IPropertyControl<TSaveable, TProperty> where TSaveable : Saveable where TProperty : List<TItem>
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> getSaveables;

      private bool _renderTextNoTree = false;

      public PropertyTreeView(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable),
                      $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");

         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, TProperty>)this).LoadToGui;
         this.getSaveables = getSaveables;
      }

      public virtual void SetFromGui()
      {
         throw new NotImplementedException();
      }

      public virtual void SetDefault()
      {
         _renderTextNoTree = true;
         Invalidate();
      }

      public virtual IErrorHandle GetFromGui(out TProperty value)
      {
         throw new NotImplementedException();
      }

      public virtual void SetValue(TProperty value)
      {
         _renderTextNoTree = false;

         Invalidate();
      }
      
      protected override void OnDrawNode(DrawTreeNodeEventArgs e)
      {
         if (_renderTextNoTree)
         {
            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), e.Bounds);
         }
         else
         {
            base.OnDrawNode(e);
         }
      }

      protected override void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
      {
         base.OnNodeMouseHover(e);
      }

      // override the paint event to render nothing if _renderTextNoTree is true
      protected override void OnPaint(PaintEventArgs e)
      {
         if (_renderTextNoTree)
            e.Graphics.DrawString("Select at max one province to view history", Font, new SolidBrush(ForeColor), ClientRectangle);
         else
            base.OnPaint(e);
      }
   }
}