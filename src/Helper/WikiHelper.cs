using Editor.Loading.Enhanced.PCFL.Implementation;
using System.Linq;
using System.Reflection;
using Editor.Loading.Enhanced;
using Editor.src.Forms.PopUps;

namespace Editor.Helper
{
   public static class WikiHelper
   {

      public static void LoadEffectsFromAssembly(ListView view)
      {
         view.BeginUpdate();

         view.Columns.Clear();
         var headerNames = new[] { "Name", "Description", "Example" };
         for (var i = 0; i < headerNames.Length; i++)
            view.Columns.Add($"{headerNames[i]}Header", headerNames[i], -2);


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

            view.Items.Add(item);
         }
         view.EndUpdate();
      }
      
      private static IEnumerable<Type> GetDerivedTypes(Type genericBaseType, Assembly assembly)
      {
         return assembly.GetTypes()
                        .Where(t => t.BaseType is { IsGenericType: true } &&
                                    t.BaseType.GetGenericTypeDefinition() == genericBaseType);
      }

      public static void LoadScriptedEffects(ListView view)
      {
         view.BeginUpdate();

         view.Columns.Clear();
         var headerNames = new[] { "Name", "Parameters", "Num of Param Usage", "Filename"};
         for (var i = 0; i < headerNames.Length; i++)
            view.Columns.Add($"{headerNames[i]}Header", headerNames[i], -2);

         foreach (var scrEff in Globals.ScriptedEffects)
         {

            var item = new ListViewItem(scrEff.name);
            item.SubItems.Add($"'{string.Join("', '", scrEff._replaceSources.DistinctBy(x => x.key).Select(x => x.key))}'");
            item.SubItems.Add(scrEff._replaceSources.Count.ToString());
            item.SubItems.Add(scrEff.Po.GetFileName());
            
            view.Items.Add(item);
         }
         view.EndUpdate();
      }

      public static ContextMenuStrip ScriptedEffectContextMenu()
      {
         var contextMenu = new ContextMenuStrip();

         var openFileItem = new ToolStripMenuItem("Open File");
         openFileItem.Click += (sender, args) =>
         {
            if (GetClickedItem(sender) is not { } item)
               return;

            var scrpEff = Globals.ScriptedEffects.FirstOrDefault(x => x.name.Equals(item.SubItems[0].Text), null);
            if (scrpEff is null)
               return;

            ProcessHelper.OpenFileAtLine(scrpEff.Po.GetPath(), scrpEff.StartLine, -1);
         };

         var showFullString = new ToolStripMenuItem("Show Full String");
         showFullString.Click += (sender, args) =>
         {
            if (GetClickedItem(sender) is not { } item)
               return;

            var scrpEff = Globals.ScriptedEffects.FirstOrDefault(x => x.name.Equals(item.SubItems[0].Text), null);
            if (scrpEff is null)
               return;

            StringPopUp.ShowDialog(scrpEff.effect, scrpEff.name);
         };

         var copyUsage = new ToolStripMenuItem("Copy Usage");
         copyUsage.Click += (sender, args) =>
         {
            if (GetClickedItem(sender) is not { } item)
               return;

            var scrpEff = Globals.ScriptedEffects.FirstOrDefault(x => x.name.Equals(item.SubItems[0].Text), null);
            if (scrpEff is null)
               return;

            Clipboard.SetText(scrpEff.GetUsage());
            
         };

         contextMenu.Items.Add(openFileItem);
         contextMenu.Items.Add(showFullString);
         contextMenu.Items.Add(copyUsage);

         return contextMenu;
      }
      private static ListViewItem? GetClickedItem(object? sender)
      {
         if (sender is ToolStripMenuItem menuItem && menuItem.GetCurrentParent() is ContextMenuStrip contextMenu)
            return contextMenu.Tag as ListViewItem;

         return null;
      }


   }
}