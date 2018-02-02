using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;

namespace GenesisVision.Tournament.Core.Models
{
    public static class DtoConvertors
    {
        public static TradeServer ToTradeServer(this TradeServers data)
        {
            return new TradeServer
                   {
                       Id = data.Id,
                       Description = data.Description,
                       Host = data.Host,
                       Title = data.Title,
                       Type = data.Type
                   };
        }

        public static ParticipantRequest ToParticipantRequest(this Participants request)
        {
            return new ParticipantRequest
                   {
                       Email = request.Email,
                       Id = request.Id,
                       Name = request.Name
                   };
        }

        public static TradeAccount ToTradeAccount(this TradeAccounts t)
        {
            return new TradeAccount
                   {
                       Id = t.Id,
                       Login = t.Login
                   };
        }

        public static ViewModels.TradeServer.Tournament ToTournament(this Tournaments t)
        {
            return new ViewModels.TradeServer.Tournament
                   {
                       Id = t.Id,
                       Title = t.Title,
                       Description = t.Description,
                       DateFrom = t.DateFrom,
                       DateTo = t.DateTo,
                       IsEnabled = t.IsEnabled,
                       RegisterDateFrom = t.RegisterDateFrom,
                       RegisterDateTo = t.RegisterDateTo
                   };
        }
    }
}
