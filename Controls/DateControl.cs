using System.Globalization;
using Editor.Helper;
using static Editor.Helper.ProvinceEventHandler;

namespace Editor.Controls
{
   public enum DateControlLayout
   {
      Horizontal,
      Vertical
   }
   public class DateControl : Control
   {
      private TableLayoutPanel _tableLayoutPanel= new ();

      private TextBox _dateTextBox = new ()
      {
         Dock = DockStyle.Fill,
         TextAlign = HorizontalAlignment.Center,
         Margin = new(0,1,0,0),
         Padding = new(0),
      };
      
      private readonly DecreaseButton _yearDecreaseButton = new ();
      private readonly DecreaseButton _monthDecreaseButton = new ();
      private readonly DecreaseButton _dayDecreaseButton = new ();

      private readonly IncreaseButton _yearIncreaseButton = new ();
      private readonly IncreaseButton _monthIncreaseButton = new ();
      private readonly IncreaseButton _dayIncreaseButton = new ();

      public DateControl(DateTime date, DateControlLayout layout)
      {
         Date = date;
         DateControlLayout = layout;

         if (DateControlLayout == DateControlLayout.Vertical) 
            InitVertical();
         else
            InitHorizontal();

         _dateTextBox.TextChanged += OnDateTextChanged;

         _yearIncreaseButton.Click += OnYearIncrease;
         _monthIncreaseButton.Click += OnMonthIncrease;
         _dayIncreaseButton.Click += OnDayIncrease;
         _yearDecreaseButton.Click += OnYearDecrease;
         _monthDecreaseButton.Click += OnMonthDecrease;
         _dayDecreaseButton.Click += OnDayDecrease;
         
         Size = _tableLayoutPanel.Size;
         Margin = new(0);
         Padding = new(0);
         Controls.Add(_tableLayoutPanel);
      }


      private void InitVertical()
      {
         _tableLayoutPanel = new()
         {
            RowStyles =
            {
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 27),
               new(SizeType.Absolute, 20),
            },
            ColumnStyles =
            {
               new(SizeType.Absolute, 40),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
            },
            Size = new(100, 67),
            Padding = new(0),
            Margin = new(0),
         };

         _tableLayoutPanel.Controls.Add(_dateTextBox, 0, 1);
         _tableLayoutPanel.SetColumnSpan(_dateTextBox, 3);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 0, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 0, 2);
         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 1, 2);
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 2, 2);
      }

      private void InitHorizontal()
      {
         _tableLayoutPanel = new()
         {
            RowStyles =
            {
               new(SizeType.Absolute, 25),
            },
            ColumnStyles =
            {
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 90), //Date
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
               new(SizeType.Absolute, 20),
            },
            Size = new(210, 25),
            Padding = new(0),
            Margin = new(0),
         };

         _tableLayoutPanel.Controls.Add(_monthDecreaseButton, 1, 0);
         _tableLayoutPanel.Controls.Add(_dayDecreaseButton, 0, 0);
         _tableLayoutPanel.Controls.Add(_yearDecreaseButton, 2, 0);

         _tableLayoutPanel.Controls.Add(_dateTextBox, 3, 0);

         _tableLayoutPanel.Controls.Add(_yearIncreaseButton, 4, 0);
         _tableLayoutPanel.Controls.Add(_monthIncreaseButton, 5, 0);
         _tableLayoutPanel.Controls.Add(_dayIncreaseButton, 6, 0);

         _yearDecreaseButton.Text = "<";
         _monthDecreaseButton.Text = "<";
         _dayDecreaseButton.Text = "<";

         _yearIncreaseButton.Text = ">";
         _monthIncreaseButton.Text = ">";
         _dayIncreaseButton.Text = ">";
      }

      private DateTime _date;
      public DateTime Date
      {
         get => _date;
         set
         {
            if (_date == value)
               return;
            _date = value;
            _dateTextBox.Text = _date.ToString("yyyy-MM-dd");
            OnDateChanged?.Invoke(this, EventArgs.Empty);
         }
      } 

      public DateControlLayout DateControlLayout { get; set; }

      public static event EventHandler<EventArgs> OnDateChanged = delegate { };

      private void OnDateTextChanged(object? sender, EventArgs e)
      {
         if (Parsing.TryParseFullDate(_dateTextBox.Text, out var date))
            Date = date;
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