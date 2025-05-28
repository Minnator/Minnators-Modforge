using System.Diagnostics;
using System.Reflection;
using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;

namespace Editor.Controls.PROPERTY
{
   public sealed class ProvinceHistoryEntryTreeView : PropertyTreeView<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>
   {
      private readonly ContextMenuStrip _itemMenuStrip;
      private ProvinceHistoryEntry? _clickedEntry;
      private int _clickedTokenIndex = -1;

      public ProvinceHistoryEntryTreeView(
         PropertyInfo? propertyInfo,
         ref LoadGuiEvents.LoadAction<Province> loadHandle,
         Func<List<Province>> getSaveables) :
         base(propertyInfo, ref loadHandle, getSaveables)
      {
         BorderStyle = BorderStyle.FixedSingle;
         Margin = new(3, 0, 0, 0);

         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, List<ProvinceHistoryEntry>>)this).LoadToGui;
         
         _itemMenuStrip = new()
         {
            ShowImageMargin = false,
            AutoSize = true,
         };

         var deleteButton = new ToolStripButton
         {
            Text = "Delete Entry",
            Name = "DeleteEntry",
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            AutoSize = true,
         };
         deleteButton.Click += DeleteEntry;

         var addNewHistoryEntry = new ToolStripButton
         {
            Text = "Add New Entry",
            Name = "AddNewEntry", 
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            AutoSize = true,
         };
         addNewHistoryEntry.Click += AddNewEntry;

         _itemMenuStrip.Items.Add(deleteButton);
         _itemMenuStrip.Items.Add(addNewHistoryEntry);
         
         //ContextMenuStrip = _itemMenuStrip;

         NodeMouseClick += OnNodeMouseClick;
      }

      private void AddNewEntry(object? sender, EventArgs e)
      {
         var saveables = GetSaveables();
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
            Saveable.SetFieldEditCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(GetSaveables.Invoke(), [entry], [], PropertyInfo);
         else
            Saveable.InsertInFieldCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(GetSaveables.Invoke()[0], entry, index, PropertyInfo);
      }

      private void DeleteEntry(object? sender, EventArgs e)
      {
         if (_clickedEntry is null)
            return;

         var saveables = GetSaveables();
         if (saveables.Count != 1)
            return;
         var index = saveables[0].History.BinarySearch(_clickedEntry);

         if (index == -1)
         {
            Saveable.RemoveInFieldCollection<Province, List<ProvinceHistoryEntry>, ProvinceHistoryEntry>(saveables[0], index, PropertyInfo);
            return;
         }

         Saveable.RemoveInNestedFieldCollection<Province, ProvinceHistoryEntry, List<IToken>, IToken>(saveables[0],
                                                                                                      _clickedTokenIndex,
                                                                                                      index,
                                                                                                      typeof(ProvinceHistoryEntry)
                                                                                                         .GetProperty(nameof(ProvinceHistoryEntry.Effects))!,
                                                                                                      PropertyInfo);
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
      
      // Returns the province history entry below the cursor also if it is not directly clicked but one of its children in which case we return the index of the child
      private (ProvinceHistoryEntry? entry, int index) GetEntryBelowCursorAll(TreeNodeMouseClickEventArgs e)
      {
         int index;
         var saveables = GetSaveables();
         if (e.Node.Parent is { } parent)
         {
            index = Nodes.IndexOf(parent);
            return (saveables[0].History[index], parent.Nodes.IndexOf(e.Node));
         }

         index = Nodes.IndexOf(e.Node);
         if (index == -1)
            return (null, -1);
         return (saveables[0].History[index], -1);
      }
      private void OnNodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
      {
         (_clickedEntry, _clickedTokenIndex) = GetEntryBelowCursorAll(e);
         if (e.Button == MouseButtons.Right)
         {
            _itemMenuStrip.Items[0].Enabled = _clickedTokenIndex >= 0;
            _itemMenuStrip.Items[0].Text = _itemMenuStrip.Items[0].Enabled
                                              ? $"Delete Entry {_clickedEntry!.Effects[_clickedTokenIndex].GetTokenName()}"
                                              : "Delete Entry";
            _itemMenuStrip.PerformLayout();
            
            SelectedNode = e.Node;
            _itemMenuStrip.Show(PointToScreen(e.Location));
         }
         else if (e.Button == MouseButtons.Left && _clickedEntry != null)
         {
            SelectedNode = e.Node;
            ProvinceHistoryManager.LoadDate(_clickedEntry.Date);
         }
      }
   }

   public class PropertyTreeView<TSaveable, TProperty, TItem> : TreeView, IPropertyControl<TSaveable, TProperty> where TSaveable : Saveable where TProperty : List<TItem>
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;

      private bool _renderTextNoTree = false;

      public PropertyTreeView(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable),
                      $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");

         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, TProperty>)this).LoadToGui;
         this.GetSaveables = getSaveables;
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