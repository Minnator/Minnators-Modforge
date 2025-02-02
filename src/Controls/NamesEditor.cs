using System.Text.RegularExpressions;
using System.Threading.Channels;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;
using Editor.Parser;
using Editor.Properties;

namespace Editor.Controls
{
   public sealed class NamesEditor : Control
   {
      public SmartTextBox TextBox { get; set; } = new();
      public Label Description { get; set; } = new();
      private TableLayoutPanel _tbl;

      public string DescriptionText { get; set; }

      public NamesEditor(List<string> names, string description)
      {
         DescriptionText = description;

         TextBox.Multiline = true;
         TextBox.ScrollBars = ScrollBars.Vertical;
         TextBox.Dock = DockStyle.Fill;
         SetNames(names);

         Description.Text = DescriptionText;
         Description.Dock = DockStyle.Top;
         Description.Margin = new (3);

         _tbl = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new (0)
         };

         _tbl.Controls.Add(Description, 0, 0);
         _tbl.Controls.Add(TextBox, 0, 1);

         Controls.Add(_tbl);

         Dock = DockStyle.Fill;
         Margin = new (0);
      }

      public List<string> GetNames()
      {
         return TextBox.Text.Split([','], StringSplitOptions.RemoveEmptyEntries)
                            .Select(name => name.Trim())
                            .ToList();
      }

      public void SetNames(List<string> names)
      {
         TextBox.Text = string.Join(", ", names);
      }

      public void Clear()
      {
         TextBox.Text = "";
      }
   }

}