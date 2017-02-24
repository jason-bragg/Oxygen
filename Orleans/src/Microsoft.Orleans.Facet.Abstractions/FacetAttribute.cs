
using System;

namespace Microsoft.Orleans.Facet.Abstractions
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FacetAttribute : Attribute
    {
        public string Name { get; }
        public FacetAttribute(string facetTypeName)
        {
            this.Name = facetTypeName;
        }
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class FacetConstructorAttribute : Attribute
    {
    }
}
