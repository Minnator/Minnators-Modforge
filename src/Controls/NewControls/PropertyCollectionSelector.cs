using System.Diagnostics;
using System.Reflection;
using Windows.System.UserProfile;
using Editor.Controls.NewControls;
using Editor.ErrorHandling;
using Editor.Events;
using Editor.Helper;
using Editor.Saving;
using Editor.src.Forms.Feature;

namespace Editor.src.Controls.NewControls
{
   public sealed class PropertyCollectionSelector<TSaveable, TProperty, TPropertyItem> : Control, IPropertyControlList<TSaveable, TProperty, TPropertyItem>
      where TSaveable : Saveable where TProperty : List<TPropertyItem>, new() where TPropertyItem : notnull
   {
      public PropertyInfo PropertyInfo { get; init; }
      private readonly Func<List<TSaveable>> _getSaveables;
      private CollectionSelectorBase _collectionSelectorBase;
      private PropertyInfo? _displayMember;
      private List<TPropertyItem> _startList = [];
      private Bitmap? _image = null;

      // Visual part
      private TableLayoutPanel _tableLayoutPanel;
      private Label _propertyNameLabel;
      private Button _modifyButton;
      private ListBox _previewList;
      private PictureBox _iconBox;
      private ToolTip _toolTip;

      private bool _rawStringMode;

      public PropertyCollectionSelector(PropertyInfo? propertyInfo, ref LoadGuiEvents.LoadAction<TSaveable> loadHandle, Func<List<TSaveable>> getSaveables,
                                        List<TPropertyItem> sourceItems, PropertyInfo? displayMember)
      {
         Debug.Assert(propertyInfo is not null && propertyInfo.DeclaringType == typeof(TSaveable),
                      $"PropInfo: {propertyInfo} declaring type is not of type {typeof(TSaveable)} but of type {propertyInfo.DeclaringType}");
         Debug.Assert(propertyInfo.PropertyType == typeof(TProperty),
                      $"PropInfo: {propertyInfo} is not of type {typeof(TProperty)} but of type {propertyInfo.PropertyType}");
         if (sourceItems.Count >= 1)
            if (displayMember == null && typeof(TPropertyItem) == typeof(string))
               _rawStringMode = true;
            else
               Debug.Assert(displayMember != null && displayMember.GetValue(sourceItems[0]) != null, $"{typeof(TPropertyItem)} does not define a display member \"{displayMember}\"");

         PropertyInfo = propertyInfo;
         _getSaveables = getSaveables;
         loadHandle += ((IPropertyControlList<TSaveable, TProperty, TPropertyItem>)this).LoadToGui;
         _displayMember = displayMember;

         if (!_rawStringMode)
            _collectionSelectorBase = new(AttributeHelper.GetDisplayMember(sourceItems, displayMember));
         else
            _collectionSelectorBase = new([.. sourceItems.Cast<string>()]);


         if (AttributeHelper.GetSharedAttributeList<TSaveable, TProperty, TPropertyItem>(PropertyInfo, out var items, getSaveables.Invoke()))
            if (_rawStringMode)
               _collectionSelectorBase.SetSelectedItems([.. items.Cast<string>()]);
            else
               _collectionSelectorBase.SetSelectedItems(AttributeHelper.GetDisplayMember(items, displayMember));

         Text = "Modify";

         SetImageIcon();

         InitializeGui();
      }

      private void ListBoxOnMouseMove(object? sender, MouseEventArgs mouseEventArgs)
      {
         var listbox = sender as ListBox;
         if (listbox == null) return;

         // set tool tip for listbox
         var strTip = string.Empty;
         var index = listbox.IndexFromPoint(mouseEventArgs.Location);

         if (index >= 0 && index < listbox.Items.Count)
            strTip = listbox.Items[index].ToString();

         if (_toolTip.GetToolTip(listbox) != strTip)
         {
            _toolTip.SetToolTip(listbox, strTip);
         }
      }

      private void SetImageIcon()
      {
         var attr = AttributeHelper.GetAttribute<GameIcon>(PropertyInfo);
         if (attr != null)
            _image = GameIconDefinition.GetIcon(attr.Icon);
      }

      private void InitializeGui()
      {
         Dock = DockStyle.Fill;
         Margin = Padding.Empty;
         MinimumSize = new(90, 90);

         _toolTip = new()
         {
            AutoPopDelay = 5000,
            InitialDelay = 1000,
            ReshowDelay = 500,
            ShowAlways = true
         };

         _tableLayoutPanel = new()
         {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            RowCount = 3,
            ColumnCount = 2,
            RowStyles =
            {
               new (SizeType.Absolute, 40),
               new (SizeType.Percent, 100),
               new (SizeType.Absolute, 27),
            },
            ColumnStyles =
            {
               new (SizeType.Percent, 50),
               new (SizeType.Percent, 50),
            },
         };

         _propertyNameLabel = new()
         {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = AttributeHelper.GetStringWithoutCamelCase(PropertyInfo.Name),
            Margin = new(2, 2, 1, 1),
            BorderStyle = BorderStyle.FixedSingle,
            Font = new("Arial", 8, FontStyle.Bold)
         };

         _modifyButton = new()
         {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = new(1, 1, 0, 1),
            Text = "modify"
         };

         _previewList = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(1, 2, 2, 2),
            BorderStyle = BorderStyle.FixedSingle,
            IntegralHeight = false,
         };
         _previewList.MouseMove += ListBoxOnMouseMove;

         _iconBox = new()
         {
            Dock = DockStyle.Fill,
            Margin = new(2, 1, 1, 0),
            BorderStyle = BorderStyle.FixedSingle,
            Image = _image,
            SizeMode = PictureBoxSizeMode.Zoom
         };
         _iconBox.Paint += (sender, args) =>
         {
            if (!Enabled)
               args.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), _iconBox.ClientRectangle);
         };

         Controls.Add(_tableLayoutPanel);

         _tableLayoutPanel.Controls.Add(_propertyNameLabel, 0, 0);
         _tableLayoutPanel.Controls.Add(_modifyButton, 0, 2);
         _tableLayoutPanel.Controls.Add(_iconBox, 0, 1);
         _tableLayoutPanel.Controls.Add(_previewList, 1, 0);
         _tableLayoutPanel.SetRowSpan(_previewList, 3);

         _propertyNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         _modifyButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         _previewList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
         _iconBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

         _modifyButton.Click += (sender, e) =>
         {
            _startList.Clear();
            GetFromGui(out var startList).Log();
            _startList = startList;
            _collectionSelectorBase.ShowDialog();
            SetFromGui();
         };
      }



      public void SetFromGui()
      {
         if (Globals.State == State.Running && GetFromGui(out var value).Log())
         {
            if (AttributeHelper.ScrambledListsEquals(_startList, value))
               return;
            //TODO make it more performant
            var remove = _startList.Except(value).ToHashSet();
            var add = value.Except(_startList).ToHashSet();
            Saveable.SetFieldEditCollection<TSaveable, TProperty, TPropertyItem>(_getSaveables.Invoke(), add, remove, PropertyInfo);
            if (!_rawStringMode)
               SetPreview(AttributeHelper.GetDisplayMember(value, _displayMember));
            else
               SetPreview(value.Cast<string>().ToList());
         }
      }

      public void SetDefault()
      {
         SetValue((TProperty)new List<TPropertyItem>());
      }

      public IErrorHandle GetFromGui(out TProperty value)
      {
         var strs = _collectionSelectorBase.GetSelectedItems();
         List<TPropertyItem> items = new(strs.Count);

         foreach (var str in strs)
            if (Converter.Convert<TPropertyItem>(str, PropertyInfo, out var partValue).Log())
               items.Add(partValue);

         value = (TProperty)items;
         return ErrorHandle.Sucess;
      }

      public void SetValue(TProperty value)
      {
         var values = _rawStringMode ? value.Cast<string>().ToList() : AttributeHelper.GetDisplayMember(value, _displayMember);
         _collectionSelectorBase.SetSelectedItems(values);
         SetPreview(values);
      }

      private void SetPreview(List<string> values)
      {
         _previewList.Items.Clear();
         _previewList.Items.AddRange([.. values]);
      }

      public void SetItems(List<string> newItems)
      {
         _collectionSelectorBase.SetConstSourceItems(newItems);
      }
   }
}