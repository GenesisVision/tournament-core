using System;
using System.Collections.Generic;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.ViewModels.Tournament;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface ITournamentService
    {
        OperationResult<bool> CheckEmailExists(NewParticipant model);

        OperationResult RegisterParticipant(NewParticipant model);

        OperationResult<ParticipantsSummaryViewModel> GetParticipantsSummary();

        OperationResult<(List<ParticipantViewModel>, int)> GetParticipants(ParticipantsFilter filter);

        OperationResult<ParticipantViewModel> GetParticipant(Guid participantId);

        OperationResult<(List<TradeViewModel>, int)> GetParticipantTrades(TradesFilter filter);
    }
}
