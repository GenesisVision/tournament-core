using System;

namespace GenesisVision.Tournament.Core.ViewModels.TradeServer
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime? RegisterDateFrom { get; set; }
        public DateTime? RegisterDateTo { get; set; }
        public bool IsEnabled { get; set; }
    }
}
