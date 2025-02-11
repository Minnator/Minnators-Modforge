using System.Diagnostics;
using System.Reflection;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Saving;

namespace Editor.Controls.NewControls
{
   public class PropertyColorButton<TSaveable> : Button, IPropertyControl<TSaveable, Color> where TSaveable : Saveable
   {
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<TSaveable>> _getSaveables;

      public PropertyColorButton(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable), $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(Color), $"PropInfo: {propertyInfo} is not of type {typeof(Color)} but of type {propertyInfo.PropertyType}");

         PropertyInfo = propertyInfo;
         _getSaveables = getSaveables;
         loadHandle += ((IPropertyControl<TSaveable, Color>)this).LoadToGui;
      }

      protected override void OnClick(EventArgs e)
      {
         using var colorDialog = new ColorDialog();
         colorDialog.Color = BackColor;
         if (colorDialog.ShowDialog() == DialogResult.OK)
            SetValue(colorDialog.Color);
         base.OnClick(e);
         SetFromGui();
      }

      public void SetFromGui()
      {
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
            Saveable.SetFieldMultiple(_getSaveables.Invoke(), value, PropertyInfo);
      }

      public void SetDefault()
      {
         SetValue(Color.FromArgb(255, 255, 255));
      }

      public IErrorHandle GetFromGui(out Color value)
      {
         value = BackColor;
         return ErrorHandle.Success;
      }

      public void SetValue(Color value)
      {
         BackColor = value;
         Text = $"({value.R}/{value.G}/{value.B})";
      }
   }
}