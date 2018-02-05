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
	[EnableCors("AllowSpecificOrigin")]
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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ParticipantsViewModel))]
        public IActionResult Participants([FromBody]ParticipantsFilter filter)
        {
            return Ok(new ParticipantsViewModel
                      {
                          Participants = new List<ParticipantViewModel>(),
                          Total = 0
                      });
        }
    }
}
