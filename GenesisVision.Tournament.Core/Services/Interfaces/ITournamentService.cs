using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.ViewModels.Tournament;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface ITournamentService
    {
        OperationResult<bool> CheckEmailExists(NewParticipant model);

        OperationResult RegisterParticipant(NewParticipant model);
    }
}
