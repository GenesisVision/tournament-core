using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class NewTrade
    {
        public Guid TradeAccountId { get; set; }
        public long Ticket { get; set; }
        public TradeDirection Direction { get; set; }
        public TradeEntry Entry { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        public decimal Volume { get; set; }
        public DateTime Date { get; set; }
    }
}
