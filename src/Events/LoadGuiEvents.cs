using System.Reflection;
using Editor.Controls.NewControls;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;

namespace Editor.Events
{
   public static class LoadGuiEvents
   {
      public static Action<List<Province>, PropertyInfo, bool> ProvLoadAction = delegate { };

      public static void ReloadProvinces()
      {
         ProvLoadAction.Invoke(Selection.GetSelectedProvinces, null!, true);
      }

      public static void TriggerGuiUpdate(Type type, PropertyInfo info)
      {
         if (type == typeof(Province))
            ProvLoadAction.Invoke(Selection.GetSelectedProvinces, info, false);

      }

   }

   public static class Theory
   {
      public static void TestTheory()
      {
         var propCheckBox = new PropertyCheckBox<Province>(typeof(Province).GetProperty(nameof(Province.IsHre))!, ref LoadGuiEvents.ProvLoadAction, Selection.GetSelectedProvinces);
      }
   }
}