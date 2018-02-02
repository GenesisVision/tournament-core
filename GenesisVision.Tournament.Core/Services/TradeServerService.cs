using System;
using System.Collections.Generic;
using System.Linq;
using GenesisVision.DataModel;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;

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

                return new TradeServerViewModel
                       {
                           Tournament = tournament,
                           ParticipantRequest = newParticipants
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
    }
}
