using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.REST.Entities
{
    public class LimitOrderRequest
    {
        public string Symbol { get; set; }
        public string Side { get; set; }
        public double SimpleOrderQty { get; set; }

    }
}
