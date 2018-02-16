using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace GenesisVision.Tournament.Core.Services
{
    public class TradeServerService : ITradeServerService
    {
        private readonly ApplicationDbContext context;
        private readonly IStatisticService statisticService;
        private readonly IIpfsService ipfsService;

        public TradeServerService(ApplicationDbContext context, IStatisticService statisticService, IIpfsService ipfsService)
        {
            this.context = context;
            this.statisticService = statisticService;
            this.ipfsService = ipfsService;
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

                var trades = GetTradeHistory(account);
                if (trades.IsSuccess)
                {
                    var ipfsHash = ipfsService.WriteIpfsText(trades.Data);
                    if (ipfsHash.IsSuccess)
                    {
                        account.IpfsHash = ipfsHash.Data;
                        context.SaveChanges();
                    }
                }

                statisticService.RecalculateChart(account);
                statisticService.RecalculatePlaces();
            });
        }

        private static OperationResult<string> GetTradeHistory(TradeAccounts account)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

                var csv = new StringBuilder($"\"Login\";\"Ticket\";\"Symbol\";\"Price\";\"Profit\";\"Volume\";\"Date\";\"Direction\";{Environment.NewLine}");

                foreach (var trade in account.Trades.OrderBy(x => x.Date))
                {
                    csv.AppendLine($"\"{account.Login}\";" +
                                   $"\"{trade.Ticket}\";" +
                                   $"\"{trade.Symbol}\";" +
                                   $"\"{trade.Price}\";" +
                                   $"\"{trade.Profit}\";" +
                                   $"\"{trade.Volume}\";" +
                                   $"\"{trade.Date:yyyy-MM-dd HH:mm:ss}\";" +
                                   $"\"{trade.Direction}\";");
                }
                return csv.ToString();
            });
        }
    }
}
