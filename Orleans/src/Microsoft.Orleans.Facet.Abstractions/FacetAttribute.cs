
using System;

namespace Microsoft.Orleans.Facet.Abstractions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class FacetAttribute : Attribute
    {
        public string Name { get; }
        public FacetAttribute(string facetTypeName)
        {
            this.Name = facetTypeName;
        }
    }
}
