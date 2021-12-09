using Chess.Domain.DomianModel.ChessModel;
using Chess.Domain.DomianModel.ChessModel.Commands;
using Chess.Domain.DomianModel.ChessModel.Queries;
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
        private readonly ICommandBus _commandBus;
        private readonly IQueryProcessor _queryProcessor;

        public ChessController(
            ICommandBus commandBus, 
            IQueryProcessor queryProcessor)
        {
            _commandBus = commandBus;
            _queryProcessor = queryProcessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard()
        {
            return Ok(await _commandBus
                .PublishAsync(new CreateBoardCommand(BoardId.New), CancellationToken.None));
        }

        [HttpGet]
        public async Task<IActionResult> GetBoard([FromQuery] string boardId)
        {
            return Ok(await _queryProcessor
                .ProcessAsync(new GetBoardQuery(new BoardId(boardId)), CancellationToken.None));
        }
    }
}
