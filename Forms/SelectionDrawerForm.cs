using System.Drawing.Imaging;
using Editor.Helper;

namespace Editor.Forms
{
   public partial class SelectionDrawerForm : Form
   {
      private SelectionExportSettings ExportSettings { get; set; } = new();

      public SelectionDrawerForm()
      {
         InitializeComponent();

         ExportSettingsPropertyGrid.PropertyValueChanged += ExportSettingsPropertyChanged;
         ExportSettingsPropertyGrid.SelectedObject = ExportSettings;
      }

      private void ExportSettingsPropertyChanged(object? s, PropertyValueChangedEventArgs e)
      {
         ExportSettings = (SelectionExportSettings)ExportSettingsPropertyGrid.SelectedObject;
         RenderImage();
      }

      private void SelectFolderButton(object sender, EventArgs e)
      {
         IO.OpenFileDialog(Globals.MapWindow.Project.ModPath, "select a folder where to save the image", out var path);
         PathTextBox.Text = path;
      }

      private void FocusOn(Point point)
      {
         PreviewMapPanel.AutoScrollPosition = new(point.X - PreviewMapPanel.Width / 2, point.Y - PreviewMapPanel.Height / 2);
      }

      private void RenderImage()
      {
         var bitmap = new Bitmap(Globals.MapWidth, Globals.MapHeight, PixelFormat.Format24bppRgb);

         using (var g = Graphics.FromImage(bitmap))
         {
            g.Clear(ExportSettings.BackColor);
         }

         switch (ExportSettings.PrimaryProvinceDrawing)
         {
            case PrimaryProvinceDrawing.None:
               break;
            case PrimaryProvinceDrawing.Selection:
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, Globals.Selection.GetSelectedProvincesIds);
               break;
            case PrimaryProvinceDrawing.Land:
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, Globals.LandProvinceIds);
               break;
            case PrimaryProvinceDrawing.All:
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, Globals.Provinces.Keys.ToArray());
               break;
         }

         DrawSecondary(bitmap, ExportSettings.SecondaryProvinceDrawing);

         DrawSecondary(bitmap, ExportSettings.TertiaryProvinceDrawing);

         switch (ExportSettings.BorderDrawing)
         {
            case BorderDrawing.None:
               break;
            case BorderDrawing.Selection:
               foreach (var prov in Globals.Selection.GetSelectedProvinces)
                  MapDrawHelper.DrawProvinceBorder(prov.Id, Color.Black, bitmap);
               break;
            case BorderDrawing.All:
               MapDrawHelper.DrawAllProvinceBorders(bitmap, Color.Black);
               break;
         }

         PreviewPictureBox.Image?.Dispose();
         PreviewPictureBox.Size = bitmap.Size;
         PreviewPictureBox.Image = bitmap;

         if (Globals.Selection.SelectedProvinces.Count == 0)
            return;

         var rect = Geometry.GetBounds(Globals.Selection.SelectedProvinces);
         var centerProvince = Geometry.GetProvinceClosestToPoint(new(rect.X + rect.Width / 2,rect.Y + rect.Height / 2), Globals.Selection.GetSelectedProvinces);
         FocusOn(centerProvince.Center);
      }

      private void DrawSecondary(Bitmap bitmap, SecondaryProvinceDrawing secondary)
      {
         switch (secondary)
         {
            case SecondaryProvinceDrawing.None:
               break;
            case SecondaryProvinceDrawing.NeighboringProvinces:
               var neighboringProvinces = Geometry.GetAllNeighboringProvinces(Globals.Selection.GetSelectedProvinces);
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, [.. neighboringProvinces]);
               break;
            case SecondaryProvinceDrawing.NeighboringCountries:
               var neighboringCountries = Geometry.GetAllNeighboringCountries(Globals.Selection.GetSelectedProvinces);
               List<int> allCountryProvinces = [];
               foreach (var country in neighboringCountries)
               {
                  var provinces = Globals.Countries[country].GetProvinces().ToList();
                  foreach (var province in provinces)
                     allCountryProvinces.Add(province.Id);
               }
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, [.. allCountryProvinces]);
               break;
            case SecondaryProvinceDrawing.CoastalOutline:
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, Globals.SeaProvinces.ToArray());
               break;
            case SecondaryProvinceDrawing.SeaProvinces:
               BitMapHelper.ModifyByProvinceCollection(bitmap, Globals.NonLandProvinceIds, i => Globals.Provinces[i].Color);
               break;
            case SecondaryProvinceDrawing.All:
               Globals.MapModeManager.DrawProvinceCollectionFromCurrentMapMode(bitmap, Globals.Provinces.Keys.ToArray());
               break;
         }
      }

      private void SaveButton_Click(object sender, EventArgs e)
      {
         switch (ExportSettings.ImageSize)
         {
            case ImageSize.Original:
               PreviewPictureBox.Image.Save(Path.Combine(PathTextBox.Text, $"{Globals.MapModeManager.CurrentMapMode.GetMapModeName()}.png"), ImageFormat.Png);
               break;
            case ImageSize.Selection:
               var rect = Geometry.GetBounds(Globals.Selection.SelectedProvinces);
               var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
               // copy the selected area to the new bitmap
               using (var g = Graphics.FromImage(bitmap))
               {
                  g.DrawImage(PreviewPictureBox.Image, rect with { X = 0, Y = 0 }, rect, GraphicsUnit.Pixel);
               }
               bitmap.Save(Path.Combine(PathTextBox.Text, $"{Globals.MapModeManager.CurrentMapMode.GetMapModeName()}.png"), ImageFormat.Png);
               bitmap?.Dispose();
               break;
         }
      }
   }

   public enum ImageSize
   {
      Original,
      Selection,

   }

   public enum BorderDrawing
   {
      None,
      Selection,
      All
   }

   public enum PrimaryProvinceDrawing
   {
      None,
      Selection,
      Land,
      All
   }

   public enum SecondaryProvinceDrawing
   {
      None,
      NeighboringProvinces,
      NeighboringCountries,
      SeaProvinces,
      CoastalOutline,
      All
   }


   public class SelectionExportSettings
   {
      public BorderDrawing BorderDrawing { get; set; }
      public PrimaryProvinceDrawing PrimaryProvinceDrawing { get; set; }
      public SecondaryProvinceDrawing SecondaryProvinceDrawing { get; set; }
      public SecondaryProvinceDrawing TertiaryProvinceDrawing { get; set; }
      public ImageSize ImageSize { get; set; }
      public Color BackColor { get; set; } = Color.Black;
   }
}
