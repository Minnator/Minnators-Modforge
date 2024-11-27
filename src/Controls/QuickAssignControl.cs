using System.ComponentModel;
using Editor.DataClasses.GameDataClasses;
using Editor.Forms.PopUps;
using Editor.Helper;

namespace Editor.Controls
{
   public class QuickAssignControl<T> : Control 
   {
      private readonly Button _openEditor;
      private readonly Button _autoButton;
      private readonly Button _clearButton;
      private readonly Button _randomButton;

      private readonly ToolTip _toolTip = new();

      private readonly List<T> _source;
      private List<T> _items;

      public readonly int MaxItems;

      private Func<int, List<T>>? _autoSelectFunc;

      private IGetSetProperty _propertyProvider;
      private readonly string _propName;

      public QuickAssignControl(ICollection<T> source, IGetSetProperty propertyProvider, string propName, string description, int maxItems)
      {
         _source = source.ToList();
         _propertyProvider = propertyProvider;
         _propName = propName;
         _items = propertyProvider.GetProperty(propName) as List<T> ?? [];
         MaxItems = maxItems;
         
         TableLayoutPanel tableLayoutPanel = new()
         {
            Margin = new (0),
            RowCount = 1,
            ColumnCount = 5,
            Dock = DockStyle.Fill,
            ColumnStyles =
            {
               new (SizeType.Absolute, 100),
               new (SizeType.Percent, 25),
               new (SizeType.Percent, 25),
               new (SizeType.Percent, 25),
               new (SizeType.Percent, 25),
            }
         };

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
            Margin = new(1)
         };

         _autoButton = new()
         {
            Text = "auto",
            Dock = DockStyle.Fill,
            Margin = new(1)
         };

         _clearButton = new()
         {
            Text = "clear",
            Dock = DockStyle.Fill,
            Margin = new(1)
         };

         _randomButton = new()
         {
            Text = "rand",
            Dock = DockStyle.Fill,
            Margin = new(1)
         };

         tableLayoutPanel.Controls.Add(label, 0, 0);
         tableLayoutPanel.Controls.Add(_openEditor, 1, 0);
         tableLayoutPanel.Controls.Add(_autoButton, 2, 0);
         tableLayoutPanel.Controls.Add(_randomButton, 3, 0);
         tableLayoutPanel.Controls.Add(_clearButton, 4, 0);

         base.Dock = DockStyle.Fill;
         Margin = new (0);
         Controls.Add(tableLayoutPanel);

         _openEditor.Click += OpenEditor_Click;
         _autoButton.Click += AutoButton_Click;
         _clearButton.Click += ClearButton_Click;
         _randomButton.Click += RandomButton_Click;

         _toolTip.SetToolTip(_openEditor, $"Open the editor for {description}");
         _toolTip.SetToolTip(_autoButton, $"Auto select items, following a predefined algorithm");
         _toolTip.SetToolTip(_clearButton, $"Clear the items");
         _toolTip.SetToolTip(_randomButton, $"Randomly select items (only works properly if there is a hardcoded max otherwise assigns one item)");

      }

      public void SetAutoSelectFunc(Func<int, List<T>> autoSelectFunc) => _autoSelectFunc = autoSelectFunc;
      public void SetItems(List<T> items) => _items = items;

      public void UpdateCountry(IGetSetProperty provider)
      {
         _propertyProvider = provider;
         UpdateItems();
      }


      public void UpdateItems()
      {
         _items = _propertyProvider.GetProperty(_propName) as List<T> ?? [];
      }

      public void Clear() => _items.Clear();

      private void RandomButton_Click(object? sender, EventArgs e)
      {
         if (MaxItems < 1)// we only do one random item as there is no fixed number of max items
         {
            _items = [_source[Globals.Random.Next(_source.Count)]];
            SaveItems();
            return;
         }

         _items.Clear();
         for (var i = 0; i < MaxItems; i++) 
            _items.Add(_source[Globals.Random.Next(_source.Count)]);

         SaveItems();
      }

      private void ClearButton_Click(object? sender, EventArgs e)
      {
         _items.Clear();
         SaveItems();
      }

      private void AutoButton_Click(object? sender, EventArgs e)
      {
         if (_autoSelectFunc == null)
            return;

         _items = _autoSelectFunc(MaxItems);
         SaveItems();
      }

      private void OpenEditor_Click(object? sender, EventArgs e)
      {
         UpdateItems();
         using StringCollectionEditor editor = new(_source.Select(x => x.ToString()).ToList()!, [.._items.Select(x => x.ToString())!]);
         editor.ShowDialog();
         _items = ConvertItems(editor.Items);
         SaveItems();
      }

      private List<T> ConvertItems(BindingList<string> itemNames)
      {
         if (typeof(T) == typeof(string))
            return itemNames.Cast<T>().ToList();
         List<T> items = [];
         foreach (var itemName in itemNames)
         {
            var item = _source.FirstOrDefault(x => x?.ToString() == itemName);
            if (item != null)
               items.Add(item);
         }
         return items;
      }

      private void SaveItems()
      {
         _propertyProvider.SetProperty(_propName, _items);
      }
   }
}