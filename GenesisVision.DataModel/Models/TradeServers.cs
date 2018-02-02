using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class TradeServers
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }
        public TradeServerType Type { get; set; }

        public ICollection<TradeAccounts> TradeAccounts { get; set; }
    }
}
