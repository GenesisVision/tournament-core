using System;
using System.Linq;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;

namespace GenesisVision.Tournament.Core.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext context;

        public TournamentService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<bool> CheckEmailExists(NewParticipant model)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                return context.Participants.Any(x => x.Email.ToLower() == model.Email.ToLower().Trim());
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
