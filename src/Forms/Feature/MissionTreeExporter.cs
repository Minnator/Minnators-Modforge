using System.Drawing.Imaging;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;
using Editor.Helper;
using CheckBox = System.Windows.Forms.CheckBox;

namespace Editor.src.Forms.Feature
{
   public partial class MissionTreeExporter : Form
   {
      public MissionTreeExporter()
      {
         InitializeComponent();
         SetMissionFileName();
         SetEnums();
         SetCountries();

         SaveTextLabel.Text = $"The exported mission tree can be found at \n'Settings>Saving>MissionExportPath' with the name: '<Tag>_<MissionFileName>' or the provided one.";
      }

      private void SetCountries()
      {
         var tags = Globals.Countries.Keys.ToList();
         tags.Sort();
         CountrySelection.Items.AddRange([.. tags]);
      }

      private void SetEnums()
      {
         foreach (var item in Enum.GetNames(typeof(MissionView.CompletionType)))
            MissionEffectIcon.Items.Add($"Completion type: {item}");
         MissionEffectIcon.SelectedIndex = 0;
         foreach (var item in Enum.GetNames(typeof(MissionView.FrameType)))
            MissionFrameIcon.Items.Add($"Frame type: {item}");
         MissionFrameIcon.SelectedIndex = 0;
      }

      private void SetMissionFileName()
      {
         HashSet<string> slotNames = [];
         foreach (var slot in Globals.MissionSlots)
            slotNames.Add(slot.PathObj.GetFileName());

         SelectMissionFile.Items.AddRange([.. slotNames.ToArray()]);

         SelectMissionFile.SelectedItem = "DH_Bengali_Missions.txt";
      }

      private void SelectMissionFile_SelectedIndexChanged(object sender, EventArgs e)
      {


      }

      public void ExportMissionTree()
      {
         //var slottedLayout = MissionLayoutEngine.LayoutFile("Pomerania_Missions.txt");
         var slots = MissionLayoutEngine.LayoutSlotsOfFile(SelectMissionFile.Text);
         if (slots.Count == 0)
            return;
         
         var targetCountry = CountrySelection.SelectedIndex == -1 ? Globals.Countries["REB"] : Globals.Countries[CountrySelection.Text];
         var effect = (MissionView.CompletionType)MissionEffectIcon.SelectedIndex;
         var frame = (MissionView.FrameType)MissionFrameIcon.SelectedIndex;
         var missionView = MissionLayoutEngine.LayoutSlotsToTransparentImage(slots, effect, frame, targetCountry);

         var fileName = string.IsNullOrWhiteSpace(FileNameTextBox.Text) ? $"{targetCountry.Tag}_{RemoveTxt(SelectMissionFile.Text)}.png" : $"{FileNameTextBox.Text}.png";

         if (BackgroundCheckBox.Checked)
         {
            var mWBackground = MissionLayoutEngine.DrawMissionOnBackGround(missionView, !FullBgBox.Checked, MissionNameBox.Checked, slots, targetCountry);
            PrewViewBox.Image = mWBackground;
            IO.SaveImage(Path.Combine(Globals.Settings.Saving.MissionExportPath, fileName), mWBackground);
         }
         else
         {
            IO.SaveImage(Path.Combine(Globals.Settings.Saving.MissionExportPath, fileName), missionView);
            PrewViewBox.Image = missionView;
         }
      }

      private string RemoveTxt(string fileName)
      {
         if (fileName.EndsWith(".txt"))
            fileName = fileName[..^4];
         return fileName;
      }

      private void ExportButton_Click(object sender, EventArgs e)
      {
         ExportMissionTree();
      }

      private void BackgroundCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         if (sender is not CheckBox checkBox)
            return;

         MissionNameBox.Enabled = checkBox.Checked;
         FullBgBox.Enabled = checkBox.Checked;
      }

      private void FullBgBox_CheckedChanged(object sender, EventArgs e)
      {
      }
   }
}
