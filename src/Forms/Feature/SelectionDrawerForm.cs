using System.Drawing.Imaging;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Saveables;
using Editor.DataClasses.Settings;
using Editor.Helper;

namespace Editor.Forms.Feature
{
   public partial class SelectionDrawerForm : Form
   {
      private ZoomControl ZoomControl { get; set; } = new(new(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format32bppArgb));
      private List<DrawingLayer> layers = [];

      public SelectionDrawerForm()
      {
         InitializeComponent();

         MainLayoutPanel.Controls.Add(ZoomControl, 1, 0);
         ZoomControl.FocusOn(new Rectangle(0, 0, Globals.MapWidth, Globals.MapHeight));
         
         MapModeManager.MapModeChanged += OnMapModeChanged;
         Selection.OnProvinceGroupDeselected += RenderOnEvent;
         Selection.OnProvinceGroupSelected += RenderOnEvent;

         if (Globals.Settings.Gui.SelectionDrawerAlwaysOnTop)
            TopMost = true;

         FormClosing += OnFormClose;

         LayerListView.ItemMoved += ListBoxOnItemMoved;
      }

      private void ListBoxOnItemMoved(object? sender, SwappEventArgs e)
      {
         var layer = layers[e.From];
         layers.RemoveAt(e.From);
         layers.Insert(e.To, layer);
         RenderImage();
      }

      private void OnMapModeChanged(object? s, MapMode e) => RenderImage();

      private void RenderOnEvent(object? s, List<Province> e) => RenderImage();

      private void OnFormClose(object? sender, EventArgs e)
      {
         Selection.OnProvinceGroupDeselected -= RenderOnEvent;
         Selection.OnProvinceGroupSelected -= RenderOnEvent;
         MapModeManager.MapModeChanged -= OnMapModeChanged;
      }

      private void SelectFolderButton(object sender, EventArgs e)
      {
         IO.OpenFolderDialog(Globals.ModPath, "select a folder where to save the image", out var path);
         Globals.Settings.Saving.MapModeExportPath = path;
      }

      private void RenderImage()
      {
         MapDrawing.Clear(ZoomControl, Color.DimGray);
         var rectangle = Rectangle.Empty;
         foreach (var layer in layers)
         {
            var map = layer.RenderToMap(ZoomControl);
            if (rectangle == Rectangle.Empty)
               rectangle = map;
            rectangle = Geometry.GetBounds(rectangle, map);
         }
         if (Selection.Count == 0)
            return;
         ZoomControl.FocusOn(rectangle);
         ZoomControl.Invalidate();
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         if (string.IsNullOrWhiteSpace(PathTextBox.Text) || LayerListView.Items.Count == 0)
            return;
         using (var map = ZoomControl.Map)
         {
            var path = Path.Combine(Globals.Settings.Saving.MapModeExportPath, PathTextBox.Text + ".png");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
               MessageBox.Show("The directory does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            else
            {
               switch (GetImageSize())
               {
                  case ImageSize.ImageSizeSelection:
                     var bounds = Geometry.GetBounds(Selection.GetSelectedProvinces);
                     var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format24bppRgb);
                     using (var graphics = Graphics.FromImage(bitmap))
                        graphics.DrawImage(map, bounds with
                        {
                           X = 0,
                           Y = 0
                        }, bounds, GraphicsUnit.Pixel);
                     bitmap.Save(path, ImageFormat.Png);
                     bitmap?.Dispose();
                     break;
                  case ImageSize.ImageSizeOriginal:
                     map.Save(path, ImageFormat.Png);
                     break;
               }
            }
         }
      }

      private ImageSize GetImageSize()
      {
         ImageSize result;
         return !Enum.TryParse<ImageSize>(ImageSizeBox.Text, true, out result) ? ImageSize.ImageSizeOriginal : result;
      }

      private DrawingOptions GetDrawingOption()
      {
         DrawingOptions result;
         return !Enum.TryParse<DrawingOptions>(OptionComboBox.Text, true, out result) ? DrawingOptions.Selection : result;
      }

      private void SelectionDrawerForm_FormClosing(object? sender, FormClosingEventArgs e)
      {
         ZoomControl.Dispose();
      }

      private void AddButton_Click(object sender, EventArgs e)
      {
         if (OptionComboBox.SelectedItem == null)
            return;
         LayerListView.Items.Add(new ListViewItem(OptionComboBox.Text));
         layers.Add(new DrawingLayer(GetDrawingOption()));
         RenderImage();
      }
   }

   public enum ImageSize
   {
      ImageSizeOriginal,
      ImageSizeSelection,

   }

   public enum DrawingOptions
   {
      Country,
      Area,
      Region,
      SuperRegion,
      ProvinceCollections,
      ColonialRegion,
      TradeNode,
      TradeCompanyRegion,
      Selection,
      NeighboringProvinces,
      NeighboringCountries,
      SeaProvinces,
      CoastalOutline,
      AllCoast,
      AllLand,
      AllSea,
      Everything,
   }

   public class DrawingLayer
   {
      private Func<List<Province>> ProvinceGetter;
      private DrawingOptions _options;

      public MapMode MapMode { get; set; } = MapModeManager.IdMapMode;

      public RenderingSettings.BorderMergeType Style { get; set; } = RenderingSettings.BorderMergeType.Merge;

      public PixelsOrBorders pixelsOrBorders { get; set; } = PixelsOrBorders.Both;

      public Color Shading { get; set; } = Color.FromArgb(0, 0, 0, 0);



      public DrawingOptions Options
      {
         get => _options;
         set
         {
            _options = value;
            SetProvinceGetter();
         }
      }

      public DrawingLayer(DrawingOptions options) => Options = options;

      private void SetProvinceGetter()
      {
         switch (Options)
         {
            case DrawingOptions.Selection:
               ProvinceGetter = Selection.GetSelectedProvincesFunc;
               break;
            default:
               ProvinceGetter = Selection.GetSelectedProvincesFunc;
               break;
         }
      }

      private int GetColor(Province province)
      {
         var factor = Shading.A / 255.0f;
         var inverse = 1f - factor;
         var color = Color.FromArgb(MapMode.GetProvinceColor(province));
         var R = (byte)(color.R * factor + Shading.R * inverse);
         var G = (byte)(color.G * factor + Shading.G * inverse);
         var B = (byte)(color.B * factor + Shading.B * inverse);
         return (R << 16 | G << 8 | B);
      }

      public Rectangle RenderToMap(ZoomControl control)
      {
         List<Province> provinceList = ProvinceGetter();
         MapDrawing.DrawOnMap(provinceList, GetColor, control, pixelsOrBorders);
         return Geometry.GetBounds(provinceList);
      }

      public override string ToString() => Options.ToString();
   }
}
