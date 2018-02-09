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

namespace GenesisVision.Tournament.Core.Services
{
    public class TradeServerService : ITradeServerService
    {
        private readonly ApplicationDbContext context;

        public TradeServerService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public OperationResult<TradeServerViewModel> GetInitData(Guid tradeServerId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                //var data = context.TradeServers
                //                  .First(x => x.Id == tradeServerId)
                //                  .ToTradeServer();

                var tournament = context.Tournaments
                                        .FirstOrDefault(x => x.IsEnabled && x.DateFrom > DateTime.Now)?
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

                RecalculateChart(account);

                context.SaveChanges();
            });
        }

        private void RecalculateChart(TradeAccounts account, int pointsCount = 30, ChartType type = ChartType.ByProfit)
        {
            var result = new List<decimal> {0};
            
            var startBalance = account.StartBalance;
            var profits = account.Trades
                                 .OrderBy(x => x.Date)
                                 .Select(x => x.Profit)
                                 .ToList();

            var statistic = new List<decimal>();
            var balances = new List<decimal>();
            for (var i = 0; i < profits.Count; i++)
            {
                if (i == 0)
                {
                    statistic.Add((startBalance + profits[i]) / startBalance * 100m - 100m);
                    balances.Add(startBalance + profits[i]);
                }
                else
                {
                    statistic.Add((balances[i - 1] + profits[i]) / startBalance * 100m - 100m);
                    balances.Add(balances[i - 1] + profits[i]);
                }
            }

            var list = new List<List<decimal>>();
            var step = statistic.Count <= pointsCount
                ? 1
                : statistic.Count % pointsCount >= pointsCount / 3
                    ? statistic.Count / pointsCount + 1
                    : statistic.Count / pointsCount;

            var count = 0;
            do
            {
                list.Add(statistic.Skip(count).Take(step).ToList());
                count += step;
            } while (count < statistic.Count);

            if (!list.Any() || !list.First().Any())
                return;

            switch (type)
            {
                case ChartType.ByProfit:
                    result = list
                        .Select(x => Math.Round(x.Average(y => y), 2))
                        .ToList();
                    break;
            }

            result.Add(account.TotalProfitInPercent);

            context.RemoveRange(account.Charts.Where(x => x.Type == type));

            var index = 0;
            foreach (var chart in result)
            {
                context.Add(new Charts
                            {
                                Id = Guid.NewGuid(),
                                Type = type,
                                Index = index,
                                TradeAccountId = account.Id,
                                Value = chart
                            });
                index++;
            }
        }
    }
}
