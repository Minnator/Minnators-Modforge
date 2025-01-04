﻿using System.Diagnostics;
using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Forms.Feature.AdvancedSelections;

public class Deselection : ISelectionModifier
{
   public string Name { get; set; } = "Deselection";

   public void Execute(ProvinceSource source, Operations operation, string attr, object value)
   {
      HistoryManager.AddCommand(new CCollectionSelection(GetProvinceViaOperation.GetProvinces(operation, attr, value, GetProvinceViaOperation.GetProvincesToCheck(source)), true));
   }


}

public class Select : ISelectionModifier
{
   public string Name { get; set; } = "Selection";

   public void Execute(ProvinceSource source, Operations operation, string attr, object value)
   {

      HistoryManager.AddCommand(new CCollectionSelection(GetProvinceViaOperation.GetProvinces(operation, attr, value, GetProvinceViaOperation.GetProvincesToCheck(source))));
   }
}

public static class GetProvinceViaOperation
{
   private static bool GetProvinceCollectionsFromSourceInSelection(ProvinceSource source, out List<ProvinceComposite> collections)
   {
      collections = [];
      SaveableType type;

      switch (source)
      {
         case ProvinceSource.AreasFromSelection:
            type = SaveableType.Area;
            break;
         case ProvinceSource.RegionsFromSelection:
            type = SaveableType.Region;
            break;
         case ProvinceSource.SuperRegionsFromSelection:
            type = SaveableType.SuperRegion;
            break;
         case ProvinceSource.ContinentsFromSelection:
            type = SaveableType.Continent;
            break;
         case ProvinceSource.TradeNodesFromSelection:
            type = SaveableType.TradeNode;
            break;
         case ProvinceSource.TradeCompaniesFromSelection:
            type = SaveableType.TradeCompany;
            break;
         case ProvinceSource.ColonialRegionsFromSelection:
            type = SaveableType.ColonialRegion;
            break;
         case ProvinceSource.Selection:
         case ProvinceSource.AllProvinces:
         case ProvinceSource.LandProvinces:
         case ProvinceSource.SeaProvinces:
         default:
            return false;
      }

      HashSet<ProvinceComposite> parents = [];
      foreach (var i in Selection.GetSelectedProvinces)
      {
         var parent = i.GetFirstParentOfType(type);
         if (parent != ProvinceComposite.Empty)
            parents.Add(parent);
      }
      
      collections = parents.ToList();

      return collections.Count > 0;
   }

   private static List<Province> GetProvincesFromCollections(List<ProvinceComposite> collections)
   {
      List<Province> provinces = [];
      foreach (var collection in collections) 
         provinces.AddRange(collection.GetProvinces());
      return provinces;
   }

   public static ICollection<Province> GetProvincesToCheck(ProvinceSource source)
   {
      switch (source)
      {
         case ProvinceSource.Selection:
            return Selection.GetSelectedProvinces;
         case ProvinceSource.AreasFromSelection:
         case ProvinceSource.RegionsFromSelection:
         case ProvinceSource.SuperRegionsFromSelection:
         case ProvinceSource.ContinentsFromSelection:
         case ProvinceSource.TradeNodesFromSelection:
         case ProvinceSource.TradeCompaniesFromSelection:
         case ProvinceSource.ColonialRegionsFromSelection:
            if (GetProvinceCollectionsFromSourceInSelection(source, out var provinces))
               return GetProvincesFromCollections(provinces);
            return Selection.GetSelectedProvinces;
         case ProvinceSource.AllProvinces:
            return Globals.Provinces;
         case ProvinceSource.LandProvinces:
            return Globals.LandProvinces;
         case ProvinceSource.SeaProvinces:
            return Globals.SeaProvinces;
         default:
            return [];
      }
   }

   public static List<Province> GetProvinces(Operations operation, string propName, object value, ICollection<Province> provincesToCheck)
   {
      var provInfo = provincesToCheck.First().GetPropertyInfo(propName);
      Debug.Assert(provInfo != null, "provInfo != null");
      
      if (!int.TryParse(value.ToString(), out var intValue) && provInfo.PropertyType != typeof(int))
         return [];
      if (!float.TryParse(value.ToString(), out var floatValue) && provInfo.PropertyType != typeof(float))
         return [];
      List<Province> provinces = [];
      foreach (var prov in provincesToCheck)
      {
         switch (operation)
         {
            case Operations.Equal:
               if (provInfo.GetValue(prov)!.Equals(value))
                  provinces.Add(prov);
               break;
            case Operations.NotEqual:
               if (!provInfo.GetValue(prov)!.Equals(value))
                  provinces.Add(prov);
               break;
            case Operations.LessThan:
               if (provInfo.PropertyType == typeof(int))
               {
                  if (int.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo))
                     if (compareTo < intValue)
                        provinces.Add(prov);
               }
               else if (provInfo.PropertyType == typeof(float))
               {
                  if (float.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo1))
                     if (compareTo1 < floatValue)
                        provinces.Add(prov);
               }
               break;
            case Operations.GreaterThan:
               if (provInfo.PropertyType == typeof(int))
               {
                  if (int.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo2))
                     if (compareTo2 > intValue)
                        provinces.Add(prov);
               }
               else if (provInfo.PropertyType == typeof(float))
               {
                  if (float.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo3))
                     if (compareTo3 > floatValue)
                        provinces.Add(prov);
               }
               break;
            case Operations.GreaterThanOrEqual:
               if (provInfo.PropertyType == typeof(int))
               {
                  if (int.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo4))
                     if (compareTo4 >= intValue)
                        provinces.Add(prov);
               }
               else if (provInfo.PropertyType == typeof(float))
               {
                  if (float.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo5))
                     if (compareTo5 >= floatValue)
                        provinces.Add(prov);
               }
               break;
            case Operations.LessThanOrEqual:
               if (provInfo.PropertyType == typeof(int))
               {
                  if (int.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo6))
                     if (compareTo6 <= intValue)
                        provinces.Add(prov);
               }
               else if (provInfo.PropertyType == typeof(float))
               {
                  if (float.TryParse(provInfo.GetValue(prov)!.ToString(), out var compareTo7))
                     if (compareTo7 <= floatValue)
                        provinces.Add(prov);
               }
               break;
         }
      }
      return provinces;
   }
}