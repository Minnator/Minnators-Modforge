using System.Drawing.Imaging;
using Editor.Controls;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.DataClasses.Saveables;
using Editor.DataClasses.Settings;
using Editor.Helper;
using Editor.Properties;
using Region = Editor.DataClasses.Saveables.Region;

namespace Editor.Forms.Feature
{
   public partial class SelectionDrawerForm : Form
   {
      private ZoomControl ZoomControl { get; set; } = new(new(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format32bppArgb));
      private List<DrawingLayer> Layers = [];

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

         LayerListView.FullRowSelect = true;

         LayerListView.ItemMoved += ListBoxOnItemMoved;

         LayerListView.KeyDown += (s, e) =>
         {
            if (e.KeyCode == Keys.Delete)
            {
               if (LayerListView.SelectedIndices.Count == 1)
               {
                  Layers.RemoveAt(LayerListView.SelectedIndices[0]);
                  LayerListView.Items.RemoveAt(LayerListView.SelectedIndices[0]);
                  RenderImage();
               }
            }
         };

         LayerListView.MouseDoubleClick += (s, e) =>
         {
            if (LayerListView.SelectedItems.Count == 1)
            {
               var layer = Layers[LayerListView.SelectedIndices[0]];
               var popup = new PopUpForm(layer)
               {
                  StartPosition = FormStartPosition.CenterParent,
                  TopMost = true
               };

               // Set the owner to the always-on-top form
               popup.PropertyChanged += (s, e) => RenderImage();
               popup.ShowDialog(this);
               RenderImage();

            }
         };

         OptionComboBox.Items.AddRange([.. Enum.GetNames(typeof(DrawingOptions))]);
         MapModeselection.Items.AddRange([.. Enum.GetNames(typeof(MapModeType))]);
         ImageSizeBox.Items.AddRange([.. Enum.GetNames(typeof(ImageSize))]);
         ImageSizeBox.SelectedIndex = 0;
         MapModeselection.SelectedIndex = 0;

      }

      private void ListBoxOnItemMoved(object? sender, SwappEventArgs e)
      {
         var layer = Layers[e.From];
         Layers.RemoveAt(e.From);
         Layers.Insert(e.To, layer);
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
         IO.OpenFolderDialog(Globals.Settings.Saving.MapModeExportPath, "select a folder where to save the image", out var path);
         Globals.Settings.Saving.MapModeExportPath = path;
      }

