using System.Data;
using Editor.DataClasses.GameDataClasses;
using Editor.Helper;
using Editor.Saving;

namespace Editor.DataClasses.Commands
{
   public abstract class CTextEditingWrapper : SaveableCommandBasic
   {
      public abstract void SetValue(string value);
   }

   public abstract class CTextEditingFactory<T>
   {
      public abstract T Create(ICollection<Saveable> saveables);
   }

   public class CLocObjFactory(bool isTitle) : CTextEditingFactory<CModifyLocalisation>
   {
      public override CModifyLocalisation Create(ICollection<Saveable> saveables)
      {
         List<LocObject> locs = [];

         foreach (var sav in saveables)
         {
            if (sav is not ITitleAdjProvider titleAdjProvider)
               throw new EvilActions("Illegal objects type in CLocObjFactory for command creation!");

            if (Localisation.GetLocObject(isTitle ? titleAdjProvider.GetTitleKey() : titleAdjProvider.GetAdjectiveKey(), out var locObject))
               locs.Add(locObject!);
            else
            {
               MessageBox.Show("Error: LocObject not found");
               return null!;
            }
         }
         return new(locs, "dummyLoc", false);
      }
   }

   public class CProvinceAttributeFactory(ProvinceEnumHelper.ProvAttrGet pa, ProvinceEnumHelper.ProvAttrSet ps) : CTextEditingFactory<CProvinceAttributeChange>
   {
      public override CProvinceAttributeChange Create(ICollection<Saveable> saveables)
      {

         return new(saveables.Cast<Province>().ToList(), "dummyAttribute" ,pa ,ps, false);
      }
   }

}