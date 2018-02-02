using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.DataModel.Models
{
    public class Charts
    {
        public Guid Id { get; set; }
        public ChartType Type { get; set; }
        public int Index { get; set; }
        public decimal Value { get; set; }

        public TradeAccounts TradeAccount { get; set; }
        public Guid TradeAccountId { get; set; }
    }
}