      private void RenderImage()
      {
         MapDrawing.Clear(ZoomControl, Color.DimGray);
         var rectangle = Rectangle.Empty;
         foreach (var layer in Layers)
         {
            var map = layer.RenderToMap(ZoomControl, MapModeManager.GetMapMode(layer.MapMode));
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

         using var map = ZoomControl.Map;
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


      private ImageSize GetImageSize()
      {
         return !Enum.TryParse(ImageSizeBox.Text, true, out ImageSize result) ? ImageSize.ImageSizeOriginal : result;
      }

      private DrawingOptions GetDrawingOption()
      {
         return !Enum.TryParse(OptionComboBox.Text, true, out DrawingOptions result) ? DrawingOptions.Selection : result;
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
         Layers.Add(new (GetDrawingOption(), Enum.Parse<MapModeType>(MapModeselection.Text)));
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

      private MapModeType mapMode;
      public MapModeType MapMode
      {
         get => mapMode;
         set
         {
            mapMode = value;
            _mode = MapModeManager.GetMapMode(value);
         }
      }

      private MapMode _mode;
      public RenderingSettings.BorderMergeType Style { get; set; } = RenderingSettings.BorderMergeType.Merge;
      public PixelsOrBorders pixelsOrBorders { get; set; } = PixelsOrBorders.Both;
      public byte Opacity { get; set; } = 0;
      public Color Shading { get; set; } = Color.White;
      public Color BorderColor { get; set; } = Color.Black;


      public DrawingOptions Options
      {
         get => _options;
         set
         {
            _options = value;
            SetProvinceGetter();
         }
      }

      public DrawingLayer(DrawingOptions options, MapModeType mode)
      {
         MapMode = mode;
         Options = options;
      }
      
      private void SetProvinceGetter()
      {
         switch (Options)
         {
            case DrawingOptions.Selection:
               ProvinceGetter = Selection.GetSelectedProvincesFunc;
               break;
            case DrawingOptions.Country:
               ProvinceGetter = () =>
               {
                  HashSet<Country> countries = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     countries.Add(province.Owner);
                  return countries.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.Area:
               ProvinceGetter = () =>
               {
                  HashSet<Area> areas = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     areas.Add(province.Area);
                  return areas.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.Region:
               ProvinceGetter = () =>
               {
                  HashSet<Region> regions = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     regions.Add(province.Area.Region);
                  return regions.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.SuperRegion:
               ProvinceGetter = () =>
               {
                  HashSet<SuperRegion> srs = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     srs.Add(province.Area.Region.SuperRegion);
                  return srs.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.ProvinceCollections:
               ProvinceGetter = () =>
               {
                  HashSet<ProvinceGroup> groups = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     foreach (var group in Globals.ProvinceGroups.Values)
                        if (!groups.Contains(group) && group.GetProvinces().Contains(province))
                              groups.Add(group);
                  return groups.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.ColonialRegion:
               ProvinceGetter = () =>
               {
                  HashSet<ColonialRegion> regions = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     foreach (var group in Globals.ColonialRegions.Values)
                        if (!regions.Contains(group) && group.GetProvinces().Contains(province))
                           regions.Add(group);
                  return regions.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.TradeNode:
               ProvinceGetter = () =>
               {
                  HashSet<TradeNode> nodes = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     foreach (var group in Globals.TradeNodes.Values)
                        if (!nodes.Contains(group) && group.GetProvinces().Contains(province))
                           nodes.Add(group);
                  return nodes.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.TradeCompanyRegion:
               ProvinceGetter = () =>
               {
                  HashSet<TradeCompany> companies = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     foreach (var group in Globals.TradeCompanies.Values)
                        if (!companies.Contains(group) && group.GetProvinces().Contains(province))
                           companies.Add(group);
                  return companies.SelectMany(collection => collection.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.NeighboringProvinces:
               ProvinceGetter = () =>
               {
                  HashSet<Province> provinces = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     provinces.UnionWith(province.Neighbors);
                  return provinces.ToList();
               };
               break;
            case DrawingOptions.NeighboringCountries:
               ProvinceGetter = () =>
               {
                  HashSet<Country> countries = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     countries.UnionWith(province.Neighbors.Select(p => p.Owner));
                  return countries.SelectMany(country => country.GetProvinces()).ToList();
               };
               break;
            case DrawingOptions.SeaProvinces:
               ProvinceGetter = () =>
               {
                  HashSet<Province> coastalProvinces = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     if (Globals.SeaProvinces.Contains(province))
                        coastalProvinces.Add(province);
                  return coastalProvinces.ToList();
               };
               break;
            case DrawingOptions.CoastalOutline:
               ProvinceGetter = () =>
               {
                  HashSet<Province> coastalProvinces = [];
                  foreach (var province in Selection.GetSelectedProvinces)
                     foreach (var nei in province.Neighbors)
                        if (Globals.SeaProvinces.Contains(nei))
                           coastalProvinces.Add(province);
                  return coastalProvinces.ToList();
               };
               break;
            case DrawingOptions.AllCoast:
               ProvinceGetter = () =>
               {
                  HashSet<Province> coastalProvinces = [];
                  foreach (var province in Globals.LandProvinces)
                     foreach (var nei in province.Neighbors)
                        if (Globals.SeaProvinces.Contains(nei))
                           return coastalProvinces.ToList();
                  return coastalProvinces.ToList();
               };
               break;
            case DrawingOptions.AllLand:
               ProvinceGetter = () => Globals.LandProvinces.ToList();
               break;
            case DrawingOptions.AllSea:
               ProvinceGetter = () => Globals.SeaProvinces.ToList();
               break;
            case DrawingOptions.Everything:
               ProvinceGetter = () => Globals.Provinces.ToList();
               break;
            default:
               ProvinceGetter = Selection.GetSelectedProvincesFunc;
               break;
         }
      }

      private int GetColor(Province province)
      {
         var factor = Opacity / 255.0f;
         var inverse = 1f - factor;
         var color = Color.FromArgb(_mode.GetProvinceColor(province));
         var R = (byte)(color.R * inverse + Shading.R * factor);
         var G = (byte)(color.G * inverse + Shading.G * factor);
         var B = (byte)(color.B * inverse + Shading.B * factor);
         return (R << 16 | G << 8 | B);
      }

      public Rectangle RenderToMap(ZoomControl control, MapMode mode)
      { 
         var provinceList = ProvinceGetter();
         Dictionary<Province, int> cache = new (provinceList.Count);
         //MapModeManager.ConstructClearCache(Globals.Provinces, _mode, cache);
         var defaultColor = Color.LightGray.ToArgb();
         foreach (var province in Globals.Provinces.Except(provinceList))
            cache[province] = defaultColor;
         MapModeManager.ConstructCache(provinceList, mode, cache);


         switch (pixelsOrBorders)
         {
            case PixelsOrBorders.Pixels:
               MapDrawing.DrawOnMap(provinceList, GetColor, control, PixelsOrBorders.Both, cache);
               break;
            case PixelsOrBorders.Borders:
               MapDrawing.DrawOnMap(provinceList, BorderColor.ToArgb(), control, PixelsOrBorders.Borders, cache);
               break;
            case PixelsOrBorders.Both:
               MapDrawing.DrawOnMap(provinceList, GetColor, control, PixelsOrBorders.Pixels, cache);
               MapDrawing.DrawOnMap(provinceList, BorderColor.ToArgb(), control, PixelsOrBorders.Borders, cache);
               break;
         }

         
         return Geometry.GetBounds(provinceList);
      }

      public override string ToString() => Options.ToString();
   }

   public class PopUpForm : Form
   {
      private PropertyGrid _propGrid;

      public event EventHandler? PropertyChanged;

      public PopUpForm(object selectedObject)
      {
         var propertyGrid = new PropertyGrid();
         propertyGrid.Dock = DockStyle.Fill;
         _propGrid = propertyGrid;

         Controls.Add(_propGrid);
         _propGrid.SelectedObject = selectedObject;
         _propGrid.PropertyValueChanged += OnPropertyChanged;

         Text = selectedObject.GetType().Name;
      }

      public void OnPropertyChanged(object? sender, EventArgs e)
      {
         OnPropertyChanged();
      }

      protected virtual void OnPropertyChanged()
      {
         PropertyChanged?.Invoke(this, EventArgs.Empty);
      }
   }
}
