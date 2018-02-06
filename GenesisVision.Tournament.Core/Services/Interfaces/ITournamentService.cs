using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.ViewModels.Tournament;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface ITournamentService
    {
        OperationResult CheckNewParticipant(NewParticipant model);

        OperationResult RegisterParticipant(NewParticipant model);

        OperationResult<ParticipantsSummaryViewModel> GetParticipantsSummary();
    }
}
