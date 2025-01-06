using System.Reflection;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.Events
{
   public static class LoadGuiEvents
   {
      public delegate void LoadAction<T>(List<T> objects, PropertyInfo propertyInfo, bool force) where T : Saveable;

      public static LoadAction<Province> ProvLoadAction = delegate { };
      public static LoadAction<Country> CountryLoadAction = delegate { };

      public static void ReloadProvinces()
      {
         Globals.State = State.Loading;
         ProvLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);
         Globals.State = State.Running;
      }

      public static void TriggerGuiUpdate(Type type, PropertyInfo info)
      {
         if (type == typeof(Province))
            ProvLoadAction.Invoke(Selection.GetSelectedProvinces, info, false);
         else if (type == typeof(Country))
            CountryLoadAction.Invoke([Selection.SelectedCountry], info, false);
      }

   }

   public static class Theory
   {
      public static void TestTheory()
      {
         
      }
   }
}