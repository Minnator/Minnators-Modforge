using Editor.DataClasses.GameDataClasses;

namespace Editor.Events
{
   public class CountryEditingEventArgs(CountrySetter setter, object value);

   public static class CountryEditingEvents
   {
      public static readonly EventHandler<CountryEditingEventArgs> CountryColorChanged = delegate { };


   }
}