using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Editor.Controls.MMF_DARK
{
   [DefaultEvent("_TextChanged")]
   public partial class MmfTextBox : UserControl
   {
      public enum FocusModeEnum
      {
         Normal,
         Fade
      }

      public MmfTextBox()
      {
         InitializeComponent();
      }
      //Fields
      private Color _borderColor = Globals.Settings.Achievements.AchievementWindowBackColor;
      private int _borderWidth = 1;
      private Color _borderFocusColor = Globals.Settings.Achievements.AchievementProgressBarColor;
      private bool _isFocused;
      private int _cornerRadius;
      private FocusModeEnum _focusMode = FocusModeEnum.Normal;
      private string _placeHolderText = string.Empty;

      //Events
      public event EventHandler? _TextChanged;

      //Properties

      [Category("Minnator's Control")]
      public string PlaceHolderText
      {
         get => BaseTextBox.PlaceholderText;
         set => BaseTextBox.PlaceholderText = value;
      }

      [Category("Minnator's Control")]
      public FocusModeEnum FocusMode
      {
         get => _focusMode;
         set => _focusMode = value;
      }

      [Category("Minnator's Control")]
      public int CornerRadius
      {
         get => _cornerRadius;
         set => _cornerRadius = value;
      }

      [Category("Minnator's Control")]
      public Color BorderColor
      {
         get { return _borderColor; }
         set
         {
            _borderColor = value;
            Invalidate();
         }
      }
      [Category("Minnator's Control")]
      public int BorderWidth
      {
         get { return _borderWidth; }
         set
         {
            _borderWidth = value;
            Invalidate();
         }
      }


      [Category("Minnator's Control")]
      public bool PasswordChar
      {
         get { return BaseTextBox.UseSystemPasswordChar; }
         set { BaseTextBox.UseSystemPasswordChar = value; }
      }

      [Category("Minnator's Control")]
      public bool Multiline
      {
         get { return BaseTextBox.Multiline; }
         set { BaseTextBox.Multiline = value; }
      }

      [Category("Minnator's Control")]
      public override Color BackColor
      {
         get { return base.BackColor; }
         set
         {
            base.BackColor = value;
            BaseTextBox.BackColor = value;
         }
      }

      [Category("Minnator's Control")]
      public override Color ForeColor
      {
         get { return base.ForeColor; }
         set
         {
            base.ForeColor = value;
            BaseTextBox.ForeColor = value;
         }
      }

      [Category("Minnator's Control")]
      public override Font Font
      {
         get { return base.Font; }
         set
         {
            base.Font = value;
            BaseTextBox.Font = value;
            if (DesignMode)
               UpdateControlHeight();
         }
      }

      [Category("Minnator's Control")]
      public string Texts
      {
         get { return BaseTextBox.Text; }
         set { BaseTextBox.Text = value; }
      }

      [Category("Minnator's Control")]
      public Color BorderFocusColor
      {
         get { return _borderFocusColor; }
         set { _borderFocusColor = value; }
      }


      protected override void OnPaint(PaintEventArgs e)
      {
         var g = e.Graphics;
         if (Parent != null && CornerRadius > 0)
            g.Clear(Parent.BackColor);

         base.OnPaint(e);

         //Draw border
         using var penBorder = new Pen(_borderColor, _borderWidth);
         penBorder.Alignment = PenAlignment.Center;
         if (_isFocused) 
            penBorder.Color = _borderFocusColor;
         var rect = ClientRectangle with{Width = ClientRectangle.Width - BorderWidth, Height = ClientRectangle.Height - BorderWidth };
         if (CornerRadius > 0)
         {
            using var path = ControlHelper.CreateRoundedRectanglePath(rect, CornerRadius + BorderWidth / 2);
            g.FillPath(new SolidBrush(BackColor), path);
            if (_isFocused)
               switch (FocusMode)
               {
                  case FocusModeEnum.Normal:
                     g.SmoothingMode = SmoothingMode.AntiAlias;
                     g.DrawPath(penBorder, path);
                     break;
                  case FocusModeEnum.Fade:
                     penBorder.Width = MathF.Min(4.5f, Math.Max(4.5f, BorderWidth));
                     using (var fadePath = ControlHelper.CreateRoundedRectanglePath(rect, (int)penBorder.Width + BorderWidth / 2))
                     {
                        penBorder.Color = Parent?.BackColor ?? BackColor;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawPath(penBorder, fadePath);
                        break;
                     }
               }
            else
            {
               g.SmoothingMode = SmoothingMode.AntiAlias;
               g.DrawPath(penBorder, path);
            }
         }
         else //Normal Style
         {
            g.DrawRectangle(penBorder, 0, 0, Width - 0.5F, Height - 0.5F);

            if (_isFocused)
               switch (FocusMode)
               {
                  case FocusModeEnum.Fade:
                  case FocusModeEnum.Normal:
                     g.SmoothingMode = SmoothingMode.AntiAlias;
                     g.DrawRectangle(penBorder, 0, 0, Width - 0.5F, Height - 0.5F);
                     break;
               }
         }
      }

      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
         if (DesignMode)
            UpdateControlHeight();
      }

      protected override void OnLoad(EventArgs e)
      {
         base.OnLoad(e);
         UpdateControlHeight();
      }

      //Private methods
      private void UpdateControlHeight()
      {
         if (BaseTextBox.Multiline == false)
         {
            int txtHeight = TextRenderer.MeasureText("Text", Font).Height + 1;
            BaseTextBox.Multiline = true;
            BaseTextBox.MinimumSize = new Size(0, txtHeight);
            BaseTextBox.Multiline = false;

            Height = BaseTextBox.Height + Padding.Top + Padding.Bottom;
         }
      }

      //TextBox events
      private void textBox1_TextChanged(object sender, EventArgs e)
      {
         if (_TextChanged != null)
            _TextChanged.Invoke(sender, e);
      }

      private void textBox1_Click(object sender, EventArgs e)
      {
         OnClick(e);
      }

      private void textBox1_MouseEnter(object sender, EventArgs e)
      {
         OnMouseEnter(e);
      }

      private void textBox1_MouseLeave(object sender, EventArgs e)
      {
         OnMouseLeave(e);
      }

      private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
      {
         OnKeyPress(e);
      }

      private void textBox1_Enter(object sender, EventArgs e)
      {
         _isFocused = true;
         Invalidate();
      }

      private void textBox1_Leave(object sender, EventArgs e)
      {
         _isFocused = false;
         Invalidate();
      }
   }
}
