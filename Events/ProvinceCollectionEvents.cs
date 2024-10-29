using Editor.DataClasses.GameDataClasses;

namespace Editor.Events
{
   public class ProvinceCollectionEvents
   {
      public static EventHandler<ICollection<ProvinceComposite>> ProvinceCollectionEvent = delegate { };
   }
}