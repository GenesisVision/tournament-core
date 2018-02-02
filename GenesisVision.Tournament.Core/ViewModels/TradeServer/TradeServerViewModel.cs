using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class TradeServerViewModel
    {
        public Tournament Tournament { get; set; }

        public List<ParticipantRequest> ParticipantRequest { get; set; }

        public List<TradeAccount> TradeAccounts { get; set; }
    }
}
