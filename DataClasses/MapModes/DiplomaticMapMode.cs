﻿using System.Diagnostics;
using Editor.DataClasses.GameDataClasses;
using Editor.Events;
using Editor.Helper;
using static Editor.Helper.ProvinceEnumHelper;

namespace Editor.DataClasses.MapModes
{
   public class DiplomaticMapMode : MapMode
   {
      public override bool IsLandOnly => true;
      public override bool ShowCapitals => true;
      public bool ClearPreviousCoresClaims = false;
      private List<Province> CoresAndClaims = [];


      public DiplomaticMapMode()
      {
         ProvinceEventHandler.OnProvinceClaimsChanged += UpdateProvince!;
         ProvinceEventHandler.OnProvinceCoresChanged += UpdateProvince!;
         //TODO add permanent claims
      }

      public override MapModeType GetMapModeName()
      {
         return MapModeType.Diplomatic;
      }

      public override string GetSpecificToolTip(Province province)
      {
         var tooltip = string.Empty;

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

      public override int GetProvinceColor(Province id)
      {
         if (Globals.Provinces.TryGetValue(id, out var province))
            if (province.Owner != Tag.Empty)
               if (Globals.Countries.TryGetValue(province.Owner, out var country))
                  return country.Color.ToArgb();
         return Color.DimGray.ToArgb();
      }

      public override void RenderMapMode(Func<Province, int> method)
      {
         base.RenderMapMode(method);
         if (ClearPreviousCoresClaims)
            return;
         RenderClaimsAndCores();
      }

      public override void Update(Province id, bool invalidate = true)
      {
         base.Update(id, invalidate);
         if (!ClearPreviousCoresClaims) 
            RenderClaimsAndCores();
      }

      public override void Update(ICollection<Province> ids)
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
         if (Selection.SelectedCountry.Equals(Country.Empty))
            return;

         var claims = ProvinceCollectionHelper.GetProvincesWithAttribute(ProvAttrGet.claims, Selection.SelectedCountry.Tag);
         claims.AddRange(ProvinceCollectionHelper.GetProvincesWithAttribute(ProvAttrGet.permanent_claims, Selection.SelectedCountry.Tag));
         var cores = ProvinceCollectionHelper.GetProvincesWithAttribute(ProvAttrGet.cores, Selection.SelectedCountry.Tag);

         CoresAndClaims.AddRange(claims);
         CoresAndClaims.AddRange(cores);

         for (var i = claims.Count - 1; i >= 0; i--)
         {
            if (claims[i].Owner == Selection.SelectedCountry.Tag)
               claims.RemoveAt(i);
         }

         for (var i = cores.Count - 1; i >= 0; i--)
         {
            if (cores[i].Owner == Selection.SelectedCountry.Tag)
               cores.RemoveAt(i);
         }

         Selection.SelectionCoresAndClaims.Clear();
         Selection.SelectionCoresAndClaims.AddRange(claims);
         Selection.SelectionCoresAndClaims.AddRange(cores);

         MapDrawing.DrawStripes(Color.DarkGoldenrod.ToArgb(), claims, Globals.ZoomControl);
         MapDrawing.DrawStripes(Color.LawnGreen.ToArgb(), cores, Globals.ZoomControl);

         Globals.ZoomControl.Invalidate();

      }
   }
}