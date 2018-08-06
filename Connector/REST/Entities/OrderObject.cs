using System;
using Newtonsoft.Json;

namespace Connector.REST.Entities
{
    public class OrderObject : BaseRestObject
    {
        public string OrderId { get; set; }
        [JsonProperty("clOrdID")]
        public string ClientOrderId { get; set; }
        [JsonProperty("clOrdLinkID")]
        public string ClientOrderLinkId { get; set; }
        public int Account { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public int SimpleOrderQty { get; set; }
        public int OrderQty { get; set; }
        public int Price { get; set; }
        public int DisplayQty { get; set; }
        public int StopPx { get; set; }
        public int PegOffsetValue { get; set; }
        public string PegPriceType { get; set; }
        public string Currency { get; set; }
        public string SettlCurrency { get; set; }
        public string OrdType { get; set; }
        public string TimeInForce { get; set; }
        public string ExecInst { get; set; }
        public string ContingencyType { get; set; }
        public string ExDestination { get; set; }
        public string OrdStatus { get; set; }
        public string Triggered { get; set; }
        public bool WorkingIndicator { get; set; }
        public string OrdRejReason { get; set; }
        public int SimpleLeavesQty { get; set; }
        public int LeavesQty { get; set; }
        public int SimpleCumQty { get; set; }
        public int CumQty { get; set; }
        public int AvgPx { get; set; }
        public string MultiLegReportingType { get; set; }
        public string Text { get; set; }
        public DateTime TransactTime { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
