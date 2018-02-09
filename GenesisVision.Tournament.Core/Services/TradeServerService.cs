using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenesisVision.Tournament.Core.Services
{
    public class TradeServerService : ITradeServerService
    {
        private readonly ApplicationDbContext context;
        private readonly IStatisticService statisticService;

        public TradeServerService(ApplicationDbContext context, IStatisticService statisticService)
        {
            this.context = context;
            this.statisticService = statisticService;
        }

        public OperationResult<TradeServerViewModel> GetInitData(Guid tradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                //var data = context.TradeServers
                //                  .First(x => x.Id == tradeServerId)
                //                  .ToTradeServer();

                var tournament = context.Tournaments
                                        .FirstOrDefault(x => x.IsEnabled && x.DateTo > DateTime.Now)?
                                        .ToTournament();

                var newParticipants = context.Participants
                                             .Where(x => x.TradeAccount == null)
                                             .Select(x => x.ToParticipantRequest())
                                             .ToList();

                var tradeAccounts = context.TradeAccounts
                                           .Where(x => x.TradeServerId == tradeServerId)
                                           .Select(x => x.ToTradeAccount())
                                           .ToList();

                return new TradeServerViewModel
                       {
                           Tournament = tournament,
                           ParticipantRequest = newParticipants,
                           TradeAccounts = tradeAccounts
                       };
            });
        }

        public OperationResult TradeAccountsCreated(List<AccountCreated> accounts)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                foreach (var accountData in accounts)
                {
                    var participant = context.Participants.FirstOrDefault(x => x.Id == accountData.ParticipantId);
                    if (participant == null)
                        continue;

                    var tournament = context.Tournaments.FirstOrDefault(x => x.IsEnabled);
                    if (tournament == null)
                        continue;

                    var account = new TradeAccounts
                                  {
                                      Id = Guid.NewGuid(),
                                      TournamentId = tournament.Id,
                                      Login = accountData.Login,
                                      Password = accountData.Password,
                                      OrdersCount = 0,
                                      StartBalance = accountData.StartBalance,
                                      ParticipantId = accountData.ParticipantId,
                                      TotalProfit = 0,
                                      TotalProfitInPercent = 0,
                                      TradeServerId = accountData.TradeServerId
                                  };
                    context.Add(account);
                    context.SaveChanges();
                }
            });
        }

        public OperationResult NewTrade(NewTrade trade)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var account = context.TradeAccounts
                                     .Include(x => x.Trades)
                                     .Include(x => x.Charts)
                                     .FirstOrDefault(x => x.Id == trade.TradeAccountId);
                if (account == null)
                    return;

                var t = new Trades
                        {
                            Id = Guid.NewGuid(),
                            TradeAccountId = account.Id,
                            Date = trade.Date,
                            Direction = trade.Direction,
                            Price = trade.Price,
                            Profit = trade.Profit,
                            Symbol = trade.Symbol,
                            Ticket = trade.Ticket,
                            Volume = trade.Volume
                        };
                context.Add(t);

                account.OrdersCount += 1;
                account.TotalProfit += trade.Profit;
                account.TotalProfitInPercent = account.TotalProfit / account.StartBalance * 100m;

                context.SaveChanges();

                statisticService.RecalculateChart(account);
                statisticService.RecalculatePlaces();
            });
        }
    }
}
