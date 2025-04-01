using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor.src.Forms.PopUps
{
   public partial class StringPopUp : Form
   {
      public StringPopUp()
      {
         InitializeComponent();
      }

      public static void ShowDialog(string text, string caption)
      {
         var dialog = new StringPopUp();
         dialog.Text = caption;
         dialog.StringTextBox.Text = text;
         dialog.ShowDialog();
      }
   }
}
