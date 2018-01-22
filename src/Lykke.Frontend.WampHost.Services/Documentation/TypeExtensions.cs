using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" },
            { typeof(byte?), "byte?" },
            { typeof(sbyte?), "sbyte?" },
            { typeof(short?), "short?" },
            { typeof(ushort?), "ushort?" },
            { typeof(int?), "int?" },
            { typeof(uint?), "uint?" },
            { typeof(long?), "long?" },
            { typeof(ulong?), "ulong?" },
            { typeof(float?), "float?" },
            { typeof(double?), "double?" },
            { typeof(decimal?), "decimal?" },
            { typeof(bool?), "bool?" },
            { typeof(char?), "char?" },
            { typeof(DateTime?), "DateTime?" },
            { typeof(string[]), "string[]" }
        };

        public static string GetTypeDefinition(this Type type)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"// {type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name}");
            sb.AppendLine("{");

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var propertiesAdded = 0;

            foreach (var property in properties)
            {
                if (propertiesAdded > 0)
                {
                    sb.AppendLine();
                }

                string propertyTypeDefinition;

                if (property.PropertyType.IsDictionary())
                    propertyTypeDefinition = GetDictionaryPropertyTypeDefinition(property);
                else if (property.PropertyType.IsList())
                    propertyTypeDefinition = GetListPropertyTypeDefinition(property);
                 else
                    propertyTypeDefinition = GetPropertyTypeDefinition(property);

                sb.AppendLine($"  // {property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name}");
                sb.AppendLine($"  // Type: {propertyTypeDefinition}");

                if (property.PropertyType.IsEnum)
                {
                    GetEnumDefinition(sb, property.PropertyType);
                }

                sb.Append($"  \"{property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name}\": ");

                GetPropertyValueDefinition(sb, property);

                propertiesAdded++;
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void GetEnumDefinition(StringBuilder sb, Type enumType)
        {
            sb.AppendLine("  // Values:");

            foreach (var name in Enum.GetNames(enumType))
            {
                sb.AppendLine($"  // - {name}: {enumType.GetField(name).GetCustomAttribute<DisplayNameAttribute>()?.DisplayName}");
            }
        }

        private static void GetPropertyValueDefinition(StringBuilder sb, PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                sb.AppendLine("\"\",");
            }
            else if (property.PropertyType.IsDictionary())
            {
                sb.AppendLine("{},");
            }
            else if (property.PropertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                sb.AppendLine("[],");
            }
            else if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(DateTime))
            {
                var serialized = JsonConvert.SerializeObject(Activator.CreateInstance(property.PropertyType));

                sb.AppendLine($"{serialized},");
            }
            else if (property.PropertyType.IsEnum)
            {
                sb.AppendLine("\"\",");
            }
            else if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                var serialized = JsonConvert.SerializeObject(Activator.CreateInstance(Nullable.GetUnderlyingType(property.PropertyType)));

                sb.AppendLine($"null | {serialized},");
            }
            else
            {
                sb.AppendLine("Doc not implemented,");
            }
        }

        public static string GetPropertyTypeAlias(this Type type)
        {
            return TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.GetTypeName();
        }

        public static string GetTypeName(this Type type)
        {
            return !type.GetTypeInfo().IsGenericType
                ? type.Name
                : type.Name.Split('`')[0];
        }

        public static bool IsUserDefinedClass(this Type type)
        {
            return !type.Namespace.StartsWith("System");
        }

        public static bool IsDictionary(this Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }
        
        public static bool IsList(this Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        private static string GetPropertyTypeDefinition(PropertyInfo property)
        {
            return $"{GetPropertyTypeAlias(property.PropertyType)}";
        }

        private static string GetDictionaryPropertyTypeDefinition(PropertyInfo property)
        {
            return $"Dictionary<{GetPropertyTypeAlias(property.PropertyType.GenericTypeArguments[0])}, {GetPropertyTypeAlias(property.PropertyType.GenericTypeArguments[1])}>";
        }
        
        private static string GetListPropertyTypeDefinition(PropertyInfo property)
        {
            return $"{GetPropertyTypeAlias(property.PropertyType.GenericTypeArguments[0])}[]";
        }
    }
}
