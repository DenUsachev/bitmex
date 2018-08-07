using System.Collections.Generic;
using Newtonsoft.Json;

namespace Connector.REST
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, string> ToStringDicrionary(this object o)
        {
            var resultDictionary = new Dictionary<string, string>();
            var type = o.GetType();
            foreach (var property in type.GetProperties())
            {
                if (property.CanRead)
                {
                    var propAtts = property.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                    if (propAtts.Length > 0)
                        continue;

                    var propertyValue = property.GetValue(o).ToString();
                    resultDictionary.Add(property.Name, propertyValue);
                }
            }

            return resultDictionary;
        }
    }
}