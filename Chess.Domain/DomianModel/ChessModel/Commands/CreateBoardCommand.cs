using Chess.Domain.DomianModel.ChessModel.Entities;
using Chess.Domain.DomianModel.ChessModel.ValueObjects.LookupValueObjects;
using Microservice.Framework.Domain.Commands;
using Microservice.Framework.Domain.ExecutionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chess.Domain.DomianModel.ChessModel.Commands
{
    public class CreateBoardCommand
        : Command<Board, BoardId>
    {
        #region Constructors

        public CreateBoardCommand(
            BoardId boardId, 
            IReadOnlyList<Block> blocks,
            bool requireKings = false)
            : base(boardId)
        {
            Blocks = blocks;
            RequireKings = requireKings;
        }

        #endregion

        #region Properties

        public IReadOnlyList<Block> Blocks { get; }
        public bool RequireKings { get; }

        #endregion
    }

    public class CreateBoardCommandHandler
        : CommandHandler<Board, BoardId, CreateBoardCommand>
    {
        #region Virtual Methods

        public override Task<IExecutionResult> ExecuteAsync(
            Board aggregate, 
            CreateBoardCommand command, 
            CancellationToken cancellationToken)
        {
            aggregate
                .CreateBoard(command.Blocks, command.RequireKings);

            return Task.FromResult(ExecutionResult.Success(aggregate.Id));
        }

        #endregion
    }
}
