using System;
using System.Collections.Generic;

namespace GenesisVision.DataModel.Models
{
    public class TradeAccounts
    {
        public Guid Id { get; set; }
        public long Login { get; set; }
        public string Password { get; set; }
        public string IpfsHash { get; set; }
        public int OrdersCount { get; set; }
        public decimal StartBalance { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalProfitInPercent { get; set; }

        public TradeServers TradeServer { get; set; }
        public Guid TradeServerId { get; set; }

        public Participants Participant { get; set; }
        public Guid? ParticipantId { get; set; }

        public Tournaments Tournament { get; set; }
        public Guid TournamentId { get; set; }

        public ICollection<Trades> Trades { get; set; }
        public ICollection<Charts> Charts { get; set; }
    }
}
