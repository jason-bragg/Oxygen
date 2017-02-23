
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Orleans.Factory.Abstractions;

namespace Microsoft.Orleans.Facet.Abstractions
{
    public class TypeFacetInfo
    {
        private readonly List<Action<object, IServiceProvider>> setters;

        public TypeFacetInfo(Type type)
        {
            this.setters = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fi => typeof(IGrainFacet).IsAssignableFrom(fi.FieldType))
                .Select(fi => MakeFieldSetter(fi))
                .ToList();
        }

        public void SetFields(object obj, IServiceProvider serviceProvider)
        {
            this.setters.ForEach(set => set(obj, serviceProvider));
        }

        private Action<object, IServiceProvider> MakeFieldSetter(FieldInfo fieldInfo)
        {
            FacetAttribute attribute = fieldInfo.GetCustomAttribute<FacetAttribute>();
            Type factoryType = attribute == null
                ? FactoryType.CreateFactoryInterfaceType(fieldInfo.FieldType)
                : FactoryType.CreateFactoryInterfaceType(typeof(string), fieldInfo.FieldType);
            MethodInfo createFn = factoryType.GetMethod("Create");
            return (obj, sp) =>
            {
                object factory = sp.GetService(factoryType);
                object[] args = attribute == null ? null : new[] { attribute?.Name };
                object instance = createFn.Invoke(factory, args);
                fieldInfo.SetValue(obj, instance);
            };
        }
    }
}
