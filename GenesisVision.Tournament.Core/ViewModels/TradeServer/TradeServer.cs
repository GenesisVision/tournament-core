using System;
using GenesisVision.DataModel.Enums;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class TradeServer
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Host { get; set; }
        public TradeServerType Type { get; set; }
    }
}
