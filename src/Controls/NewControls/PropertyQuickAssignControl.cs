using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.PopUps;
using Editor.Helper;
using Editor.Saving;
using Editor.src.Forms.Feature;

namespace Editor.Controls.NewControls
{
   public class PropertyQuickAssignControl<TSaveable, TProperty, TItem> : TableLayoutPanel, IPropertyControlList<TSaveable, TProperty, TItem> where TSaveable : Saveable where TProperty : List<TItem>, new() where TItem : notnull
   {
      // Controls
      private readonly Button _openEditor;
      private readonly Button _autoButton;
      private readonly Button _clearButton;
      private readonly Button _randomButton;
      private readonly ToolTip _toolTip = new();
      private CollectionSelectorBase _collectionSelectorBase;

      private readonly List<TItem> _source;
      private List<TItem> _startList = [];
      public readonly int MaxItems;

      private Func<int, List<TItem>> _autoSelectFunc;

      public PropertyInfo PropertyInfo { get; init; }
      private readonly PropertyInfo? _displayMember;

      protected readonly Func<List<TSaveable>> _getSaveables;
      private bool _rawStringMode;
      public PropertyQuickAssignControl(
         List<TItem> source,
         PropertyInfo prop,
         PropertyInfo? displayMember,
         string description,
         int maxItems,
         Func<int, List<TItem>> autoSelect,
         Func<List<TSaveable>> getSaveables,
         ref LoadGuiEvents.LoadAction<TSaveable> loadHandle)
      {
         Debug.Assert(prop is not null && prop.DeclaringType == typeof(TSaveable),
                      $"PropInfo: {prop} declaring type is not of type {typeof(TSaveable)} but of type {prop.DeclaringType}");
         Debug.Assert(prop.PropertyType == typeof(TProperty),
                      $"PropInfo: {prop} is not of type {typeof(TProperty)} but of type {prop.PropertyType}");
         if (displayMember == null)
         {
            Debug.Assert(typeof(TItem) == typeof(string) || typeof(TItem) == typeof(Tag), $"{typeof(TItem)} is not able to use raw sting mode");
            _rawStringMode = true;
         }
         else
            Debug.Assert(displayMember != null && displayMember.GetValue(source[0]) != null, $"{typeof(TItem)} does not define a display member \"{displayMember}\"");


         _source = source.ToList();
         PropertyInfo = prop;
         _displayMember = displayMember;
         _getSaveables = getSaveables;
         _autoSelectFunc = autoSelect;
         MaxItems = maxItems;
         if (!_rawStringMode)
            _collectionSelectorBase = new(AttributeHelper.GetDisplayMember(source, displayMember), maxItems);
         else
            _collectionSelectorBase = new([.. source.Cast<string>()], maxItems);

         if (AttributeHelper.GetSharedAttributeList<TSaveable, TProperty, TItem>(PropertyInfo, out var items, getSaveables.Invoke()))
            if (_rawStringMode)
               _collectionSelectorBase.SetSelectedItems([.. items.Cast<string>()]);
            else
               _collectionSelectorBase.SetSelectedItems(AttributeHelper.GetDisplayMember(items, displayMember));



         Label label = new()
         {
            Text = description,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };

         _openEditor = new()
         {
            Text = "open",
            Dock = DockStyle.Fill,
            Margin = new(1),
            BackColor = SystemColors.Control
         };

         _autoButton = new()
         {
            Text = "auto",
            Dock = DockStyle.Fill,
            Margin = new(1),
            BackColor = SystemColors.Control
         };

         _clearButton = new()
         {
            Text = "clear",
            Dock = DockStyle.Fill,
            Margin = new(1),
            BackColor = SystemColors.Control
         };

         _randomButton = new()
         {
            Text = "rand",
            Dock = DockStyle.Fill,
            Margin = new(1),
            BackColor = SystemColors.Control
         };

         Controls.Add(label, 0, 0);
         Controls.Add(_openEditor, 1, 0);
         Controls.Add(_autoButton, 2, 0);
         Controls.Add(_randomButton, 3, 0);
         Controls.Add(_clearButton, 4, 0);

         Margin = new(0);
         RowCount = 1;
         ColumnCount = 5;
         base.Dock = DockStyle.Fill;
         base.BackColor = Color.LightGray;

         ColumnStyles.Add(new(SizeType.Absolute, 100));
         ColumnStyles.Add(new(SizeType.Percent, 25));
         ColumnStyles.Add(new(SizeType.Percent, 25));
         ColumnStyles.Add(new(SizeType.Percent, 25));
         ColumnStyles.Add(new(SizeType.Percent, 25));

         _openEditor.Click += OpenEditor_Click;
         _autoButton.Click += AutoButton_Click;
         _clearButton.Click += ClearButton_Click;
         _randomButton.Click += RandomButton_Click;

         _toolTip.SetToolTip(_openEditor, $"Open the editor for {description}");
         _toolTip.SetToolTip(_autoButton, $"Auto select items, following a predefined algorithm");
         _toolTip.SetToolTip(_clearButton, $"Clear the items");
         _toolTip.SetToolTip(_randomButton, $"Randomly select items (only works properly if there is a hardcoded max otherwise assigns one item)");
         loadHandle += ((IPropertyControlList<TSaveable, TProperty, TItem>)this).LoadToGui;
      }

