using System;
using System.Diagnostics;
using System.Reflection;
using Editor.Controls.NewControls;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Forms.Feature;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Controls
{
   public class ThreeColorStripesButton : Button, IPropertyControl<CommonCountry, Color>
   {
      public int ColorIndex1 { get; set; }
      public int ColorIndex2 { get; set; }
      public int ColorIndex3 { get; set; }
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<CommonCountry>> _getSaveables;

      public ThreeColorStripesButton()
      {
         PropertyInfo = typeof(CommonCountry).GetProperty(nameof(CommonCountry.RevolutionaryColor))!;
         Debug.Assert(PropertyInfo is not null && PropertyInfo.DeclaringType == typeof(CommonCountry), $"PropInfo: {PropertyInfo} declaring type is not of type {typeof(CommonCountry)} but of type {PropertyInfo.DeclaringType}");
         Debug.Assert(PropertyInfo.PropertyType == typeof(Color), $"PropInfo: {PropertyInfo} is not of type {typeof(Color)} but of type {PropertyInfo.PropertyType}");
         
         LoadGuiEvents.CommonCountryLoadAction += ((IPropertyControl<CommonCountry, Color>)this).LoadToGui;

         base.ForeColor = Color.Black;
         SetDefault();
         _getSaveables = () => [Selection.SelectedCountry.CommonCountry];
         MouseUp += OnMouseUp;
      }


      public void SetFromGui()
      {
         GetFromGui(out var value);
         PropertyInfo.SetValue(_getSaveables.Invoke().First(), value);
      }

      public void SetDefault()
      {
         ColorIndex1 = 0;
         ColorIndex2 = 0;
         ColorIndex3 = 0;
         Invalidate();
      }

      public IErrorHandle GetFromGui(out Color value)
      {
         value = Color.FromArgb(ColorIndex1, ColorIndex2, ColorIndex3);
         return ErrorHandle.Success;
      }

      public void SetValue(Color value)
      {
         ColorIndex1 = value.R;
         ColorIndex2 = value.G;
         ColorIndex3 = value.B;
         Invalidate();
      }

      public void SetColorIndexes(int one, int two, int three)
      {
         ColorIndex1 = one;
         ColorIndex2 = two;
         ColorIndex3 = three;
         Invalidate();
      }

      private void OnMouseUp(object? sender, MouseEventArgs e)
      {
         // Right click to reset the color
         if (e.Button == MouseButtons.Right)
         {
            var max = Globals.RevolutionaryColors.Count;
            var index1 = Globals.Random.Next(max);
            var index2 = Globals.Random.Next(max);
            var index3 = Globals.Random.Next(max);

            SetColorIndexes(index1, index2, index3);
            SetFromGui();
            return;
         }

         var revColorPicker = new RevolutionaryColorPicker();
         revColorPicker.SetIndexes(Selection.SelectedCountry.CommonCountry.RevolutionaryColor.R, Selection.SelectedCountry.CommonCountry.RevolutionaryColor.G, Selection.SelectedCountry.CommonCountry.RevolutionaryColor.B);
         revColorPicker.OnColorsChanged += (o, tuple) =>
         {
            SetColorIndexes(tuple.Item1, tuple.Item2, tuple.Item3);
         };
         if (revColorPicker.ShowDialog() == DialogResult.OK) 
            SetFromGui();
         else
         {
            ((IPropertyControl<CommonCountry, Color>)this).LoadToGui(_getSaveables.Invoke(), PropertyInfo, true);
         }
      }
      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         Rectangle rect = new (3, 3, ClientRectangle.Width - 4, ClientRectangle.Height - 4);
         var width = (rect.Width) / 3;

         if (!Globals.RevolutionaryColors.TryGetValue(ColorIndex1, out var col1)
             || !Globals.RevolutionaryColors.TryGetValue(ColorIndex2, out var col2)
             || !Globals.RevolutionaryColors.TryGetValue(ColorIndex3, out var col3))
         {
            MessageBox.Show($"Revolutionary Color index not found: ({ColorIndex1}, {ColorIndex2}, {ColorIndex1})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
         }


         using (var brush1 = new SolidBrush(col1))
         using (var brush2 = new SolidBrush(col2))
         using (var brush3 = new SolidBrush(col3))
         {
            e.Graphics.FillRectangle(brush1, new(rect.Left, rect.Top, width, rect.Height - 2));
            e.Graphics.FillRectangle(brush2, new(rect.Left + width, rect.Top, width, rect.Height - 2));
            e.Graphics.FillRectangle(brush3, new(rect.Left + 2 * width, rect.Top, width, rect.Height - 2));
         }

         // Draw index text centered in each stripe
         TextRenderer.DrawText(e.Graphics, ColorIndex1.ToString(), Font,
            new Rectangle(rect.Left, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         TextRenderer.DrawText(e.Graphics, ColorIndex2.ToString(), Font,
            new Rectangle(rect.Left + width, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         TextRenderer.DrawText(e.Graphics, ColorIndex3.ToString(), Font,
            new Rectangle(rect.Left + 2 * width, rect.Top - 1, width, rect.Height), ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

         // Draw a border around the button
         using (var borderPen = new Pen(Color.Black, 1)) // Adjust color and thickness as needed
         {
            e.Graphics.DrawRectangle(borderPen, 2, 2, rect.Width - 1, rect.Height - 1);
         }
         
         if (!Enabled)
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), ClientRectangle);
      }


   }
}