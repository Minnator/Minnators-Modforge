using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls.NewControls
{
   public class PropertyNumeric<TSaveable> : NumericUpDown, IPropertyControl<TSaveable, int> where TSaveable : Saveable
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;
      public int DefaultValue { get; init; }
      private int _oldValue = 0;

      public event EventHandler<int> UpButtonPressed = delegate { };
      public event EventHandler<int> DownButtonPressed = delegate { };
      public new event EventHandler<int> ValueChanged = delegate { };

      private Timer _timer = new ();

      public PropertyNumeric(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(int), $"PropInfo: {propertyInfo} is not of type {typeof(int)} but of type {propertyInfo.PropertyType}");

         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, int>)this).LoadToGui;
         GetSaveables = getSaveables;

         KeyPress += TextBox_KeyPress;
         KeyDown += TextBox_KeyDown;
         Leave += OnFocusLost;
         Enter += OnFocusGot;
         _timer.Interval = 100;
         _timer.Tick += (_, _) => SetFromGui();
      }

      public void SetFromGui()
      {
         _timer.Stop();
         if (Globals.State == State.Running && GetFromGui(out var value).Log() && value != _oldValue)
            Saveable.SetFieldMultiple(GetSaveables.Invoke(), value, PropertyInfo);
      }

      public void SetDefault() => Text = DefaultValue.ToString();
      public IErrorHandle GetFromGui(out int value) => Converter.Convert(Text, out value);

      public void SetValue(int value)
      {
         Text = value.ToString();
      }

      private void OnFocusGot(object? sender, EventArgs e) => _oldValue = int.Parse(Text);

      private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Enter)
            e.Handled = true;
         else
         {
            _timer.Stop();
            _timer.Start();
            ValueChanged.Invoke(this, int.Parse(Text));
         }
      }

      private void TextBox_KeyDown(object? sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Enter)
         {
            e.SuppressKeyPress = true;
            SetFromGui();
         }
      }

      private void OnFocusLost(object? sender, EventArgs e)
      {
         SetFromGui();
      }


      public override void UpButton()
      {
         var newValue = Math.Min(Value + 1, Maximum);
         if (ModifierKeys.HasFlag(Keys.Control))
         {
            newValue = Math.Min(Value + 10, Maximum);
            UpButtonPressed.Invoke(this, 10);
         }
         else if (ModifierKeys.HasFlag(Keys.Shift))
         {
            newValue = Math.Min(Value + 5, Maximum);
            UpButtonPressed.Invoke(this, 5);
         }
         else
            UpButtonPressed.Invoke(this, 1);

         Value = newValue;
         ValueChanged.Invoke(this, int.Parse(Text));
         _timer.Stop();
         _timer.Start();
      }

      public override void DownButton()
      {
         var newValue = Math.Max(Value - 1, Minimum);
         if (ModifierKeys.HasFlag(Keys.Control))
         {
            newValue = Math.Max(Value - 10, Minimum);
            DownButtonPressed.Invoke(this, 10);
         }
         else if (ModifierKeys.HasFlag(Keys.Shift))
         {
            newValue = Math.Max(Value - 5, Minimum);
            DownButtonPressed.Invoke(this, 5);
         }
         else
            DownButtonPressed.Invoke(this, 1);

         Value = newValue;
         ValueChanged.Invoke(this, int.Parse(Text));
         _timer.Stop();
         _timer.Start();
      }
   }
}