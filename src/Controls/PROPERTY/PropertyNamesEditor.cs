using System.Reflection;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls.PROPERTY
{
   public sealed class PropertyNamesEditor<TSaveable, TProperty> : TableLayoutPanel, IPropertyControlList<TSaveable, TProperty, string> where TSaveable : Saveable where TProperty : List<string>, new ()
   {
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<TSaveable>> _getSaveables;
      private Timer _timer = new();
      private List<string> _startList;

      // Controls
      private TextBox _textBox;
      private Label _label;

      public PropertyNamesEditor(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables, string desc)
      {
         PropertyInfo = propertyInfo;

         loadHandle += ((IPropertyControlList<TSaveable, TProperty, string>)this).LoadToGui;
         _getSaveables = getSaveables;

         _timer.Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval;
         _timer.Tick += (_, _) => SetFromGui();

         Dock = DockStyle.Fill;
         ColumnCount = 1;
         RowStyles.Add(new(SizeType.Absolute, 40));
         RowStyles.Add(new(SizeType.Percent, 100));
         Margin = new(0);



         _label = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(3),
            Text = desc,
            TextAlign = ContentAlignment.MiddleLeft
         };

         _textBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(3),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
         };

         Controls.Add(_label, 0, 0);
         Controls.Add(_textBox, 0, 1);

         _textBox.KeyPress += TextBox_KeyPress;
         _textBox.Leave += (_, _) => SetFromGui();

         Globals.Settings.Gui.PropertyChanged += (sender, prop) =>
         {
            if (prop.PropertyName?.Equals(nameof(GuiSettings.TextBoxCommandCreationInterval)) ?? false) 
               _timer.Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval;
         };
      }
      
      private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Enter)
         {
            SetFromGui();
            e.Handled = true;
         }
         else
            _timer.Start();
      }

      public void SetFromGui()
      {
         _timer.Stop();
         if (Globals.State == State.Running && GetFromGui(out List<string> value).Log())
         {
            if (AttributeHelper.ScrambledListsEquals(_startList, value))
               return;

            var remove = _startList.Except(value).ToHashSet();
            var add = value.Except(_startList).ToHashSet();
            Saveable.SetFieldEditCollection<TSaveable, TProperty, string>(_getSaveables.Invoke(), add, remove, PropertyInfo);
            _startList = value;
         }
      }

      public IErrorHandle GetFromGui(out List<string> value)
      {
         value = _textBox.Text.Split([','], StringSplitOptions.RemoveEmptyEntries)
                        .Select(name => name.Trim())
                        .ToList();
         return ErrorHandle.Success;
      }

      public void SetDefault()
      {
         _timer.Stop();
         _textBox.Text = string.Empty;
      }

      public IErrorHandle GetFromGui(out TProperty value)
      {
         var error = GetFromGui(out List<string> list);
         value = (TProperty)list;
         return error;
      }

      public void SetValue(TProperty value)
      {
         _startList = value;
         _textBox.Text = string.Join(", ", _startList);
      }
   }
}