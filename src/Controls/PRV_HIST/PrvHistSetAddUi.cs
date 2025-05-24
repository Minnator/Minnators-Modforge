using System.Diagnostics;
using System.Reflection;
using System.Text;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.Commands;
using Editor.DataClasses.DataStructures;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Misc;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Helper;
using Editor.Loading.Enhanced.PCFL.Implementation;
using Editor.Saving;
using IToken = Editor.Loading.Enhanced.PCFL.Implementation.IToken;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls.PRV_HIST
{
   public class PrvHistCollectionUi<TProperty, TItem> : PrvHistSetAddUi<TProperty>, IPrvHistDualEffectPropControl<TProperty, TItem>
      where TProperty : List<TItem>, new() where TItem : notnull
   {
      private TableLayoutPanel Tlp { get; }
      private Button EditButton { get; }
      private readonly Label _shortInfoLabel;
      private ListDeltaSetSelection<TItem>? _form;

      public Func<TItem, IToken> AddEffectToken { get; init; }
      public Func<TItem, IToken> RemoveEffectToken { get; init; }
      public sealed override PropertyInfo PropertyInfo { get; init; }
      private readonly TItem _defaultValue;
      private readonly TItem[] _source;
      private List<TItem> _startList = [];

      public PrvHistCollectionUi(TItem defaultValue, string text, PropertyInfo info, Func<TItem, IToken> addEffect, Func<TItem, IToken> removeEffect, List<TItem> source, bool hasSetBox = false) 
         : base(null!, text, new TableLayoutPanel
         {
            ColumnCount = 2,
            RowCount = 1,
            Dock = DockStyle.Fill,
         }, hasSetBox)
      {
         PropertyInfo = info;
         AddEffectToken = addEffect;
         RemoveEffectToken = removeEffect;
         _defaultValue = defaultValue;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPrvHistDualEffectPropControl<TProperty, TItem>)this).LoadToGui;
         Debug.Assert(_startList != null);
         LoadGuiEvents.ProvHistoryLoadAction += (_, _, _) => SetShortInfo();
         _source = source.ToArray();


         Tlp = (TableLayoutPanel)Controls[2]; // keep a reference directly
         Tlp.ColumnStyles.Clear();
         Tlp.RowStyles.Clear();
         Tlp.ColumnStyles.Add(new(SizeType.Absolute, 30));
         Tlp.ColumnStyles.Add(new(SizeType.Percent, 100));
         Tlp.RowStyles.Add(new(SizeType.Percent, 100));
         Tlp.Dock = DockStyle.Fill;

         EditButton = new()
         {
            Dock = DockStyle.Fill,
            Text = "...",
            Font = new("Arial", 8, FontStyle.Regular),
            Padding = new(0),
            Margin = new(0),
         };

         EditButton.Click += EditButton_Click;

         _shortInfoLabel = new()
         {
            Text = "Current",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new("Arial", 8, FontStyle.Regular),
         };

         Tlp.Controls.Add(EditButton, 0, 0);
         Tlp.Controls.Add(_shortInfoLabel, 1, 0);

         SetShortInfo();
      }

      private void EditButton_Click(object? sender, EventArgs e)
      {
         _form = new("Edit", _source, GetCurrentShared().ToArray(), SetCheckBox.Checked);
         _form.ShowDialog();

         if (GetFromGui(out var value).Log())
            SetFromGui(value);
      }

      private const int MAX_CHARS_IN_INFO = 10;
      private const int BIG_SCALE_FONT = 8;
      private const int SMALL_SCALE_FONT = 6;

      private void SetShortInfo()
      {
         if (ProvinceHistoryManager.CurrentLoadedDate == Date.MinValue)
            return;

         var sb = new StringBuilder();
         foreach (var item in _startList)
         {
            sb.Append(item.ToString()?.ToUpper());
            sb.Append(", ");
         }
         if (sb.Length > 2)
            sb.Remove(sb.Length - 2, 2);

         if (sb.Length > MAX_CHARS_IN_INFO)
         {
            _shortInfoLabel.Font = new("Arial", SMALL_SCALE_FONT);
         }
         else
            _shortInfoLabel.Font = new("Arial", BIG_SCALE_FONT);

         _shortInfoLabel.Text = sb.ToString();
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

            List<IToken> tokens = [];
            foreach (var item in remove)
               tokens.Add(RemoveEffectToken(item));
            foreach (var item in add)
               tokens.Add(AddEffectToken(item));

            var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, tokens, ProvinceHistoryManager.CurrentLoadedDate, out var changed);
            if (changed)
               HistoryManager.AddCommand(command);

            SetShortInfo();
         }
      }

      private List<TItem> GetCurrentShared()
      {
         if (Selection.Count <= 1)
            return _startList;

         if (AttributeHelper.GetSharedAttributeList<Province, TProperty, TItem>(PropertyInfo, out var value, Selection.GetSelectedProvinces))
            return value;
         return [];
      }

      public override IErrorHandle GetFromGui(out TProperty value)
      {
         Debug.Assert(_form != null, "_form != null");

         List<TItem> current = [];
         foreach (var str in _form.Selection)
            if (Converter.Convert<TItem>(str, PropertyInfo, out var partValue).Log())
               current.Add(partValue);

         value = (TProperty)current;
         return ErrorHandle.Success;
      }

      public override void SetFromGui()
      {
         throw new EvilActions("Lol, Why do this? We don't do this here. Use the method with a parameter or do a world conquest to fix this.");
      }

      public override void SetDefault()
      {
         SetValue((TProperty)new List<TItem>());
      }

      public override void SetValue(TProperty value)
      {
         _startList = value;
      }

      protected override IToken GetDefaultEffectToken() => AddEffectToken(_defaultValue);
      protected IToken GetDefaultRemoveEffectToken() => RemoveEffectToken(_defaultValue);

      public new void LoadToGui(List<Province> list, PropertyInfo propInfo, bool force)
      {
         if (force || PropertyInfo.Equals(propInfo))
         {

            var (entries, shareEntry) = ProvinceHistoryManager.BinarySearchDateExactMultiple(
                                                                                             list.Select(x => x.History.ToList()).ToList(),
                                                                                             ProvinceHistoryManager.CurrentLoadedDate);

            TProperty? sharedValue = null;
            if (shareEntry)
               if (ShareEntriesValue(entries, GetDefaultEffectToken().GetTokenName(), out sharedValue))
                  SetValue(sharedValue!);
               else if (ShareEntriesValue(entries, GetDefaultRemoveEffectToken().GetTokenName(), out sharedValue))
                  SetValue(sharedValue!);
            if (AttributeHelper.GetSharedAttribute(PropertyInfo, out TProperty value, list))
            {
               LoadCurrent(value);
               if (sharedValue == null)
                  SetDefault();
            }

            else
            {
               SetDefault();
               ResetCurrent();
            }

            return;
         }
         SetDefault();
         ResetCurrent();
      }

      protected override bool ShareEntriesValue(ProvinceHistoryEntry?[] entries, string tokenName, out TProperty? value) 
      {
         if (entries.Length == 0)
         {
            value = null!;
            return false;
         }

         var allContainToken = true;
         var hasSetInitVal = false;
         value = null!;

         for (var i = 0; i < entries.Length; i++)
         {
            var fToken = entries[i]?.Effects.FindAll(x => x.GetTokenName().Equals(tokenName));
            if (fToken != null && fToken?.Count > 0)
            {
               if (!fToken.All(x => x is SimpleEffect<TItem>))
                  continue;
               var sEffects = fToken.Cast<SimpleEffect<TItem>>().ToList();
               if (hasSetInitVal == false)
               {
                  value = (TProperty)(List<TItem>) [];
                  foreach (var sEffect in sEffects)
                     if (sEffect._value.Val is { } item)
                        value.Add(item);

                  hasSetInitVal = true;
               }
               else
               {
                  if (value.Count != sEffects.Count || !AttributeHelper.ScrambledListsEquals(value, sEffects.Select(x => x._value.Val).ToList()))
                  {
                     allContainToken = false;
                     break;
                  }
               }

               continue;
            }
            allContainToken = false;
            break;
         }

         return allContainToken;
      }

      public override void LoadCurrent(TProperty value)
      {
         Debug.Assert(value != null, "value is null but must never be null");
         var sb = new StringBuilder();
         foreach (var item in value)
         {
            sb.Append(item.ToString()?.ToUpper());
            sb.Append(", ");
         }
         if (sb.Length > 2)
            sb.Remove(sb.Length - 2, 2);

         if (sb.Length > MAX_CHARS_IN_INFO)
         {
            CurrentLabel!.Font = new("Arial", SMALL_SCALE_FONT);
         }
         else
            CurrentLabel!.Font = new("Arial", BIG_SCALE_FONT);

         SetCurrentLabel(sb.ToString());
      }

      public override void ResetCurrent() => SetCurrentLabel(string.Empty);
      public void SetClipboard() { }
      public void Paste() { }
   }

   public class PrvHistFloatUi : PrvHistSetAddUiBoth<float>
   {
      public NumericUpDown FloatNumeric { get; }
      public sealed override PropertyInfo PropertyInfo { get; init; }

      private float _lastValue = -1;
      private readonly Timer _timer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval
      };

      public PrvHistFloatUi(string text,
                            PropertyInfo info,
                            Func<float, IToken> effect,
                            Func<float, IToken> setEffect,
                            float value = 0,
                            float min = 0,
                            float max = 100,
                            bool hasSet = true)
         : base(effect, setEffect, text, new NumericUpDown
         {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Right,
            DecimalPlaces = 2,
            Increment = 0.05m,
         }, hasSet)
      {
         PropertyInfo = info;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, float>)this).LoadToGui;

         FloatNumeric = (NumericUpDown)Controls[2]; // keep a reference directly
         FloatNumeric.Minimum = (decimal)min;
         FloatNumeric.Maximum = (decimal)max;
         FloatNumeric.Value = (decimal)value;

         FloatNumeric.KeyDown += NumericOnKeyDown;
         FloatNumeric.Enter += NumericOnEnter;
         FloatNumeric.Leave += NumericOnLeave;
      }
      public override void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log() || Math.Abs(value - _lastValue) < 0.005f)
            return;

         var token = SetCheckBox.Checked ? SetEffectToken(value) : EffectToken(value);
         var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, [token], ProvinceHistoryManager.CurrentLoadedDate, out var changed);
         if (changed)
            HistoryManager.AddCommand(command);

         _lastValue = value;
      }
      public override void SetDefault()
      {
         FloatNumeric.Value = FloatNumeric.Minimum;
         _lastValue = (float)FloatNumeric.Value;
      }

      public override void SetValue(float value)
      {
         FloatNumeric.Value = (decimal)value;
         _lastValue = value;
      }

      public override IErrorHandle GetFromGui(out float value)
      {
         value = (float)FloatNumeric.Value;
         return ErrorHandle.Success;
      }

      private void NumericOnKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            SetFromGui();
            _timer.Stop();
            e.Handled = true;
         }
      }

      private void NumericOnEnter(object? sender, EventArgs e)
      {
         _timer.Stop();
         _timer.Start();
      }

      private void NumericOnLeave(object? sender, EventArgs e)
      {
         _timer.Stop();
         SetFromGui();
      }

      public override void LoadCurrent(float value) => SetCurrentLabel(value.ToString("0.00"));
      public override void ResetCurrent() => SetCurrentLabel("0.00");
      protected override IToken GetDefaultEffectToken() => SetEffectToken(0f);
   }
   public class PrvHistTextBoxUi : PrvHistSetAddUi<string>
   {
      public TextBox TextBox { get; }
      private string _lastValue = string.Empty;
      public sealed override PropertyInfo PropertyInfo { get; init; }
      private readonly Timer _timer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval
      };

      public PrvHistTextBoxUi(string text, Func<string, IToken> effectToken, PropertyInfo propInfo)
         : base(effectToken, text, new TextBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         PropertyInfo = propInfo;
         TextBox = (TextBox)Controls[2]; // keep a reference directly

         _timer.Tick += (_, _) =>
         {
            SetFromGui();
         };

         TextBox.KeyDown += (_, e) =>
         {
            _timer.Stop();
            if (e.KeyCode == Keys.Enter)
            {
               SetFromGui();
               e.Handled = true;
               return;
            }
            _timer.Start();
         };
      }

      public override void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log() || _lastValue == value)
            return;

         var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, [EffectToken(value)], ProvinceHistoryManager.CurrentLoadedDate, out var changed);
         if (changed)
            HistoryManager.AddCommand(command);

         _lastValue = value;
         _timer.Stop();
      }

      public override void SetDefault()
      {
         TextBox.Text = string.Empty;
         _lastValue = string.Empty;
      }

      public override IErrorHandle GetFromGui(out string value)
      {
         value = TextBox.Text;
         return ErrorHandle.Success;
      }

      public override void SetValue(string value)
      {
         Debug.Assert(value != null, "value is null but must never be null");
         TextBox.Text = value;
         _lastValue = value;
      }

      public override void LoadCurrent(string value) => SetCurrentLabel($"\"{value}\"");
      public override void ResetCurrent() => SetCurrentLabel(string.Empty);
      protected override IToken GetDefaultEffectToken() => EffectToken(string.Empty);
   }
   public class BindablePrvHistDropDownUi<TProperty, TKey> : PrvHistDropDownUi<TProperty> where TProperty : notnull where TKey : notnull
   {
      private readonly BindingDictionary<TKey, TProperty> _items;
      public BindablePrvHistDropDownUi(string text,
                                       PropertyInfo info,
                                       TProperty defaultValue,
                                       Func<TProperty, IToken> effect,
                                       BindingDictionary<TKey, TProperty> items,
                                       bool isTagBox,
                                       bool isDropDownList = false)
         : base(text, info, defaultValue, effect, isTagBox, isDropDownList)
      {
         _items = items;
         DropDown.DataSource = new BindingSource(_items, null);
         _items.AddControl(DropDown);
      }

      ~BindablePrvHistDropDownUi()
      {
         _items.RemoveControl(DropDown);
      }
      
      public new void SetDefault()
      {
         var item = _items.EmptyItem.Key.ToString() ?? string.Empty;
         DropDown.SelectedText = item;
         DropDown.SelectedIndex = -1;
         DropDown.Text = item;
      }


      public new IErrorHandle GetFromGui(out TProperty value)
      {
         var handle = Converter.Convert(DropDown.Text, out TKey key);
         if (!handle.Ignore())
         {
            value = default!;
            return handle;
         }
         if (_items.TryGetValue(key, out value!))
            return handle;
         return new ErrorObject(ErrorType.INTERNAL_KeyNotFound, "Key not found in dictionary", LogType.Critical, addToManager: false);
      }
   }
   public class PrvHistDropDownUi<TProperty> : PrvHistSetAddUi<TProperty> where TProperty : notnull
   {
      public ComboBox DropDown { get; }
      public sealed override PropertyInfo PropertyInfo { get; init; }
      private int _lastIndex = -1;
      private readonly TProperty _defaultValue;

      public PrvHistDropDownUi(string text,
                               PropertyInfo info,
                               TProperty defaultValue,
                               Func<TProperty, IToken> effect,
                               bool isTagBox,
                               bool isDropDownList = false)
         : base(effect, text, isTagBox ? new TagComboBox
         {
            Dock = DockStyle.Fill,
         }: new ComboBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         _defaultValue = defaultValue;
         PropertyInfo = info;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, TProperty>)this).LoadToGui;

         if (isTagBox)
            DropDown = (TagComboBox)Controls[2]; // keep a reference directly
         else
            DropDown = (ComboBox)Controls[2]; // keep a reference directly 
         DropDown.DropDownStyle = isDropDownList ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;

         DropDown.SelectedIndexChanged += (_, _) => SetFromGui();
      }

      public override void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log() || _lastIndex == DropDown.SelectedIndex)
            return;

         var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, [EffectToken(value)], ProvinceHistoryManager.CurrentLoadedDate, out var changed);
         if (changed)
            HistoryManager.AddCommand(command);

         _lastIndex = DropDown.SelectedIndex;
      }
      public override void SetDefault()
      {
         DropDown.SelectedText = "";
         DropDown.SelectedIndex = -1;
         DropDown.Text = "";
      }

      public override void SetValue(TProperty value)
      {
         Debug.Assert(value != null, "value is null but must never be null");
         DropDown.Text = value.ToString();
      }
      public override IErrorHandle GetFromGui(out TProperty value) => Converter.Convert(DropDown.Text, out value);
      public override void ResetCurrent() => SetCurrentLabel(string.Empty);
      protected override IToken GetDefaultEffectToken() => EffectToken(_defaultValue);

      public override void LoadCurrent(TProperty value)
      {
         Debug.Assert(value != null, "value is null but must never be null");
         SetCurrentLabel(value.ToString() ?? string.Empty);
      }

   }
   public class PrvHistIntUi : PrvHistSetAddUiBoth<int>
   {
      private NumericUpDown IntNumeric { get; }
      public sealed override PropertyInfo PropertyInfo { get; init; }
      private readonly Timer _timer = new()
      {
         Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval
      };

      private int _lastValue = -1;

      public PrvHistIntUi(string text,
                          PropertyInfo info,
                          Func<int, IToken> effect,
                          Func<int, IToken> setEffect,
                          int value = 0,
                          int min = 0,
                          int max = 100,
                          bool hasSet = true)
         : base(effect, setEffect, text, new NumericUpDown
         {
            Dock = DockStyle.Fill,
            TextAlign = HorizontalAlignment.Right
         }, hasSet)
      {
         PropertyInfo = info;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, int>)this).LoadToGui;

         IntNumeric = (NumericUpDown)Controls[2]; // keep a reference directly
         IntNumeric.Minimum = min;
         IntNumeric.Maximum = max;
         IntNumeric.Value = value;

         IntNumeric.KeyDown += NumericOnKeyDown;
         IntNumeric.Enter += NumericOnEnter;
         IntNumeric.Leave += NumericOnLeave;

         _timer.Tick += (_, _) =>
         {
            SetFromGui();
            _timer.Stop();
         };
      }

      private void NumericOnKeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            SetFromGui();
            _timer.Stop();
            e.Handled = true;
         }
      }

      private void NumericOnEnter (object? sender, EventArgs e)
      {
         _timer.Stop();
         _timer.Start();
      }

      private void NumericOnLeave (object? sender, EventArgs e)
      {
         _timer.Stop();
         SetFromGui();
      }

      public override void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log() || _lastValue == value)
            return;

         var token = SetCheckBox.Checked ? SetEffectToken(value) : EffectToken(value);
         var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, [token], ProvinceHistoryManager.CurrentLoadedDate, out var changed);
         if(changed)
            HistoryManager.AddCommand(command);

         _lastValue = value;
      }
      public override void SetDefault()
      {
         IntNumeric.Value = IntNumeric.Minimum; 
         _lastValue = (int)IntNumeric.Value;
      }
      
      public override void SetValue(int value) => IntNumeric.Value = _lastValue = value;
      public override void LoadCurrent(int value) => SetCurrentLabel(value.ToString());
      public override void ResetCurrent() => LoadCurrent(0);
      protected override IToken GetDefaultEffectToken() => SetEffectToken(0); 

      public override IErrorHandle GetFromGui(out int value)
      {
         value = (int)IntNumeric.Value;
         return ErrorHandle.Success;
      }

   }   
   public class PrvHistBoolUi : PrvHistSetAddUi<bool>
   {
      public CheckBox BoolCheckBox { get; }
      public sealed override PropertyInfo PropertyInfo { get; init; }
      protected override IToken GetDefaultEffectToken() => EffectToken(false); 

      public PrvHistBoolUi(Func<bool, IToken> effect, string text, PropertyInfo info, bool isChecked = false)
         : base(effect, text, new CheckBox
         {
            Dock = DockStyle.Fill,
         }, false)
      {
         PropertyInfo = info;

         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, bool>)this).LoadToGui;
         {

         };
         
         BoolCheckBox = (CheckBox)Controls[2]; // keep a reference directly
         BoolCheckBox.Checked = isChecked;

         BoolCheckBox.CheckedChanged += (_, _) => SetFromGui();
      }

      public override void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log())
            return;

         var command = new PrvHistoryEntryCommand(Selection.GetSelectedProvinces, [EffectToken(value)], ProvinceHistoryManager.CurrentLoadedDate, out var changed);
         if (changed)
            HistoryManager.AddCommand(command);
      }
      public override void SetDefault() => BoolCheckBox.Checked = false;
      public override void LoadCurrent(bool value) => SetCurrentLabel(value ? "YES" : "NO");
      public override void ResetCurrent() => SetCurrentLabel("NO");
      public override void SetValue(bool value)
      {
         BoolCheckBox.Checked = value; 
      }

      public override IErrorHandle GetFromGui(out bool value)
      {
         value = BoolCheckBox.Checked;
         return ErrorHandle.Success;
      }
   }
   public abstract class PrvHistSetAddUiBoth<T> : PrvHistSetAddUi<T>, IPrvHisSetOptSinglePropControl<T> where T : notnull
   {
      public Func<T, IToken> SetEffectToken { get; init; }
      protected PrvHistSetAddUiBoth(Func<T, IToken> effectToken, Func<T, IToken> setEffectToken, string text, Control control, bool hasSetBox, bool hasCurrentLabel = true) : base(effectToken, text, control, hasSetBox, hasCurrentLabel)
      {
         SetEffectToken = setEffectToken;
      }
   }
   public abstract class PrvHistSetAddUi<T> : TableLayoutPanel, IPropertyControl<Province, T>, IPrvHistSingleEffectPropControl<T> where T : notnull
   {
      private Label Label { get; set; }
      protected Label? CurrentLabel { get; set; }
      public CheckBox SetCheckBox { get; set; }
      protected string CurPrefix = "Cur: ";
      public string CurSuffix = "";
      public Func<T, IToken> EffectToken { get; init; }

      public PrvHistSetAddUi(Func<T, IToken> effectToken, string text, Control control, bool hasSetBox, bool hasCurrentLabel = true)
      {
         EffectToken = effectToken;
         AutoSize = false;

         ColumnCount = 4;
         RowCount = 1;

         ColumnStyles.Clear();
         ColumnStyles.Add(new(SizeType.Percent, 25));
         ColumnStyles.Add(new(SizeType.Percent, 16));
         ColumnStyles.Add(new(SizeType.Percent, 29));
         ColumnStyles.Add(new(SizeType.Percent, 30));

         RowStyles.Clear();
         RowStyles.Add(new(SizeType.Percent, 100));

         Label = new()
         {
            Text = "---",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };


         SetCheckBox = new()
         {
            Checked = false,
            Dock = DockStyle.Fill,
            Font = new("Arial", 7, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Text = "Set/Add",
            Padding = new(0, 3, 0, 3),
            Margin = new(0, 3, 0, 3),
         };

         Controls.Add(Label, 0, 0);
         Controls.Add(SetCheckBox, 1, 0);
         Controls.Add(control, 2, 0);

         if (hasCurrentLabel)
         {
            CurrentLabel = new()
            {
               Text = "Cur",
               Dock = DockStyle.Fill,
               TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(CurrentLabel, 3, 0);
         }
         Label.Text = text;
         if (!hasSetBox)
         {
            SetCheckBox.Visible = false;
            ColumnStyles[0].Width = 41;
            ColumnStyles[0].SizeType = SizeType.Percent;
            ColumnStyles[1].Width = 0;
            ColumnStyles[1].SizeType = SizeType.Absolute;
         }
      }

      protected abstract IToken GetDefaultEffectToken();
      public abstract void LoadCurrent(T value);
      public abstract void ResetCurrent();

      // Default does not support cases where a token can be present multiple times
      protected virtual bool ShareEntriesValue(ProvinceHistoryEntry?[] entries, string tokenName, out T? value)
      {
         if (entries.Length == 0)
         {
            value = default!;
            return false;
         }

         var allContainToken = true;
         var hasSetInitVal = false;
         value = default!;
         for (var i = 0; i < entries.Length; i++)
         {
            var fToken = entries[i]?.Effects.Find(x => x.GetTokenName().Equals(tokenName));
            if (fToken != null)
            {
               if (fToken is not SimpleEffect<T> sEffect)
                  continue;

               if (hasSetInitVal == false)
               {
                  value = sEffect._value.Val;
                  hasSetInitVal = true;
               }
               else
               {
                  if (!value!.Equals(sEffect._value.Val))
                  {
                     allContainToken = false;
                     break;
                  }
               }

               continue;
            }
            allContainToken = false;
            break;
         }

         return allContainToken;
      }

      public void SetCurrentLabel(string str)
      {
         Debug.Assert(CurrentLabel != null, "CurrentLabel == nul");

         if (string.IsNullOrEmpty(str))
            CurrentLabel.Text = CurPrefix + "-" + CurSuffix;
         else
            CurrentLabel.Text = CurPrefix + str + CurSuffix;
      }

      public sealed override bool AutoSize
      {
         get { return base.AutoSize; }
         set { base.AutoSize = value; }
      }

      public void LoadToGui(List<Province> list, PropertyInfo propInfo, bool force)
      {
         if (force || PropertyInfo.Equals(propInfo))
         {

            var (entries, shareEntry) = ProvinceHistoryManager.BinarySearchDateExactMultiple(
                                                                                             list.Select(x => x.History.ToList()).ToList(),
                                                                                             ProvinceHistoryManager.CurrentLoadedDate);
            if (shareEntry)
               if (ShareEntriesValue(entries, GetDefaultEffectToken().GetTokenName(), out var sharedValue))
                  SetValue(sharedValue!);
            if (AttributeHelper.GetSharedAttribute(PropertyInfo, out T value, list))
            {
               LoadCurrent(value);
               if (!shareEntry)
                  SetDefault();
            }
            else
            {
               SetDefault();
               ResetCurrent();
            }
         }
      }
      public abstract PropertyInfo PropertyInfo { get; init; }
      public abstract void SetFromGui();
      public abstract void SetDefault();
      public abstract IErrorHandle GetFromGui(out T value);
      public abstract void SetValue(T value);
   }
}