﻿using Newtonsoft.Json;

namespace Connector.WS
{
    public class WebSocketCommand
    {
        public string op { get; set; }
        public string[] args { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool success { get; set; }
    }
}
