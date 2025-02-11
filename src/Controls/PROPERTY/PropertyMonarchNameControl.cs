using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Parser;
using Editor.Properties;
using Editor.Saving;

namespace Editor.Controls.NewControls
{
   public sealed class PropertyMonarchNamesControl<TSaveable, TProperty> : FlowLayoutPanel, IPropertyControlList<TSaveable, TProperty, MonarchName> where TSaveable : Saveable where TProperty : List<MonarchName>, new()
   {
      private readonly Func<List<TSaveable>> _getSaveables;

      private readonly List<MonarchNameTLP<TSaveable, TProperty>> monarchNameTLPs; 

      public PropertyInfo PropertyInfo { get; init; }

      public PropertyMonarchNamesControl(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables)
      {
         PropertyInfo = propertyInfo;

         loadHandle += ((IPropertyControlList<TSaveable, TProperty, MonarchName>)this).LoadToGui;
         _getSaveables = getSaveables;

         Dock = DockStyle.Fill;
         FlowDirection = FlowDirection.TopDown;
         Margin = new(0);
         BorderStyle = BorderStyle.FixedSingle;
         WrapContents = false;
         AutoScroll = true;
         monarchNameTLPs = new(Globals.Settings.Gui.NumOfPreloadedMonarchNameElements);
         for (var i = 0; i < Globals.Settings.Gui.NumOfPreloadedMonarchNameElements; i++)
         {
            var tlp = new MonarchNameTLP<TSaveable, TProperty>(this);
            monarchNameTLPs.Add(tlp);
            Controls.Add(tlp);
         }
      }

      public void UpdateCache(int newValue)
      {
         if (monarchNameTLPs.Count == newValue)
            return;
         if (monarchNameTLPs.Count < newValue)
         {
            var i = monarchNameTLPs.Count;
            for (; i < Controls.Count; i++)
            {
               monarchNameTLPs.Add((MonarchNameTLP<TSaveable, TProperty>)Controls[i]);
            }
            for (; i < newValue; i++)
            {
               var tlp = new MonarchNameTLP<TSaveable, TProperty>(this);
               monarchNameTLPs.Add(tlp);
               Controls.Add(tlp);
            }
         }
         else
         {
            for (var i = newValue; i < monarchNameTLPs.Count;)
            {
               monarchNameTLPs.RemoveAt(monarchNameTLPs.Count - 1);
            }
         }
      }

      public void SetFromGui()
      {
         throw new EvilActions("Lol, Why do this? We don't do this here. Use other stuff like Meth.");
      }

      public void SetDefault()
      {
         SetValue([]);
      }

      public void SetValue(TProperty value)
      {
         SuspendLayout();
         if (!Globals.MapWindow.ShowMonachrNamesCB.Checked || value.Count == 0)
         {
            var i = 0;
            for (; i < monarchNameTLPs.Count; i++)
            {
               monarchNameTLPs[i].Enabled = false;
               monarchNameTLPs[i].Visible = false;
            }
            for (; i < Controls.Count; i++)
            {
               Controls.RemoveAt(Controls.Count - 1);
            }
         }
         else
         {
            // enough cached Controls
            if (value.Count <= monarchNameTLPs.Count)
            {
               Debug.Assert(monarchNameTLPs.Count <= Globals.Settings.Gui.NumOfPreloadedMonarchNameElements, "Values should always be the same. Is an update of either causing a desync");

               var i = 0;
               for (; i < value.Count; i++)
               {
                  monarchNameTLPs[i].SetMonarchName(value[i]);
               }
               for (; i < monarchNameTLPs.Count; i++)
               {
                  monarchNameTLPs[i].Enabled = false;
                  monarchNameTLPs[i].Visible = false;
               }
               for (; i < Controls.Count; i++)
               {
                  Controls.RemoveAt(Controls.Count - 1);
               }
            }
            else // not enough cached Controls
            {
               var i = 0;
               for (; i < monarchNameTLPs.Count; i++)
               {
                  monarchNameTLPs[i].SetMonarchName(value[i]);
               }

               if (value.Count < Controls.Count)
               {
                  for (; i < value.Count; i++)
                  {
                     ((MonarchNameTLP<TSaveable, TProperty>)Controls[i]).SetMonarchName(value[i]);
                  }
                  for (;i < Controls.Count;)
                  {
                     Controls.RemoveAt(Controls.Count - 1);
                  }
               }
               else
               {
                  for (; i < Controls.Count; i++)
                  {
                     ((MonarchNameTLP<TSaveable, TProperty>)Controls[i]).SetMonarchName(value[i]);
                  }
                  for (; i < value.Count; i++)
                  {
                     Controls.Add(new MonarchNameTLP<TSaveable, TProperty>(value[i], this));
                  }
               }
            }
         }
         ResumeLayout();
      }

