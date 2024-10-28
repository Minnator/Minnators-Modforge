using Editor.DataClasses.Commands;
using Editor.DataClasses.GameDataClasses;

namespace Editor.Events
{
   public class ProvinceCollectionEventArgs : EventArgs
   {
      public string GroupKey { get; }
      public List<Province> Ids { get; }

      public ProvinceCollectionEventArgs(string groupKey, List<Province> ids)
      {
         GroupKey = groupKey;
         Ids = ids;
      }

   }


}