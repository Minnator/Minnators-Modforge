namespace Editor.Controls
{
   public class DateControl : Control
   {
      private TableLayoutPanel _tableLayoutPanel= new ()
      {
         RowStyles =
         {
            new (SizeType.Absolute, 20),
            new (SizeType.Absolute, 27),
            new (SizeType.Absolute,20),
         },
         ColumnStyles =
         {
            new (SizeType.Absolute, 40),
            new (SizeType.Absolute, 20),
            new (SizeType.Absolute, 20),
         },
         Size = new (100, 67),
         Padding = new (0),
         Margin = new (0),
      };

      private TextBox _dateTextBox = new ()
      {
         Dock = DockStyle.Fill,
         TextAlign = HorizontalAlignment.Center,
      };
      
      private readonly DecreaseButton _yearDecreaseButton = new ();
      private readonly DecreaseButton _monthDecreaseButton = new ();
      private readonly DecreaseButton _dayDecreaseButton = new ();

      private readonly IncreaseButton _yearIncreaseButton = new ();
      private readonly IncreaseButton _monthIncreaseButton = new ();
      private readonly IncreaseButton _dayIncreaseButton = new ();

      public DateControl(DateTime date)
      {
         Date = date;

         _yearIncreaseButton.Click += OnYearIncrease;
         _monthIncreaseButton.Click += OnMonthIncrease;
         _dayIncreaseButton.Click += OnDayIncrease;
         _yearDecreaseButton.Click += OnYearDecrease;
         _monthDecreaseButton.Click += OnMonthDecrease;
         _dayDecreaseButton.Click += OnDayDecrease;

         _tableLayoutPanel.Controls.Add(_dateTextBox, 0, 1);
         _tableLayoutPanel.SetColumnSpan(_dateTextBox, 3);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 0, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 0, 2);
         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 1, 2);
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 2, 2);
         
         Size = _tableLayoutPanel.Size;
         Controls.Add(_tableLayoutPanel);
      }

      public DateTime Date
      {
         get => DateTime.Parse(_dateTextBox.Text);
         set => _dateTextBox.Text = value.ToString("yyyy/MM/dd");
      }

      public event EventHandler DateChanged
      {
         add => _dateTextBox.TextChanged += value;
         remove => _dateTextBox.TextChanged -= value;
      }

      public void OnYearIncrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddYears(10),
            Keys.Shift => Date.AddYears(5),
            _ => Date.AddYears(1)
         };
      }

      public void OnMonthIncrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddMonths(10),
            Keys.Shift => Date.AddMonths(5),
            _ => Date.AddMonths(1)
         };
      }

      public void OnDayIncrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddDays(10),
            Keys.Shift => Date.AddDays(5),
            _ => Date.AddDays(1)
         };
      }

      public void OnYearDecrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddYears(-10),
            Keys.Shift => Date.AddYears(-5),
            _ => Date.AddYears(-1)
         };
      }

      public void OnMonthDecrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddMonths(-10),
            Keys.Shift => Date.AddMonths(-5),
            _ => Date.AddMonths(-1)
         };
      }

      public void OnDayDecrease (object sender, EventArgs e)
      {
         Date = ModifierKeys switch
         {
            Keys.Control => Date.AddDays(-10),
            Keys.Shift => Date.AddDays(-5),
            _ => Date.AddDays(-1)
         };
      }

   }
}

public sealed class DecreaseButton : Button
{
   public DecreaseButton()
   {
      Text = "-";
      Dock = DockStyle.Fill;
      TextAlign = ContentAlignment.MiddleCenter;
      Margin = new (0);
      Padding = new (0);
   }
}

public sealed class IncreaseButton : Button
{
   public IncreaseButton()
   {
      Text = "+";
      Dock = DockStyle.Fill;
      TextAlign = ContentAlignment.MiddleCenter;
      Margin = new(0);
      Padding = new(0);
   }
}