using System.Diagnostics;
using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.MapModes;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Events
{
   public static class LoadGuiEvents
   {
      public delegate void LoadAction<T>(List<T> objects, PropertyInfo propertyInfo, bool force) where T : Saveable;

      public static LoadAction<Province> ProvLoadAction = delegate { };
      public static LoadAction<HistoryCountry> HistoryCountryLoadAction = delegate { };
      public static LoadAction<CommonCountry> CommonCountryLoadAction = delegate { };

      public static void ReloadProvinces()
      {
         var selectedProvinces = Selection.GetSelectedProvinces;
         if (selectedProvinces.Count < 1)
            return;
         Globals.State = State.Loading;
         ProvLoadAction.Invoke(selectedProvinces, null!, true);
         Globals.State = State.Running;
      }

      public static void TriggerGuiUpdate(Type type, PropertyInfo info)
      {
         if (type == typeof(Province))
         {
            ProvLoadAction.Invoke(Selection.GetSelectedProvinces, info, false);
         }
         else if (type == typeof(HistoryCountry))
            HistoryCountryLoadAction.Invoke([Selection.SelectedCountry.HistoryCountry], info, false);
         else if (type == typeof(CommonCountry))
            CommonCountryLoadAction.Invoke([Selection.SelectedCountry.CommonCountry], info, false);



         MapModeManager.RenderMapMode(info);
      }

   }

   public static class Theory
   {
      public static void TestTheory()
      {
         
      }

      public static void OneGBRamUsage()
      {
         var list = new List<int>(1_000_000_000 / 4);
         for (var i = 0; i < 1_000_000_000 / 4; i++)
            list.Add(i);
      }
   }
}