using System;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class AccountCreated
    {
        public Guid ParticipantId { get; set; }
        public Guid TradeServerId { get; set; }
        public long Login { get; set; }
        public string Password { get; set; }
        public decimal StartBalance { get; set; }
    }
}
