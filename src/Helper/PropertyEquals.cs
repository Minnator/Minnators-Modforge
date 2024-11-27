using Editor.DataClasses.Settings;


namespace Editor.Helper
{
   public abstract class PropertyEquals
   {
      public new virtual bool Equals(object? obj)
      {
         if (obj == null || obj.GetType() != GetType())
            return false;

         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));

         foreach (var property in properties)
            if (!Equals(property.GetValue(this), property.GetValue(Convert.ChangeType(obj, GetType()))))
               return false;

         return true;
      }

      public new virtual int GetHashCode()
      {
         var properties = GetType().GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(CompareInEquals)));
         var hash = 17;

         foreach (var property in properties)
            hash = unchecked(hash * 31 + (property.GetValue(this)?.GetHashCode() ?? 0));

         return hash;
      }
   }

   public interface IGetSetProperty
   {
      public object? GetProperty(string propertyName);
      public void SetProperty(string propertyName, object value);
   }
}