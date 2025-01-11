using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public abstract class CTextEditingFactory<C, T> where C : ICommand
   {
      public abstract C Create(ICollection<Saveable> saveables, T newValue);
   }

   public class CLocObjFactory(bool isTitle) : CTextEditingFactory<CModifyLocalisation, string>
   {
      public override CModifyLocalisation Create(ICollection<Saveable> saveables, string newValue)
      {
         List<LocObject> locs = [];

         foreach (var sav in saveables)
         {
            if (sav is not ITitleAdjProvider titleAdjProvider)
               throw new EvilActions("Illegal objects type in CLocObjFactory for command creation!");

            if (Localisation.GetLocObject(isTitle ? titleAdjProvider.TitleKey : titleAdjProvider.AdjectiveKey, out var locObject))
               locs.Add(locObject!);
            else
            {
               MessageBox.Show("Error: LocObject not found");
               return null!;
            }
         }
         return new(locs, newValue, false);
      }
   }
}