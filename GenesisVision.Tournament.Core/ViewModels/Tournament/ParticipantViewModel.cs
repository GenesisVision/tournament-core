using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.ViewModels.Tournament
{
    public class ParticipantViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime RegDate { get; set; }
        public long Login { get; set; }
        public string IpfsHash { get; set; }
        public int OrdersCount { get; set; }
        public decimal StartBalance { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalProfitInPercent { get; set; }
        public List<decimal> Chart { get; set; }
        public int Place { get; set; }
    }
}
