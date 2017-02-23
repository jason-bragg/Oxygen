
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
            return (obj, sp) =>
            {
                Type factoryType = FactoryType.CreateFactoryInterfaceType(fieldInfo.FieldType);
                object factory = sp.GetService(factoryType);
                var createFn = factoryType.GetMethod("Create");
                object instance = createFn.Invoke(factory, null);
                fieldInfo.SetValue(obj, instance);
            };
        }
    }
}
