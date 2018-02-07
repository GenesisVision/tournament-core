using System;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Controllers
{
	[Route("api/tournament")]
    public class TournamentController : BaseController
    {
        private readonly ITournamentService tournamentService;
        private readonly ILogger<TournamentController> logger;

        public TournamentController(ITournamentService tournamentService, ILogger<TournamentController> logger)
        {
            this.tournamentService = tournamentService;
            this.logger = logger;
        }

        /// <summary>
        /// Registration for the tournament
        /// </summary>
        [HttpPost]
        [Route("register")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult RegisterParticipant([FromBody]NewParticipant model)
        {
            logger.LogInformation($"#RegisterParticipant. {model?.Email}, {model?.Name}, {model?.EthAddress}, {model?.Avatar}");

            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var state = tournamentService.CheckEmailExists(model);
            if (!state.IsSuccess || state.Data)
                return BadRequest(ErrorResult.GetResult(new List<string> {"Email already registered"}, ErrorCodes.ValidationError));

            var res = tournamentService.RegisterParticipant(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            logger.LogInformation($"#RegisterParticipant. {model?.Email}: OK");
            return Ok();
        }

        /// <summary>
        /// Participants list
        /// </summary>
        [HttpPost]
        [Route("participants")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ParticipantsViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult Participants([FromBody]ParticipantsFilter filter)
        {
            var res = tournamentService.GetParticipants(filter);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok(new ParticipantsViewModel
                      {
                          Participants = res.Data.Item1,
                          Total = res.Data.Item2
                      });
        }

        /// <summary>
        /// Participants summary
        /// </summary>
        [HttpPost]
        [Route("participants/count")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ParticipantsSummaryViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetParticipantsSummary()
        {
            var res = tournamentService.GetParticipantsSummary();
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok(res.Data);
        }

        /// <summary>
        /// Participant info
        /// </summary>
        [HttpGet]
        [Route("participant")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ParticipantViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetParticipant(Guid participantId)
        {
            var res = tournamentService.GetParticipant(participantId);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok(res.Data);
        }

        /// <summary>
        /// Participant trades history
        /// </summary>
        [HttpPost]
        [Route("participant/trades")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TradesViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorViewModel))]
        public IActionResult GetParticipantTrades([FromBody]TradesFilter filter)
        {
            var res = tournamentService.GetParticipantTrades(filter);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

            return Ok(new TradesViewModel
                      {
                          Trades = res.Data.Item1,
                          Total = res.Data.Item2
                      });
        }
    }
}
