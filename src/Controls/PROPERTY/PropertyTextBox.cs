using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.Settings;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Controls.PROPERTY
{
   public class PropertyTextBox<TSaveable> : TextBox, IPropertyControl<TSaveable, string> where TSaveable : Saveable
   {
      public PropertyInfo PropertyInfo { get; init; }
      protected readonly Func<List<TSaveable>> GetSaveables;
      protected Timer _timer = new ();

      public PropertyTextBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(string), $"PropInfo: {propertyInfo} is not of type {typeof(string)} but of type {propertyInfo.PropertyType}");
         PropertyInfo = propertyInfo;
         loadHandle += ((IPropertyControl<TSaveable, string>)this).LoadToGui;
         GetSaveables = getSaveables;

         _timer.Interval = Globals.Settings.Gui.TextBoxCommandCreationInterval;
         _timer.Tick += (_, _) => SetFromGui();
         KeyPress += TextBox_KeyPress;
         Leave += (_, _) => SetFromGui();

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

      public virtual void SetFromGui()
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
         return ErrorHandle.Success;
      }

      public void SetValue(string value)
      {
         Text = value;
      }

   }

   public class LocalisationTextBox<TSaveable> : PropertyTextBox<TSaveable> where TSaveable : Saveable
   {
      public LocalisationTextBox(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables) : base(propertyInfo, ref loadHandle, getSaveables)
      {
      }

      public override void SetFromGui()
      {
         _timer.Stop();
         // TODO: First get all the property values and then set them using the collection setters
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
            Saveable.SetFieldMultipleSilent(GetSaveables.Invoke(), value, PropertyInfo);
      }
   }
}