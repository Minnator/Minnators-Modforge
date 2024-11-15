using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Properties;

namespace Editor.Controls
{
   public sealed class NamesEditor : Control
   {
      public TextBox TextBox { get; set; } = new();
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

   public sealed class MonarchNameControl : Control
   {
      internal TableLayoutPanel _tbl;
      private TextBox _nameBox;
      private TextBox _regnalBox;
      private TextBox _chanceBox;
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

         _regnalBox = new()
         {
            Text = name.OrdinalNumber.ToString(),
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
         _tbl.Controls.Add(_regnalBox, 1, 0);
         _tbl.Controls.Add(_chanceBox, 2, 0);
         _tbl.Controls.Add(_deleteButton, 3, 0);

         Controls.Add(_tbl);

         Size = size;
      }

      public bool GetNames(out MonarchName mName)
      {
         if (InputHelper.GetIntIfNotEmpty(_chanceBox, out var chance) &&
             InputHelper.GetIntIfNotEmpty(_regnalBox, out var regnal) &&
             InputHelper.GetStringIfNotEmpty(_nameBox, out var name))
         {
            mName = new(name, regnal, chance);
            return true;
         }

         mName = MonarchName.Empty;
         return false;
      }

      public void OnDeleteButton_Click(object? sender, EventArgs e)
      {
         if (Selection.SelectedCountry == Country.Empty)
            return;
         if (!GetNames(out var mName))
            return;
         Selection.SelectedCountry.CommonCountry.MonarchNames.Remove(mName);
         Parent?.Controls.Remove(this);
      }

   }
}