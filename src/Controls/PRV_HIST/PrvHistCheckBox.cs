using System.Diagnostics;
using System.Reflection;
using Editor.Controls.PROPERTY;
using Editor.DataClasses.Saveables;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Loading.Enhanced.PCFL.Implementation;

namespace Editor.Controls.PRV_HIST
{
   public class PrvHistCheckBox : TableLayoutPanel, IPrvHistSimpleEffectPropControl<bool>
   {
      public Label Label { get; set; }
      public CheckBox CheckBox { get; set; }
      public SimpleEffect<bool> Effect { get; init; }

      public PrvHistCheckBox(PropertyInfo info, SimpleEffect<bool> effect)
      {
         Debug.Assert(PropertyInfo?.PropertyType == typeof(bool), $"PropInfo: {PropertyInfo} is not of type {typeof(bool)} but of type {PropertyInfo.PropertyType}");

         PropertyInfo = info;
         Effect = effect;
         LoadGuiEvents.ProvHistoryLoadAction += ((IPropertyControl<Province, bool>)this).LoadToGui;

         InitializeComponent();
      }

      public PrvHistCheckBox(string text, PropertyInfo info, SimpleEffect<bool> effect) : this(info, effect)
      {
         Label.Text = text;
      }
      
      private void InitializeComponent()
      {
         AutoSize = false;
         ColumnCount = 2;
         RowCount = 1;

         ColumnStyles.Clear();
         ColumnStyles.Add(new(SizeType.Percent, 45));
         ColumnStyles.Add(new(SizeType.Percent, 55));

         RowStyles.Clear();
         RowStyles.Add(new(SizeType.Percent, 100));

         Label = new()
         {
            Text = "---",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
         };

         CheckBox = new()
         {
            Checked = false,
            Dock = DockStyle.Fill,
         };

         CheckBox.CheckedChanged += (_, _) => SetFromGui();

         Controls.Add(Label, 0, 0);
         Controls.Add(CheckBox, 1, 0);
      }
      public sealed override bool AutoSize
      {
         get { return base.AutoSize; }
         set { base.AutoSize = value; }
      }

      public PropertyInfo PropertyInfo { get; init; }
      public void SetFromGui()
      {
         if (Globals.State != State.Running || !GetFromGui(out var value).Log())
            return;
         // TODO add a history command
      }
      public void SetDefault() => CheckBox.Checked = false;
      public void SetValue(bool value) => CheckBox.Checked = value;
      public IErrorHandle GetFromGui(out bool value)
      {
         value = CheckBox.Checked;
         return ErrorHandle.Success;
      }

   }
}