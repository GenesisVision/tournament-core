using GenesisVision.DataModel.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Tournament.Core.ViewModels.Tournament
{
    public class TradesFilter
    {
        [Required]
        public Guid ParticipantId { get; set; }
        public string Symbol { get; set; }
        public TradeDirection? Direction { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}
