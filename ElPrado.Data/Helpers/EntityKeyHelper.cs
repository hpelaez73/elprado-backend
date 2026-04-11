using System.Reflection;

namespace ElPrado.Data.Helpers
{
    public static class EntityKeyHelper
    {
        public static PropertyInfo? GetKeyProperty(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            // Busca propiedades públicas (incluye heredadas)
            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                  .Where(p => !Attribute.IsDefined(p, typeof(NotMappedAttribute)));

            // 1) Propiedad marcada con [Key]
            var keyProp = props.FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));
            if (keyProp != null) return keyProp;

            // 2) Convenciones comunes: "Cod" o "{Tipo}Id"
            var byName = props.FirstOrDefault(p => string.Equals(p.Name, "Cod", StringComparison.OrdinalIgnoreCase))
                         ?? props.FirstOrDefault(p => p.Name.StartsWith("Cod"))
                         ?? props.FirstOrDefault(p => string.Equals(p.Name, entityType.Name + "Cod", StringComparison.OrdinalIgnoreCase));
            return byName;
        }

        public static PropertyInfo? GetKeyProperty<T>() => GetKeyProperty(typeof(T));

        public static object? GetKeyValue(object entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var prop = GetKeyProperty(entity.GetType());
            return prop?.GetValue(entity);
        }
    }
}