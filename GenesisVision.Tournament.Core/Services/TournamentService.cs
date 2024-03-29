﻿using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Tournament.Core.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext context;
        private readonly IStatisticService statisticService;

        public TournamentService(ApplicationDbContext context, IStatisticService statisticService)
        {
            this.context = context;
            this.statisticService = statisticService;
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

        public OperationResult<(List<ParticipantViewModel>, int)> GetParticipants(ParticipantsFilter filter)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var filteredIds = new List<Guid>();
                if (!string.IsNullOrEmpty(filter?.Name))
                {
                    var tmp = filter.Name.ToLower().Trim();
                    filteredIds = context.Participants
                                         .Where(x => x.TradeAccount != null && x.Name.ToLower().Contains(tmp))
                                         .Select(x => x.Id)
                                         .ToList();

                    if (!filteredIds.Any())
                        return (new List<ParticipantViewModel>(), 0);
                }

                var participants = statisticService.GetParticipantsByPlace(filter?.Skip, filter?.Take, filteredIds);
                if (!participants.Any())
                    return (new List<ParticipantViewModel>(), 0);

                var total = filteredIds.Any()
                    ? filteredIds.Count
                    : context.Participants.Count(x => x.TradeAccount != null);

                var query = context.Participants
                                   .Include(x => x.TradeAccount)
                                   .ThenInclude(x => x.Charts)
                                   .Where(x => participants.Contains(x.Id));
                
                var result = query
                    .Select(x => x.ToParticipantViewModel())
                    .ToList()
                    .OrderBy(x => participants.IndexOf(x.Id))
                    .ToList();
                
                foreach (var x in result)
                {
                    x.Place = statisticService.GetParticipantPlace(x.Id);
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
                                         .ThenInclude(x => x.Trades)
                                         .FirstOrDefault(x => x.Id == participantId);
                
                var res = participant.ToParticipantFullChartViewModel();
                res.Place = statisticService.GetParticipantPlace(participantId);

                return res;
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