      public void EditMonarchName(MonarchName oldMonarch, MonarchName newMonarch)
      {
         if (Globals.State == State.Running)
         {
            Saveable.SetFieldEditCollection<TSaveable, TProperty, MonarchName>(_getSaveables.Invoke(), [newMonarch], [oldMonarch], PropertyInfo);
         }
      }

      public void AddMonarchName(MonarchName mName)
      {
         if (Globals.State == State.Running)
         {
            Saveable.SetFieldEditCollection<TSaveable, TProperty, MonarchName>(_getSaveables.Invoke(), [mName], [], PropertyInfo);
         }
      }

      public void DeleteMonarchName(MonarchNameTLP<TSaveable, TProperty> mntlp)
      {
         if (Globals.State == State.Running && mntlp.GetFromGui(out var monarch).Log())
         {
            Saveable.SetFieldEditCollection<TSaveable, TProperty, MonarchName>(_getSaveables.Invoke(), [], [monarch], PropertyInfo);
         }
      }
   }

   public sealed class MonarchNameTLP<TSaveable, TProperty> : TableLayoutPanel where TSaveable : Saveable where TProperty : List<MonarchName>, new()
   {
      private TextBox _nameTextBox;
      private TextBox _chanceTextBox;
      private Button _deleteButton;
      private MonarchName _monarchName;

      private PropertyMonarchNamesControl<TSaveable, TProperty> _editor;

      public MonarchNameTLP(PropertyMonarchNamesControl<TSaveable, TProperty> editor) : this(MonarchName.Empty, editor)
      {
         Enabled = false;
         Visible = false;
      }

      public MonarchNameTLP(MonarchName name, PropertyMonarchNamesControl<TSaveable, TProperty> editor)
      {
         _editor = editor;
         _monarchName = name;

         Size = new(editor.Width - 19, 25);
         ColumnCount = 3;
         RowCount = 1;
         ColumnStyles.Add(new(SizeType.Percent, 100));
         ColumnStyles.Add(new(SizeType.Absolute, 55));
         ColumnStyles.Add(new(SizeType.Absolute, 55));
         Margin = new(0);

         BackColor = Color.DarkGray;

         _nameTextBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
            ScrollBars = ScrollBars.Vertical
         };
         _nameTextBox.Leave += OnTBLeave;
         _nameTextBox.KeyPress += OnKey_PressTB;

         _chanceTextBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
         };
         _chanceTextBox.Leave += OnTBLeave;
         _chanceTextBox.KeyPress += OnKey_PressTB;
         _chanceTextBox.KeyPress += OnlyNumbersKey_Press;

         _deleteButton = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(0),
            Image = Resources.RedX,
         };
         _deleteButton.Click += OnDeleteButton_Click;

         Controls.Add(_nameTextBox, 0, 0);
         Controls.Add(_chanceTextBox, 1, 0);
         Controls.Add(_deleteButton, 2, 0);
         
         _nameTextBox.Text = name.Name;
         _chanceTextBox.Text = name.Chance.ToString();
      }

      public void SetMonarchName(MonarchName name)
      {
         _monarchName = name;
         _nameTextBox.Text = name.Name;
         _chanceTextBox.Text = name.Chance.ToString();
         Size = new(_editor.Width - 19, 25);
         Enabled = true;
         Visible = true;
      }

      private void OnlyNumbersKey_Press(object? sender, KeyPressEventArgs e)
      {
         if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != (char)Keys.Enter)
            e.Handled = true;
      }

      private void OnDeleteButton_Click(object? sender, EventArgs e)
      {
        _editor.DeleteMonarchName(this);
      }

      public void OnTBLeave(object? sender, EventArgs e)
      {
         if (GetFromGui(out var newMonarchName).Log())
            _editor.EditMonarchName(_monarchName, newMonarchName);
      }

      public void OnKey_PressTB(object? sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Enter)
         {
            if (GetFromGui(out var newMonarchName).Log())
               _editor.EditMonarchName(_monarchName, newMonarchName);
            e.Handled = true;
         }
      }

      public IErrorHandle GetFromGui(out MonarchName mName)
      {
         return Parsing.GetMonarchNameFromTextBoxes(_nameTextBox, _chanceTextBox, out mName);
      }
   }
}