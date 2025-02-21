using Editor.NameGenerator;

namespace Editor.src.Forms.Feature
{
   public partial class NameGeneratorForm : Form
   {
      private NameGenConfig Config { get; set; } = new();

      private NameGeneratorForm()
      {
         InitializeComponent();

         ResultsListBox.Items.Clear();
      }

      public NameGeneratorForm(NameGenConfig config) : this()
      {
         Config = config;

         ConfigPropertyGrid.SelectedObject = Config;
         ConfigPropertyGrid.ExpandAllGridItems();

         StartPosition = FormStartPosition.CenterScreen;
      }

      private void GenerateButton_Click(object sender, EventArgs e)
      {
         ResultsListBox.Items.Clear();
         var config = (NameGenConfig)ConfigPropertyGrid.SelectedObject;

         // TODO optimize to not need to train every time the button is clicked
         var data = DataSampler.Sample(config.Generator.Source);
         var generator = new NameGenerator.NameGenerator(data, config.Generator.Order, config.Generator.Smoothing);

         if (string.IsNullOrWhiteSpace(config.Generation.SimilarTo))
         {
            if (config.Generator.DefaultRandom)
            {
               var names = generator.GenerateNames(config.Generation.Count, config.Generation.MinLength, config.Generation.MaxLength, Globals.Random);
               ResultsListBox.Items.AddRange([.. names]);
            }
            else
            {
               var names = generator.GenerateNames(config.Generation.Count, config.Generation.MinLength, config.Generation.MaxLength, new ());
               ResultsListBox.Items.AddRange([.. names]);
            }
         }
         else
         {
            var names = generator.GenerateNames(config.Generation.Count, config.Generation.MinLength, config.Generation.MaxLength, config.Generation.MaxDistance, config.Generation.SimilarTo, Globals.Random);
            ResultsListBox.Items.AddRange([.. names]);
         }

      }
   }
}
