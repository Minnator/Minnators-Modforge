using Editor.DataClasses.GameDataClasses;
using Editor.DataClasses.Saveables;

namespace Editor.Events
{
   public class ProvinceCollectionEvents
   {
      public static EventHandler<ICollection<ProvinceComposite>> ProvinceCollectionEvent = delegate { };
   }
}