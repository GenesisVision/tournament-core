using System;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class ParticipantRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
