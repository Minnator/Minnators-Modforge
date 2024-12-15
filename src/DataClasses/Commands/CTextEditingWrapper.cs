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

            if (Localisation.GetLocObject(isTitle ? titleAdjProvider.GetTitleKey() : titleAdjProvider.GetAdjectiveKey(), out var locObject))
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

   public class CProvinceAttributeFactory(ProvinceEnumHelper.ProvAttrGet pa, ProvinceEnumHelper.ProvAttrSet ps) : CTextEditingFactory<CProvinceAttributeChange, string>
   {
      public override CProvinceAttributeChange Create(ICollection<Saveable> saveables, string newValue)
      {
         return new(saveables.Cast<Province>().ToList(), newValue, pa ,ps, false);
      }
   }

   
   public class CCountryPropertyChangeFactory<T> : CTextEditingFactory<CModifyProperty<T>, T>
   {
      public readonly string PropName;

      public CCountryPropertyChangeFactory(string propName)
      {
         PropName = propName;
      }

      public override CModifyProperty<T> Create(ICollection<Saveable> saveables, T newValue)
      {
         var country = saveables.First();
         return new(PropName, country, newValue, (T)country.GetType().GetProperty(PropName)!.GetValue(country)!);
      }
   }
}