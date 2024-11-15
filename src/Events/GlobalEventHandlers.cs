namespace Editor.Events;

public static class GlobalEventHandlers
{
   public class ProvinceCollectionModified(string name, bool add) : EventArgs;

   public static event EventHandler<ProvinceCollectionModified> OnCountryListChanged = delegate { };
   public static void RaiseCountryListChanged()
   {
      OnCountryListChanged.Invoke(null, new (string.Empty, false));
   }
}
