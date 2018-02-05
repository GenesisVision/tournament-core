using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using GenesisVision.Tournament.Core.ViewModels.Tournament;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace GenesisVision.Tournament.Core.Controllers
{
	[EnableCors("AllowSpecificOrigin")]
	[Route("api/tournament")]
    public class TournamentController : BaseController
    {
        private readonly ITournamentService tournamentService;

        public TournamentController(ITournamentService tournamentService)
        {
            this.tournamentService = tournamentService;
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
            if (!ModelState.IsValid)
                return BadRequest(ErrorResult.GetResult(ModelState));

            var state = tournamentService.CheckEmailExists(model);
            if (!state.IsSuccess || state.Data)
                return BadRequest(ErrorResult.GetResult(new List<string> {"Email already registered"}, ErrorCodes.ValidationError));

            var res = tournamentService.RegisterParticipant(model);
            if (!res.IsSuccess)
                return BadRequest(ErrorResult.GetResult(res));

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
