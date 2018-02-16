using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface IStatisticService
    {
        int GetParticipantPlace(Guid participantId);

        List<Guid> GetParticipantsByPlace(int? skip, int? take, List<Guid> filteredIds);

        void RecalculateChart(TradeAccounts account, int pointsCount = 30, ChartType type = ChartType.ByProfit);

        void RecalculatePlaces();
    }
}