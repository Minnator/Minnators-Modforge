using System.Diagnostics;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Forms.PopUps
{
   public partial class CreateCountryForm : Form
   {
      private ColorPickerButton _colorPickerButton;

      public Dictionary<char, int> DynamicTags = new()
      {
         { 'C', 75 },
         { 'K', 100 },
         { 'E', 50 },
         { 'F', 20 },
         { 'T', 75 },
         { 'D', 75 },
         { 'O', 10 },
         { 'S', 10 },
      };

      public CreateCountryForm()
      {
         InitializeComponent();
         StartPosition = FormStartPosition.CenterParent;
         _colorPickerButton = ControlFactory.GetColorPickerButton();
         MTLP.Controls.Add(_colorPickerButton, 1, 4);
      }

      private void CreateButton_Click(object sender, EventArgs e)
      {
         if (!VerifyTag())
            return;

         Localisation.AddOrModifyLocObject(TagBox.Text, CountryNameTextBox.Text);
         Localisation.AddOrModifyLocObject(TagBox.Text + "_ADJ", CountryAdjTextBox.Text);
         Country.Create(TagBox.Text, _colorPickerButton.GetColor);
         Close();
      }

      private void CancelButton_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void TagBox_SelectedIndexChanged(object sender, EventArgs e)
      {
      }

      private void TagBox_TextUpdate(object sender, EventArgs e)
      {
         if (TagBox.Text.Length == 0)
            return;

         if (VerifyTag()) 
            SuggestTags();
      }

      public void SuggestTags(int number = 4)
      {
         if (TagBox.Text.Length != 3)
            return;

         var tags = new List<string>();
         var tag = TagBox.Text;
         var start = tag[..2];

         var cnt = 0;
         while (tags.Count < number / 2)
         {
            var newTag = start + (char)(tag[1] + cnt--);
            if (VerifyTag())
               tags.Add(newTag);
         }

         cnt = 0;
         while (tags.Count < number)
         {
            var newTag = start + (char)(tag[1] + cnt++);
            if (VerifyTag())
               tags.Add(newTag);
         }

         TagBox.Items.Clear();
         TagBox.Items.AddRange([.. tags.ToArray()]);
         TagBox.SelectionStart = TagBox.Text.Length;
      }

      public bool VerifyTag()
      {
         if (TagBox.Text.Length <= 1)
         {
            SetInfo(Info.ToShort);
            SetOk(false);
            return false;
         }
         var firstTextChar = TagBox.Text[0];
         if (DynamicTags.ContainsKey(firstTextChar))
         {
            if (int.TryParse(TagBox.Text[1..], out var num))
            {
               foreach (var tag in DynamicTags)
               {
                  if (firstTextChar == tag.Key && num <= tag.Value)
                  {
                     SetOk(false);
                     SetInfo(Info.DynamicTag);
                     return false;
                  }
               }
            }
         }

         if (TagBox.Text.Length < 3)
         {
            SetOk(false);
            SetInfo(Info.ToShort);
            return false;
         }

         var isFree = !Globals.Countries.ContainsKey(TagBox.Text);
         SetOk(isFree);
         SetInfo(!isFree ? Info.Used : Info.Ok);
         return isFree;
      }

      private void TagBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (TagBox.Text.Length > 2 && !char.IsControl(e.KeyChar))
         {
            e.Handled = true;
            return;
         }
         e.KeyChar = char.ToUpper(e.KeyChar);
      }

      public void SetInfo(Info info)
      {
         switch (info)
         {
            case Info.Ok:
               InfoLabel.Text = "Ok";
               InfoLabel.ForeColor = Color.Green;
               break;
            case Info.ToShort:
               InfoLabel.Text = "Tag is too short!";
               InfoLabel.ForeColor = Color.Red;
               break;
            case Info.Used:
               InfoLabel.Text = "Tag is already used!";
               InfoLabel.ForeColor = Color.Red;
               break;
            case Info.DynamicTag:
               InfoLabel.Text = "Dynamic Tag";
               InfoLabel.ForeColor = Color.Red;
               break;
         }
      }

      public enum Info
      {
         Ok,
         ToShort,
         Used,
         DynamicTag
      }

      public void SetOk(bool ok)
      {
         AvailabilityLabel.Image = GameIcon.GetIcon(ok ? GameIcons.Yes : GameIcons.No);
      }
   }
}
