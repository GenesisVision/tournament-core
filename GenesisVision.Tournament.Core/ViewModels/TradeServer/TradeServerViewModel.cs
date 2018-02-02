using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class TradeServerViewModel
    {
        public List<ParticipantRequest> ParticipantRequest { get; set; }

        public Tournament Tournament { get; set; }
    }
}
