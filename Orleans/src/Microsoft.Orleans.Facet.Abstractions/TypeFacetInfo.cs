
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Orleans.Factory.Abstractions;

namespace Microsoft.Orleans.Facet.Abstractions
{
    public class TypeFacetInfo
    {
        private static readonly List<Func<IServiceProvider, object>> Empty = new List<Func<IServiceProvider, object>>();
        private readonly List<Action<object, IServiceProvider>> setters;
        private readonly List<Func<IServiceProvider, object>> constructorParameterFactories;

        public TypeFacetInfo(Type type)
        {
            this.setters = CreateFieldSetters(type);
            this.constructorParameterFactories = CreateConstructorParameterFactories(type);
        }

        public void SetFields(object obj, IServiceProvider serviceProvider)
        {
            this.setters.ForEach(set => set(obj, serviceProvider));
        }

        public object[] CreateConstructorParameterFacets(IServiceProvider serviceProvider)
        {
            return this.constructorParameterFactories.Select(factory => factory.Invoke(serviceProvider)).ToArray();
        }

        private static List<Action<object, IServiceProvider>> CreateFieldSetters(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fi => typeof(IGrainFacet).IsAssignableFrom(fi.FieldType))
                .Select(CreateFieldSetter)
                .ToList();
        }

        private static Action<object, IServiceProvider> CreateFieldSetter(FieldInfo fieldInfo)
        {
            FacetAttribute attribute = fieldInfo.GetCustomAttribute<FacetAttribute>();
            Type factoryType = attribute == null
                ? FactoryType.CreateFactoryInterfaceType(fieldInfo.FieldType)
                : FactoryType.CreateFactoryInterfaceType(typeof(string), fieldInfo.FieldType);
            MethodInfo createFn = factoryType.GetMethod("Create");
            return (obj, sp) =>
            {
                object factory = sp.GetService(factoryType);
                object[] args = attribute == null ? null : new object[] { attribute?.Name };
                object instance = createFn.Invoke(factory, args);
                fieldInfo.SetValue(obj, instance);
            };
        }

        private static List<Func<IServiceProvider, object>> CreateConstructorParameterFactories(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            ConstructorInfo constructor = constructors.Length < 2
                ? constructors.FirstOrDefault()
                : constructors.FirstOrDefault(c => c.GetCustomAttribute<FacetConstructorAttribute>() != null);
            return constructor == null
                ? Empty
                : constructor
                    .GetParameters()
                    .Where(pi => typeof(IGrainFacet).IsAssignableFrom(pi.ParameterType))
                    .Select(CreateConstructorParameterFactory)
                    .ToList();
        }

        private static Func<IServiceProvider, object> CreateConstructorParameterFactory(ParameterInfo parameter)
        {
            FacetAttribute attribute = parameter.GetCustomAttribute<FacetAttribute>();
            Type factoryType = attribute == null
                ? FactoryType.CreateFactoryInterfaceType(parameter.ParameterType)
                : FactoryType.CreateFactoryInterfaceType(typeof(string), parameter.ParameterType);
            MethodInfo createFn = factoryType.GetMethod("Create");
            return sp =>
            {
                object factory = sp.GetService(factoryType);
                object[] args = attribute == null ? null : new object[] { attribute.Name };
                return createFn.Invoke(factory, args);
            };
        }
    }
}
