using Editor.Controls;

namespace Editor.Forms.Feature
{
   public partial class RevolutionaryColorPicker : Form
   {
      private TableLayoutPanel _colorsToPickPanel;
      private ColorIndexButton _colorIndexButtonLeft;
      private ColorIndexButton _colorIndexButtonMiddle;
      private ColorIndexButton _colorIndexButtonRight;
      
      public EventHandler<(int, int, int)> OnColorsChanged = delegate {};

      public RevolutionaryColorPicker()
      {
         InitializeComponent();
         InitializeColorPicker();
         InitializeFlagPreview();
      }

      public void SetIndexes(int one, int two, int three)
      {
         _colorIndexButtonLeft.Index = one;
         _colorIndexButtonMiddle.Index = two;
         _colorIndexButtonRight.Index = three;
      }

      private void InitializeFlagPreview()
      {
         _colorIndexButtonLeft = new()
         {
            Index = 0,
            ShowPreview = false,
            Draggable = false,
            Margin = new(0),
            Dock = DockStyle.Fill,
            AllowDrop = true,
            BorderFlags = ColorIndexButton.Borders.Left | ColorIndexButton.Borders.Bottom | ColorIndexButton.Borders.Top,
         };
         _colorIndexButtonLeft.OnIndexChanged += OnColorChanged_Impl;

         _colorIndexButtonMiddle = new()
         {
            Index = 1,
            ShowPreview = false,
            Draggable = false,
            Margin = new(0),
            Dock = DockStyle.Fill,
            AllowDrop = true,
            BorderFlags = ColorIndexButton.Borders.Bottom | ColorIndexButton.Borders.Top,
         };
         _colorIndexButtonMiddle.OnIndexChanged += OnColorChanged_Impl;

         _colorIndexButtonRight = new()
         {
            Index = 2,
            ShowPreview = false,
            Draggable = false,
            Margin = new(0),
            Dock = DockStyle.Fill,
            AllowDrop = true,
            BorderFlags = ColorIndexButton.Borders.Right | ColorIndexButton.Borders.Bottom | ColorIndexButton.Borders.Top,
         };
         _colorIndexButtonRight.OnIndexChanged += OnColorChanged_Impl;

         FlagTLP.Controls.Add(_colorIndexButtonLeft, 1, 0);
         FlagTLP.Controls.Add(_colorIndexButtonMiddle, 2, 0);
         FlagTLP.Controls.Add(_colorIndexButtonRight, 3, 0);
      }

      public void OnColorChanged_Impl(object? sender, int index)
      {
         OnColorsChanged.Invoke(this, (_colorIndexButtonLeft.Index, _colorIndexButtonMiddle.Index, _colorIndexButtonRight.Index));
      }

      private void InitializeColorPicker()
      {
         var numOfColors = Globals.RevolutionaryColors.Count;
         _colorsToPickPanel = new()
         {
            Margin = new(3),
            Dock = DockStyle.Fill,
            ColumnCount = numOfColors,
         };

         for (var i = 0; i < numOfColors; i++)
         {
            _colorsToPickPanel.ColumnStyles.Add(new(SizeType.Percent, 100F / numOfColors));
            _colorsToPickPanel.Controls.Add(new ColorIndexButton
            {
               Index = i,
               ShowPreview = false,
               Draggable = true,
               Margin = new(0),
               Dock = DockStyle.Fill,
            }, i, 0);
         }

         MainTLP.Controls.Add(_colorsToPickPanel, 0, 0);
      }

      private void OkButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }
   }
}
