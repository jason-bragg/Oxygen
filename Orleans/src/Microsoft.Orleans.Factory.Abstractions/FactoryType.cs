
using System;

namespace Microsoft.Orleans.Factory.Abstractions
{
    public static class FactoryType
    {
        public static Type CreateFactoryInterfaceType(Type ttype)
        {
            Type[] typeArgs = { ttype };
            return typeof(IFactory<>).MakeGenericType(typeArgs);
        }
        public static Type CreateFactoryInterfaceType(Type tkey, Type ttype)
        {
            Type[] typeArgs = { tkey, ttype };
            return typeof(IFactory<,>).MakeGenericType(typeArgs);
        }
    }
}
