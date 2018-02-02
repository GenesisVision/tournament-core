using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class Trades
    {
        public Guid Id { get; set; }
        public long Ticket { get; set; }
        public DateTime Date { get; set; }
        public string Symbol { get; set; }
        public TradeDirection Direction { get; set; }
        public decimal Price { get; set; }
        public decimal Profit { get; set; }
        public decimal Volume { get; set; }

        public Guid TradeAccountId { get; set; }
        public TradeAccounts TradeAccount { get; set; }
    }
}
