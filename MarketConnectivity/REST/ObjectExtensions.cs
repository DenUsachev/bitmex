using System.Collections.Generic;
using Newtonsoft.Json;

<<<<<<< HEAD:Connector/Extensions/ObjectExtensions.cs
namespace Connector.Extensions
=======
namespace MarketConnectivity.REST
>>>>>>> * Bitmex.Runner.csproj: Projects ported to netcore:MarketConnectivity/REST/ObjectExtensions.cs
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