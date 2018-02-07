using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
using Microsoft.EntityFrameworkCore;

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

        public OperationResult<(List<ParticipantViewModel>, int)> GetParticipants(ParticipantsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.Participants
                                   .Include(x => x.TradeAccount)
                                   .ThenInclude(x => x.Charts)
                                   .OrderByDescending(x => x.TradeAccount.TotalProfit)
                                   .Where(x => x.TradeAccount != null);

                var total = query.Count();

                if (filter != null)
                {
                    if (filter.Skip.HasValue)
                        query = query.Skip(filter.Skip.Value);
                    if (filter.Take.HasValue)
                        query = query.Take(filter.Take.Value);
                }

                var result = query
                    .Select(x => x.ToParticipantViewModel())
                    .ToList();

                var place = 1;
                foreach (var x in result)
                {
                    x.Place = place;
                    place++;
                }

                return (result, total);
            });
        }

        public OperationResult<ParticipantViewModel> GetParticipant(Guid participantId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var participant = context.Participants
                                         .Include(x => x.TradeAccount)
                                         .ThenInclude(x => x.Charts)
                                         .FirstOrDefault(x => x.Id == participantId);

                return participant.ToParticipantViewModel();
            });
        }

        public OperationResult<(List<TradeViewModel>, int)> GetParticipantTrades(TradesFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var query = context.Trades.Where(x => x.TradeAccount.ParticipantId == filter.ParticipantId);

                var total = query.Count();

                if (filter.Skip.HasValue)
                    query = query.Skip(filter.Skip.Value);
                if (filter.Take.HasValue)
                    query = query.Take(filter.Take.Value);
                if (filter.Direction.HasValue)
                    query = query.Where(x => x.Direction == filter.Direction.Value);
                if (!string.IsNullOrEmpty(filter.Symbol))
                {
                    var str = filter.Symbol.ToLower().Trim();
                    query = query.Where(x => filter.Symbol.ToLower().Contains(str));
                }

                var result = query
                    .OrderByDescending(x => x.Ticket)
                    .Select(x => x.ToTradeViewModel())
                    .ToList();

                return (result, total);
            });
        }
    }
}
