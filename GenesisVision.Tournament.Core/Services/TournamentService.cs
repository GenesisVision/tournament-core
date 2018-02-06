using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
using System;
using System.Linq;

namespace GenesisVision.Tournament.Core.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext context;

        public TournamentService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult CheckNewParticipant(NewParticipant model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var tournament = context.Tournaments.FirstOrDefault();
                if (tournament == null || !tournament.IsEnabled || (tournament.RegisterDateTo.HasValue && tournament.RegisterDateTo < DateTime.Now))
                    throw new Exception("Registration is closed");

                var exist = context.Participants.Any(x => x.Email.ToLower() == model.Email.ToLower().Trim());
                if (exist)
                    throw new Exception("Email already registered");
            });
        }

        public OperationResult RegisterParticipant(NewParticipant model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var participant = new Participants
                                  {
                                      Id = Guid.NewGuid(),
                                      RegDate = DateTime.Now,
                                      Email = model.Email.Trim(),
                                      Name = model.Name.Trim(),
                                      EthAddress = model.EthAddress,
                                      Avatar = model.Avatar
                                  };
                context.Add(participant);
                context.SaveChanges();
            });
        }

        public OperationResult<ParticipantsSummaryViewModel> GetParticipantsSummary()
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var count = context.Participants.Count();
                var lastDate = context.Participants.OrderByDescending(x => x.RegDate).FirstOrDefault();

                return new ParticipantsSummaryViewModel
                       {
                           ParticipantsCount = count,
                           LastRegistrationDate = lastDate?.RegDate
                       };
            });
        }
    }
}
