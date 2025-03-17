using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.src.Forms.Feature
{
   public partial class MissionTreeExporter : Form
   {
      public MissionTreeExporter()
      {
         InitializeComponent();
         SetMissionFileName();
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

         var targetCountry = Globals.Countries["BRA"];
         var missionView = MissionLayoutEngine.LayoutSlotsToTransparentImage(slots, MissionView.CompletionType.Completed, MissionView.FrameType.Completed, targetCountry);

         missionView.SaveBmpToModforgeData("missionLayout.png");


         if (BackgroundCheckBox.Checked)
         {
            var mWBackground = MissionLayoutEngine.DrawMissionOnBackGround(missionView, !FullBgBox.Checked, MissionNameBox.Checked, slots, targetCountry);
            mWBackground.SaveBmpToModforgeData("missionLayoutWithBackground.png");

            PrewViewBox.Image = mWBackground;
         }
         else
         {
            PrewViewBox.Image = missionView;
         }
      }

      private void ExportButton_Click(object sender, EventArgs e)
      {
          ExportMissionTree();
      }
   }
}
