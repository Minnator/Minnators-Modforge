using System.Diagnostics;
using System.Reflection;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls.NewControls
{
   public class PropertyTextBox<TSaveable> : TextBox, IPropertyControl<TSaveable, string> where TSaveable : Saveable
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;
      private Timer _timer = new ();

      public PropertyTextBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(string), $"PropInfo: {propertyInfo} is not of type {typeof(string)} but of type {propertyInfo.PropertyType}");
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, string>)this).LoadToGui;
         GetSaveables = getSaveables;

         _timer.Interval = 2500;
         _timer.Tick += (_, _) => SetFromGui();
         KeyPress += TextBox_KeyPress;
         Leave += (_, _) => SetFromGui();
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
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
            Saveable.SetFieldMultiple(GetSaveables.Invoke(), value, PropertyInfo);
      }

      public void SetDefault()
      {
         _timer.Stop();
         Text = string.Empty;
      }

      public IErrorHandle GetFromGui(out string value)
      {
         value = Text;
         return ErrorHandle.Sucess;
      }

      public void SetValue(string value)
      {
         Text = value;
      }

   }
}