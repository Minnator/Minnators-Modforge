using Timer = System.Timers.Timer;

namespace Editor.Forms
{
   public partial class ToolTipForm : Form
   {
      private Graphics graphics;
      private Timer _timer;

      public ToolTipForm(int autoHideDelay)
      {
         InitializeComponent();
         graphics = ToolTipContent.CreateGraphics();
         _timer = new (autoHideDelay);
         _timer.Elapsed += (sender, args) => HideTooltip();

         MouseEnter += (sender, e) => HideTooltip();
         MouseMove += (sender, e) => HideTooltip();
         ToolTipContent.MouseEnter += (sender, e) => HideTooltip();
         ToolTipContent.MouseMove += (sender, e) => HideTooltip();

         TopLevel = true;
         ShowInTaskbar = false;
      }


      public void AdjustFormWidthToText(string text)
      {
         // Split the text into lines
         var lines = text.Split(['\r', '\n'], StringSplitOptions.None);

         var maxWidth = 0;

         // Measure each line of the text
         foreach (var line in lines) {
            var size = graphics.MeasureString(line, ToolTipContent.Font);
            if (size.Width > maxWidth) {
               maxWidth = (int)size.Width;
            }
         }

         Width = maxWidth + Padding.Left + Padding.Right;
         ToolTipContent.Text = text;
      }

      public void AdjustHeightToText(string text)
      {
         // Split the text into lines
         var lines = text.Split(['\r', '\n'], StringSplitOptions.None);

         var maxHeight = 0;

         // Measure each line of the text
         foreach (var line in lines) {
            var size = graphics.MeasureString(line, ToolTipContent.Font);
            maxHeight += (int)size.Height;
         }

         Height = maxHeight + Padding.Top + Padding.Bottom;
         ToolTipContent.Text = text;
      }

      public void SetText(string text)
      {
         SuspendLayout();
         AdjustFormWidthToText(text);
         AdjustHeightToText(text);
         ToolTipContent.Text = text;
         ResumeLayout();
      }

      public void ShowTooltipAt(Point point)
      {
         Location = point;
         Show();
      }

      public void HideTooltip()
      {
         if (InvokeRequired)
            Invoke(new MethodInvoker(Hide));
         _timer.Stop();
      }
   }
}
