using System.Diagnostics;
using System.Reflection;
using Editor.ErrorHandling;
using Editor.Loading.Enhanced;
using Editor.Loading.Enhanced.PCFL.Implementation;
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

         LoadEffectsFromAssembly();

         FormClosing += (sender, args) => _timer.Dispose(); 
      }

      private void SetUpGUI()
      {
         EffectView.View = View.Details;
         EffectView.FullRowSelect = true;
         EffectView.HotTracking = true;

         EffectView.Columns.Add("NameHeader", "Name", -2);
         EffectView.Columns.Add("DescriptionHeader", "Description", -2);
         EffectView.Columns.Add("ExampleHeader", "Example", -2);

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
            LoadEffectsFromAssembly();
            return;
         }

         EffectView.Items.Clear();

         var keys = Scopes.Province.Effects.Keys.Where(x => x.Contains(text)).ToList();
         keys.Sort();

         EffectView.BeginUpdate();
         foreach (var key in keys)
            EffectView.Items.Add(new ListViewItem(key));
         EffectView.EndUpdate();

         EffectView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      }

      private void LoadEffectsFromAssembly()
      {
         EffectView.BeginUpdate();
         var derivedTypes = GetDerivedTypes(typeof(SimpleEffect<>), Assembly.GetExecutingAssembly());
         foreach (var type in derivedTypes)
         {
            if (type.ContainsGenericParameters)
               continue;

            var instance = Activator.CreateInstance(type)!;
            var tokenName = type.GetMethod(typeof(IToken).GetMethod(nameof(IToken.GetTokenName))!.Name);
            var tokenDesc = type.GetMethod(typeof(IToken).GetMethod(nameof(IToken.GetTokenDescription))!.Name);
            var tokenExample = type.GetMethod(typeof(IToken).GetMethod(nameof(IToken.GetTokenExample))!.Name);

            var item = new ListViewItem(tokenName?.Invoke(instance, null)?.ToString());
            item.SubItems.Add(tokenDesc?.Invoke(instance, null)?.ToString());
            item.SubItems.Add(tokenExample?.Invoke(instance, null)?.ToString());

            EffectView.Items.Add(item);
         }
         EffectView.EndUpdate();
      }

      private static IEnumerable<Type> GetDerivedTypes(Type genericBaseType, Assembly assembly)
      {
         return assembly.GetTypes()
                        .Where(t => t.BaseType is { IsGenericType: true } &&
                                    t.BaseType.GetGenericTypeDefinition() == genericBaseType);
      }

   }
}
