using GenesisVision.DataModel.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace GenesisVision.Tournament.Core.ViewModels.Tournament
{
    public class TradeViewModel
    {
        public Guid Id { get; set; }
        public long Ticket { get; set; }
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeDirection Direction { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        public decimal Volume { get; set; }
    }
}
