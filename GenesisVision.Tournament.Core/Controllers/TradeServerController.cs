using GenesisVision.Tournament.Core.Infrastructure;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.TradeServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Controllers
{
    [Route("api/tradeserver")]
    public class TradeServerController : BaseController
    {
        private readonly ITradeServerService tradeServerService;
        private readonly ILogger<TradeServerController> logger;

        public TradeServerController(ITradeServerService tradeServerService, ILogger<TradeServerController> logger)
        {
            this.tradeServerService = tradeServerService;
            this.logger = logger;
        }

        /// <summary>
        /// Init data for trade server wrapper
        /// </summary>
        [HttpGet]
        [Route("initData")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TradeServerViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetInitData(Guid tradeServerId)
        {
            if (!HttpContext.IsLocalRequest())
                return BadRequest();

            var res = tradeServerService.GetInitData(tradeServerId);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok(res.Data);
        }

        /// <summary>
        /// Store trade account
        /// </summary>
        [HttpPost]
        [Route("tradeAccountCreated")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult TradeAccountCreated([FromBody]List<AccountCreated> accounts)
        {
            if (!HttpContext.IsLocalRequest())
                return BadRequest();

            var res = tradeServerService.TradeAccountsCreated(accounts);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));
            
            return Ok();
        }

        /// <summary>
        /// New trade event
        /// </summary>
        [HttpPost]
        [Route("newTrade")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult NewTrade([FromBody]NewTrade trade)
        {
            if (!HttpContext.IsLocalRequest())
                return BadRequest();

            var res = tradeServerService.NewTrade(trade);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok();
        }
    }
}
