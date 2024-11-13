using System.ComponentModel;
using Editor.DataClasses.Commands;
using Editor.Helper;

namespace Editor.Forms.PopUps
{
   public partial class RoughEditorForm : Form
   {
      private const int PROPERTY_GRID_ROW_HEIGHT = 19;
      private readonly object? _obj;
      private readonly bool _revertable;

      public RoughEditorForm(object obj, bool revertable = true)
      {
         if (revertable)
            if (!EditingHelper.DeepCopy(obj, out _obj))
               return;
         _revertable = revertable;

         SuspendLayout();
         InitializeComponent();
         PropGrid.SelectedObject = obj;
         string revertableString;
         if (revertable)
            revertableString = "(Revertable)";
         else
            revertableString = "(NOT Revertable)";
         base.Text = $"APE: {Convert.ChangeType(obj, obj.GetType())} {revertableString}";
         AdjustFormHeight();
         ResumeLayout();
      }


      private void AdjustFormHeight()
      {
         var desiredHeight = GetPropertyGridContentHeight() + SystemInformation.CaptionHeight;
         var screenHeight = Screen.FromControl(this).WorkingArea.Height;
         Height = Math.Min(desiredHeight, screenHeight);
         CenterToScreen();
      }

      private int GetPropertyGridContentHeight()
      {
         var propertyCount = PropGrid.SelectedObject.GetType()
            .GetProperties()
            .Count(prop => prop.GetCustomAttributes(typeof(BrowsableAttribute), true)
               .OfType<BrowsableAttribute>()
               .All(attr => attr.Browsable));

         return (propertyCount + 1) * PROPERTY_GRID_ROW_HEIGHT + 90; // 90 to account for description box and header
      }

      private void RoughEditorForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (_revertable)
            Globals.HistoryManager.AddCommand(new CAdvancedPropertiesEditing(_obj!, PropGrid.SelectedObject));
      }
   }
}
