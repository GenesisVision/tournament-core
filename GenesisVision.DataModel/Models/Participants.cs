using System;

namespace GenesisVision.DataModel.Models
{
    public class Participants
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime RegDate { get; set; }

        public TradeAccounts TradeAccount { get; set; }
    }
}
