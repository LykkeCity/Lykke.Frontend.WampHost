using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public class TypeDocGenerator
    {
        public MethodDocInfo[] GetDocumentation(Type type)
        {
            var result = new List<MethodDocInfo>();
            MethodInfo[] methods = GetAvailableMethods(type);

            foreach (var method in methods)
            {
                var attr = (DocMeAttribute)method.GetCustomAttribute(typeof(DocMeAttribute));
                var returnType = method.ReturnType.IsConstructedGenericType ? method.ReturnType.GenericTypeArguments[0] : method.ReturnType;

                var docInfo = new MethodDocInfo
                {
                    Id = $"{type.FullName.Replace('.', '_')}_{method.Name}_Id",
                    Name = attr.Name,
                    Output = returnType.GetTypeName(),
                    Description = attr.Description,
                    OutputTypes = new []{returnType}
                };

                result.Add(docInfo);
            }

            return result.ToArray();
        }

        private MethodInfo[] GetAvailableMethods(Type type)
        {
            return type.GetMethods().Where(item => item.CustomAttributes.Any(a => a.AttributeType == typeof(DocMeAttribute))).ToArray();
        }

        private List<Type> GetTypes(Type type)
        {
            if (type == null)
                return new List<Type>();

            var elementType = type.GetElementType() ?? type;
            PropertyInfo[] properties = elementType.GetProperties();

            if (properties.Length == 0)
                return new List<Type> { type };

            var types = new List<Type> { elementType };

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.IsUserDefinedClass())
                {
                    types.AddRange(GetTypes(property.PropertyType));
                }

                if (property.PropertyType.IsDictionary())
                {
                    foreach (var dictType in property.PropertyType.GenericTypeArguments)
                    {
                        if (dictType.IsUserDefinedClass())
                        {
                            types.AddRange(GetTypes(dictType));
                        }
                    }
                }
            }

            return types.Distinct().Where(t => !t.IsEnum).ToList();
        }
    }
}
