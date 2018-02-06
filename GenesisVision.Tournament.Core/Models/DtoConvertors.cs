using System.Collections.Generic;
using System.Linq;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
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
                       Name = request.Name,
                       EthAddress = request.EthAddress,
                       Avatar = request.Avatar
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

        public static ParticipantViewModel ToParticipantViewModel(this Participants x)
        {
            return new ParticipantViewModel
                   {
                       Id = x.Id,
                       RegDate = x.RegDate,
                       Name = x.Name,
                       Avatar = x.Avatar,
                       Place = 0,
                       IpfsHash = x.TradeAccount?.IpfsHash,
                       Login = x.TradeAccount?.Login ?? 0,
                       OrdersCount = x.TradeAccount?.OrdersCount ?? 0,
                       TotalProfit = x.TradeAccount?.TotalProfit ?? 0,
                       TotalProfitInPercent = x.TradeAccount?.TotalProfitInPercent ?? 0,
                       Chart = x.TradeAccount?
                                .Charts?
                                .Where(c => c.Type == ChartType.ByProfit)
                                .OrderBy(c => c.Index)
                                .Select(c => c.Value)
                                .ToList() ?? new List<decimal>()
                   };
        }

        public static TradeViewModel ToTradeViewModel(this Trades t)
        {
            return new TradeViewModel
                   {
                       Id = t.Id,
                       Date = t.Date,
                       Direction = t.Direction,
                       Price = t.Price,
                       Profit = t.Profit,
                       Symbol = t.Symbol,
                       Ticket = t.Ticket,
                       Volume = t.Volume
                   };
        }
    }
}
