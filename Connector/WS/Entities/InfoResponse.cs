using System;

namespace Connector.WS.Entities
{
    public class InfoResponse : ResponseBase
    {
        public string Info { get; set; }
        public string Version { get; set; }
        public string Timestamp { get; set; }
    }
}
