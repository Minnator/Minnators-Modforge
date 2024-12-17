using Editor.ErrorHandling;

namespace Editor.Forms.PopUps
{
   public partial class LogEntryViewer : Form
   {
      public LogEntryViewer(IExtendedLogInformationProvider provider)
      {
         InitializeComponent();
         MessageLabel.Text = provider.GetMessage();
         DescriptionLabel.Text = provider.GetDescription();
         ResolutionLabel.Text = provider.GetResolution();
      }
   }
}
