using System;
using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class TradeServer
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeServerType Type { get; set; }
    }
}
