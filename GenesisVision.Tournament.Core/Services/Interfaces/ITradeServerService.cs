using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;
using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Services.Interfaces
{
    public interface ITradeServerService
    {
        OperationResult<TradeServerViewModel> GetInitData(Guid tradeServerId);

        OperationResult TradeAccountsCreated(List<AccountCreated> accounts);
    }
}
