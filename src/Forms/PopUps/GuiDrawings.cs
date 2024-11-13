using Editor.Helper;

namespace Editor.Forms.PopUps
{
   public partial class GuiDrawings : Form
   {
      public GuiDrawings()
      {
         InitializeComponent();
         SetCheckBoxes();
      }

      private void SetCheckBoxes()
      {
         var flags = GuiDrawing.CurrentElements;
         ShowTradeRoutesCheckBox.Checked = flags.HasFlag(GuiDrawing.GuiElements.TradeRoutes);
         CapitalsCheckBox.Checked = flags.HasFlag(GuiDrawing.GuiElements.Captitals);
         StraitsCheckBox.Checked = flags.HasFlag(GuiDrawing.GuiElements.Straits);
      }

      private void ShowTradeRoutesCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         GuiDrawing.AddOrRemoveGuiElement(GuiDrawing.GuiElements.TradeRoutes, ShowTradeRoutesCheckBox.Checked);
         Globals.ZoomControl.Invalidate();
      }

      private void CapitalsCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         GuiDrawing.AddOrRemoveGuiElement(GuiDrawing.GuiElements.Captitals, CapitalsCheckBox.Checked);
         Globals.ZoomControl.Invalidate();
      }

      private void StraitsCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         GuiDrawing.AddOrRemoveGuiElement(GuiDrawing.GuiElements.Straits, StraitsCheckBox.Checked);
         Globals.ZoomControl.Invalidate();
      }

      private void RiversCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         GuiDrawing.AddOrRemoveGuiElement(GuiDrawing.GuiElements.Rivers, RiversCheckBox.Checked);
         Globals.ZoomControl.Invalidate();
      }
   }
}