      private void RandomButton_Click(object? sender, EventArgs e)
      {
         List<TItem> items = [];

         if (MaxItems < 1)// we only do one random item as there is no fixed number of max items
            items = [_source[Globals.Random.Next(_source.Count)]];
         else
            for (var i = 0; i < MaxItems; i++)
               items.Add(_source[Globals.Random.Next(_source.Count)]);

         SetFromGui(items);
      }

      private void ClearButton_Click(object? sender, EventArgs e)
      {
         if (Globals.State != State.Running)
            return;
         Saveable.SetFieldEditCollection<TSaveable, TProperty, TItem>(_getSaveables.Invoke(), [], [.._startList], PropertyInfo);
         SetDefault();
      }

      private void AutoButton_Click(object? sender, EventArgs e)
      {
         SetFromGui(_autoSelectFunc(MaxItems));
      }

      private void OpenEditor_Click(object? sender, EventArgs e)
      {
         _collectionSelectorBase.ShowDialog();
         if(GetFromGui(out var value).Log())
            SetFromGui(value);
      }

      public void SetFromGui(List<TItem> value)
      {
         if (Globals.State == State.Running)
         {
            if (AttributeHelper.ScrambledListsEquals(_startList, value))
               return;
            //TODO make it more performant
            var remove = _startList.Except(value).ToHashSet();
            var add = value.Except(_startList).ToHashSet();
            Saveable.SetFieldEditCollection<TSaveable, TProperty, TItem>(_getSaveables.Invoke(), add, remove, PropertyInfo);
         }
      }

      public IErrorHandle GetFromGui(out TProperty value)
      {
         var strs = _collectionSelectorBase.GetSelectedItems();
         List<TItem> items = new(strs.Count);

         foreach (var str in strs)
            if (Converter.Convert<TItem>(str, PropertyInfo, out var partValue).Log())
               items.Add(partValue);

         value = (TProperty)items;
         return ErrorHandle.Success;
      }

      public void SetFromGui()
      {
         throw new EvilActions("Lol, Why do this? We don't do this here. Use the method with a parameter or do a world conquest to fix this.");
      }

      public void SetDefault()
      {
         SetValue((TProperty)new List<TItem>());
      }

      public void SetValue(TProperty value)
      {
         _startList = value;
         var values = _rawStringMode ? value.Cast<string>().ToList() : AttributeHelper.GetDisplayMember(value, _displayMember);
         _collectionSelectorBase.SetSelectedItems(values);
      }
   }
}