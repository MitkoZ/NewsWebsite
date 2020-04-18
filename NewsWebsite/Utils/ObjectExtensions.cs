using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NewsWebsite.Utils
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            return obj.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(property => property.Name, property => property.GetValue(obj, null));
        }
    }
}
