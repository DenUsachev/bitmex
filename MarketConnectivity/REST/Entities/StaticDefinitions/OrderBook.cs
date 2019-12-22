using System;
using System.Linq;
using Newtonsoft.Json;

namespace MarketConnectivity.REST.Entities.StaticDefinitions
{
    [JsonObject]
    public class OrderBook
    {
        public OrderBook()
        {
        }

        public string Symbol { get; set; }

        public UInt64 Id { get; set; }

        public UInt64 Size { get; set; }

        public Decimal Price { get; set; }

        [JsonProperty("side")]
        public string SideRaw { get; set; }

        public TradingSide Side
        {
            get
            {
               var enumValueName = Enum.GetNames(typeof(TradingSide)).Where(it => it == this.SideRaw);
               return TradingSide.Buy;
            }
        }
    }
}
