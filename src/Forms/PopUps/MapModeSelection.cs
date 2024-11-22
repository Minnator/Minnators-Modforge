using Editor.Controls;
using Editor.DataClasses.MapModes;

namespace Editor.Forms.PopUps
{
   public partial class MapModeSelection : Form
   {
      private readonly MapModeButton _button;
      public MapModeType SelectedMapMode { get; private set; } = MapModeType.None;

      public MapModeSelection(MapModeButton button)
      {
         _button = button;
         InitializeComponent();

         MMSelectionBox.Items.AddRange([.. Enum.GetNames<MapModeType>()]);
         MMSelectionBox.SelectedIndexChanged += MMSelectionBox_SelectedIndexChanged;
         Load += OnLoad;

      }

      private void OnLoad(object? sender, EventArgs e)
      {
         MMSelectionBox.DroppedDown = true;
      }

      //close on ESC
      protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
      {
         if (keyData == Keys.Escape)
         {
            Close();
            return true;
         }
         return base.ProcessCmdKey(ref msg, keyData);
      }
      
      private void MMSelectionBox_SelectedIndexChanged(object? sender, EventArgs e)
      {
         if (MMSelectionBox.SelectedItem == null || string.IsNullOrEmpty(MMSelectionBox.SelectedItem.ToString()))
            return;
         var enumFromItem = Enum.Parse<MapModeType>(MMSelectionBox.SelectedItem!.ToString()!);
         _button.SetMapMode(enumFromItem);
         SelectedMapMode = enumFromItem;
         Close();
      }
   }
}
