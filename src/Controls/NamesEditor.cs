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

   public partial class MonarchNameControl : Control
   {
      

      internal TableLayoutPanel _tbl;
      private SmartTextBox _nameBox;
      private SmartTextBox _chanceBox;
      private Button _deleteButton;

      public MonarchNameControl(MonarchName name, Size size)
      {
         _tbl = new()
         {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            ColumnStyles =
            {
               new (SizeType.Percent, 100),
               new (SizeType.Absolute, 55),
               new (SizeType.Absolute, 55),
            },
            RowCount = 1,
            Margin = new (0)
         };

         _nameBox = new()
         {
            Text = name.Name,
            Dock = DockStyle.Fill
         };

         _chanceBox = new()
         {
            Text = name.Chance.ToString(),
            Dock = DockStyle.Fill
         };

         _deleteButton = new()
         {
            Image = Resources.RedX,
            Dock = DockStyle.Fill
         };
         _deleteButton.Click += OnDeleteButton_Click;

         _tbl.Controls.Add(_nameBox, 0, 0);
         _tbl.Controls.Add(_chanceBox, 1, 0);
         _tbl.Controls.Add(_deleteButton, 2, 0);

         Controls.Add(_tbl);

         Size = size;

         _nameBox.ContentModified += ((sender, s) =>
         {
            CountryGuiEvents.MonarchName_ContentModified(sender, _nameBox.OldText, s, int.Parse(_chanceBox.Text));
         });

         _chanceBox.ContentModified += ((sender, s) =>
         {
            CountryGuiEvents.MonarchName_ContentModified(sender, _nameBox.Text, _nameBox.Text, int.Parse(s));
         });
      }

      public bool GetNames(out MonarchName mName)
      {
         var rNum = Parsing.GetRegnalFromString(_nameBox.Text);
         if (InputHelper.GetIntIfNotEmpty(_chanceBox, out var chance) &&
             rNum != int.MinValue &&
             InputHelper.GetStringIfNotEmpty(_nameBox, out var name))
         {
            mName = new(name, rNum, chance);
            return true;
         }

         mName = MonarchName.Empty;
         return false;
      }


      public void OnDeleteButton_Click(object? sender, EventArgs e)
      {
         CountryGuiEvents.MonarchName_DeleteButton_Click(sender, _nameBox.Text);
         Parent?.Controls.Remove(this);
      }

      
   }
}