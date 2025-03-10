using Editor.ErrorHandling;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation.ProvinceScope;
using Editor.Saving;
using Timer = System.Windows.Forms.Timer;

namespace Editor.Forms.Feature
{
   public partial class WikiBrowser : Form
   {
      private Timer _timer = new();

      public WikiBrowser()
      {
         InitializeComponent();

         SetUpGUI();

         Load += LoadEffects;
         FormClosing += (sender, args) => _timer.Dispose(); 
      }

      private void SetUpGUI()
      {
         EffectView.View = View.Details;
         EffectView.FullRowSelect = true;
         EffectView.HotTracking = true;

         EffectView.Columns.Add("Name");
         EffectView.Columns.Add("Description");

         _timer.Interval = 300;

         _timer.Tick += OnTextChange;

         SearchTextBox.TextChanged += (sender, args) =>
         {
            _timer.Stop();
            _timer.Start();
         };
      }

      private void OnTextChange(object? sender, EventArgs eventArgs)
      {
         _timer.Stop();
         var text = SearchTextBox.Text;

         if (string.IsNullOrEmpty(text))
         {
            EffectView.Items.Clear();
            LoadEffects(sender, eventArgs);
            return;
         }

         EffectView.Items.Clear();

         var keys = ProvinceScopes.Scope.Effects.Keys.Where(x => x.Contains(text)).ToList();
         keys.Sort();

         EffectView.BeginUpdate();
         foreach (var key in keys)
            EffectView.Items.Add(new ListViewItem(key));
         EffectView.EndUpdate();

         EffectView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }

      private void LoadEffects(object? sender, EventArgs eventArgs)
      {
         EffectView.BeginUpdate();
         foreach (var effect in ProvinceScopes.Scope.Effects)
         {
            var item = new ListViewItem(effect.Key);

            EffectView.Items.Add(item);
         }

         EffectView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
         EffectView.EndUpdate();
      }




   }
}
