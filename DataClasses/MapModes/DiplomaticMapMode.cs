using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.DataClasses.MapModes
{
   public class DiplomaticMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowCapitals => true;
      public bool ClearPreviousCoresClaims = false;
      private List<int> CoresAndClaims = [];


      public DiplomaticMapMode()
      {
         ProvinceEventHandler.OnProvinceClaimsChanged += UpdateProvince!;
         ProvinceEventHandler.OnProvinceCoresChanged += UpdateProvince!;
         //TODO add permanent claims
      }

      public override string GetMapModeName()
      {
         return "Diplomatic";
      }

      public override string GetSpecificToolTip(int provinceId)
      {
         var tooltip = string.Empty;

         var province = Globals.Provinces[provinceId];
         if (province.Owner != Tag.Empty)
         {
            if (!Globals.Countries.TryGetValue(province.Owner, out var country))
               return "No Country to show diplomacy for";
            tooltip = $"Diplomacy for {province.Owner} ({country.GetLocalisation()})";
            if (province.Claims.Contains(country.Tag))
               tooltip += $"\nClaims of {country.Tag}";
            if (province.Cores.Contains(country.Tag))
               tooltip += $"\nCores of {country.Tag}";
         }

         return tooltip;
      }

      public override Color GetProvinceColor(int id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            if (province.Owner != Tag.Empty)
               if (Globals.Countries.TryGetValue(province.Owner, out var country))
                  return country.Color;
         return Color.DimGray;
      }

      public override void RenderMapMode(Func<int, Color> method)
      {
         base.RenderMapMode(method);
         if (ClearPreviousCoresClaims)
            return;
         RenderClaimsAndCores();
      }

      public override void Update(int id, bool invalidate = true)
      {
         base.Update(id, invalidate);
         if (!ClearPreviousCoresClaims) 
            RenderClaimsAndCores();
      }

      public override void Update(List<int> ids)
      {
         var sw = Stopwatch.StartNew();
         base.Update(ids);
         sw.Stop();
         Debug.WriteLine($"Update {GetMapModeName()} took {sw.ElapsedMilliseconds}ms");
         if (!ClearPreviousCoresClaims) 
            RenderClaimsAndCores();
      }
      
      private void RenderClaimsAndCores()
      {
         //Draw claims and cores of selected country if any
         if (Globals.Selection.SelectedCountry.Equals(Country.Empty))
            return;

         var claims = ProvinceCollectionHelper.GetProvincesWithAttribute(ProvinceAttribute.claims, Globals.Selection.SelectedCountry.Tag);
         claims.AddRange(ProvinceCollectionHelper.GetProvincesWithAttribute(ProvinceAttribute.permanent_claims, Globals.Selection.SelectedCountry.Tag));
         var cores = ProvinceCollectionHelper.GetProvincesWithAttribute(ProvinceAttribute.cores, Globals.Selection.SelectedCountry.Tag);

         CoresAndClaims.AddRange(claims);
         CoresAndClaims.AddRange(cores);

         for (var i = claims.Count - 1; i >= 0; i--)
         {
            var province = Globals.Provinces[claims[i]];
            if (province.Owner == Globals.Selection.SelectedCountry.Tag)
               claims.RemoveAt(i);
         }

         for (var i = cores.Count - 1; i >= 0; i--)
         {
            var province = Globals.Provinces[cores[i]];
            if (province.Owner == Globals.Selection.SelectedCountry.Tag)
               cores.RemoveAt(i);
         }

         Globals.Selection.SelectionCoresAndClaims.Clear();
         Globals.Selection.SelectionCoresAndClaims.AddRange(claims);
         Globals.Selection.SelectionCoresAndClaims.AddRange(cores);

         switch (Globals.MapModeRendering)
         {
            case MapModeRendering.Live:
            case MapModeRendering.LiveBackground:
               var rect1 = MapDrawHelper.DrawStripes(Color.DarkGoldenrod, claims, Globals.MapModeManager.ShareLiveBitmap);
               var rect2 = MapDrawHelper.DrawStripes(Color.LawnGreen, cores, Globals.MapModeManager.ShareLiveBitmap);
               Globals.MapWindow.MapPictureBox.Invalidate(rect1);
               Globals.MapWindow.MapPictureBox.Invalidate(rect2);
               break;
            case MapModeRendering.Cached:
               var rect3 = MapDrawHelper.DrawStripes(Color.DarkGoldenrod, claims, Bitmap);
               var rect4 = MapDrawHelper.DrawStripes(Color.LawnGreen, cores, Bitmap);
               Globals.MapModeManager.PictureBox.Invalidate(rect3);
               Globals.MapModeManager.PictureBox.Invalidate(rect4);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }

      }
   }
}