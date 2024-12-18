using Editor.Helper;

namespace Editor.Forms.Feature
{
   public partial class ModifierSuggestion : Form
   {
      private readonly Random rand;
      public ModifierSuggestion()
      {
         rand = new();
         InitializeComponent();
         ModNameBox.Items.AddRange(Globals.ModifierKeys);
      }

      private void button1_Click(object sender, EventArgs e)
      {
         var index = rand.Next(0, Globals.ModifierKeys.Length);
         ModNameBox.SelectedIndex = index;

         if (!Globals.ModifierValueTypes.TryGetValue(index, out var type))
            return;

         ValueIdeaBox.Items.Clear();
         ValueIdeaBox.Items.AddRange(FormsHelper.GetCompletionSuggestion(type).ToArray());

         ValueIdeaBox.SelectedIndex = rand.Next(0, ValueIdeaBox.Items.Count - 1);
      }

      private void ModifierSuggestion_FormClosing(object sender, FormClosingEventArgs e)
      {
      }
   }
}
