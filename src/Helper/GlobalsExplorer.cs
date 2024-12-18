using System.ComponentModel;
using System.Reflection;

namespace Editor.Helper
{
   public class GlobalsDynamicWrapper : ICustomTypeDescriptor
   {
      private readonly Type globalsType = typeof(Globals);
      
      public PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
      {
         var properties = new PropertyDescriptorCollection(null);

         foreach (var field in globalsType.GetFields(BindingFlags.Static | BindingFlags.Public))
         {
            var propertyDescriptor = new StaticPropertyDescriptor(field);
            properties.Add(propertyDescriptor);
         }

         return properties;
      }

      public object? GetPropertyOwner(PropertyDescriptor? pd) => globalsType;
      public AttributeCollection GetAttributes() => new();
      public string GetClassName() => globalsType.Name;
      public string? GetComponentName() => null;
      public TypeConverter? GetConverter() => null;
      public EventDescriptor? GetDefaultEvent() => null;
      public PropertyDescriptor? GetDefaultProperty() => null;
      public object? GetEditor(Type editorBaseType) => null;
      public EventDescriptorCollection GetEvents() => throw new NotImplementedException();
      public EventDescriptorCollection GetEvents(Attribute[]? attributes) => null;
      public PropertyDescriptorCollection GetProperties() => GetProperties(null);
   }

   public class StaticPropertyDescriptor(FieldInfo field) : PropertyDescriptor(field.Name, null)
   {
      public override bool CanResetValue(object component) => true;
      public override object? GetValue(object? component) => field.GetValue(null); 
      public override void SetValue(object? onent, object? value) => field.SetValue(null, value); 
      public override bool IsReadOnly => false;
      public override Type PropertyType => field.FieldType;
      public override Type ComponentType => typeof(Globals);
      public override void ResetValue(object component) => field.SetValue(null, Activator.CreateInstance(field.FieldType));
      public override bool ShouldSerializeValue(object component) => true;
   }
}