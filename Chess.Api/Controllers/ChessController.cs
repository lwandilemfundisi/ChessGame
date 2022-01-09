using AutoMapper;
using Chess.Api.Mappers;
using Chess.Api.Models.RequestModels;
using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Commands;
using Chess.Domain.DomianModel.ChessModel.Queries;
using Chess.Domain.DomianModel.ChessModel.ValueObjects;
using Chess.Domain.Extensions;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandBus _commandBus;
        private readonly IQueryProcessor _queryProcessor;

        public ChessController(
            IMapper mapper,
            ICommandBus commandBus, 
            IQueryProcessor queryProcessor)
        {
            _mapper = mapper;
            _commandBus = commandBus;
            _queryProcessor = queryProcessor;
        }

        [HttpPost("createboard")]
        public async Task<IActionResult> CreateBoard()
        {
            return Ok(await _commandBus
                .PublishAsync(
                new CreateBoardCommand(
                    BoardId.New, 
                    ChessExtensions.BuildBoard().PlaceAllPieces()), CancellationToken.None));
        }

        [HttpGet("getboard")]
        public async Task<IActionResult> GetBoard([FromQuery] string boardId)
        {
            return Ok(await _queryProcessor
                .ProcessAsync(new GetBoardQuery(new BoardId(boardId)), CancellationToken.None));
        }

        [HttpPost("move")]
        public async Task<IActionResult> Move(MoveRequestModel model)
        {
            if(ModelState.IsValid)
            {
                return Ok(await _commandBus
                .PublishAsync(new MovePieceCommand(
                    model.BoardId,
                    new MoveMapper(model).Map()), CancellationToken.None));
            }
            else
            {
                return BadRequest(ModelState.Values);
            }
        }
    }
}
