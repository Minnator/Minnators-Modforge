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
   public class PropertyNumeric<TSaveable, TProperty> : NumericUpDown, IPropertyControl<TSaveable, TProperty> where TSaveable : Saveable where TProperty : INumber<TProperty>
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;
      public TProperty DefaultValue { get; init; }
      private TProperty _oldValue;
      private decimal _multiplier;

      private Timer _timer = new ();

      public PropertyNumeric(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables, decimal multiplier)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(float), $"PropInfo: {propertyInfo} is not of type {typeof(int)}/{typeof(float)} but of type {propertyInfo.PropertyType}");
         Debug.Assert(!(propertyInfo.PropertyType == typeof(int) && Math.Floor(multiplier) != multiplier), "Multiplier is not a multiple of 1");
         if (propertyInfo.PropertyType == typeof(float)) 
            DecimalPlaces = 2;
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, TProperty>)this).LoadToGui;
         GetSaveables = getSaveables;
         _multiplier = multiplier;
         KeyPress += TextBox_KeyPress;
         KeyDown += TextBox_KeyDown;
         Leave += OnFocusLost;
         Enter += OnFocusGot;
         _timer.Interval = 2500;
         _timer.Tick += (_, _) => SetFromGui();
      }

      public void SetFromGui()
      {
         _timer.Stop();
         if (Globals.State == State.Running && GetFromGui(out var value).Log() && value != _oldValue)
            Saveable.SetFieldMultiple(GetSaveables.Invoke(), value, PropertyInfo);
      }

      public void SetDefault()
      {
         _timer.Stop();
         Text = DefaultValue.ToString();
      }

      public IErrorHandle GetFromGui(out TProperty value) => Converter.Convert(Text, out value);


      public void SetValue(TProperty value)
      {
         Text = value.ToString();
      }

      private void OnFocusGot(object? sender, EventArgs e) => GetFromGui(out _oldValue).Log();

      private void TextBox_KeyPress(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Enter)
            e.Handled = true;
         else
         {
            _timer.Stop();
            _timer.Start();
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
         var newValue = Math.Min(Value + _multiplier, Maximum);
         if (ModifierKeys.HasFlag(Keys.Control))
            newValue = Math.Min(Value + 10 * _multiplier, Maximum);
         else if (ModifierKeys.HasFlag(Keys.Shift)) 
            newValue = Math.Min(Value + 5 * _multiplier, Maximum);

         Value = newValue;
         _timer.Stop();
         _timer.Start();
      }

      public override void DownButton()
      {
         var newValue = Math.Max(Value - _multiplier, Minimum);
         if (ModifierKeys.HasFlag(Keys.Control))
            newValue = Math.Max(Value - 10 * _multiplier, Minimum);
         else if (ModifierKeys.HasFlag(Keys.Shift)) 
            newValue = Math.Max(Value - 5 * _multiplier, Minimum);

         Value = newValue;
         _timer.Stop();
         _timer.Start();
      }

      public void Clear()
      {
         _timer.Stop();
         SetDefault();
      }
   }
}