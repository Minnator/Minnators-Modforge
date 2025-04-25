using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Properties;
using Editor.Saving;
using Editor.src.Forms.GetUserInput;
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
      private Button _buttonImport;

      public PropertyNamesEditor(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables, string desc)
      {
         PropertyInfo = propertyInfo;

         loadHandle += ((IPropertyControlList<TSaveable, TProperty, string>)this).LoadToGui;
         _getSaveables = getSaveables;

         _timer.Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval;
         _timer.Tick += (_, _) => SetFromGui();

         Dock = DockStyle.Fill;
         ColumnCount = 2;
         ColumnStyles.Add(new(SizeType.Percent, 100));
         ColumnStyles.Add(new(SizeType.Absolute, 27));
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

         _buttonImport = new()
         {
            Height = 20,
            Width = 20,
            Anchor = AnchorStyles.None,
            Dock = DockStyle.None,
            Margin = new(1),
            Image = Resources.ImportSmall,
            ImageAlign = ContentAlignment.MiddleCenter,
         };

         Controls.Add(_label, 0, 0);
         Controls.Add(_textBox, 0, 1);
         Controls.Add(_buttonImport, 1, 0);
         SetColumnSpan(_textBox, 2);

         _textBox.KeyPress += TextBox_KeyPress;
         _textBox.Leave += (_, _) => SetFromGui();
         _buttonImport.Click += ButtonImport_Click;

         Globals.Settings.Gui.PropertyChanged += (sender, prop) =>
         {
            if (prop.PropertyName?.Equals(nameof(GuiSettings.TextBoxCommandCreationInterval)) ?? false) 
               _timer.Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval;
         };
      }

      private void ButtonImport_Click(object? sender, EventArgs e)
      {
         var nInputObjs = new NIntInputForm.NIntInputObj[2];
         nInputObjs[0] = new("Number of names to import", 0, -1, 15, false);
         nInputObjs[1] = new("Female name fraction", 0, 1, 0.1f, true);
         var result = NIntInputForm.ShowGet(nInputObjs, $"Import {PropertyInfo.Name} from Culture");

         if (result[0] < 0 || result[1] < 0)
            return;

         if (Selection.SelectedCountry != Country.Empty && Selection.SelectedCountry.HistoryCountry.PrimaryCulture != Culture.Empty)
         {
            var names = Selection.SelectedCountry.HistoryCountry.PrimaryCulture.SampleXNames((int)result[0], result[1]).ToList();
            if (GetFromGui(out List<string> current).Log())
            {
               current.AddRange(names);
               _textBox.Text = string.Join(", ", current);
               SetFromGui();
            }
         }
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