using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Orleans.Facet.Abstractions
{
    public class TypeFacetInfo
    {

        public TypeFacetInfo(Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var setters = fields.Where(fi => typeof(IGrainFacet).IsAssignableFrom(fi.FieldType)).Select(fi => MakeFieldSetter(fi))
        }

        Action<object, IServiceProvider> MakeFieldSetter(FieldInfo fieldInfo)
        {
            
        }
    }
}
